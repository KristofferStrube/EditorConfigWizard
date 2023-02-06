namespace KristofferStrube.EditorConfigWizard.Models;

public record CodeStyleCategory(string Title, string Description, List<CodeStyleRule> CodeStyleRules)
{
    public bool Used { get; set; }
}