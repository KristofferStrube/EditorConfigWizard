using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Models.Options;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Wizard
{
    CodeStyleRule? currentCodeStyleRule = null;
    RuleOption? currentRuleOption = null;
    List<string?> optionChoices = new();
    List<CodeStyleRule> codeStyleRules = new();
    string fadeClass = "";
    int codeRuleIndex = -1;
    int ruleOptionIndex = 0;

    [Inject]
    public EditorConfigService ConfigService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        codeStyleRules = await ConfigService.GetRules();
    }

    async Task IncrementCodeRule()
    {
        await ChangeAsync(async () =>
        {
            codeRuleIndex++;
            ruleOptionIndex = 0;
            currentCodeStyleRule = codeStyleRules[codeRuleIndex];
            currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
            optionChoices = currentCodeStyleRule.Options
            .Select(o => o.ValueOptions is OrderedSetWithOneOrMoreOfManyValueOptions ? o.ValueOptions.DefaultOptionValue : null).ToList();
        });
    }
    async Task IncrementRuleOption()
    {
        await ChangeAsync(() =>
        {
            ruleOptionIndex++;
            if (ruleOptionIndex == currentCodeStyleRule.Options.Count)
            {
                currentRuleOption = null;
            }
            else
            {
                currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
            }
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
        await IncrementRuleOption();
        StateHasChanged();
    }

    async Task SetSeverity(string severity)
    {
        await IncrementCodeRule();
    }
}