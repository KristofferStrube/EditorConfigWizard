using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Wizard
{
    CodeStyleRule? currentCodeStyleRule = null;
    List<string?> optionChoices = new();
    List<CodeStyleRule> codeStyleRules = new();
    string fadeClass = "";
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
            optionChoices = currentCodeStyleRule.Options.Select<RuleOption, string?>(_ => null).ToList();
        });
    }

    async Task ChangeAsync(Action action)
    {
        fadeClass = "fade-out";
        StateHasChanged();
        await Task.Delay(500);
        action();
        fadeClass = "fade-in";
        StateHasChanged();
        await Task.Delay(500);
        fadeClass = "";
        StateHasChanged();
    }

    async Task SelectOption(int choice, string value)
    {
        optionChoices[choice] = value;
        if (optionChoices.All(o => o is not null))
        {
            await Increment();
            return;
        }
        StateHasChanged();
    }
}