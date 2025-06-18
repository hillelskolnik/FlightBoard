using FlightBoardSystem.Models;
using FlightBoardSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightBoardSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDto>>> GetFlights(
            [FromQuery] string? status = null,
            [FromQuery] string? destination = null)
        {
            var flights = await _flightService.GetAllFlightsAsync(status, destination);
            return Ok(flights);
        }

        [HttpPost]
        public async Task<ActionResult<FlightDto>> AddFlight([FromBody] CreateFlightDto flightDto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(flightDto.FlightNumber))
                return BadRequest("Flight number is required");

            if (string.IsNullOrWhiteSpace(flightDto.Destination))
                return BadRequest("Destination is required");

            if (string.IsNullOrWhiteSpace(flightDto.Gate))
                return BadRequest("Gate is required");

            if (flightDto.DepartureTime <= DateTime.Now)
                return BadRequest("Departure time must be in the future");

            try
            {
                var flight = await _flightService.AddFlightAsync(flightDto);
                return CreatedAtAction(nameof(GetFlights), new { id = flight.Id }, flight);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            var result = await _flightService.DeleteFlightAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
