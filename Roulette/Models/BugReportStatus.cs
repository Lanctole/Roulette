using System.ComponentModel.DataAnnotations;

namespace Roulette.Models;

/// <summary>
/// Статусы отчёта о баге, используемые для отслеживания этапов обработки.
/// </summary>
public enum BugReportStatus
{
    [Display(Name = "Новое")]
    New,

    [Display(Name = "В процессе")]
    InProgress,

    [Display(Name = "Решено")]
    Resolved,

    [Display(Name = "Отложено")]
    Deferred
}