using FlightBoardSystem.Models;

namespace FlightBoardSystem.Services
{
    public interface IFlightService
    {
        Task<IEnumerable<FlightDto>> GetAllFlightsAsync(string? status = null, string? destination = null);
        Task<FlightDto?> GetFlightByIdAsync(int id);
        Task<FlightDto> AddFlightAsync(CreateFlightDto flightDto);
        Task<bool> DeleteFlightAsync(int id);
    }

}
