using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Index
{
    private List<CodeStyleRule> codeStyleRules = new();

    [Inject]
    public EditorConfigService ConfigService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        codeStyleRules = await ConfigService.GetRules();
    }
}