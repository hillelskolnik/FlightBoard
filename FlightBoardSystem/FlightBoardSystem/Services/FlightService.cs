using FlightBoardSystem.Models;
using System.Collections.Concurrent;

namespace FlightBoardSystem.Services
{
    public class FlightService : IFlightService
    {
        private static readonly ConcurrentDictionary<int, Flight> _flights = new();
        private static int _nextId = 1;

        public Task<IEnumerable<FlightDto>> GetAllFlightsAsync(string? status = null, string? destination = null)
        {
            var flights = _flights.Values.Select(f => new FlightDto
            {
                Id = f.Id,
                FlightNumber = f.FlightNumber,
                Destination = f.Destination,
                DepartureTime = f.DepartureTime,
                Gate = f.Gate
            });

            // Filter by destination
            if (!string.IsNullOrWhiteSpace(destination))
            {
                flights = flights.Where(f => f.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by status (calculated client-side, but we'll filter based on time here)
            if (!string.IsNullOrWhiteSpace(status))
            {
                var now = GetIsraelTime();
                flights = flights.Where(f =>
                {
                    var diff = (f.DepartureTime - now).TotalMinutes;
                    var calculatedStatus = GetFlightStatus(diff);
                    return calculatedStatus.Equals(status, StringComparison.OrdinalIgnoreCase);
                });
            }

            return Task.FromResult(flights);
        }

        private DateTime GetIsraelTime()
        {
            TimeZoneInfo israelTimeZone;
            try
            {
                // נסה Windows time zone ID
                israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            }
            catch
            {
                try
                {
                    // נסה Linux/Mac time zone ID
                    israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jerusalem");
                }
                catch
                {
                    // אם כלום לא עובד, תחזיר UTC + 3
                    return DateTime.UtcNow.AddHours(3);
                }
            }

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
        }

        public Task<FlightDto?> GetFlightByIdAsync(int id)
        {
            if (_flights.TryGetValue(id, out var flight))
            {
                return Task.FromResult<FlightDto?>(new FlightDto
                {
                    Id = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    Destination = flight.Destination,
                    DepartureTime = flight.DepartureTime,
                    Gate = flight.Gate
                });
            }
            return Task.FromResult<FlightDto?>(null);
        }

        public Task<FlightDto> AddFlightAsync(CreateFlightDto flightDto)
        {
            // Check if flight number already exists
            if (_flights.Values.Any(f => f.FlightNumber.Equals(flightDto.FlightNumber, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Flight number must be unique");
            }

            var flight = new Flight
            {
                Id = _nextId++,
                FlightNumber = flightDto.FlightNumber,
                Destination = flightDto.Destination,
                DepartureTime = flightDto.DepartureTime,
                Gate = flightDto.Gate
            };

            _flights.TryAdd(flight.Id, flight);

            return Task.FromResult(new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                Gate = flight.Gate
            });
        }

        public Task<bool> DeleteFlightAsync(int id)
        {
            return Task.FromResult(_flights.TryRemove(id, out _));
        }

        private string GetFlightStatus(double diffInMinutes)
        {
            if (diffInMinutes > 30) return "Scheduled";
            if (diffInMinutes > 10) return "Boarding";
            if (diffInMinutes >= -60) return "Departed";
            if (diffInMinutes < -60) return "Landed";
            if (diffInMinutes < -15) return "Delayed";
            return "Scheduled";
        }
    }

}
