using System.ComponentModel.DataAnnotations;

namespace Roulette.Models
{
    public enum BugReportStatus
    {
        [Display(Name = "Новое")]
        New,

        [Display(Name = "В процессе")]
        InProgress,

        [Display(Name = "Решёно")]
        Resolved,

        [Display(Name = "Отложено")]
        Deferred
    }
}
