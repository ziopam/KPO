using KR3.OrderService.Services;
using KR3.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace KR3.OrderService.Controllers
{
    /// <summary>
    /// Controller for managing orders
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IOrderService orderService, ILogger<OrdersController> logger) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrdersController> _logger = logger;

        /// <summary>
        /// Creates a new order
        /// </summary>
        /// <param name="request">Order creation request containing user ID, amount, and description</param>
        /// <returns>Created order details</returns>
        /// <response code="201">Order created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Received order creation request for user {UserId}", request.UserId);

                var order = await _orderService.CreateOrderAsync(request);

                _logger.LogInformation("Order created: {OrderId}", order.OrderId);
                return CreatedAtAction(nameof(GetOrderById), new { orderId = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "An error occurred while creating the order");
            }
        }

        /// <summary>
        /// Gets an order by its ID and user ID
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve</param>
        /// <param name="userId">The ID of the user who owns the order</param>
        /// <returns>Order details</returns>
        /// <response code="200">Order found and returned</response>
        /// <response code="404">Order not found</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("{orderId:guid}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderResponse>> GetOrderById(Guid orderId, [FromQuery] Guid userId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId, userId);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order {OrderId}", orderId);
                return StatusCode(500, "An error occurred while retrieving the order");
            }
        }

        /// <summary>
        /// Gets all orders for a specific user
        /// </summary>
        /// <param name="userId">ID of the user whose orders to retrieve</param>
        /// <returns>List of user orders</returns>
        /// <response code="200">Orders found and returned</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetUserOrders([FromQuery] Guid userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersForUserAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving orders");
            }
        }
    }
}
