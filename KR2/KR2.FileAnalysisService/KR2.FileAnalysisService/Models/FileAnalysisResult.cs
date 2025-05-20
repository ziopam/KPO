namespace KR2.FileAnalysisService.Models;

public class FileAnalysisResult
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public int ParagraphCount { get; set; }
    public int WordCount { get; set; }
    public int CharacterCount { get; set; }
    public string? WordCloudImagePath { get; set; }
    public DateTime AnalysisDate { get; set; }
    public List<FileSimilarityResult> SimilarityResults { get; set; } = [];
}
