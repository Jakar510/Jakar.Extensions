using System;
using Jakar.Extensions.Http;
using NUnit.Framework;


namespace Jakar.Extensions.Tests.Http;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class MimeType_Extension_Tests : Assert
{
    [Test]
    public void Test_FromExtension( [Values] MimeType mime )
    {
        if ( mime is MimeType.Unknown or MimeType.NotSet )
        {
            Throws<ArgumentOutOfRangeException>(() => AreEqual(mime.ToExtension().FromExtension(), mime));
            return;
        }

        AreEqual(mime.ToExtension().FromExtension(), mime);
    }
}
