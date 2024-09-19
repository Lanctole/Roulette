using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Roulette.Attributes;

/// <summary>
///     Атрибут для проверки корректности формата email-адреса.
/// </summary>
public class CustomEmailAttribute : ValidationAttribute
{
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    /// <summary>
    ///     Проверяет корректность формата email-адреса.
    /// </summary>
    /// <param name="value">Значение для проверки.</param>
    /// <param name="validationContext">Контекст проверки.</param>
    /// <returns>
    ///     Результат проверки. Если формат некорректен, возвращается сообщение об ошибке. В противном случае возвращается
    ///     успешный результат.
    /// </returns>
    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        var email = value as string;

        if (email != null && !Regex.IsMatch(email, EmailPattern))
            return new ValidationResult("Некорректный формат email.");

        return ValidationResult.Success;
    }
}