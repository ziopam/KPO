using Microsoft.AspNetCore.Mvc;

namespace KR2.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlagiarismController(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<PlagiarismController> logger) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<PlagiarismController> _logger = logger;

    /// <summary>
    /// Uploads a file to the FileStoringService.
    /// </summary>
    /// <response code="200">File was succesfully uploaded. Returns Guid of it.</response>
    /// <response code="400">Empty, not .txt or already existing file was uploaded.</response>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(UploadResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A non-empty file is required");
        }
        var fileStoringServiceUrl = _configuration["FileStoringService:Url"];

        try
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();

            var fileBytes = new byte[file.Length];
            await fileStream.ReadExactlyAsync(fileBytes, 0, (int)file.Length);
            var fileContent = new ByteArrayContent(fileBytes);

            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            _logger.LogInformation("Sending file {FileName} (size {FileSize}) to FileStoringService at {Url}",
                file.FileName, file.Length, $"{fileStoringServiceUrl}/api/Files/upload"); var uploadResponse = await _httpClient.PostAsync($"{fileStoringServiceUrl}/api/Files/upload", content);
            var responseBody = await uploadResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Received response from FileStoringService: {StatusCode}, Body: {ResponseBody}",
                uploadResponse.StatusCode, responseBody);

            if (uploadResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                _logger.LogWarning("File upload failed with BadRequest: {Response}", responseBody);
                return BadRequest(responseBody);
            }

            if (!uploadResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to upload file to FileStoringService. Status code: {StatusCode}, Response: {Response}",
                    uploadResponse.StatusCode, responseBody);
                return StatusCode((int)uploadResponse.StatusCode, $"Error uploading file: {responseBody}");
            }

            var uploadResult = await uploadResponse.Content.ReadFromJsonAsync<UploadResult>();
            if (uploadResult == null)
            {
                _logger.LogError("Invalid response from File Storing Service after file upload");
                return StatusCode(500, "Error processing file upload");
            }

            return Ok(uploadResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected exception occurred while uploading file to File Storing Service");
            return StatusCode(500, $"Error uploading file: {ex.Message}");
        }
    }


    /// <summary>
    /// Analyzes a file using the FileAnalysisService.
    /// </summary>
    /// <response code="200">File was successfully analyzed or result was found in base. Returns analysis result.</response>
    [HttpPost("analyze/{fileId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AnalyzeFile(Guid fileId)
    {
        var fileAnalysisServiceUrl = _configuration["FileAnalysisService:Url"];

        var response = await _httpClient.PostAsync($"{fileAnalysisServiceUrl}/api/Analysis/{fileId}", null);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to analyze file {FileId}. Status code: {StatusCode}", fileId, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error analyzing file");
        }

        var analysisResult = await response.Content.ReadFromJsonAsync<object>();
        return Ok(analysisResult);
    }

    /// <summary>
    /// Retrieves a file from the FileStoringService.
    /// </summary>
    /// <response code="200">File was successfully retrieved.</response>
    [HttpGet("file/{fileId}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFile(Guid fileId)
    {
        var fileStoringServiceUrl = _configuration["FileStoringService:Url"];

        var response = await _httpClient.GetAsync($"{fileStoringServiceUrl}/api/Files/{fileId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogInformation("File {FileId} not found", fileId);
            return NotFound("File not found");
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get file {FileId}. Status code: {StatusCode}", fileId, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error retrieving file");
        }

        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileName ?? $"{fileId}.txt";

        var stream = await response.Content.ReadAsStreamAsync();
        return File(stream, contentType, fileName);
    }

    /// <summary>
    /// Retrieves analysis results for a file from the FileAnalysisService.
    /// </summary>
    [HttpGet("analysis/{fileId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnalysis(Guid fileId)
    {
        var fileAnalysisServiceUrl = _configuration["FileAnalysisService:Url"];

        var response = await _httpClient.GetAsync($"{fileAnalysisServiceUrl}/api/Analysis/{fileId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogInformation("Analysis for file {FileId} not found", fileId);
            return NotFound("Analysis not found");
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to get analysis for file {FileId}. Status code: {StatusCode}", fileId, response.StatusCode);
            return StatusCode((int)response.StatusCode, "Error retrieving analysis");
        }

        var analysisResult = await response.Content.ReadFromJsonAsync<object>();
        return Ok(analysisResult);
    }

    /// <summary>
    /// Retrieves the word cloud image for a given file ID.
    /// </summary>
    /// <response code="200">Returns the word cloud image.</response>
    /// <response code="404">Word cloud not found or not yet generated.</response>
    [HttpGet("word-cloud/{fileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWordCloud(Guid fileId)
    {
        var fileAnalysisServiceUrl = _configuration["FileAnalysisService:Url"]; try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{fileAnalysisServiceUrl}/api/Analysis/word-cloud/{fileId}");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/png"));

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound ||
                response.StatusCode == System.Net.HttpStatusCode.NotAcceptable)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Word cloud not found for file {FileId}. Status: {Status}, Response: {Response}",
                    fileId, response.StatusCode, errorContent);
                return NotFound("Word cloud not yet generated. Please call the analyze endpoint first.");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get word cloud for file {FileId}. Status code: {StatusCode}",
                    fileId, response.StatusCode);
                return StatusCode((int)response.StatusCode, "Error retrieving word cloud");
            }

            var imageStream = await response.Content.ReadAsStreamAsync();
            return File(imageStream, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving word cloud for file {FileId}", fileId);
            return StatusCode(500, "An error occurred while retrieving the word cloud");
        }
    }

    private class UploadResult
    {
        public Guid FileId { get; set; }
    }
}
