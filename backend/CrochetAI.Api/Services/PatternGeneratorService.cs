using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.Logging;

namespace CrochetAI.Api.Services;

public class PatternGeneratorService : IPatternGeneratorService
{
    private readonly AnthropicClient _client;
    private readonly ILogger<PatternGeneratorService> _logger;
    private readonly IRedisService _redisService;
    private readonly string _model;

    public PatternGeneratorService(
        IConfiguration configuration,
        ILogger<PatternGeneratorService> logger,
        IRedisService redisService)
    {
        var apiKey = configuration["Anthropic:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Anthropic API key is not configured");
        }

        _client = new AnthropicClient(apiKey);
        _model = configuration["Anthropic:Model"] ?? "claude-sonnet-4-20250514";
        _logger = logger;
        _redisService = redisService;
    }

    public async Task<PatternGenerationResult> GeneratePatternAsync(PatternGenerationRequest request, CancellationToken cancellationToken = default)
    {
        // Check cache first
        var cacheKey = $"pattern:{request.ImageAnalysis.GetHashCode()}";
        var cached = await _redisService.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogInformation("Returning cached pattern generation result");
            return System.Text.Json.JsonSerializer.Deserialize<PatternGenerationResult>(cached)!;
        }

        try
        {
            var prompt = $@"Based on this image analysis, generate a complete crochet pattern in JSON format:

Image Analysis:
{request.ImageAnalysis}

{(string.IsNullOrEmpty(request.UserPreferences) ? "" : $"User Preferences: {request.UserPreferences}")}

Generate a JSON object with the following structure:
{{
  ""title"": ""Pattern title"",
  ""description"": ""Brief description"",
  ""difficulty"": ""Beginner|Intermediate|Advanced"",
  ""category"": ""Amigurumi|Clothing|Home|Accessories"",
  ""materials"": [""yarn type and color"", ""hook size"", ""other materials""],
  ""instructions"": ""Complete step-by-step instructions"",
  ""notes"": ""Additional notes or tips""
}}

Return ONLY valid JSON, no markdown formatting or additional text.";

            var messageRequest = new MessageRequest
            {
                Model = _model,
                MaxTokens = 4096,
                Messages = new[]
                {
                    new Message
                    {
                        Role = "user",
                        Content = new[]
                        {
                            new TextContentBlock
                            {
                                Text = prompt
                            }
                        }
                    }
                }
            };

            var response = await _client.Messages.GetClaudeMessageAsync(messageRequest, cancellationToken);
            var jsonText = response.Content.FirstOrDefault()?.Text ?? "{}";

            // Clean JSON if wrapped in markdown
            jsonText = jsonText.Trim();
            if (jsonText.StartsWith("```json"))
            {
                jsonText = jsonText.Substring(7);
            }
            if (jsonText.StartsWith("```"))
            {
                jsonText = jsonText.Substring(3);
            }
            if (jsonText.EndsWith("```"))
            {
                jsonText = jsonText.Substring(0, jsonText.Length - 3);
            }
            jsonText = jsonText.Trim();

            var result = System.Text.Json.JsonSerializer.Deserialize<PatternGenerationResult>(jsonText) 
                ?? new PatternGenerationResult();

            // Cache the result for 24 hours
            await _redisService.SetStringAsync(
                cacheKey,
                System.Text.Json.JsonSerializer.Serialize(result),
                TimeSpan.FromHours(24));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate pattern");
            throw;
        }
    }
}
