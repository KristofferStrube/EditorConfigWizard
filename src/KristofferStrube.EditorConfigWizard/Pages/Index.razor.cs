using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Index
{
    private List<CodeStyleRule> codeStyleRules = new();

    private IQueryable<CodeStyleRule> FilteredCodeStyleRules => codeStyleRules
        .Where(r => r.Id.Contains(search, StringComparison.OrdinalIgnoreCase) || r.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
        .AsQueryable();

    [Inject]
    public EditorConfigService ConfigService { get; set; }

    private string search = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        codeStyleRules = (await ConfigService.LanguagesRulesAsync()).CodeStyleRules
            .Concat((await ConfigService.UnnecessaryColeRulesAsync()).CodeStyleRules)
            .ToList();
    }
}