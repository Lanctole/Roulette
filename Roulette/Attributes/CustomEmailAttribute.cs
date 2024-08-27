namespace Roulette.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    public class CustomEmailAttribute : ValidationAttribute
    {
        private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (!Regex.IsMatch(email, EmailPattern))
            {
                return new ValidationResult("Некорректный формат email.");
            }

            return ValidationResult.Success;
        }
    }

}
