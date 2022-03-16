using System.Collections.Generic;
using Jakar.Extensions.General;
using Jakar.Extensions.Strings;
using NUnit.Framework;


namespace Jakar.Extensions.Tests;


[TestFixture]
public class ValidateTests : Assert
{
    [Test]
    [TestCase("wwww.google.com", false)]
    [TestCase("http://google.com", true)]
    [TestCase("http://wwww.google.com", true)]
    [TestCase("google.com", false)]
    public void Web( string s, bool expected ) { AreEqual(s.IsWebAddress(), expected); }

    [Test]
    [TestCase(0, false)]
    [TestCase(10, true)]
    [TestCase(8080, true)]
    [TestCase(65534, true)]
    [TestCase(655350, false)]
    public void Port( int s, bool expected ) { AreEqual(s.IsValidPort(), expected); }

    [Test]
    [TestCase("0", true)]
    [TestCase("10", true)]
    [TestCase("8080", true)]
    [TestCase("655a35", false)]
    [TestCase("65 5350", false)]
    [TestCase("65..0", false)]
    [TestCase("65.0", false)]
    public void IsInteger( string s, bool expected ) { AreEqual(s.IsInteger(), expected); }

    [Test]
    [TestCase("demo ", false)]
    [TestCase("dem o", false)]
    [TestCase("655a35", false)]
    [TestCase("65 5350", false)]
    [TestCase("DEMO", true)]
    [TestCase("demo", true)]
    [TestCase("Demo", true)]
    public void IsDemo( string s, bool expected ) { AreEqual(s.IsDemo(), expected); }

    [Test]
    [TestCase(1, "1")]
    [TestCase(1d, "1")]
    [TestCase(1.1, "1.1")]
    [TestCase(1.01, "1.01")]
    [TestCase(1.001, "1.001")]
    [TestCase(1.0001, "1.0001")]
    public void FormatNumber( object s, string expected ) { AreEqual(s.FormatNumber(), expected); }

    [Test]
    [TestCase("bob@bob.com", true)]
    [TestCase("bob@random.com", true)]
    [TestCase("bob@random.net", true)]
    [TestCase("bob@random.co", true)]
    [TestCase("bob-tom@random.com", true)]
    [TestCase("bob_tom@random.com", true)]
    [TestCase("bob@random", false)]
    [TestCase("bob~@random.com", false)]
    [TestCase("bob@random,com", false)]
    public void IsEmailAddress( string s, bool expected ) { AreEqual(s.IsEmailAddress(), expected); }

    [Test]
    [TestCase("0", true)]
    [TestCase("10", true)]
    [TestCase("8080", true)]
    [TestCase("655a35", false)]
    [TestCase("65 5350", false)]
    [TestCase("65..0", false)]
    [TestCase("65.0", true)]
    public void IsDouble( string s, bool expected ) { AreEqual(s.IsDouble(), expected); }


    [Test]
    [TestCase("1.1.1.1", true)]
    [TestCase("1.1.1.1 ", true)]
    [TestCase(" 1.1.1.1", true)]
    [TestCase(" 1.1.1.1 ", true)]
    [TestCase("1.1 .1.1", false)]
    [TestCase("1.1..1.1", false)]
    [TestCase("1.111.1.1", true)]
    [TestCase("1.1111.1.1", false)]
    [TestCase("11.11.11.11", true)]
    [TestCase("111.111.111.111", true)]
    [TestCase("111.111.111.1111", false)]
    public void Ip( string s, bool expected ) { AreEqual(s.IsIPv4(), expected); }
}
