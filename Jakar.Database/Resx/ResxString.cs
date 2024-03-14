// Jakar.Extensions :: Jakar.Database
// 09/19/2023  11:06 AM

namespace Jakar.Database.Resx;


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
    private readonly string _neutral = neutral;


#pragma warning disable RS1035
    protected override string GetText( IFormatProvider? formatProvider ) => GetText( formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture );
#pragma warning restore RS1035


    public string GetText( CultureInfo info ) => GetText( info.GetSupportedLanguage() ?? SupportedLanguage.Unspecified );
    public string GetText( SupportedLanguage language ) =>
        language switch
        {
            SupportedLanguage.English     => english,
            SupportedLanguage.Spanish     => spanish,
            SupportedLanguage.French      => french,
            SupportedLanguage.Swedish     => swedish,
            SupportedLanguage.German      => german,
            SupportedLanguage.Chinese     => chinese,
            SupportedLanguage.Polish      => polish,
            SupportedLanguage.Thai        => thai,
            SupportedLanguage.Japanese    => japanese,
            SupportedLanguage.Czech       => czech,
            SupportedLanguage.Portuguese  => portuguese,
            SupportedLanguage.Dutch       => dutch,
            SupportedLanguage.Korean      => korean,
            SupportedLanguage.Arabic      => arabic,
            SupportedLanguage.Unspecified => _neutral,
            _                             => _neutral
        };
    protected override int  GetHash()                 => _neutral.GetHashCode();
    protected override bool AreEqual( object? other ) => other is ResxString resx && Equals( resx );
    public bool Equals( ResxString? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( _neutral, other._neutral, StringComparison.Ordinal );
    }
    public int CompareTo( ResxString? other ) => string.Compare( _neutral, other?._neutral, StringComparison.Ordinal );
}
