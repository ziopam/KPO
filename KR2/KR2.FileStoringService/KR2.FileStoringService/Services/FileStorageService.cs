namespace KR2.FileStoringService.Services;

public class FileStorageService
{
    public readonly string _storageBasePath;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _storageBasePath = configuration.GetValue<string>("FileStorage:BasePath") ?? "/app/FileStorage";
        _logger = logger;

        if (!Directory.Exists(_storageBasePath))
        {
            Directory.CreateDirectory(_storageBasePath);
            _logger.LogInformation("Created file storage directory at {Path}", _storageBasePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string fileHash)
    {
        try
        {
            if (!Directory.Exists(_storageBasePath))
            {
                _logger.LogInformation("Storage directory does not exist, creating it: {Path}", _storageBasePath);
                Directory.CreateDirectory(_storageBasePath);
            }

            var fileExtension = Path.GetExtension(fileName);
            var storageFileName = $"{fileHash}{fileExtension}";
            var storagePath = Path.Combine(_storageBasePath, storageFileName);

            _logger.LogInformation("Saving file to path: {StoragePath}", storagePath);

            using (var fileStream2 = new FileStream(storagePath, FileMode.Create))
            {
                fileStream.Position = 0;
                await fileStream.CopyToAsync(fileStream2);
                _logger.LogInformation("Successfully wrote {Length} bytes to file", fileStream2.Length);
            }

            return storageFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file to {StoragePath}: {ErrorMessage}",
                Path.Combine(_storageBasePath, $"{fileHash}{Path.GetExtension(fileName)}"), ex.Message);
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string storageLocation)
    {
        var filePath = Path.Combine(_storageBasePath, storageLocation);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("File not found at {FilePath}", filePath);
            throw new FileNotFoundException($"File not found at {filePath}");
        }

        var stream = new MemoryStream();
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            await fileStream.CopyToAsync(stream);
        }

        stream.Position = 0;
        return stream;
    }
}
