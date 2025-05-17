using KR2.FileAnalysisService.Models;
using System.Text.RegularExpressions;

namespace KR2.FileAnalysisService.Services;

public class TextAnalysisService
{
    private readonly ILogger<TextAnalysisService> _logger;

    public TextAnalysisService(ILogger<TextAnalysisService> logger)
    {
        _logger = logger;
    }

    public (int paragraphCount, int wordCount, int characterCount) AnalyzeText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            _logger.LogWarning("Attempting to analyze empty text");
            return (0, 0, 0);
        }

        int characterCount = text.Length;
        string[] paragraphs = Regex.Split(text, @"(\r\n|\n|\r){2,}")
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToArray();
        int paragraphCount = paragraphs.Length;

        string[] words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int wordCount = words.Length;

        _logger.LogInformation("Text analysis completed: {ParagraphCount} paragraphs, {WordCount} words, {CharacterCount} characters",
            paragraphCount, wordCount, characterCount);

        return (paragraphCount, wordCount, characterCount);
    }

    public FileAnalysisResult AnalyzeText(string text, Guid fileId)
    {
        var (paragraphCount, wordCount, characterCount) = AnalyzeText(text);

        var result = new FileAnalysisResult
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            ParagraphCount = paragraphCount,
            WordCount = wordCount,
            CharacterCount = characterCount,
            AnalysisDate = DateTime.UtcNow
        };

        _logger.LogInformation("Created analysis result for file {FileId}", fileId);

        return result;
    }

    public double CalculateSimilarity(string text1, string text2)
    {
        if (string.IsNullOrEmpty(text1) || string.IsNullOrEmpty(text2))
        {
            return 0.0;
        }

        if (text1 == text2)
        {
            return 1.0;
        }

        var words1 = text1.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .ToList();
        var words2 = text2.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLowerInvariant())
            .ToList();

        if (!words1.Any() || !words2.Any())
        {
            return 0.0;
        }

        var wordCounts1 = words1.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());
        var wordCounts2 = words2.GroupBy(w => w).ToDictionary(g => g.Key, g => g.Count());

        double dotProduct = 0;
        foreach (var word in wordCounts1.Keys)
        {
            if (wordCounts2.TryGetValue(word, out int count2))
            {
                dotProduct += wordCounts1[word] * count2;
            }
        }

        double magnitude1 = Math.Sqrt(wordCounts1.Sum(wc => wc.Value * wc.Value));
        double magnitude2 = Math.Sqrt(wordCounts2.Sum(wc => wc.Value * wc.Value));

        if (magnitude1 * magnitude2 == 0)
        {
            return 0.0;
        }

        double similarity = dotProduct / (magnitude1 * magnitude2);

        _logger.LogInformation("Similarity calculation completed: {Similarity}", similarity);

        return similarity;
    }
}
