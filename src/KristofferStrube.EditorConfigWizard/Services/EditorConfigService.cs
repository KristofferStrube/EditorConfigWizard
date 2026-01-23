using KristofferStrube.EditorConfigWizard.Models;
using System.Net.Http.Json;

namespace KristofferStrube.EditorConfigWizard.Services;

public class EditorConfigService
{
    private readonly Lazy<Task<List<CodeStyleRule>>> languagesRulesTask;
    private readonly Lazy<Task<List<CodeStyleRule>>> unnecessaryCodeRulesTask;
    private readonly Lazy<Task<List<NamingRule>>> namingRulesTask;
    private readonly Lazy<Task<List<CodeStyleRule>>> formattingRulesTask;
    private readonly Lazy<Task<List<CodeStyleRule>>> namingConventionsTask;

    public EditorConfigService(HttpClient httpClient)
    {
        languagesRulesTask = new(async () => await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("language_rules.json") ?? new());
        unnecessaryCodeRulesTask = new(async () => await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("unnecessary_code_rules.json") ?? new());
        namingRulesTask = new(async () => await httpClient.GetFromJsonAsync<List<NamingRule>>("naming_rules.json") ?? new());
        formattingRulesTask = new(async () => await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("formatting_rules.json") ?? new());
        namingConventionsTask = new(async () => await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("naming_conventions.json") ?? new());
    }

    public async Task<CodeStyleCategory> LanguagesRulesAsync()
    {
        return new("Language rules", "Rules that pertain to the C# language. For example, you can specify rules that regard the use of var when defining variables, or whether expression-bodied members are preferred.", await languagesRulesTask.Value)
        {
            Used = true,
        };
    }

    public async Task<CodeStyleCategory> UnnecessaryColeRulesAsync()
    {
        return new("Unnecessary code rules", "Rules that pertain to unnecessary code that indicates a potential readability, maintainability, performance, or functional problem. For example, unreachable code within methods or unused private fields, properties, or methods is unnecessary code.", await unnecessaryCodeRulesTask.Value)
        {
            Used = true,
        };
    }

    public async Task<NamingRuleCategory> NamingRulesAsync()
    {
        return new("Naming conventions", "Rules that define naming conventions for code elements like classes, methods, properties, and fields. These rules help ensure consistent naming across your codebase.", await namingRulesTask.Value)
        {
            Used = true,
        };
    }

    public async Task<CodeStyleCategory> FormattingRulesAsync()
    {
        return new("Formatting rules", "Rules that control code formatting including indentation, spacing, new line placement, and wrapping. These rules define how your code is visually structured.", await formattingRulesTask.Value)
        {
            Used = true,
        };
    }

    public async Task<CodeStyleCategory> NamingConventionsAsync()
    {
        return new("Naming conventions", "Rules that define naming conventions for code elements like classes, methods, properties, and fields. These rules help ensure consistent naming across your codebase.", await namingConventionsTask.Value)
        {
            Used = true,
        };
    }
}
