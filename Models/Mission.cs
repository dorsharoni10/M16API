using System.ComponentModel.DataAnnotations;

namespace AlignAPI.Models
{
    public class Mission
    {
        public int Id { set; get; }

        [Required(ErrorMessage = "Agent is required")]
        public string Agent { set; get; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { set; get; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { set; get; }

        [Required(ErrorMessage = "Date and time are required")]
        public string Date { set; get; }
    }
}
