using Roulette.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Roulette.Models
{
    public class BugReport
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Тема не может превышать 100 символов.")]
        public string Title { get; set; }

        [Required]
        [StringLength(10000, ErrorMessage = "Описание не может превышать 10000 символов.")]
        public string Description { get; set; }
        
        public string? UserEmail { get; set; }
        public BugReportStatus Status { get; set; } = BugReportStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
