namespace KR2.FileStoringService.Models;

public class FileRecord
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileHash { get; set; } = null!;
    public string StorageLocation { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadDate { get; set; }
    public string ContentType { get; set; } = "text/plain";
}
