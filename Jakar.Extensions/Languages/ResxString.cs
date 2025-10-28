// Jakar.Extensions :: Jakar.Database
// 09/19/2023  11:06 AM

using Microsoft.CodeAnalysis;



namespace Jakar.Extensions;


[Serializable]
[method: JsonConstructor]
public sealed class ResxString( string neutral,
                                string arabic,
                                string chinese,
                                string czech,
                                string dutch,
                                string english,
                                string french,
                                string german,
                                string japanese,
                                string korean,
                                string polish,
                                string portuguese,
                                string spanish,
                                string swedish,
                                string thai ) : LocalizableString, IEquatable<ResxString>, IComparable<ResxString>
{
    public readonly string Arabic     = arabic;
    public readonly string Chinese    = chinese;
    public readonly string Czech      = czech;
    public readonly string Dutch      = dutch;
    public readonly string English    = english;
    public readonly string French     = french;
    public readonly string German     = german;
    public readonly string Japanese   = japanese;
    public readonly string Korean     = korean;
    public readonly string Neutral    = neutral;
    public readonly string Polish     = polish;
    public readonly string Portuguese = portuguese;
    public readonly string Spanish    = spanish;
    public readonly string Swedish    = swedish;
    public readonly string Thai       = thai;


#pragma warning disable RS1035
    protected override string GetText( IFormatProvider? formatProvider ) => GetText(formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture);
#pragma warning restore RS1035


    public string GetText( CultureInfo info ) => GetText(info.GetSupportedLanguage() ?? SupportedLanguage.Unspecified);
    public string GetText( SupportedLanguage language ) =>
        language switch
        {
            SupportedLanguage.English     => English,
            SupportedLanguage.Spanish     => Spanish,
            SupportedLanguage.French      => French,
            SupportedLanguage.Swedish     => Swedish,
            SupportedLanguage.German      => German,
            SupportedLanguage.Chinese     => Chinese,
            SupportedLanguage.Polish      => Polish,
            SupportedLanguage.Thai        => Thai,
            SupportedLanguage.Japanese    => Japanese,
            SupportedLanguage.Czech       => Czech,
            SupportedLanguage.Portuguese  => Portuguese,
            SupportedLanguage.Dutch       => Dutch,
            SupportedLanguage.Korean      => Korean,
            SupportedLanguage.Arabic      => Arabic,
            SupportedLanguage.Unspecified => Neutral,
            _                             => Neutral
        };


    protected override int  GetHash()                 => Neutral.GetHashCode();
    protected override bool AreEqual( object? other ) => other is ResxString resx && Equals(resx);
    public bool Equals( ResxString? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(Neutral, other.Neutral, StringComparison.Ordinal);
    }
    public int CompareTo( ResxString? other ) => string.Compare(Neutral, other?.Neutral, StringComparison.Ordinal);
}
