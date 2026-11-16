using System.ComponentModel.DataAnnotations;

namespace RaceDay.MVC.Models
{
    public class CreateCategoryViewModel
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int EventId { get; set; }
    }
}