using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateEnrolmentViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int EventId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }
}