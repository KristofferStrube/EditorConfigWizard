namespace KristofferStrube.EditorConfigWizard.Models.Options
{
    public class OrderedSetWithOneOrMoreOfManyValueOptions : ValueOptions
    {
        public List<string> Options { get; set; } = new();
        public string Description { get; set; } = "";
        public string EncodedValueSeperator { get; set; } = " ";
        public string PresenterSeperator { get; set; } = " ";
        public PresenterTemplate[] PresenterTemplates { get; set; } = default!;
    }
}
