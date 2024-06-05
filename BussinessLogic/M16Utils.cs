using AlignAPI.Models;
using Geocoding;
using Geocoding.Google;

namespace AlignAPI.BussinessLogic
{
    public class M16Utils : IM16Utils
    {
        private readonly GoogleGeocoder _geocoder;

        public M16Utils()
        {
            _geocoder = new GoogleGeocoder("Write your API key here");
        }

        public HashSet<string> GetLegalAgents(List<Mission> missions)
        {
            return missions
                    .GroupBy(mission => mission.Agent)
                    .ToDictionary(agent => agent.Key, agent => agent.Count())
                    .Where(agentForOneCountry => agentForOneCountry.Value == 1)
                    .Select(agentForOneCountry => agentForOneCountry.Key)
                    .ToHashSet();
        }

        public KeyValuePair<string, int> GetMostIsolationcountry(List<Mission> missions, HashSet<string> legalAgents)
        {
            return missions
                    .Where(mission => legalAgents.Contains(mission.Agent))
                    .GroupBy(m => m.Country)
                    .ToDictionary(countries => countries.Key, countries => countries.Count())
                    .OrderByDescending(countriesCount => countriesCount.Value)
                    .FirstOrDefault();
        }

        public async Task<string> FindClosestLocation(LocationAddress locationAddress, List<Mission> missions)
        {
            var minDistance = double.MaxValue;
            var closestLocation = string.Empty;

            var targetLocation = await GetAndValidateAddressFromGoogle(locationAddress.TargetLocation);

            foreach (var mission in missions)
            {
                var missionLocation = (await _geocoder.GeocodeAsync(mission.Address)).FirstOrDefault();
                if (missionLocation == null) continue;

                var currentDistance = CalculateDistanceBlackBox(targetLocation.Coordinates, missionLocation.Coordinates);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestLocation = mission.Address;
                }
            }

            return closestLocation;
        }

        public async Task<GoogleAddress> GetAndValidateAddressFromGoogle(string address)
        {
            var targetLocation = (await _geocoder.GeocodeAsync(address)).FirstOrDefault();
            
            return targetLocation == null
                ? throw new GoogleGeocodingException(new Exception("Failed to get the target location."))
                : targetLocation;
        }

        public DateTime ValidateAndParseDateTime(string date)
        {
            if (!DateTime.TryParse(date, out var parsedDate))
                throw new FormatException("Invalid date format. Please use valid format.");

            return parsedDate;
        }

        private static double CalculateDistanceBlackBox(Location point1, Location point2)
        {
            double R = 6371;
            double lat = (point2.Latitude - point1.Latitude) * Math.PI / 180;
            double lon = (point2.Longitude - point1.Longitude) * Math.PI / 180;
            double a = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                       Math.Cos(point1.Latitude * Math.PI / 180) * Math.Cos(point2.Latitude * Math.PI / 180) *
                       Math.Sin(lon / 2) * Math.Sin(lon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
