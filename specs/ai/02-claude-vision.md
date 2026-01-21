# AI Spec: Claude Vision Integration

**File**: `specs/ai/02-claude-vision.md`  
**Task**: TASK-033  
**Dependencies**: TASK-029 (Blob Storage), TASK-032 (AI Interfaces)

---

## 📋 Overview

Integrate Anthropic Claude Vision API to analyze uploaded crochet images and extract detailed information about the pattern.

---

## 🎯 Service Interface

```csharp
public interface IClaudeVisionService
{
    Task<CrochetAnalysis> AnalyzeImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}

public class CrochetAnalysis
{
    public string Shape { get; set; } // "circular", "rectangular", "3d-amigurumi", etc.
    public List<string> Stitches { get; set; } // ["single-crochet", "double-crochet"]
    public string Technique { get; set; } // "worked-in-rounds", "worked-in-rows"
    public List<string> Colors { get; set; } // ["pink", "white", "black"]
    public string Size { get; set; } // "small-5cm", "medium-15cm", "large-30cm"
    public string Difficulty { get; set; } // "beginner", "intermediate", "advanced"
    public string YarnType { get; set; } // "cotton-dk", "acrylic-worsted"
    public string HookSize { get; set; } // "3mm", "4mm"
    public List<string> Details { get; set; } // ["safety-eyes", "embroidered-nose"]
    public int EstimatedRounds { get; set; } // Approximate number of rounds/rows
    public string RawResponse { get; set; } // Full Claude response for debugging
}
```

---

## 🔧 Implementation

### 1. Install Anthropic SDK

```xml
<PackageReference Include="Anthropic.SDK" Version="0.1.0" />
```

### 2. Configuration

```csharp
// appsettings.json
{
  "Anthropic": {
    "ApiKey": "sk-ant-...",
    "Model": "claude-sonnet-4-20250514",
    "MaxTokens": 1024,
    "Timeout": 30000 // 30 seconds
  }
}

// Configuration class
public class AnthropicSettings
{
    public string ApiKey { get; set; }
    public string Model { get; set; }
    public int MaxTokens { get; set; }
    public int Timeout { get; set; }
}

// Register in Program.cs
builder.Services.Configure<AnthropicSettings>(
    builder.Configuration.GetSection("Anthropic"));
builder.Services.AddSingleton<IClaudeVisionService, ClaudeVisionService>();
```

### 3. Service Implementation

```csharp
public class ClaudeVisionService : IClaudeVisionService
{
    private readonly AnthropicClient _client;
    private readonly AnthropicSettings _settings;
    private readonly ILogger<ClaudeVisionService> _logger;

    public ClaudeVisionService(
        IOptions<AnthropicSettings> settings,
        ILogger<ClaudeVisionService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _client = new AnthropicClient(_settings.ApiKey);
    }

    public async Task<CrochetAnalysis> AnalyzeImageAsync(
        string imageUrl, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Download image to base64
            var base64Image = await DownloadImageAsBase64Async(imageUrl, cancellationToken);

            // Create prompt
            var prompt = BuildAnalysisPrompt();

            // Call Claude Vision API
            var response = await _client.Messages.CreateAsync(
                model: _settings.Model,
                maxTokens: _settings.MaxTokens,
                messages: new[]
                {
                    new Message
                    {
                        Role = "user",
                        Content = new ContentBlock[]
                        {
                            new ImageContentBlock
                            {
                                Source = new ImageSource
                                {
                                    Type = "base64",
                                    MediaType = "image/jpeg",
                                    Data = base64Image
                                }
                            },
                            new TextContentBlock { Text = prompt }
                        }
                    }
                },
                cancellationToken: cancellationToken
            );

            // Parse response
            var textContent = response.Content
                .OfType<TextContentBlock>()
                .FirstOrDefault()?.Text ?? throw new Exception("No text in response");

            var analysis = ParseAnalysisResponse(textContent);
            analysis.RawResponse = textContent;

            _logger.LogInformation(
                "Successfully analyzed image. Difficulty: {Difficulty}, Rounds: {Rounds}",
                analysis.Difficulty, analysis.EstimatedRounds);

            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze image: {ImageUrl}", imageUrl);
            throw;
        }
    }

    private string BuildAnalysisPrompt()
    {
        return @"
Analyze this crochet work photo and provide a detailed technical analysis.

Respond in JSON format with these exact fields:
{
  ""shape"": ""<circular|rectangular|square|oval|3d-amigurumi|other>"",
  ""stitches"": [""<single-crochet|double-crochet|half-double-crochet|slip-stitch|other>""],
  ""technique"": ""<worked-in-rounds|worked-in-rows|spiral|other>"",
  ""colors"": [""<color-names>""],
  ""size"": ""<small-<10cm|medium-10-20cm|large->20cm>"",
  ""difficulty"": ""<beginner|intermediate|advanced>"",
  ""yarnType"": ""<cotton-dk|acrylic-worsted|cotton-sport|wool-bulky|other>"",
  ""hookSize"": ""<2mm|2.5mm|3mm|3.5mm|4mm|4.5mm|5mm|5.5mm|6mm|other>"",
  ""details"": [""<safety-eyes|embroidered-features|appliques|other-details>""],
  ""estimatedRounds"": <number>
}

Be specific and technical. If something is not visible or unclear, mark it as ""unknown"".
Estimate the number of rounds/rows based on visible structure.

IMPORTANT: Respond ONLY with valid JSON, no other text.
";
    }

    private CrochetAnalysis ParseAnalysisResponse(string jsonResponse)
    {
        try
        {
            // Remove markdown code fences if present
            var cleaned = jsonResponse
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            var json = JsonDocument.Parse(cleaned);
            var root = json.RootElement;

            return new CrochetAnalysis
            {
                Shape = root.GetProperty("shape").GetString(),
                Stitches = root.GetProperty("stitches")
                    .EnumerateArray()
                    .Select(x => x.GetString())
                    .ToList(),
                Technique = root.GetProperty("technique").GetString(),
                Colors = root.GetProperty("colors")
                    .EnumerateArray()
                    .Select(x => x.GetString())
                    .ToList(),
                Size = root.GetProperty("size").GetString(),
                Difficulty = root.GetProperty("difficulty").GetString(),
                YarnType = root.GetProperty("yarnType").GetString(),
                HookSize = root.GetProperty("hookSize").GetString(),
                Details = root.GetProperty("details")
                    .EnumerateArray()
                    .Select(x => x.GetString())
                    .ToList(),
                EstimatedRounds = root.GetProperty("estimatedRounds").GetInt32()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse Claude response: {Response}", jsonResponse);
            throw new InvalidOperationException("Claude returned invalid JSON", ex);
        }
    }

    private async Task<string> DownloadImageAsBase64Async(
        string imageUrl, 
        CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl, cancellationToken);
        
        // Validate image size (max 10MB)
        if (imageBytes.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("Image too large (max 10MB)");

        return Convert.ToBase64String(imageBytes);
    }
}
```

---

## 🧪 Testing

### Unit Tests

```csharp
public class ClaudeVisionServiceTests
{
    [Fact]
    public async Task AnalyzeImageAsync_ValidImage_ReturnsAnalysis()
    {
        // Arrange
        var mockSettings = Options.Create(new AnthropicSettings
        {
            ApiKey = "test-key",
            Model = "claude-sonnet-4-20250514",
            MaxTokens = 1024,
            Timeout = 30000
        });
        var mockLogger = new Mock<ILogger<ClaudeVisionService>>();
        var service = new ClaudeVisionService(mockSettings, mockLogger.Object);

        // Mock HTTP client to return test image
        // Mock Anthropic client to return test response

        // Act
        var result = await service.AnalyzeImageAsync("https://example.com/test.jpg");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Shape);
        Assert.NotEmpty(result.Stitches);
    }

    [Fact]
    public async Task AnalyzeImageAsync_ImageTooLarge_ThrowsException()
    {
        // Test that images over 10MB are rejected
    }

    [Fact]
    public async Task AnalyzeImageAsync_InvalidJson_ThrowsException()
    {
        // Test that malformed Claude responses are handled
    }

    [Fact]
    public async Task AnalyzeImageAsync_ApiTimeout_ThrowsException()
    {
        // Test timeout handling
    }
}
```

---

## 💰 Cost Estimation

**Claude Sonnet 4 Pricing**:
- Input: $3 per million tokens
- Output: $15 per million tokens
- Images: ~$0.05 per image (varies by size)

**Typical Request**:
- Image: ~1,500 tokens (for a standard photo)
- Prompt: ~200 tokens
- Response: ~300 tokens
- **Total Cost**: ~$0.05 per analysis

**Monthly Projections**:
- 100 users × 2 generations = 200 analyses = $10
- 1,000 users × 2 generations = 2,000 analyses = $100
- 10,000 users × 2 generations = 20,000 analyses = $1,000

**Mitigation**:
- Cache results by image hash (SHA256)
- Rate limit per user tier
- Use cheaper model for simple patterns

---

## 🔒 Security Considerations

1. **API Key Protection**: Store in Azure Key Vault (production), never commit to git
2. **Rate Limiting**: Enforce per-user limits to prevent cost attacks
3. **Image Validation**: Verify image is actually crochet (use confidence score)
4. **Timeout**: 30 second max to prevent hanging requests
5. **Error Handling**: Never expose API key in error messages
6. **Audit Logging**: Log all API calls with user ID and cost

---

## 🔄 Retry Logic

```csharp
public async Task<CrochetAnalysis> AnalyzeImageWithRetryAsync(
    string imageUrl, 
    int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await AnalyzeImageAsync(imageUrl);
        }
        catch (HttpRequestException ex) when (attempt < maxRetries)
        {
            _logger.LogWarning(ex, 
                "Attempt {Attempt}/{MaxRetries} failed, retrying...", 
                attempt, maxRetries);
            
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Exponential backoff
        }
    }

    throw new InvalidOperationException($"Failed after {maxRetries} attempts");
}
```

---

## 📊 Monitoring

**Metrics to Track**:
- API call success rate
- Average response time
- Cost per analysis
- Cache hit rate
- Error types and frequency

**Logging**:
```csharp
_logger.LogInformation(
    "Claude Vision API call: ImageUrl={ImageUrl}, UserId={UserId}, Duration={Duration}ms, Cost=${Cost}",
    imageUrl, userId, duration, cost);
```

---

## ✅ Definition of Done

- [ ] `ClaudeVisionService` implemented
- [ ] Configuration in appsettings.json
- [ ] Registered in DI container
- [ ] Unit tests written and passing (4+ tests)
- [ ] Retry logic implemented
- [ ] Error handling comprehensive
- [ ] Logging in place
- [ ] API key in environment variable (not hardcoded)
- [ ] Cost tracking working
- [ ] Integration test with real API (optional, manual)

---

**Next Steps**: Proceed to TASK-034 (Pattern Generator Service) which uses the analysis output to generate actual pattern text.
