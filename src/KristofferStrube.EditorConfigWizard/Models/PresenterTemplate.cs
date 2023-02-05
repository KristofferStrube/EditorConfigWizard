using KristofferStrube.EditorConfigWizard.JsonConverters;
using System.Text.Json.Serialization;

namespace KristofferStrube.EditorConfigWizard.Models;

public class PresenterTemplate
{
    public string[] OptionsApplying { get; set; } = default!;

    [JsonConverter(typeof(MultilineStringConverter))]
    public string Template { get; set; } = "";

    public string TemplateToken { get; set; } = "";
}

