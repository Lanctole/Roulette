using Roulette.Models;
using System.ComponentModel;
using System.Reflection;

namespace Roulette.Helpers;

public static class EnumHelper
{
    /// <summary>
    /// Получает описание для значения перечисления, используя атрибут <see cref="DescriptionAttribute"/>.
    /// </summary>
    /// <param name="value">Значение перечисления для получения описания.</param>
    /// <returns>Описание значения перечисления, если атрибут <see cref="DescriptionAttribute"/> присутствует; иначе возвращает строковое представление значения.</returns>
    /// <exception cref="ArgumentNullException">Если переданное значение перечисления равно <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">Если значение перечисления не имеет соответствующего имени поля.</exception>
    public static string GetEnumDescription(Enum value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var fieldName = value.ToString();
        var field = value.GetType().GetField(fieldName);
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? fieldName;
    }

    /// <summary>
    /// Получает список значений и их описаний для указанного перечисления.
    /// </summary>
    /// <param name="enumType">Тип перечисления.</param>
    /// <returns>Список значений и их описаний.</returns>
    /// <exception cref="ArgumentException">Если указанный тип не является перечислением.</exception>
    public static List<LabeledValue> GetEnumValuesWithDescriptions(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type must be an enum", nameof(enumType));

        return Enum.GetValues(enumType)
            .Cast<Enum>()
            .Select(e => new LabeledValue(e.ToString(), GetEnumDescription(e)))
            .ToList();
    }
}