// Jakar.Extensions :: Jakar.Extensions
// 04/27/2024  18:04

namespace Jakar.Extensions;


[method: JsonConstructor]
public readonly record struct Email( string Value )
{
    public static implicit operator string( Email email ) => email.Value;
    public static implicit operator Email( string value ) => new(value);
    public override                 string ToString()     => Value;
}
