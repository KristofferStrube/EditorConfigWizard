using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Models.Options;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Wizard : ComponentBase
{
    private const int TRANSITION_TIME = 200;

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
    private int totalQuestions;
    private int progress;

    private readonly List<CodeStyleCategory> codeStyleCategories = new();

    [Inject]
    public EditorConfigService ConfigService { get; set; } = default!;

    [Inject]
    public IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        codeStyleCategories.Add(await ConfigService.LanguagesRulesAsync());
        codeStyleCategories.Add(await ConfigService.UnnecessaryColeRulesAsync());
        NavigationManager.RegisterLocationChangingHandler(async (context) =>
        {
            if (context.HistoryEntryState is null && context.TargetLocation.EndsWith("wizard"))
            {
                await ChangeAsync(() =>
                {
                    codeRuleIndex = -1;
                    currentCodeStyleRule = null;
                    progress = 0;
                    StateHasChanged();
                });
            }
            else if (context.HistoryEntryState == (progress - 1).ToString())
            {
                await ChangeAsync(() =>
                {
                    progress--;
                    if (codeRuleIndex == codeStyleRules.Count && currentCodeStyleRule is null)
                    {
                        codeRuleIndex--;
                        currentCodeStyleRule = codeStyleRules.Last();
                        ruleOptionIndex = currentCodeStyleRule.Options.Count;
                        currentRuleOption = null;
                        optionChoices = currentCodeStyleRule.Options
                            .Select(o => o.ValueOptions is OrderedSetWithOneOrMoreOfManyValueOptions ? o.ValueOptions.DefaultOptionValue : null)
                            .ToList();
                    }
                    else if (ruleOptionIndex is not 0)
                    {
                        ruleOptionIndex--;
                        currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
                    }
                    else
                    {
                        codeRuleIndex--;
                        currentCodeStyleRule = codeStyleRules[codeRuleIndex];
                        ruleOptionIndex = currentCodeStyleRule.Options.Count;
                        currentRuleOption = null;
                        optionChoices = currentCodeStyleRule.Options
                            .Select(o => o.ValueOptions is OrderedSetWithOneOrMoreOfManyValueOptions ? o.ValueOptions.DefaultOptionValue : null)
                            .ToList();
                    }
                    StateHasChanged();
                });
            }
        });
    }

    private async Task Begin()
    {
        codeStyleRules = codeStyleCategories
            .Where(category => category.Used)
            .SelectMany(category => category.CodeStyleRules)
            .ToList();
        totalQuestions = codeStyleRules.Count + codeStyleRules.Sum(rule => rule.Options.Count);
        await IncrementCodeRule();
    }

    private async Task IncrementCodeRule()
    {
        await ChangeAsync(() =>
        {
            progress++;
            codeRuleIndex++;
            ruleOptionIndex = 0;
            if (codeRuleIndex == codeStyleRules.Count)
            {
                GenerateNewEditorConfig();
                currentCodeStyleRule = null;
                NavigationManager.NavigateTo("wizard", new NavigationOptions() { HistoryEntryState = progress.ToString() });
                return;
            }
            currentCodeStyleRule = codeStyleRules[codeRuleIndex];
            if (ruleOptionIndex != currentCodeStyleRule.Options.Count)
            {
                currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
                optionChoices = currentCodeStyleRule.Options
                    .Select(o => o.ValueOptions is OrderedSetWithOneOrMoreOfManyValueOptions ? o.ValueOptions.DefaultOptionValue : null)
                    .ToList();
            }
            NavigationManager.NavigateTo("wizard", new NavigationOptions() { HistoryEntryState = progress.ToString() });
        });
    }

    private async Task IncrementRuleOption()
    {
        await ChangeAsync(() =>
        {
            progress++;
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
            NavigationManager.NavigateTo("wizard", new NavigationOptions() { HistoryEntryState = progress.ToString() });
        });
    }

    private void GenerateNewEditorConfig()
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
    }

    private async Task ChangeAsync(Action action)
    {
        fadeClass = "fade-out";
        StateHasChanged();
        await Task.Delay(TRANSITION_TIME);
        action();
        fadeClass = "fade-in";
        StateHasChanged();
        await Task.Delay(TRANSITION_TIME);
        fadeClass = "";
        StateHasChanged();
    }

    private async Task SelectOptionAsync(int choice, string value)
    {
        optionChoices[choice] = value;
        await IncrementRuleOption();
        StateHasChanged();
    }

    private async Task SetSeverityAsync(string severity)
    {
        severities[currentCodeStyleRule.Id] = severity;
        await IncrementCodeRule();
    }

    private async Task CopyToClipboardAsync()
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", newEditorConfig);
    }

    private async Task GoBackAsync()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }
}