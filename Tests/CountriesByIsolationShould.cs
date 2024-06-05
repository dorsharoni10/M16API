using AlignAPI.BussinessLogic;
using AlignAPI.Controllers;
using AlignAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AlignAPI.Tests
{
    public class CountriesByIsolationShould
    {
        private readonly Mock<ILogger<MI6Controller>> _mockLogger;
        private readonly Mock<IM16Utils> _m16UtilsMock;

        public CountriesByIsolationShould()
        {
            _mockLogger = new Mock<ILogger<MI6Controller>>();
            _m16UtilsMock = new Mock<IM16Utils>();
        }

        [Fact]
        public async Task ReturnsOkResultWithMostIsolatedCountry()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB1").Options;
            var controller = new MI6Controller(new M16DB(options), _mockLogger.Object, _m16UtilsMock.Object);
            _m16UtilsMock.Setup(m => m.GetLegalAgents(It.IsAny<List<Mission>>())).Returns(It.IsAny<HashSet<string>>);
            _m16UtilsMock.Setup(m => m.GetMostIsolationcountry(It.IsAny<List<Mission>>(), It.IsAny<HashSet<string>>())).Returns(new KeyValuePair<string, int>("Country A", 3)) ;
            var mission = new Mission { Agent = "Agent", Country = "TestCountry", Date = DateTime.Now.ToString(), Address = "Derech Yitzhak Rabin 1, Petah Tikva" };

            var result = await controller.AddMission(mission);
            result = await controller.GetCountriesByIsolation();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value as dynamic;

            Assert.Equal("Country A", responseObject?.Country);
            Assert.Equal(3, responseObject.IsolationDegreeCount);

        }

        [Fact]
        public async Task NotFoundWhenNoMissions()
        {
            var options = new DbContextOptionsBuilder<M16DB>().UseInMemoryDatabase(databaseName: "TestDB2").Options;
            var controller = new MI6Controller(new M16DB(options), _mockLogger.Object, _m16UtilsMock.Object);
            
            var result = await controller.GetCountriesByIsolation();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No missions found.", notFoundResult.Value);
        }
    }
}
