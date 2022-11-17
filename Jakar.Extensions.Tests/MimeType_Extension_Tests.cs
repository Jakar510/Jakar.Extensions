#nullable enable
using System;
using NUnit.Framework;



namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class MimeType_Extension_Tests : Assert
{
    [Test]
    public void Test_FromExtension( [Values] MimeType mime )
    {
        if ( mime is MimeType.NotSet )
        {
            Throws<ArgumentOutOfRangeException>( () => AreEqual( mime.ToExtension()
                                                                     .FromExtension(),
                                                                 mime ) );

            return;
        }

        AreEqual( mime.ToExtension()
                      .FromExtension(),
                  mime );
    }
}
