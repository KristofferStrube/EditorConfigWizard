using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Models.Options;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Wizard
{
    CodeStyleRule? currentCodeStyleRule = null;
    List<string> optionChoices = new();
    List<CodeStyleRule> codeStyleRules = new();
    string fadeClass;
    string text = "Hey";
    int index = -1;

    [Inject]
    public EditorConfigService ConfigService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        codeStyleRules = await ConfigService.GetRules();
    }

    async Task Increment()
    {
        await ChangeAsync(() =>
        {
            index++;
            currentCodeStyleRule = codeStyleRules[index];
            optionChoices = currentCodeStyleRule.Options.Select(o => "").ToList();
        });
    }

    async Task ChangeAsync(Action action)
    {
        fadeClass = "fade-out";
        StateHasChanged();
        await Task.Delay(500);
        action();
        fadeClass = "faded-in";
        StateHasChanged();
        fadeClass = "fade-in";
        StateHasChanged();
        await Task.Delay(500);
    }
}