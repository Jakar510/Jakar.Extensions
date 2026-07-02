// Jakar.Extensions :: Jakar.Extensions.Tests


namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Permissions<>))]
public class Permissions_Tests : Assert
{
    private enum Right
    {
        None,
        Read,
        Write,
        Delete
    }


    [Test] public void ToString_is_exact_length_with_no_null_padding()
    {
        using Permissions<Right> permissions = Permissions<Right>.SA();
        string                   value       = permissions.ToString();

        Multiple(() =>
                 {
                     That(value.Length, Is.EqualTo(Permissions<Right>.Count));
                     That(value,        Does.Not.Contain("\0"));
                     That(value,        Is.EqualTo(new string(Permissions<Right>.ValidChar, Permissions<Right>.Count)));
                 });
    }


    [Test] public void ToString_round_trips_through_Create()
    {
        using Permissions<Right> original = Permissions<Right>.Create(Right.Read, Right.Delete);
        string                   encoded  = original.ToString();

        using Permissions<Right> parsed = Permissions<Right>.Create(encoded.AsSpan());

        Multiple(() =>
                 {
                     That(encoded,                  Does.Not.Contain("\0"));
                     That(encoded.Length,           Is.EqualTo(Permissions<Right>.Count));
                     That(parsed.ToString(),        Is.EqualTo(encoded));
                     That(parsed.Has(Right.Read),   Is.True);
                     That(parsed.Has(Right.Delete), Is.True);
                     That(parsed.Has(Right.Write),  Is.False);
                 });
    }
}
