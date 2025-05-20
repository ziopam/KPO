using KR2.FileStoringService.Data;
using KR2.FileStoringService.Models;
using KR2.FileStoringService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KR2.FileStoringService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController(
    FileStoringDbContext dbContext,
    FileHashService fileHashService,
    FileStorageService fileStorageService,
    ILogger<FilesController> logger) : ControllerBase
{
    /// <summary>
    /// Uploads a file to the file storage. 
    /// </summary>
    /// <response code="200">File was successfully uploaded. Returns Guid of it.</response>
    /// <response code="400">Empty, not .txt or already existing file was uploaded.</response>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            logger.LogInformation("Received upload request for file: {FileName}, Size: {FileSize}",
                file?.FileName ?? "null", file?.Length.ToString() ?? "0");

            if (file == null || file.Length == 0)
            {
                logger.LogWarning("Upload request received with null or empty file");
                return BadRequest("A non-empty file is required");
            }

            if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Upload request received with non-txt file: {FileName}", file.FileName);
                return BadRequest("Only .txt files are supported");
            }

            logger.LogInformation("Processing file upload for: {FileName}", file.FileName);

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileHash = await fileHashService.CalculateHashAsync(memoryStream);
            logger.LogInformation("Calculated hash for file {FileName}: {FileHash}", file.FileName, fileHash); var existingFile = await dbContext.Files
                .FirstOrDefaultAsync(f => f.FileHash == fileHash);

            if (existingFile != null)
            {
                logger.LogInformation("File with hash {FileHash} already exists with ID {FileId}", fileHash, existingFile.Id);
                return BadRequest("File with the same data already exists");
            }

            logger.LogInformation("Saving file to storage location with base path: {BasePath}", fileStorageService._storageBasePath);
            string storageLocation;
            try
            {
                storageLocation = await fileStorageService.SaveFileAsync(memoryStream, file.FileName, fileHash);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving file to storage: {ErrorMessage}", ex.Message);
                return StatusCode(500, $"Internal server error while saving file: {ex.Message}");
            }
            logger.LogInformation("File saved to storage location: {StorageLocation}", storageLocation);

            var fileRecord = new FileRecord
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FileHash = fileHash,
                StorageLocation = storageLocation,
                FileSize = file.Length,
                UploadDate = DateTime.UtcNow,
                ContentType = file.ContentType
            };

            dbContext.Files.Add(fileRecord);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("File {FileName} uploaded successfully with ID {FileId}", file.FileName, fileRecord.Id);
            return Ok(new { FileId = fileRecord.Id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during file upload: {ErrorMessage}", ex.Message);
            return StatusCode(500, $"Internal server error during file upload: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves a file by its ID.
    /// </summary>
    /// <response code="200">File was successfully retrieved.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(File), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var fileRecord = await dbContext.Files.FindAsync(id);
        if (fileRecord == null)
        {
            logger.LogWarning("File with ID {FileId} not found", id);
            return NotFound();
        }

        try
        {
            var fileStream = await fileStorageService.GetFileAsync(fileRecord.StorageLocation);
            return File(fileStream, "text/plain", fileRecord.FileName);
        }
        catch (FileNotFoundException)
        {
            logger.LogError("File {FileName} with ID {FileId} not found in storage", fileRecord.FileName, id);
            return NotFound();
        }
    }


    /// <summary>
    /// Retrieves metadata for a file by its ID.
    /// </summary>
    /// <response code="200">File metadata was successfully retrieved.</response>
    [HttpGet("{id}/metadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFileMetadata(Guid id)
    {
        var fileRecord = await dbContext.Files.FindAsync(id);
        if (fileRecord == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            fileRecord.Id,
            fileRecord.FileName,
            fileRecord.FileSize,
            fileRecord.UploadDate,
            fileRecord.ContentType
        });
    }
}
