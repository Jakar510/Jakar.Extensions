namespace Jakar.Extensions.Tests;


[ TestFixture, TestOf( typeof(MimeTypes) ) ]

// ReSharper disable once InconsistentNaming
public class MimeType_Extension_Tests : Assert
{
    [ Test ]
    public void Test_FromExtension( [ Values ] MimeType mime )
    {
        if ( mime is MimeType.NotSet )
        {
            Throws<ArgumentOutOfRangeException>( () => this.AreEqual( mime.ToExtension().FromExtension(), mime ) );

            return;
        }

        this.AreEqual( mime.ToExtension().FromExtension(), mime );
    }
}
