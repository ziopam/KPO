using KR3.PaymentService.Services;
using KR3.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace KR3.PaymentService.Controllers
{
    /// <summary>
    /// Controller for managing user accounts and payment operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IPaymentService paymentService, ILogger<AccountsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="request">Account creation request containing user ID</param>
        /// <returns>Created account details</returns>
        /// <response code="201">Account created successfully</response>
        /// <response code="409">Account already exists for the user</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountResponse>> CreateAccount([FromBody] CreateAccountRequest request)
        {
            try
            {
                _logger.LogInformation("Received account creation request for user {UserId}", request.UserId);

                var account = await _paymentService.CreateAccountAsync(request);

                _logger.LogInformation("Account created: {AccountId} for user {UserId}", account.AccountId, account.UserId);
                return CreatedAtAction(nameof(GetAccount), new { userId = account.UserId }, account);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Account creation failed for user {UserId}", request.UserId);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account for user {UserId}", request.UserId);

                var errorMessage = ex.InnerException != null ?
                    $"Error: {ex.Message}. Inner error: {ex.InnerException.Message}" :
                    $"Error: {ex.Message}";

                return StatusCode(500, new { message = errorMessage });
            }
        }

        /// <summary>
        /// Gets account information for a specific user
        /// </summary>
        /// <param name="userId">ID of the user whose account to retrieve</param>
        /// <returns>Account details</returns>
        /// <response code="200">Account found and returned</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Server error occurred</response>
        [HttpGet("{userId:guid}")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountResponse>> GetAccount(Guid userId)
        {
            try
            {
                _logger.LogInformation("Getting account for user {UserId}", userId);

                var account = await _paymentService.GetAccountAsync(userId);

                return Ok(account);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Account not found for user {userId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving the account");
            }
        }

        /// <summary>
        /// Deposits funds into a user's account
        /// </summary>
        /// <param name="request">Deposit request containing user ID and amount</param>
        /// <returns>Updated account details</returns>
        /// <response code="200">Deposit successful</response>
        /// <response code="400">Invalid deposit amount</response>
        /// <response code="404">Account not found</response>
        /// <response code="500">Server error occurred</response>
        [HttpPost("deposit")]
        [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountResponse>> DepositToAccount([FromBody] DepositRequest request)
        {
            try
            {
                _logger.LogInformation("Deposit request for user {UserId}, amount {Amount}", request.UserId, request.Amount);

                var account = await _paymentService.DepositToAccountAsync(request);

                _logger.LogInformation("Deposit successful for user {UserId}, new balance: {Balance}",
                    request.UserId, account.Balance);

                return Ok(account);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Account not found for user {request.UserId}" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deposit for user {UserId}", request.UserId);
                return StatusCode(500, "An error occurred while processing the deposit");
            }
        }
    }
}
