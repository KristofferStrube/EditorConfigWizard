namespace KristofferStrube.EditorConfigWizard.Models;

public class NamingStyle
{
    public required string Capitalization { get; set; }
    public string? RequiredPrefix { get; set; }
    public string? RequiredSuffix { get; set; }
}
