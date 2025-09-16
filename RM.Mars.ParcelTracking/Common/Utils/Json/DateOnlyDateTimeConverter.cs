using System.Text.Json;
using System.Text.Json.Serialization;

namespace RM.Mars.ParcelTracking.Common.Utils.Json;

/// <summary>
/// Serializes DateTime as date-only (yyyy-MM-dd).
/// </summary>
public sealed class DateOnlyDateTimeConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }
        if (DateTime.TryParse(value, out DateTime parsed))
        {
            return DateTime.SpecifyKind(parsed.Date, DateTimeKind.Unspecified);
        }
        throw new JsonException($"Invalid date format. Expected '{Format}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Date.ToString(Format));
    }
}
