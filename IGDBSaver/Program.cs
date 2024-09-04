using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using CsvHelper;

namespace SteamDB;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Введите путь до CSV-файла:");
        var csvFilePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(csvFilePath) || !File.Exists(csvFilePath))
        {
            Console.WriteLine("Указанный файл не найден. Проверьте путь и попробуйте снова.");
            return;
        }

        Console.WriteLine(
            "Введите путь для сохранения JSON-файла (например, D:\\steamdb-main\\games_may2024_cleaned.json):");
        var jsonFilePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(jsonFilePath))
        {
            Console.WriteLine("Некорректный путь для сохранения JSON-файла.");
            return;
        }

        try
        {
            var jsonRecords = new List<object>();

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>().ToList();

                Parallel.ForEach(records, record =>
                {
                    var filteredRecord = new
                    {
                        AppID = Convert.ToInt64(record.AppID),
                        Cost = Convert.ToDouble(record.price, CultureInfo.InvariantCulture),
                        Name = Convert.ToString(record.name),
                        ReleaseDate = Convert.ToDateTime(record.release_date).ToString("yyyy-MM-dd"),
                        ShortDescription = Convert.ToString(record.short_description),
                        HeaderImage = Convert.ToString(record.header_image),
                        SupportedLanguages = ConvertStringToJsonArray(record.supported_languages),
                        Genres = ConvertStringToJsonArray(record.genres),
                        SteamScore = Convert.ToInt32(record.pct_pos_total),
                        MetacriticScore = Convert.ToInt32(record.metacritic_score)
                    };

                    lock (jsonRecords)
                    {
                        jsonRecords.Add(filteredRecord);
                    }
                });
            }

            var json = JsonSerializer.Serialize(jsonRecords, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(jsonFilePath, json);
            Console.WriteLine("CSV файл успешно преобразован в JSON и сохранен по адресу: " + jsonFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка при преобразовании: " + ex.Message);
        }
    }

    private static JsonArray ConvertStringToJsonArray(string csvString)
    {
        if (string.IsNullOrEmpty(csvString)) return new JsonArray();
        var cleanedString = csvString.Trim();
        cleanedString = cleanedString.Replace("'", "\"");

        if (!cleanedString.StartsWith("[")) cleanedString = "[" + cleanedString;
        if (!cleanedString.EndsWith("]")) cleanedString = cleanedString + "]";

        try
        {
            using (var doc = JsonDocument.Parse(cleanedString))
            {
                var jsonArray = new JsonArray();
                foreach (var element in doc.RootElement.EnumerateArray()) jsonArray.Add(element.Clone());
                return jsonArray;
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Ошибка при парсинге JSON: {ex.Message}");
            return new JsonArray();
        }
    }
}