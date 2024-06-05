using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AlignAPI.Models
{
    public class LocationAddress
    {
        [JsonPropertyName("target-location")]
        [Required(ErrorMessage = "target-location is required")]
        public string TargetLocation { set; get; }
    }
}
