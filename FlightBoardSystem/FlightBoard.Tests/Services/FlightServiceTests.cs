using FlightBoardSystem.Models;
using FlightBoardSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace FlightBoard.Tests.Services
{
    public class FlightServiceTests
    {
            private readonly FlightService _flightService;

            public FlightServiceTests()
            {
                _flightService = new FlightService();
            }

            [Fact]
            public async Task AddFlightAsync_ValidFlight_ShouldAddSuccessfully()
            {
                // Arrange
                var flightDto = new CreateFlightDto
                {
                    FlightNumber = "TEST123",
                    Destination = "Tel Aviv",
                    DepartureTime = DateTime.Now.AddHours(5),
                    Gate = "A1"
                };

                // Act
                var result = await _flightService.AddFlightAsync(flightDto);

                // Assert
                result.Should().NotBeNull();
                result.FlightNumber.Should().Be("TEST123");
                result.Destination.Should().Be("Tel Aviv");
                result.Gate.Should().Be("A1");
                result.Id.Should().BePositive();
            }

            [Fact]
            public async Task AddFlightAsync_DuplicateFlightNumber_ShouldThrowException()
            {
                // Arrange
                var flightDto = new CreateFlightDto
                {
                    FlightNumber = "DUP123",
                    Destination = "New York",
                    DepartureTime = DateTime.Now.AddHours(5),
                    Gate = "B2"
                };

                // Add first flight
                await _flightService.AddFlightAsync(flightDto);

                // Act & Assert
                await Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await _flightService.AddFlightAsync(flightDto)
                );
            }

            [Fact]
            public async Task GetAllFlightsAsync_NoFilters_ShouldReturnAllFlights()
            {
                // Arrange
                var flight1 = new CreateFlightDto
                {
                    FlightNumber = "FL001",
                    Destination = "London",
                    DepartureTime = DateTime.Now.AddHours(2),
                    Gate = "C1"
                };

                var flight2 = new CreateFlightDto
                {
                    FlightNumber = "FL002",
                    Destination = "Paris",
                    DepartureTime = DateTime.Now.AddHours(3),
                    Gate = "C2"
                };

                await _flightService.AddFlightAsync(flight1);
                await _flightService.AddFlightAsync(flight2);

                // Act
                var result = await _flightService.GetAllFlightsAsync();

                // Assert
                result.Should().HaveCountGreaterThanOrEqualTo(2);
                result.Should().Contain(f => f.FlightNumber == "FL001");
                result.Should().Contain(f => f.FlightNumber == "FL002");
            }

            [Fact]
            public async Task GetAllFlightsAsync_WithDestinationFilter_ShouldReturnFilteredFlights()
            {
                // Arrange
                var londonFlight = new CreateFlightDto
                {
                    FlightNumber = "LON001",
                    Destination = "London",
                    DepartureTime = DateTime.Now.AddHours(2),
                    Gate = "D1"
                };

                var parisFlight = new CreateFlightDto
                {
                    FlightNumber = "PAR001",
                    Destination = "Paris",
                    DepartureTime = DateTime.Now.AddHours(3),
                    Gate = "D2"
                };

                await _flightService.AddFlightAsync(londonFlight);
                await _flightService.AddFlightAsync(parisFlight);

                // Act
                var result = await _flightService.GetAllFlightsAsync(destination: "London");

                // Assert
                result.Should().Contain(f => f.Destination == "London");
                result.Should().NotContain(f => f.Destination == "Paris");
            }

            [Fact]
            public async Task GetAllFlightsAsync_WithStatusFilter_ShouldReturnCorrectFlights()
            {
                // Arrange
                var scheduledFlight = new CreateFlightDto
                {
                    FlightNumber = "SCH001",
                    Destination = "Rome",
                    DepartureTime = DateTime.Now.AddHours(40), // Will be "Scheduled"
                    Gate = "E1"
                };

                await _flightService.AddFlightAsync(scheduledFlight);

                // Act
                var result = await _flightService.GetAllFlightsAsync(status: "Scheduled");

                // Assert
                result.Should().Contain(f => f.FlightNumber == "SCH001");
            }

            [Fact]
            public async Task DeleteFlightAsync_ExistingFlight_ShouldReturnTrue()
            {
                // Arrange
                var flight = new CreateFlightDto
                {
                    FlightNumber = "DEL001",
                    Destination = "Berlin",
                    DepartureTime = DateTime.Now.AddHours(2),
                    Gate = "F1"
                };

                var addedFlight = await _flightService.AddFlightAsync(flight);

                // Act
                var result = await _flightService.DeleteFlightAsync(addedFlight.Id);

                // Assert
                result.Should().BeTrue();

                // Verify flight is deleted
                var allFlights = await _flightService.GetAllFlightsAsync();
                allFlights.Should().NotContain(f => f.Id == addedFlight.Id);
            }

            [Fact]
            public async Task DeleteFlightAsync_NonExistingFlight_ShouldReturnFalse()
            {
                // Act
                var result = await _flightService.DeleteFlightAsync(99999);

                // Assert
                result.Should().BeFalse();
            }

            [Fact]
            public async Task GetFlightByIdAsync_ExistingFlight_ShouldReturnFlight()
            {
                // Arrange
                var flight = new CreateFlightDto
                {
                    FlightNumber = "GET001",
                    Destination = "Madrid",
                    DepartureTime = DateTime.Now.AddHours(2),
                    Gate = "G1"
                };

                var addedFlight = await _flightService.AddFlightAsync(flight);

                // Act
                var result = await _flightService.GetFlightByIdAsync(addedFlight.Id);

                // Assert
                result.Should().NotBeNull();
                result!.FlightNumber.Should().Be("GET001");
                result.Destination.Should().Be("Madrid");
            }

            [Fact]
            public async Task GetFlightByIdAsync_NonExistingFlight_ShouldReturnNull()
            {
                // Act
                var result = await _flightService.GetFlightByIdAsync(99999);

                // Assert
                result.Should().BeNull();
            }
        }
    }
