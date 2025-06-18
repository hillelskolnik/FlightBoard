using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightBoardSystem.Controllers;
using FlightBoardSystem.Models;
using FlightBoardSystem.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FlightBoard.Tests.Controllers
{
    public class FlightsControllerTests
    {
        private readonly Mock<IFlightService> _mockFlightService;
        private readonly FlightsController _controller;

        public FlightsControllerTests()
        {
            _mockFlightService = new Mock<IFlightService>();
            _controller = new FlightsController(_mockFlightService.Object);
        }

        [Fact]
        public async Task GetFlights_NoFilters_ShouldReturnOkWithAllFlights()
        {
            // Arrange
            var expectedFlights = new List<FlightDto>
            {
                new FlightDto
                {
                    Id = 1,
                    FlightNumber = "TEST001",
                    Destination = "London",
                    DepartureTime = DateTime.Now.AddHours(2),
                    Gate = "A1"
                },
                new FlightDto
                {
                    Id = 2,
                    FlightNumber = "TEST002",
                    Destination = "Paris",
                    DepartureTime = DateTime.Now.AddHours(3),
                    Gate = "B2"
                }
            };

            _mockFlightService
                .Setup(x => x.GetAllFlightsAsync(null, null))
                .ReturnsAsync(expectedFlights);

            // Act
            var result = await _controller.GetFlights();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var flights = okResult.Value.Should().BeAssignableTo<IEnumerable<FlightDto>>().Subject;
            flights.Should().HaveCount(2);

            _mockFlightService.Verify(x => x.GetAllFlightsAsync(null, null), Times.Once);
        }

        [Fact]
        public async Task GetFlights_WithFilters_ShouldCallServiceWithCorrectParameters()
        {
            // Arrange
            var status = "Boarding";
            var destination = "Tel Aviv";

            _mockFlightService
                .Setup(x => x.GetAllFlightsAsync(status, destination))
                .ReturnsAsync(new List<FlightDto>());

            // Act
            await _controller.GetFlights(status, destination);

            // Assert
            _mockFlightService.Verify(x => x.GetAllFlightsAsync(status, destination), Times.Once);
        }

        [Fact]
        public async Task AddFlight_ValidFlight_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateFlightDto
            {
                FlightNumber = "NEW001",
                Destination = "Rome",
                DepartureTime = DateTime.Now.AddHours(5),
                Gate = "C3"
            };

            var expectedFlight = new FlightDto
            {
                Id = 1,
                FlightNumber = createDto.FlightNumber,
                Destination = createDto.Destination,
                DepartureTime = createDto.DepartureTime,
                Gate = createDto.Gate
            };

            _mockFlightService
                .Setup(x => x.AddFlightAsync(It.IsAny<CreateFlightDto>()))
                .ReturnsAsync(expectedFlight);

            // Act
            var result = await _controller.AddFlight(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetFlights));
            createdResult.Value.Should().BeEquivalentTo(expectedFlight);
        }

        [Theory]
        [InlineData("", "Destination", "A1")] // Empty flight number
        [InlineData("FL001", "", "A1")] // Empty destination
        [InlineData("FL001", "Destination", "")] // Empty gate
        public async Task AddFlight_MissingRequiredFields_ShouldReturnBadRequest(
            string flightNumber, string destination, string gate)
        {
            // Arrange
            var createDto = new CreateFlightDto
            {
                FlightNumber = flightNumber,
                Destination = destination,
                DepartureTime = DateTime.Now.AddHours(5),
                Gate = gate
            };

            // Act
            var result = await _controller.AddFlight(createDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();

            // Service should not be called
            _mockFlightService.Verify(x => x.AddFlightAsync(It.IsAny<CreateFlightDto>()), Times.Never);
        }

        [Fact]
        public async Task AddFlight_PastDepartureTime_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new CreateFlightDto
            {
                FlightNumber = "PAST001",
                Destination = "Athens",
                DepartureTime = DateTime.Now.AddHours(-1), // Past time
                Gate = "D4"
            };

            // Act
            var result = await _controller.AddFlight(createDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Departure time must be in the future");
        }

        [Fact]
        public async Task AddFlight_DuplicateFlightNumber_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new CreateFlightDto
            {
                FlightNumber = "DUP001",
                Destination = "Vienna",
                DepartureTime = DateTime.Now.AddHours(5),
                Gate = "E5"
            };

            _mockFlightService
                .Setup(x => x.AddFlightAsync(It.IsAny<CreateFlightDto>()))
                .ThrowsAsync(new InvalidOperationException("Flight number must be unique"));

            // Act
            var result = await _controller.AddFlight(createDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Flight number must be unique");
        }

        [Fact]
        public async Task DeleteFlight_ExistingFlight_ShouldReturnNoContent()
        {
            // Arrange
            var flightId = 1;
            _mockFlightService
                .Setup(x => x.DeleteFlightAsync(flightId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteFlight(flightId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockFlightService.Verify(x => x.DeleteFlightAsync(flightId), Times.Once);
        }

        [Fact]
        public async Task DeleteFlight_NonExistingFlight_ShouldReturnNotFound()
        {
            // Arrange
            var flightId = 999;
            _mockFlightService
                .Setup(x => x.DeleteFlightAsync(flightId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteFlight(flightId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
