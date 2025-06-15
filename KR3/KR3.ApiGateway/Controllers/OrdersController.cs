using KR3.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace KR3.ApiGateway.Controllers
{
    /// <summary>
    /// API Gateway controller for Order Service operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IHttpClientFactory httpClientFactory, ILogger<OrdersController> logger, IConfiguration configuration) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly ILogger<OrdersController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Creates a new order through the Order Service
        /// </summary>
        /// <param name="request">Order creation request</param>
        /// <returns>Order creation response from the Order Service</returns>
        /// <response code="201">Order created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var orderServiceUrl = _configuration["ServiceUrls:OrderService"];
            var url = $"{orderServiceUrl}/api/orders";

            _logger.LogInformation("Forwarding create order request to {Url}", url);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                var responseBody = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding create order request");
                return StatusCode(500, new { message = "Error processing request" });
            }
        }

        /// <summary>
        /// Gets an order by ID through the Order Service
        /// </summary>
        /// <param name="orderId">ID of the order to retrieve</param>
        /// <param name="userId">ID of the user who owns the order</param>
        /// <returns>Order details from the Order Service</returns>
        /// <response code="200">Order found and returned</response>
        /// <response code="404">Order not found</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("{orderId:guid}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrderById(Guid orderId, [FromQuery] Guid userId)
        {
            var orderServiceUrl = _configuration["ServiceUrls:OrderService"];
            var url = $"{orderServiceUrl}/api/orders/{orderId}?userId={userId}";

            _logger.LogInformation("Forwarding get order request to {Url}", url);

            try
            {
                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Ok(JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }));
                }

                return StatusCode((int)response.StatusCode, responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding get order request");
                return StatusCode(500, new { message = "Error processing request" });
            }
        }

        /// <summary>
        /// Gets all orders for a specific user through the Order Service
        /// </summary>
        /// <param name="userId">ID of the user whose orders to retrieve</param>
        /// <returns>List of orders for the specified user</returns>
        /// <response code="200">Orders found and returned</response>
        /// <response code="404">User not found or no orders exist</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserOrders([FromQuery] Guid userId)
        {
            var orderServiceUrl = _configuration["ServiceUrls:OrderService"];
            var url = $"{orderServiceUrl}/api/orders?userId={userId}";

            _logger.LogInformation("Forwarding get user orders request to {Url}", url);

            try
            {
                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                return StatusCode((int)response.StatusCode, JsonSerializer.Deserialize<object>(responseBody));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error forwarding get user orders request");
                return StatusCode(500, new { message = "Error processing request" });
            }
        }
    }
}
