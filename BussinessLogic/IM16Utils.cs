using AlignAPI.Models;
using Geocoding.Google;

namespace AlignAPI.BussinessLogic
{
    public interface IM16Utils
    {
        HashSet<string> GetLegalAgents(List<Mission> missions);
        KeyValuePair<string, int> GetMostIsolationcountry(List<Mission> missions, HashSet<string> legalAgents);
        Task<string> FindClosestLocation(LocationAddress locationAddress, List<Mission> missions);
        Task<GoogleAddress> GetAndValidateAddressFromGoogle(string address);
        DateTime ValidateAndParseDateTime(string date);
    }
}
