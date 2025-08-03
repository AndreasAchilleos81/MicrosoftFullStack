using Microsoft.AspNetCore.Mvc;

namespace ErrorHandlingProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorHandlingController : ControllerBase
    {
        ILogger<ErrorHandlingController> _logger;
        public ErrorHandlingController(ILogger<ErrorHandlingController> logger)
        {
            // Logger can be used for logging errors or information
            _logger = logger;
        }


        [HttpGet("division")]
        public IActionResult GetDivisionResults(int numerator, int denominator)
        {
            throw new Exception("This is a test exception to demonstrate error handling");
			try
            {
                // Attempt to perform division
                int result = numerator / denominator;
                
                return Ok(new { 
                    Numerator = numerator, 
                    Denominator = denominator, 
                    Result = result 
                });
            }
            catch (DivideByZeroException)
            {
                // Log the error to console
                Console.WriteLine("Division by zero is not allowed");
                _logger.LogError("Division by zero attempted with numerator {Numerator} and denominator {Denominator}", numerator, denominator);
                
                // Return a bad request response
                return BadRequest(new
                {
                    Error = "Division by zero is not allowed",
                    Message = "Please provide a non-zero denominator"
                });
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                Console.WriteLine($"Unexpected error: {ex.Message}");
                _logger.LogError(ex, "An unexpected error occurred while processing division with numerator {Numerator} and denominator {Denominator}", numerator, denominator);
                return StatusCode(500, new { 
                    Error = "An unexpected error occurred",
                    Message = "Please try again later"
                });
            }
        }
    }
}