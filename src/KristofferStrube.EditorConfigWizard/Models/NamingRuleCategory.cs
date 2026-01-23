namespace KristofferStrube.EditorConfigWizard.Models;

public record NamingRuleCategory(
    string Title,
    string Description,
    List<NamingRule> NamingRules,
    bool Used = false
);
