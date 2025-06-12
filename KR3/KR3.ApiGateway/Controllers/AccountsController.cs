using KR3.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace KR3.ApiGateway.Controllers
{
    /// <summary>
    /// API Gateway controller for Account and Payment Service operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController(IHttpClientFactory httpClientFactory, ILogger<AccountsController> logger) : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly ILogger<AccountsController> _logger = logger;

        /// <summary>
        /// Creates a new account through the Payment Service
        /// </summary>
        /// <param name="request">Account creation request containing user information</param>
        /// <returns>Account creation response from the Payment Service</returns>
        /// <response code="201">Account created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="503">Payment service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            _logger.LogInformation("Forwarding create account request for user {UserId}", request.UserId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentService");
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/api/accounts", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Payment service response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var accountResponse = JsonSerializer.Deserialize<AccountResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return CreatedAtAction(nameof(GetAccount), new { userId = accountResponse.UserId }, accountResponse);
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Payment service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Payment Service failed for user {UserId}", request.UserId);
                return StatusCode(503, new { message = "Payment service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding create account request for user {UserId}", request.UserId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets account information through the Payment Service
        /// </summary>
        /// <param name="userId">ID of the user whose account to retrieve</param>
        /// <returns>Account details from the Payment Service</returns>
        /// <response code="200">Account found and returned</response>
        /// <response code="404">Account not found</response>
        /// <response code="503">Payment service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAccount(Guid userId)
        {
            _logger.LogInformation("Forwarding get account request for user {UserId}", userId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentService");
                var response = await httpClient.GetAsync($"/api/accounts/{userId}");
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Payment service response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(JsonSerializer.Deserialize<AccountResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }));
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Payment service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Payment Service failed for user {UserId}", userId);
                return StatusCode(503, new { message = "Payment service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding get account request for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Deposits funds to a user account through the Payment Service
        /// </summary>
        /// <param name="request">Deposit request containing user ID and amount to deposit</param>
        /// <returns>Updated account information after deposit</returns>
        /// <response code="200">Deposit processed successfully</response>
        /// <response code="400">Invalid request data or insufficient funds</response>
        /// <response code="404">Account not found</response>
        /// <response code="503">Payment service is unavailable</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost("deposit")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DepositToAccount([FromBody] DepositRequest request)
        {
            _logger.LogInformation("Forwarding deposit request for user {UserId}", request.UserId);

            try
            {
                var httpClient = _httpClientFactory.CreateClient("PaymentService");
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/api/accounts/deposit", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Payment service response status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(JsonSerializer.Deserialize<AccountResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }));
                }

                return StatusCode((int)response.StatusCode, string.IsNullOrEmpty(responseBody)
                    ? new { message = $"Payment service returned status code {response.StatusCode}" }
                    : JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Connection to Payment Service failed for user {UserId}", request.UserId);
                return StatusCode(503, new { message = "Payment service is currently unavailable" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding deposit request for user {UserId}", request.UserId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
