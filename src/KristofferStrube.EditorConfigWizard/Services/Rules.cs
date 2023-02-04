using KristofferStrube.EditorConfigWizard.Models;
using System.Net.Http.Json;

namespace KristofferStrube.EditorConfigWizard.Services;

public class EditorConfigService
{
    private Lazy<Task<List<CodeStyleRule>>> rulesTask;

    public EditorConfigService(HttpClient httpClient)
    {
        rulesTask = new(async () => (await httpClient.GetFromJsonAsync<List<CodeStyleRule>>("Rules.json") ?? new()));
    }

    public async Task<List<CodeStyleRule>> GetRules() => await rulesTask.Value;
}
