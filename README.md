# EditorConfig Wizard
This is a EditorConfig Wizard written in Blazor WASM.
The current goal is to support all the standard C#/.NET EditorConfig rules.

## Demo
The current progress can be seen at https://kristofferstrube.github.io/EditorConfigWizard/

## Goals
- [ ] Define all Rules and be able to present them.
- [ ] Be able to parse an existing EditorConfig file.
- [ ] Be able to edit an EditorConfig file.
- [x] Be able to generate new EditorConfig files.
- [ ] Be able to compare two EditorConfig files and present their differences.

## Example
An EditorConfig file could have the following content:
```ini
[*.{cs,vb}]
dotnet_style_parentheses_in_arithmetic_binary_operators = always_for_clarity
dotnet_diagnostic.IDE0048.severity = suggestion
```

The first line specifies that this applies to `.cs` and `.vb` files.

The second line specifies that the editor will try to always have parenthesis around arithmetic sub expressions where it could be confusing to know which would be calculated first like in `1 + 2 * 3` which might be read like `(1 + 2) * 3` if you are not trained in the precedence of arithmetic operators whereas it would be more clear to write `1 + (2 * 3)`. The other possible value for this option is `never_if_unnecessary` which obviously has the opposite effect.

*(`dotnet_style_parentheses_in_arithmetic_binary_operators` is a part of the rule with Id `IDE0048`)*

The third line specifies that this rule should be prompted to the user in the form of a suggestion. We could likewise have chosen to have it appear as a `warning` or an `error` or have hidden or disabled it with `silent` or `none`.

We would like to be able to present a sample for this which could be the following so that people can make a qualified choice with a quick glance.
### always_for_clarity
Prefer parentheses to clarify arithmetic operator precedence.
```csharp
var v = a + (b * c);
```
### never_if_unnecessary
Prefer no parentheses when arithmetic operator precedence is obvious.
```csharp
var v = a + b * c;
```
