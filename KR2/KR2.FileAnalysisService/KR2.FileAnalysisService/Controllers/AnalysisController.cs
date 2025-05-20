using KR2.FileAnalysisService.Data;
using KR2.FileAnalysisService.Models;
using KR2.FileAnalysisService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KR2.FileAnalysisService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController(
    FileAnalysisDbContext dbContext,
    TextAnalysisService textAnalysisService,
    FileStoringServiceClient fileStoringServiceClient,
    WordCloudService wordCloudService,
    ILogger<AnalysisController> logger) : ControllerBase
{

    /// <summary>
    /// Analyzes a file by its ID. If the analysis already exists, it returns the existing result.
    /// </summary>
    /// <response code="200">Returns the analysis result.</response>
    [HttpPost("{fileId}")]
    [ProducesResponseType(typeof(FileAnalysisResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AnalyzeFile(Guid fileId)
    {
        var existingAnalysis = await dbContext.AnalysisResults
            .Include(a => a.SimilarityResults)
            .FirstOrDefaultAsync(a => a.FileId == fileId);

        if (existingAnalysis != null)
        {
            logger.LogInformation("Analysis for file {FileId} already exists", fileId);
            return Ok(existingAnalysis);
        }

        var fileContent = await fileStoringServiceClient.GetFileContentAsync(fileId);
        if (string.IsNullOrEmpty(fileContent))
        {
            logger.LogError("Failed to retrieve file {FileId} content", fileId);
            return NotFound($"File with ID {fileId} not found");
        }

        var analysisResult = textAnalysisService.AnalyzeText(fileContent, fileId);
        string? wordCloudImagePath = await wordCloudService.GenerateWordCloudAsync(fileContent, fileId);
        analysisResult.WordCloudImagePath = wordCloudImagePath;

        var otherAnalysisResults = await dbContext.AnalysisResults
            .Where(a => a.FileId != fileId)
            .ToListAsync();

        foreach (var otherAnalysis in otherAnalysisResults)
        {
            var (otherSuccess, otherFileName) = await fileStoringServiceClient.GetFileMetadataAsync(otherAnalysis.FileId);
            if (!otherSuccess)
            {
                continue;
            }

            var otherFileContent = await fileStoringServiceClient.GetFileContentAsync(otherAnalysis.FileId);
            if (string.IsNullOrEmpty(otherFileContent))
            {
                continue;
            }

            double similarityScore = textAnalysisService.CalculateSimilarity(fileContent, otherFileContent);
            if (similarityScore > 0.5)
            {
                analysisResult.SimilarityResults.Add(new FileSimilarityResult
                {
                    Id = Guid.NewGuid(),
                    FileAnalysisResultId = analysisResult.Id,
                    SimilarFileId = otherAnalysis.FileId,
                    SimilarFileName = otherFileName,
                    SimilarityScore = similarityScore
                });
            }
        }

        dbContext.AnalysisResults.Add(analysisResult);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Analysis for file {FileId} completed", fileId);
        return Ok(analysisResult);
    }

    /// <summary>
    /// Retrieves the analysis result for a given file ID.
    /// </summary>
    /// <response code="200">Returns the analysis result.</response>
    [HttpGet("{fileId}")]
    [ProducesResponseType(typeof(FileAnalysisResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAnalysis(Guid fileId)
    {
        var analysisResult = await dbContext.AnalysisResults
            .Include(a => a.SimilarityResults)
            .FirstOrDefaultAsync(a => a.FileId == fileId);

        if (analysisResult == null)
        {
            logger.LogWarning("Analysis for file {FileId} not found", fileId);
            return NotFound();
        }

        return Ok(analysisResult);
    }

    /// <summary>
    /// Retrieves the word cloud image for a given file ID.
    /// </summary>
    /// <response code="200">Returns the word cloud image.</response>
    /// <response code="404">Word cloud image not found.</response>
    [HttpGet("word-cloud/{fileId}")]
    [Produces("image/png", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWordCloud(Guid fileId)
    {
        var analysisResult = await dbContext.AnalysisResults
            .FirstOrDefaultAsync(a => a.FileId == fileId);

        if (analysisResult == null || string.IsNullOrEmpty(analysisResult.WordCloudImagePath))
        {
            Response.ContentType = "text/plain";
            return NotFound("Word cloud not yet generated. Please call the analyze endpoint first.");
        }

        var imagePath = Path.Combine(wordCloudService.WordCloudStoragePath, analysisResult.WordCloudImagePath);

        if (!System.IO.File.Exists(imagePath))
        {
            logger.LogWarning("Recorded word cloud image file not found at {Path}", imagePath);
            Response.ContentType = "text/plain";
            return NotFound("Word cloud image file not found. Please re-analyze the file.");
        }

        logger.LogInformation("Returning word cloud image for file {FileId}", fileId);
        return PhysicalFile(imagePath, "image/png");
    }
}
