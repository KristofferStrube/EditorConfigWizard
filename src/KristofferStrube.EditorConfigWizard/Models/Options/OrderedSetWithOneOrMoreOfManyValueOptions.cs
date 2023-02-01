namespace KristofferStrube.EditorConfigWizard.Models.Options
{
    public class OrderedSetWithOneOrMoreOfManyValueOptions : ValueOptions
    {
        public List<string> Options { get; set; }

        public string DefaultOptionValue { get; set; }
    }
}
