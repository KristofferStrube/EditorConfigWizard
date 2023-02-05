using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Models.Options;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Wizard
{
    private CodeStyleRule? currentCodeStyleRule = null;
    private RuleOption? currentRuleOption = null;
    private List<string?> optionChoices = new();
    private List<CodeStyleRule> codeStyleRules = new();
    private string fadeClass = "";
    private int codeRuleIndex = -1;
    private int ruleOptionIndex = 0;
    private readonly Dictionary<string, string> ruleOptionChoices = new();
    private readonly Dictionary<string, string> severities = new();
    private string newEditorConfig = "";

    [Inject]
    public EditorConfigService ConfigService { get; set; }

    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    protected override async Task OnInitializedAsync()
    {
        codeStyleRules = await ConfigService.GetRules();
    }

    private async Task IncrementCodeRule()
    {
        await ChangeAsync(async () =>
        {
            codeRuleIndex++;
            ruleOptionIndex = 0;
            if (codeRuleIndex == codeStyleRules.Count)
            {
                StringBuilder editorConfigSB = new StringBuilder();
                editorConfigSB.AppendLine("[*]");
                editorConfigSB.AppendLine("# All files");
                foreach (CodeStyleRule codeRule in codeStyleRules)
                {
                    foreach (RuleOption option in codeRule.Options)
                    {
                        editorConfigSB.AppendLine($"{option.Name} = {ruleOptionChoices[option.Name]}");
                    }
                    foreach (string id in codeRule.Id.Split(','))
                    {
                        editorConfigSB.AppendLine($"dotnet_diagnostic.{id}.severity = {severities[codeRule.Id]}");
                    }
                }
                newEditorConfig = editorConfigSB.ToString();
                currentCodeStyleRule = null;
                return;
            }
            currentCodeStyleRule = codeStyleRules[codeRuleIndex];
            currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
            optionChoices = currentCodeStyleRule.Options
            .Select(o => o.ValueOptions is OrderedSetWithOneOrMoreOfManyValueOptions ? o.ValueOptions.DefaultOptionValue : null).ToList();
        });
    }

    private async Task IncrementRuleOption()
    {
        await ChangeAsync(() =>
        {
            ruleOptionChoices[currentRuleOption.Name] = optionChoices[ruleOptionIndex];
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

    private async Task ChangeAsync(Action action)
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

    private async Task SelectOption(int choice, string value)
    {
        optionChoices[choice] = value;
        await IncrementRuleOption();
        StateHasChanged();
    }

    private async Task SetSeverity(string severity)
    {
        severities[currentCodeStyleRule.Id] = severity;
        await IncrementCodeRule();
    }

    private async Task CopyToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", newEditorConfig);
    }
}