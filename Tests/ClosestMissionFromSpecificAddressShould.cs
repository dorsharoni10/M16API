using AlignAPI.BussinessLogic;
using AlignAPI.Controllers;
using AlignAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AlignAPI.Tests
{
    public class ClosestMissionFromSpecificAddressShould
    {
        private readonly Mock<ILogger<MI6Controller>> _mockLogger;
        private readonly Mock<IM16Utils> _m16UtilsMock;

        public ClosestMissionFromSpecificAddressShould()
        {
            _mockLogger = new Mock<ILogger<MI6Controller>>();
            _m16UtilsMock = new Mock<IM16Utils>();
        }

        [Fact]
        public async Task ReturnClosestAddress()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB2").Options;
            var controller = new MI6Controller(new M16DB(options), _mockLogger.Object, _m16UtilsMock.Object);
            _m16UtilsMock.Setup(x => x.FindClosestLocation(It.IsAny<LocationAddress>(), It.IsAny<List<Mission>>()))
                                 .ReturnsAsync("Test closest address");
            var mission = new Mission { Agent = "Agent", Country = "TestCountry", Date = DateTime.Now.ToString(), Address = "Derech Yitzhak Rabin 1, Petah Tikva" };

            var result = await controller.AddMission(mission);
            result = await controller.FindClosestMissionFromSpecificAddress(new LocationAddress { TargetLocation = "Netanya" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCountry = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Test closest address", returnedCountry);
        }

        [Fact]
        public async Task WithNoMissionsReturnNoMissionFound()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB2").Options;
            var controller = new MI6Controller(new M16DB(options), _mockLogger.Object, _m16UtilsMock.Object);
            _m16UtilsMock.Setup(x => x.FindClosestLocation(It.IsAny<LocationAddress>(), It.IsAny<List<Mission>>()))
                                 .ReturnsAsync("Test closest address");
            
            var result = await controller.FindClosestMissionFromSpecificAddress(new LocationAddress { TargetLocation = "Netanya" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No missions found.", notFoundResult.Value);
        }
    }
}
