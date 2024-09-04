using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Roulette.Helpers
{
    public class JsonListConverterHelper : ValueConverter<List<string>, string>
    {
        public JsonListConverterHelper() : base(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null))
        {
        }
    }
}
