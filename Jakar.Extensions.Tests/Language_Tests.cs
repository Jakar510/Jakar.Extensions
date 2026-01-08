// Jakar.Extensions :: Jakar.Extensions.Tests
// 01/08/2026  17:09

using System.Globalization;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Language))]
public class Language_Tests : Assert
{
    [Test]
    [TestCase(SupportedLanguage.English,    "en")]
    [TestCase(SupportedLanguage.Spanish,    "es")]
    [TestCase(SupportedLanguage.French,     "fr")]
    [TestCase(SupportedLanguage.Swedish,    "sv")]
    [TestCase(SupportedLanguage.German,     "de")]
    [TestCase(SupportedLanguage.Chinese,    "zh-Hans")]
    [TestCase(SupportedLanguage.Polish,     "pl")]
    [TestCase(SupportedLanguage.Thai,       "th")]
    [TestCase(SupportedLanguage.Japanese,   "ja")]
    [TestCase(SupportedLanguage.Czech,      "cs")]
    [TestCase(SupportedLanguage.Portuguese, "pt")]
    [TestCase(SupportedLanguage.Dutch,      "nl")]
    [TestCase(SupportedLanguage.Korean,     "ko")]
    [TestCase(SupportedLanguage.Arabic,     "ar")]
    public void GetShortName( SupportedLanguage language, string expected ) => this.AreEqual(expected, language.GetShortName());


    [Test]
    [TestCase(SupportedLanguage.English,    "en")]
    [TestCase(SupportedLanguage.Spanish,    "es")]
    [TestCase(SupportedLanguage.French,     "fr")]
    [TestCase(SupportedLanguage.Swedish,    "sv")]
    [TestCase(SupportedLanguage.German,     "de")]
    [TestCase(SupportedLanguage.Chinese,    "zh-Hans")]
    [TestCase(SupportedLanguage.Polish,     "pl")]
    [TestCase(SupportedLanguage.Thai,       "th")]
    [TestCase(SupportedLanguage.Japanese,   "ja")]
    [TestCase(SupportedLanguage.Czech,      "cs")]
    [TestCase(SupportedLanguage.Portuguese, "pt")]
    [TestCase(SupportedLanguage.Dutch,      "nl")]
    [TestCase(SupportedLanguage.Korean,     "ko")]
    [TestCase(SupportedLanguage.Arabic,     "ar")]
    public void GetCulture( SupportedLanguage language, string expected ) => this.AreEqual(CultureInfo.GetCultureInfo(expected), language.AsCultureInfo());
}
