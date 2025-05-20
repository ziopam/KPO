namespace KR2.FileAnalysisService.Services;

public class FileStoringServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<FileStoringServiceClient> logger)
{
    private readonly string _fileStoringServiceUrl = configuration.GetValue<string>("FileStoringService:Url") ?? "http://file-storing-service";

    public async Task<string> GetFileContentAsync(Guid fileId)
    {
        try
        {
            var response = await httpClient.GetAsync($"{_fileStoringServiceUrl}/api/Files/{fileId}");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to retrieve file {FileId} from File Storing Service. Status code: {StatusCode}",
                    fileId, response.StatusCode);
                return string.Empty;
            }

            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving file {FileId} from File Storing Service", fileId);
            return string.Empty;
        }
    }

    public async Task<(bool success, string fileName)> GetFileMetadataAsync(Guid fileId)
    {
        try
        {
            var response = await httpClient.GetAsync($"{_fileStoringServiceUrl}/api/Files/{fileId}/metadata");
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("Failed to retrieve file metadata for {FileId} from File Storing Service. Status code: {StatusCode}",
                    fileId, response.StatusCode);
                return (false, string.Empty);
            }

            var content = await response.Content.ReadFromJsonAsync<FileMetadata>();
            return (true, content?.FileName ?? string.Empty);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving file metadata for {FileId} from File Storing Service", fileId);
            return (false, string.Empty);
        }
    }

    private class FileMetadata
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
