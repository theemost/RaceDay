using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class EditEventViewModel
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public int EventTypeId { get; set; }
    }
}