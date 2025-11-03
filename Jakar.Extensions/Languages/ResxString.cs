// Jakar.Extensions :: Jakar.Database
// 09/19/2023  11:06 AM

using Microsoft.CodeAnalysis;



namespace Jakar.Extensions;


public interface IResxString : IUniqueID<long>
{
    string Arabic     { get; }
    string Chinese    { get; }
    string Czech      { get; }
    string Dutch      { get; }
    string English    { get; }
    string French     { get; }
    string German     { get; }
    string Japanese   { get; }
    string Korean     { get; }
    string Neutral    { get; }
    string Polish     { get; }
    string Portuguese { get; }
    string Spanish    { get; }
    string Swedish    { get; }
    string Thai       { get; }
}



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
                                string thai ) : LocalizableString, IResxString, IEquatable<ResxString>, IComparable<ResxString>
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
    string IResxString.    Arabic     => Arabic;
    string IResxString.    Chinese    => Chinese;
    string IResxString.    Czech      => Czech;
    string IResxString.    Dutch      => Dutch;
    string IResxString.    English    => English;
    string IResxString.    French     => French;
    string IResxString.    German     => German;
    public long            ID         { get; init; }
    string IResxString.    Japanese   => Japanese;
    string IResxString.    Korean     => Korean;
    string IResxString.    Neutral    => Neutral;
    string IResxString.    Polish     => Polish;
    string IResxString.    Portuguese => Portuguese;
    string IResxString.    Spanish    => Spanish;
    string IResxString.    Swedish    => Swedish;
    string IResxString.    Thai       => Thai;


    public ResxString( IResxString resx ) : this(resx.Neutral,
                                                 resx.Arabic,
                                                 resx.Chinese,
                                                 resx.Czech,
                                                 resx.Dutch,
                                                 resx.English,
                                                 resx.French,
                                                 resx.German,
                                                 resx.Japanese,
                                                 resx.Korean,
                                                 resx.Polish,
                                                 resx.Portuguese,
                                                 resx.Spanish,
                                                 resx.Swedish,
                                                 resx.Thai) => ID = resx.ID;


    protected override string GetText( IFormatProvider? formatProvider ) => GetText(formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture);
    public             string GetText( CultureInfo      info )           => GetText(info.GetSupportedLanguage());
    public string GetText( SupportedLanguage language ) => language switch
                                                           {
                                                               SupportedLanguage.English    => English,
                                                               SupportedLanguage.Spanish    => Spanish,
                                                               SupportedLanguage.French     => French,
                                                               SupportedLanguage.Swedish    => Swedish,
                                                               SupportedLanguage.German     => German,
                                                               SupportedLanguage.Chinese    => Chinese,
                                                               SupportedLanguage.Polish     => Polish,
                                                               SupportedLanguage.Thai       => Thai,
                                                               SupportedLanguage.Japanese   => Japanese,
                                                               SupportedLanguage.Czech      => Czech,
                                                               SupportedLanguage.Portuguese => Portuguese,
                                                               SupportedLanguage.Dutch      => Dutch,
                                                               SupportedLanguage.Korean     => Korean,
                                                               SupportedLanguage.Arabic     => Arabic,
                                                               _                            => Neutral
                                                           };


    protected override int  GetHash()                      => Neutral.GetHashCode();
    protected override bool AreEqual( object?      other ) => other is ResxString resx && Equals(resx);
    public             bool Equals( ResxString?    other ) => ReferenceEquals(this, other) || string.Equals(Neutral, other?.Neutral, StringComparison.Ordinal);
    public             int  CompareTo( ResxString? other ) => string.Compare(Neutral, other?.Neutral, StringComparison.Ordinal);
}
