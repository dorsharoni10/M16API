using AlignAPI.BussinessLogic;
using AlignAPI.Exceptions;
using AlignAPI.Models;
using Geocoding.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

namespace AlignAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MI6Controller : ControllerBase
    {
        private readonly M16DB _m16Db;
        private readonly ILogger<MI6Controller> _logger;
        private readonly IM16Utils _m16Utils;

        public MI6Controller(M16DB m16Db, ILogger<MI6Controller> logger, IM16Utils m16Utils)
        {
            _m16Db = m16Db;
            _logger = logger;
            _m16Utils = m16Utils;
        }

        [HttpPost("mission")]
        public async Task<IActionResult> AddMission(Mission mission)
        {
            try
            {
                mission.Date = _m16Utils.ValidateAndParseDateTime(mission.Date).ToString();               
                _m16Db.Missions.Add(mission);
                _m16Db.SaveChanges();
                _logger.LogInformation("Mission added successfully. Country: {0}, Agent: {1}, Date:{2}", mission.Country, mission.Agent, mission.Date);

                return Ok(mission);
            }
            catch (GoogleGeocodingException ex)
            {
                throw new InvalidAddressException("Failed to validate address.");
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidSaveException("An error occurred while saving the mission to the database.");
            }
            catch (FormatException ex)
            {
                throw new InvalidDateException("Date format error: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidArgumentException("Argument error: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error: " + ex.Message);
            }
        }

        [HttpGet("countries-by-isolation")]
        public async Task<IActionResult> GetCountriesByIsolation()
        {
            try
            {
                var missions = await _m16Db.Missions.ToListAsync();
                if (missions == null || !missions.Any())
                {
                    _logger.LogWarning("No missions found.");
                    return NotFound("No missions found.");
                }

                var legalAgents = _m16Utils.GetLegalAgents(missions);
                var mostIsolationcountry = _m16Utils.GetMostIsolationcountry(missions, legalAgents);

                return Ok(new {Country = mostIsolationcountry.Key, IsolationDegreeCount = mostIsolationcountry.Value});
            }
            catch (ArgumentException ex)
            {
                throw new InvalidArgumentException("Argument error: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error: " + ex.Message);
            }
        }

        [HttpPost("find-closest")]
        public async Task<IActionResult> FindClosestMissionFromSpecificAddress([FromBody] LocationAddress locationAddress)
        {
            try
            {
                var missions = await _m16Db.Missions.ToListAsync();
                if (missions == null || !missions.Any())
                {
                    _logger.LogWarning("No missions found.");
                    return NotFound("No missions found.");
                }

                var closestAddress = await _m16Utils.FindClosestLocation(locationAddress, missions);
                return Ok(closestAddress);
            }
            catch (GoogleGeocodingException ex)
            {
                throw new InvalidAddressException("Failed to validate address.");
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error: " + ex.Message);
            }
        }
    }
}
