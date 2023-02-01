using System.Text.Json.Serialization;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace KristofferStrube.EditorConfigWizard.JsonConverters;

public class OneOrMultipleConverter<T> : JsonConverter<List<T>?>
{
    public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out JsonDocument? doc))
        {
            if (doc.RootElement.ValueKind is JsonValueKind.Array)
            {
                return doc.RootElement.EnumerateArray().Select(element => element.Deserialize<T>()!).ToList();
            }
            return new List<T>() { doc.Deserialize<T>()! };
        }
        throw new JsonException("Could not be parsed as a JsonDocument.");
    }

    public override void Write(Utf8JsonWriter writer, List<T>? value, JsonSerializerOptions options)
    {
        if (value?.Count() is 1)
        {
            writer.WriteRawValue(Serialize(value.First(), options));
        }
        else if (value is not null)
        {
            writer.WriteStartArray();
            foreach (T element in value)
            {
                writer.WriteRawValue(Serialize(element, options));
            }
            writer.WriteEndArray();
        }
    }
}
