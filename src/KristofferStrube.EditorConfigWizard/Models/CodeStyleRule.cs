using KristofferStrube.EditorConfigWizard.JsonConverters;
using System.Text.Json.Serialization;

namespace KristofferStrube.EditorConfigWizard.Models;

public class CodeStyleRule
{

    public string Id { get; set; } = "";

    public string Title { get; set; } = "";

    [JsonConverter(typeof(MultilineStringConverter))]
    public string Sample { get; set; } = "";

    [JsonConverter(typeof(MultilineStringConverter))]
    public string FixedSample { get; set; } = "";

    public List<RuleOption> Options { get; set; } = new();
}
