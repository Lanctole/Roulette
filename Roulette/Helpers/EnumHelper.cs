using Roulette.Models;
using System.ComponentModel;
using System.Reflection;

namespace Roulette.Helpers
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static List<LabeledValue> GetEnumValuesWithDescriptions(Type enumType)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException("Type must be an enum");

            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new LabeledValue(e.ToString(), GetEnumDescription(e)))
                .ToList();
        }
    }
}
