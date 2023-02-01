using KristofferStrube.EditorConfigWizard.JsonConverters;
using System.Text.Json.Serialization;

namespace KristofferStrube.EditorConfigWizard.Models;

public class OptionValue
{
    public string Value { get; set; }

    public string Description { get; set; }

    [JsonConverter(typeof(OneOrMultipleConverter<string>))]
    public List<string> Sample { get; set; }
}