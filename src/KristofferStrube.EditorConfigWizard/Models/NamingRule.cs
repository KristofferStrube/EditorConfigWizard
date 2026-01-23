namespace KristofferStrube.EditorConfigWizard.Models;

public class NamingRule
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string SymbolKinds { get; set; }
    public required string Accessibilities { get; set; }
    public string? RequiredModifiers { get; set; }
    public required NamingStyle NamingStyle { get; set; }
    public required string DefaultSeverity { get; set; }
    public required NamingExamples Examples { get; set; }
}

public class NamingExamples
{
    public required string Correct { get; set; }
    public required string Incorrect { get; set; }
}
