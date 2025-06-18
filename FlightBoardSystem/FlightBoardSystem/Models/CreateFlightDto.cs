namespace FlightBoardSystem.Models
{
    public class CreateFlightDto
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string Gate { get; set; } = string.Empty;
    }
}
