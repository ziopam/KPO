using System.Text.RegularExpressions;

namespace KR2.FileAnalysisService.Services;

public class WordCloudService
{
    private readonly ILogger<WordCloudService> _logger;
    private readonly string _wordCloudStoragePath;
    private readonly HttpClient _httpClient;
    private const string QuickChartUrl = "https://quickchart.io/wordcloud";

    public string WordCloudStoragePath => _wordCloudStoragePath;

    public WordCloudService(IConfiguration configuration, ILogger<WordCloudService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _wordCloudStoragePath = configuration.GetValue<string>("WordCloudStorage:BasePath") ?? "/app/WordCloudStorage";
        _httpClient = httpClientFactory.CreateClient("QuickChart");

        if (!Directory.Exists(_wordCloudStoragePath))
        {
            Directory.CreateDirectory(_wordCloudStoragePath);
            _logger.LogInformation("Created word cloud storage directory at {Path}", _wordCloudStoragePath);
        }
    }
    public async Task<string?> GenerateWordCloudAsync(string text, Guid fileId)
    {
        if (string.IsNullOrEmpty(text))
        {
            _logger.LogWarning("Cannot generate word cloud for empty text");
            return null;
        }

        try
        {
            var wordFrequencies = ProcessText(text);
            if (wordFrequencies.Count == 0)
            {
                _logger.LogWarning("No meaningful words found in text for file {FileId}", fileId);
                return null;
            }

            string imagePath = Path.Combine(_wordCloudStoragePath, $"{fileId}.png");
            var success = await GenerateWordCloudWithQuickChartAsync(wordFrequencies, imagePath);
            if (!success)
            {
                _logger.LogError("Failed to generate word cloud for file {FileId}", fileId);
                return null;
            }

            _logger.LogInformation("Word cloud generated successfully for file {FileId}", fileId);
            return $"{fileId}.png";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating word cloud for file {FileId}", fileId);
            return null;
        }
    }

    public Task<string?> GetWordCloudPathAsync(Guid fileId)
    {
        string imagePath = Path.Combine(_wordCloudStoragePath, $"{fileId}.png");
        if (!File.Exists(imagePath))
        {
            _logger.LogWarning("Word cloud image does not exist for file {FileId}", fileId);
            return Task.FromResult<string?>(null);
        }
        return Task.FromResult<string?>(imagePath);
    }

    private Dictionary<string, int> ProcessText(string text)
    {
        var wordFrequencies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        text = Regex.Replace(text, @"[^\w\s]", " ").ToLower();
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (word.Length >= 3)
            {
                if (wordFrequencies.ContainsKey(word))
                {
                    wordFrequencies[word]++;
                }
                else
                {
                    wordFrequencies[word] = 1;
                }
            }
        }

        return wordFrequencies
            .OrderByDescending(pair => pair.Value)
            .Take(100)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    private async Task<bool> GenerateWordCloudWithQuickChartAsync(Dictionary<string, int> wordFrequencies, string outputPath)
    {
        try
        {
            var words = wordFrequencies
                .Select(pair => new { text = pair.Key, value = pair.Value })
                .ToArray();

            var requestBody = new
            {
                format = "png",
                width = 800,
                height = 600,
                fontFamily = "sans-serif",
                fontScale = 15,
                scale = "linear",
                colors = new[] { "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd", "#8c564b" },
                rotation = 0,
                minRotation = 0,
                maxRotation = 0,
                rotationSteps = 2,
                backgroundColor = "white",
                text = string.Join(" ", wordFrequencies.SelectMany(wf => Enumerable.Repeat(wf.Key, wf.Value)))
            };

            var response = await _httpClient.PostAsJsonAsync(QuickChartUrl, requestBody);
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType?.StartsWith("image/") == true)
            {
                using (var imageStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    await imageStream.CopyToAsync(fileStream);
                }
                return true;
            }
            else if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("QuickChart API returned error: {StatusCode}, {ErrorContent}",
                    response.StatusCode, errorContent);
                return false;
            }
            else
            {
                _logger.LogError("Unexpected response from QuickChart API: {ContentType}", contentType);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling QuickChart API");
            return false;
        }
    }
}
