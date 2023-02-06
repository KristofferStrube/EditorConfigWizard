using KristofferStrube.EditorConfigWizard.Models;
using System.Net.Http.Json;

namespace KristofferStrube.EditorConfigWizard.Services;

public class EditorConfigService
{
    private readonly Lazy<Task<List<CodeStyleRule>>> languagesRulesTask;
    private readonly Lazy<Task<List<CodeStyleRule>>> unnecessaryCodeRulesTask;

    public EditorConfigService(HttpClient httpClient)
    {
        languagesRulesTask = new(async () => await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("language_rules.json") ?? new());
        unnecessaryCodeRulesTask = new(async () => await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("unnecessary_code_rules.json") ?? new());
    }

    public async Task<CodeStyleCategory> LanguagesRulesAsync()
    {
        return new("Language Rules", "Rules that pertain to the C#. For example, you can specify rules that regard the use of var when defining variables, or whether expression-bodied members are preferred.", await languagesRulesTask.Value)
        {
            Used = true,
        };
    }

    public async Task<CodeStyleCategory> UnnecessaryColeRulesAsync()
    {
        return new("Unnecessary code rules", "Rules that pertain to unnecessary code that indicates a potential readability, maintainability, performance, or functional problem. For example, unreachable code within methods or unused private fields, properties, or methods is unnecessary code.\r\n\r\n", await unnecessaryCodeRulesTask.Value)
        {
            Used = true,
        };

    }
}
