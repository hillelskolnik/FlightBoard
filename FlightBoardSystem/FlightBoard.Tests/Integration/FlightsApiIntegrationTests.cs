using System.Net;
using System.Net.Http.Json;
using FlightBoardSystem.Models;
using FlightBoardSystem;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FlightBoard.Tests.Integration
{
    public class FlightsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public FlightsApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetFlights_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/flights");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task AddFlight_ValidData_ShouldCreateFlight()
        {
            // Arrange
            var newFlight = new CreateFlightDto
            {
                FlightNumber = $"INT{Guid.NewGuid().ToString().Substring(0, 5)}",
                Destination = "Integration Test City",
                DepartureTime = DateTime.Now.AddHours(6),
                Gate = "Z99"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/flights", newFlight);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var createdFlight = await response.Content.ReadFromJsonAsync<FlightDto>();
            createdFlight.Should().NotBeNull();
            createdFlight!.FlightNumber.Should().Be(newFlight.FlightNumber);
        }

        [Fact]
        public async Task AddFlight_InvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidFlight = new CreateFlightDto
            {
                FlightNumber = "", // Invalid - empty
                Destination = "Test",
                DepartureTime = DateTime.Now.AddHours(1),
                Gate = "A1"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/flights", invalidFlight);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteFlight_ExistingFlight_ShouldReturnNoContent()
        {
            // Arrange - First create a flight
            var newFlight = new CreateFlightDto
            {
                FlightNumber = $"DEL{Guid.NewGuid().ToString().Substring(0, 5)}",
                Destination = "Delete Test",
                DepartureTime = DateTime.Now.AddHours(4),
                Gate = "X1"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/flights", newFlight);
            var createdFlight = await createResponse.Content.ReadFromJsonAsync<FlightDto>();

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/flights/{createdFlight!.Id}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await _client.GetAsync("/api/flights");
            var flights = await getResponse.Content.ReadFromJsonAsync<FlightDto[]>();
            flights.Should().NotContain(f => f.Id == createdFlight.Id);
        }

        [Fact]
        public async Task GetFlights_WithFilters_ShouldReturnFilteredResults()
        {
            // Arrange - Create test flights
            var testDestination = "FilterTest" + Guid.NewGuid().ToString().Substring(0, 5);

            var flight1 = new CreateFlightDto
            {
                FlightNumber = "FLT001",
                Destination = testDestination,
                DepartureTime = DateTime.Now.AddHours(35), // Will be "Scheduled"
                Gate = "T1"
            };

            var flight2 = new CreateFlightDto
            {
                FlightNumber = "FLT002",
                Destination = "OtherCity",
                DepartureTime = DateTime.Now.AddHours(35),
                Gate = "T2"
            };

            await _client.PostAsJsonAsync("/api/flights", flight1);
            await _client.PostAsJsonAsync("/api/flights", flight2);

            // Act
            var response = await _client.GetAsync($"/api/flights?destination={testDestination}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var flights = await response.Content.ReadFromJsonAsync<FlightDto[]>();
            flights.Should().Contain(f => f.Destination == testDestination);
            flights.Should().NotContain(f => f.Destination == "OtherCity");
        }
    }
}
