namespace KristofferStrube.EditorConfigWizard.Models.Options
{
    public class OneOfManyValueOptions : ValueOptions
    {
        public List<OptionValue> Options { get; set; }

        public string DefaultOptionValue { get; set; }
    }
}
