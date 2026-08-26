// Jakar.Extensions :: Jakar.Extensions.Tests


namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Permissions<>))]
public class Permissions_Tests : Assert
{
    // NOTE: the null-padding checks below use string.Contains(char), which is ORDINAL.
    // Do not switch them back to Does.Not.Contain("\0") - NUnit's SubstringConstraint compares with
    // StringComparison.CurrentCulture, and under ICU collation U+0000 is a zero-weight ignorable
    // character, so "++++".IndexOf("\0", CurrentCulture) returns 0. That assertion can never pass,
    // for any string, including string.Empty - it reports a defect that is not there.



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
                     That(value.Length,         Is.EqualTo(Permissions<Right>.Count));
                     That(value.Contains('\0'), Is.False);
                     That(value,                Is.EqualTo(new string(Permissions<Right>.ValidChar, Permissions<Right>.Count)));
                 });
    }


    [Test] public void ToString_round_trips_through_Create()
    {
        using Permissions<Right> original = Permissions<Right>.Create(Right.Read, Right.Delete);
        string                   encoded  = original.ToString();


        Multiple(() =>
                 {
                     using Permissions<Right> parsed = Permissions<Right>.Create(encoded);
                     That(encoded.Contains('\0'),   Is.False);
                     That(encoded.Length,           Is.EqualTo(Permissions<Right>.Count));
                     That(parsed.ToString(),        Is.EqualTo(encoded));
                     That(parsed.Has(Right.Read),   Is.True);
                     That(parsed.Has(Right.Delete), Is.True);
                     That(parsed.Has(Right.Write),  Is.False);
                 });
    }
}
