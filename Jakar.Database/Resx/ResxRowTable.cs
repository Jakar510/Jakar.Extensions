namespace Jakar.Database.Resx;


[Serializable]
[Table( "Resx" )]
public sealed record ResxRowTable : TableRecord<ResxRowTable>
{
    public string  Key        { get; init; } = string.Empty;
    public string  Neutral    { get; init; } = string.Empty;
    public string? Arabic     { get; init; }
    public string? Chinese    { get; init; }
    public string? Czech      { get; init; }
    public string? Dutch      { get; init; }
    public string? English    { get; init; }
    public string? French     { get; init; }
    public string? German     { get; init; }
    public string? Japanese   { get; init; }
    public string? Korean     { get; init; }
    public string? Polish     { get; init; }
    public string? Portuguese { get; init; }
    public string? Spanish    { get; init; }
    public string? Swedish    { get; init; }
    public string? Thai       { get; init; }
    public long    KeyID      { get; init; }


    public ResxRowTable() { }
    public ResxRowTable( string key, long keyID, UserRecord? caller = default ) : base( Guid.NewGuid(), caller )
    {
        Key         = key;
        KeyID       = keyID;
    }
    public ResxRowTable( string key, long keyID, string neutral, UserRecord? caller = default ) : this( key, keyID, caller ) => Neutral = neutral;
    public ResxRowTable( string      key,
                         long        keyID,
                         string      neutral,
                         string      english,
                         string      spanish,
                         string      french,
                         string      swedish,
                         string      german,
                         string      chinese,
                         string      polish,
                         string      thai,
                         string      japanese,
                         string      czech,
                         string      portuguese,
                         string      dutch,
                         string      korean,
                         string      arabic,
                         UserRecord? caller = default
    ) : this( key, keyID, neutral, caller )
    {
        English    = english;
        Spanish    = spanish;
        French     = french;
        Swedish    = swedish;
        German     = german;
        Chinese    = chinese;
        Polish     = polish;
        Thai       = thai;
        Japanese   = japanese;
        Czech      = czech;
        Portuguese = portuguese;
        Dutch      = dutch;
        Korean     = korean;
        Arabic     = arabic;
    }


    public override int CompareTo( ResxRowTable? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return string.Compare( Neutral, other.Neutral, StringComparison.Ordinal );
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add( base.GetHashCode() );
        hashCode.Add( Neutral );
        hashCode.Add( English );
        hashCode.Add( Spanish );
        hashCode.Add( French );
        hashCode.Add( Swedish );
        hashCode.Add( German );
        hashCode.Add( Chinese );
        hashCode.Add( Polish );
        hashCode.Add( Thai );
        hashCode.Add( Japanese );
        hashCode.Add( Czech );
        hashCode.Add( Portuguese );
        hashCode.Add( Dutch );
        hashCode.Add( Korean );
        hashCode.Add( Arabic );
        return hashCode.ToHashCode();
    }

    public ResxRowTable Update( string english,
                                string spanish,
                                string french,
                                string swedish,
                                string german,
                                string chinese,
                                string polish,
                                string thai,
                                string japanese,
                                string czech,
                                string portuguese,
                                string dutch,
                                string korean,
                                string arabic
    ) =>
        this with
        {
            English = english,
            Spanish = spanish,
            French = french,
            Swedish = swedish,
            German = german,
            Chinese = chinese,
            Polish = polish,
            Thai = thai,
            Japanese = japanese,
            Czech = czech,
            Portuguese = portuguese,
            Dutch = dutch,
            Korean = korean,
            Arabic = arabic,
            LastModified = DateTimeOffset.UtcNow,
        };


    public string GetValue( in SupportedLanguage language ) => language switch
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
                                                                   _                            => throw new OutOfRangeException( nameof(language), language ),
                                                               } ?? Neutral;
    public override bool Equals( ResxRowTable? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Neutral == other.Neutral;
    }
}
