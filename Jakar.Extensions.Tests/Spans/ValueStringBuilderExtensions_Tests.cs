// Jakar.Extensions :: Jakar.Extensions.Tests

namespace Jakar.Extensions.Tests.Text;


/// <summary> Jakar.Extensions' helpers on <see cref="ValueStringBuilder"/> (the builder itself is tested in the Jakar.Spans repository). </summary>
[TestFixture, TestOf(typeof(ValueStringBuilder))]
public sealed class ValueStringBuilderExtensions_Tests : Assert
{
    [Test] public void IsNullOrWhiteSpace_UsesContent()
    {
        using ValueStringBuilder blank = new(64);
        blank.Append("   ");
        this.IsTrue(blank.IsNullOrWhiteSpace());

        using ValueStringBuilder text = new(64);
        text.Append(" x ");
        this.IsFalse(text.IsNullOrWhiteSpace());
    }
}
