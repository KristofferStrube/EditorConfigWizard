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
## Sample EditorConfigs out in the wild
While exploring this I have found some prominent examples used by professionals in the industry and big open source projects. Here are a few:
- [dotnet/razor/.editorconfig](https://github.com/dotnet/razor/blob/main/.editorconfig)
- [David McCarter's EditorConfig](https://gist.github.com/RealDotNetDave/dbae4d97358ba4515dd52e5b8ca87671)

We would like to eventually make it possible to cover many of the sames rules that they use and to compare ones own config to theirs.

## Checklist for rules
### Code quality rules
#### Design rules
- unquantified
#### Design rules
- unquantified
#### Documentation rules
- unquantified
#### Gloablization rules
- unquantified
#### Portability and interopability rules
- unquantified
#### Maintainability rules rules
- unquantified
#### Naming rules
- unquantified
#### Performance rules
- unquantified
#### SingleFile rules
- unquantified
#### Reliability rules
- unquantified
#### Security rules
- unquantified
#### Uage rules
- unquantified
### Code style rules
#### Language rules
- [x] IDE0003
- [x] IDE0049
- [x] IDE0036
- [x] IDE0040
- [x] IDE0044
- [x] IDE0062
- [x] IDE0047
- [x] IDE0048
- [x] IDE0010
- [x] IDE0017
- [x] IDE0018
- [x] IDE0028
- [x] IDE0032
- [x] IDE0033
- [x] IDE0034
- [x] IDE0037
- [x] IDE0039
- [x] IDE0042
- [x] IDE0045
- [x] IDE0046
- [ ] IDE0054
- [ ] IDE0056
- [ ] IDE0057
- [ ] IDE0070
- [ ] IDE0071
- [ ] IDE0072
- [ ] IDE0074
- [ ] IDE0075
- [ ] IDE0082
- [ ] IDE0090
- [ ] IDE0180
- [ ] IDE0160
- [ ] IDE0161
- [ ] IDE0016
- [ ] IDE0029
- [ ] IDE0030
- [ ] IDE0031
- [ ] IDE0041
- [ ] IDE0150
- [ ] IDE1005
- [ ] IDE0007
- [ ] IDE0008
- [ ] IDE0021
- [ ] IDE0022
- [ ] IDE0023
- [ ] IDE0024
- [ ] IDE0025
- [ ] IDE0026
- [ ] IDE0027
- [ ] IDE0053
- [ ] IDE0061
- [ ] IDE0019
- [ ] IDE0020
- [ ] IDE0038
- [ ] IDE0066
- [ ] IDE0078
- [ ] IDE0083
- [ ] IDE0084
- [ ] IDE0170
- [ ] IDE0011
- [ ] IDE0063
- [ ] IDE0065
- [ ] IDE0073
- [ ] IDE0130
#### Unnecesary code rules
- [x] IDE0001
- [x] IDE0002
- [x] IDE0004
- [x] IDE0005
- [x] IDE0035
- [x] IDE0051
- [x] IDE0052
- [x] IDE0058
- [x] IDE0059
- [ ] IDE0060
- [ ] IDE0079
- [ ] IDE0080
- [ ] IDE0081
- [ ] IDE0100
- [ ] IDE0110
- [ ] IDE0140
#### Miscelllaneous rules
- [ ] IDE0076
- [ ] IDE0077
#### Miscelllaneous rules
- [ ] IDE0055

# Issues
Feel free to open issues on the repository if you find any errors with the project or have wishes for features.

## Related articles
This project uses the idea of making a component for showing code from this article.
- [Rendering dynamic content in Blazor Wasm using DynamicComponent](https://blog.elmah.io/rendering-dynamic-content-in-blazor-wasm-using-dynamiccomponent/)
