using KristofferStrube.EditorConfigWizard.Models;
using KristofferStrube.EditorConfigWizard.Models.Options;
using KristofferStrube.EditorConfigWizard.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

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
        codeStyleCategories.Add(await ConfigService.FormattingRulesAsync());
        codeStyleCategories.Add(await ConfigService.NamingConventionsAsync());
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
                        if (currentCodeStyleRule.Options.Count > 0)
                        {
                            currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
                        }
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

            // If rule has no options, go directly to severity question
            if (currentCodeStyleRule.Options.Count == 0)
            {
                currentRuleOption = null;
                optionChoices = new List<string?>();
            }
            else if (ruleOptionIndex != currentCodeStyleRule.Options.Count)
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
            // Handle naming conventions differently
            if (codeRule.Id.StartsWith("naming_"))
            {
                GenerateNamingConvention(editorConfigSB, codeRule);
            }
            else
            {
                // Regular code style rules
                foreach (RuleOption option in codeRule.Options)
                {
                    editorConfigSB.AppendLine($"{option.Name} = {ruleOptionChoices[option.Name]}");
                }
                foreach (string id in codeRule.Id.Split(','))
                {
                    editorConfigSB.AppendLine($"dotnet_diagnostic.{id}.severity = {severities[codeRule.Id]}");
                }
            }
        }
        newEditorConfig = editorConfigSB.ToString();
    }

    private void GenerateNamingConvention(StringBuilder sb, CodeStyleRule rule)
    {
        var severity = severities[rule.Id];
        if (severity == "none") return; // Skip disabled naming rules

        string symbolGroup = rule.Id;
        string styleName = $"{rule.Id}_style";
        string ruleName = rule.Id;

        sb.AppendLine();
        sb.AppendLine($"# {rule.Title}");

        // Generate symbol group
        switch (rule.Id)
        {
            case "naming_interfaces":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = interface");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                sb.AppendLine($"dotnet_naming_style.{styleName}.required_prefix = I");
                break;
            case "naming_type_parameters":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = type_parameter");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                sb.AppendLine($"dotnet_naming_style.{styleName}.required_prefix = T");
                break;
            case "naming_classes":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = class");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_structs":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = struct");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_enums":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = enum");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_methods":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = method");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_properties":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = property");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_events":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = event");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_public_fields":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = field");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = public");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_private_fields":
                var privateFieldStyle = rule.Options.Count > 0 && ruleOptionChoices.ContainsKey("private_field_naming_style")
                    ? ruleOptionChoices["private_field_naming_style"]
                    : "_camelCase";
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = field");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = private");
                if (privateFieldStyle == "_camelCase")
                {
                    sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = camel_case");
                    sb.AppendLine($"dotnet_naming_style.{styleName}.required_prefix = _");
                }
                else if (privateFieldStyle == "camelCase")
                {
                    sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = camel_case");
                }
                else // PascalCase
                {
                    sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                }
                break;
            case "naming_constants":
                var constantStyle = rule.Options.Count > 0 && ruleOptionChoices.ContainsKey("constant_naming_style")
                    ? ruleOptionChoices["constant_naming_style"]
                    : "PascalCase";
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = field");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.required_modifiers = const");
                if (constantStyle == "PascalCase")
                {
                    sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                }
                else if (constantStyle == "ALL_UPPER")
                {
                    sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = all_upper");
                }
                else // camelCase
                {
                    sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = camel_case");
                }
                break;
            case "naming_static_readonly_fields":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = field");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.required_modifiers = static,readonly");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
            case "naming_parameters":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = parameter");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = camel_case");
                break;
            case "naming_local_variables":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = local");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = camel_case");
                break;
            case "naming_local_functions":
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_kinds = local_function");
                sb.AppendLine($"dotnet_naming_symbols.{symbolGroup}.applicable_accessibilities = *");
                sb.AppendLine($"dotnet_naming_style.{styleName}.capitalization = pascal_case");
                break;
        }

        // Generate naming rule
        sb.AppendLine($"dotnet_naming_rule.{ruleName}.symbols = {symbolGroup}");
        sb.AppendLine($"dotnet_naming_rule.{ruleName}.style = {styleName}");
        sb.AppendLine($"dotnet_naming_rule.{ruleName}.severity = {severity}");
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

    private async Task SaveProgressAsync()
    {
        var state = new WizardState
        {
            Progress = progress,
            CodeRuleIndex = codeRuleIndex,
            RuleOptionIndex = ruleOptionIndex,
            RuleOptionChoices = new Dictionary<string, string>(ruleOptionChoices),
            Severities = new Dictionary<string, string>(severities),
            SelectedCategoryTitles = codeStyleCategories
                .Where(c => c.Used)
                .Select(c => c.Title)
                .ToList(),
            SavedAt = DateTime.Now
        };

        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        var fileName = $"editorconfig-wizard-progress-{DateTime.Now:yyyyMMdd-HHmmss}.json";

        await JSRuntime.InvokeVoidAsync("downloadFile", fileName, json);
    }

    private async Task LoadProgressFromFile(InputFileChangeEventArgs e)
    {
        try
        {
            var file = e.File;
            if (file.Size > 1024 * 1024) // 1MB limit
            {
                return;
            }

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            var state = JsonSerializer.Deserialize<WizardState>(json);
            if (state == null) return;

            await ChangeAsync(() =>
            {
                // Restore category selections
                foreach (var category in codeStyleCategories)
                {
                    category.Used = state.SelectedCategoryTitles.Contains(category.Title);
                }

                // Restore choices and severities
                foreach (var kvp in state.RuleOptionChoices)
                {
                    ruleOptionChoices[kvp.Key] = kvp.Value;
                }
                foreach (var kvp in state.Severities)
                {
                    severities[kvp.Key] = kvp.Value;
                }

                // Rebuild rules list
                codeStyleRules = codeStyleCategories
                    .Where(category => category.Used)
                    .SelectMany(category => category.CodeStyleRules)
                    .ToList();
                totalQuestions = codeStyleRules.Count + codeStyleRules.Sum(rule => rule.Options.Count);

                // Restore progress
                progress = state.Progress;
                codeRuleIndex = state.CodeRuleIndex;
                ruleOptionIndex = state.RuleOptionIndex;

                // Restore current rule state
                if (codeRuleIndex >= 0 && codeRuleIndex < codeStyleRules.Count)
                {
                    currentCodeStyleRule = codeStyleRules[codeRuleIndex];

                    // Initialize optionChoices with defaults
                    optionChoices = currentCodeStyleRule.Options
                        .Select(o => o.ValueOptions is OrderedSetWithOneOrMoreOfManyValueOptions ? o.ValueOptions.DefaultOptionValue : null)
                        .ToList();

                    // Restore saved choices
                    for (int i = 0; i < currentCodeStyleRule.Options.Count; i++)
                    {
                        var optionName = currentCodeStyleRule.Options[i].Name;
                        if (ruleOptionChoices.ContainsKey(optionName))
                        {
                            optionChoices[i] = ruleOptionChoices[optionName];
                        }
                    }

                    // Set current rule option if we're not at the severity question yet
                    if (ruleOptionIndex >= 0 && ruleOptionIndex < currentCodeStyleRule.Options.Count)
                    {
                        currentRuleOption = currentCodeStyleRule.Options[ruleOptionIndex];
                    }
                    else
                    {
                        // We're at the severity question
                        currentRuleOption = null;
                    }
                }
                else if (codeRuleIndex == codeStyleRules.Count)
                {
                    // We're at the final screen
                    GenerateNewEditorConfig();
                    currentCodeStyleRule = null;
                }
                else
                {
                    // Invalid state, reset to beginning
                    progress = 0;
                    codeRuleIndex = -1;
                    currentCodeStyleRule = null;
                    currentRuleOption = null;
                }

                StateHasChanged();
            });
        }
        catch (Exception)
        {
            // Handle error - could show a message to user
        }
    }
}