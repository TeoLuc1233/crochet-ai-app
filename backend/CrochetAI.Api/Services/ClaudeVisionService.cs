using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Microsoft.Extensions.Logging;

namespace CrochetAI.Api.Services;

public class ClaudeVisionService : IClaudeVisionService
{
    private readonly AnthropicClient _client;
    private readonly ILogger<ClaudeVisionService> _logger;
    private readonly string _model;

    public ClaudeVisionService(IConfiguration configuration, ILogger<ClaudeVisionService> logger)
    {
        var apiKey = configuration["Anthropic:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Anthropic API key is not configured");
        }

        _client = new AnthropicClient(apiKey);
        _model = configuration["Anthropic:Model"] ?? "claude-sonnet-4-20250514";
        _logger = logger;
    }

    public async Task<string> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        const int maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                var request = new MessageRequest
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
                                new ImageContentBlock
                                {
                                    Source = new ImageSource
                                    {
                                        Type = "url",
                                        Url = imageUrl
                                    }
                                },
                                new TextContentBlock
                                {
                                    Text = @"Analyze this crochet image and describe:
1. What type of crochet item is shown (amigurumi, clothing, home decor, etc.)
2. The difficulty level (Beginner, Intermediate, Advanced)
3. The main colors and yarn types visible
4. Key techniques used (single crochet, double crochet, increases, decreases, etc.)
5. Estimated size/dimensions
6. Any special features or details

Provide a detailed analysis that can be used to generate a crochet pattern."
                                }
                            }
                        }
                    }
                };

                var response = await _client.Messages.GetClaudeMessageAsync(request, cancellationToken);
                return response.Content.FirstOrDefault()?.Text ?? "Analysis completed but no text returned.";
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning(ex, "Attempt {RetryCount} failed to analyze image", retryCount);

                if (retryCount >= maxRetries)
                {
                    _logger.LogError(ex, "Failed to analyze image after {MaxRetries} attempts", maxRetries);
                    throw;
                }

                await Task.Delay(1000 * retryCount, cancellationToken);
            }
        }

        throw new InvalidOperationException("Failed to analyze image after retries");
    }
}
