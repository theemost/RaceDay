using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateResultViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int EnrolmentId { get; set; }

        [Required]
        public string FinishTime { get; set; } = string.Empty;
    }
}