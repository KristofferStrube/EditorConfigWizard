namespace KristofferStrube.EditorConfigWizard.Extensions;

public static class HumanizerExtensions
{
    public static string Pluralize(this int value, string unit)
    {
        if (value == 1)
        {
            return $"{value} {unit}";
        }
        if (unit.EndsWith("y"))
        {
            return $"{value} {unit[..^1]}ies";
        }
        return $"{value} {unit}s";
    }
}
