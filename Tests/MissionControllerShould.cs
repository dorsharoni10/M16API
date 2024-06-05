using AlignAPI.BussinessLogic;
using AlignAPI.Controllers;
using AlignAPI.Exceptions;
using AlignAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AlignAPI.Tests
{
    public class MissionControllerShould
    {
        private readonly ILogger<MI6Controller> _logger;
        private readonly Mock<IM16Utils> _m16UtilsMock;

        public MissionControllerShould()
        {
            _logger = new Mock<ILogger<MI6Controller>>().Object;
            _m16UtilsMock = new Mock<IM16Utils>();
        }

        [Fact]
        public async Task AddValidMissionAndReturnsOk()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB1").Options;
            var controller = new MI6Controller(new M16DB(options), _logger, _m16UtilsMock.Object);
            var mission = new Mission { Agent = "Agent", Country = "TestCountry", Date = DateTime.Now.ToString(), Address = "Derech Yitzhak Rabin 1, Petah Tikva" };

            var result = await controller.AddMission(mission);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ThrowsInvalidDateException()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB2").Options;
            var controller = new MI6Controller(new M16DB(options), _logger, _m16UtilsMock.Object);
            var mission = new Mission();
            _m16UtilsMock.Setup(m => m.ValidateAndParseDateTime(It.IsAny<string>()))
                        .Throws(new FormatException());

            await Assert.ThrowsAsync<InvalidDateException>(() => controller.AddMission(mission));
        }

        [Fact]
        public async Task ThrowsInvalidSaveException()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB3").Options;
            var controller = new MI6Controller(new M16DB(options), _logger, _m16UtilsMock.Object);
            var mission = new Mission { 
                Date = DateTime.Now.ToString()
            };

            await Assert.ThrowsAsync<InvalidSaveException>(() => controller.AddMission(mission));
        }
    }
}
