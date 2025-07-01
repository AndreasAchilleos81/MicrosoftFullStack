using Microsoft.AspNetCore.Mvc;
using WebApiFromConsole.Models;

namespace WebApiFromConsole.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast weatherForecast)
        {
            if (weatherForecast == null)
            {
                return BadRequest("Weather forecast cannot be null.");
            }
            return Ok(weatherForecast);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WeatherForecast weatherForecast)
        {
            if (weatherForecast == null || weatherForecast.Date == default)
            {
                return BadRequest("Invalid weather forecast data.");
            }
            // Here you would typically update the weather forecast in a database or in-memory store
            return Ok($"Updated weather forecast with ID {id} to {weatherForecast}");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Here you would typically delete the weather forecast from a database or in-memory store
            // return Ok($"Deleted weather forecast with ID {id}");
            return NoContent(); // No content to return after deletion
        } 
    }
}

