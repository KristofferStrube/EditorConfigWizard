namespace KristofferStrube.EditorConfigWizard.Models;

public class WizardState
{
    public int Progress { get; set; }
    public int CodeRuleIndex { get; set; }
    public int RuleOptionIndex { get; set; }
    public Dictionary<string, string> RuleOptionChoices { get; set; } = new();
    public Dictionary<string, string> Severities { get; set; } = new();
    public List<string> SelectedCategoryTitles { get; set; } = new();
    public DateTime SavedAt { get; set; }
}
