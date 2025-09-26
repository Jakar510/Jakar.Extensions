namespace Jakar.Extensions.Tests;


[TestFixture][TestOf( typeof(MimeTypes) )]

// ReSharper disable once InconsistentNaming
public class MimeType_Extension_Tests : Assert
{
    [Test] public void Test_FromExtension( [Values] MimeType mime ) => this.AreEqual( mime.ToExtension().FromExtension(), mime );
}
