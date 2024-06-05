using AlignAPI.BussinessLogic;
using AlignAPI.Models;
using Moq;
using Xunit;

namespace AlignAPI.Tests
{
    public class M16UtilsShould
    {
        private readonly Mock<IM16Utils> _m16UtilsMock;
        private readonly List<Mission> _missions;

        public M16UtilsShould()
        {
            _m16UtilsMock = new Mock<IM16Utils>();

            _missions = new List<Mission>
            {
                new Mission { Agent = "AgentA", Country = "Brazil", Address = "Address1" },
                new Mission { Agent = "AgentB", Country = "Brazil", Address = "Address2" },
                new Mission { Agent = "AgentA", Country = "Polland", Address = "Address3" },
                new Mission { Agent = "AgentC", Country = "Polland", Address = "Address4" },
                new Mission { Agent = "AgentD", Country = "Polland", Address = "Address5" }
            };
        }

        [Fact]
        public void GetLegalAgentsReturnsLegalAgents()
        {
            var expectedLegalAgents = new HashSet<string> { "AgentB", "AgentC", "AgentD" };
            _m16UtilsMock.Setup(m => m.GetLegalAgents(_missions)).Returns(expectedLegalAgents);

            var result = _m16UtilsMock.Object.GetLegalAgents(_missions);

            Assert.Equal(expectedLegalAgents, result);
        }

        [Fact]
        public void GetMostIsolationcountryReturnsCountryWithMostIsolation()
        {
            var legalAgents = new HashSet<string> { "AgentA", "AgentB" };
            var expectedCountry = "Polland";
            _m16UtilsMock.Setup(m => m.GetMostIsolationcountry(_missions, legalAgents)).Returns(new KeyValuePair<string, int>(expectedCountry, 3));

            var result = _m16UtilsMock.Object.GetMostIsolationcountry(_missions, legalAgents);

            Assert.Equal(expectedCountry, result.Key);
        }

        [Fact]
        public async Task FindClosestLocationReturnsClosestLocation()
        {
            var locationAddress = new LocationAddress { TargetLocation = "Derech Yitzhak Rabin 1, Petah Tikva" };
            var expectedLocation = "Address5";
            _m16UtilsMock.Setup(m => m.FindClosestLocation(locationAddress, _missions)).ReturnsAsync(expectedLocation);

            var result = await _m16UtilsMock.Object.FindClosestLocation(locationAddress, _missions);

            Assert.Equal(expectedLocation, result);
        }

        [Fact]
        public async Task SentInvalidAddressToGoogleAndReturnExeption()
        {
            var address = string.Empty;
            _m16UtilsMock.Setup(m => m.GetAndValidateAddressFromGoogle(It.IsAny<string>()))
                                .ThrowsAsync(new Exception());

            await Assert.ThrowsAsync<Exception>(() => _m16UtilsMock.Object.GetAndValidateAddressFromGoogle(address));
        }

        [Fact]
        public void ValidateAndParseDateTime_ValidFormat_ReturnsDateTime()
        {
            var dateTimeNow = DateTime.Now;
            _m16UtilsMock.Setup(m => m.ValidateAndParseDateTime(It.IsAny<string>()))
                        .Throws(new FormatException());
            
            Assert.Throws<FormatException>(() => _m16UtilsMock.Object.ValidateAndParseDateTime("Invalid"));
        }
    }
}
