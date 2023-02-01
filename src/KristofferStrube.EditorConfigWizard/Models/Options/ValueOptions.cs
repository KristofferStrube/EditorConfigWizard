using System.Text.Json.Serialization;

namespace KristofferStrube.EditorConfigWizard.Models.Options;

[JsonDerivedType(typeof(OneOfManyValueOptions), "OneOfMany")]
[JsonDerivedType(typeof(OrderedSetWithOneOrMoreOfManyValueOptions), "OrderedSetWithOneOrMoreOfMany")]
public class ValueOptions
{
}
