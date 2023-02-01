namespace KristofferStrube.EditorConfigWizard;

public static class StringListExtensions
{
    public static string AsMultilineString(this List<string> list) => string.Join("\n", list);
}
