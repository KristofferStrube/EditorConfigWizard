using KristofferStrube.EditorConfigWizard.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace KristofferStrube.EditorConfigWizard.Pages;

public partial class Index
{
    private List<CodeStyleRule> CodeStyleRules = new();

    [Inject]
    HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CodeStyleRules = await HttpClient.GetFromJsonAsync<List<CodeStyleRule>>("Rules.json") ?? new();
    }
}