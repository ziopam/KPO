using System.Text.Json.Serialization;

namespace KR2.FileAnalysisService.Models;

public class FileSimilarityResult
{
    public Guid Id { get; set; }
    public Guid FileAnalysisResultId { get; set; }
    public Guid SimilarFileId { get; set; }
    public string SimilarFileName { get; set; } = null!;
    public double SimilarityScore { get; set; }

    [JsonIgnore]
    public FileAnalysisResult FileAnalysisResult { get; set; } = null!;
}
