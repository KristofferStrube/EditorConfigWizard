using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.Json.JsonSerializer;

namespace KristofferStrube.EditorConfigWizard.JsonConverters;

public class MultilineStringConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            if (doc.RootElement.ValueKind is JsonValueKind.Array)
            {
                return string.Join("\n", doc.RootElement.EnumerateArray().Select(element => element.Deserialize<string>()));
            }
            return doc.Deserialize<string>()!;
        }
        throw new JsonException("Could not be parsed as a JsonDocument.");
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        string[] lineSplittetString = value.Split('\n');
        if (lineSplittetString.Length is 1)
        {
            writer.WriteRawValue(Serialize(value, options));
        }
        else if (value is not null)
        {
            writer.WriteStartArray();
            foreach (string element in lineSplittetString)
            {
                writer.WriteRawValue(Serialize(element, options));
            }
            writer.WriteEndArray();
        }
    }
}
