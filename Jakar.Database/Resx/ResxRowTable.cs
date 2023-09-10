namespace Jakar.Database.Resx;


[ Serializable, Table( "Resx" ) ]
public sealed record ResxRowTable( long                   KeyID,
                                   string                 Key,
                                   string                 Neutral,
                                   string?                Arabic,
                                   string?                Chinese,
                                   string?                Czech,
                                   string?                Dutch,
                                   string?                English,
                                   string?                French,
                                   string?                German,
                                   string?                Japanese,
                                   string?                Korean,
                                   string?                Polish,
                                   string?                Portuguese,
                                   string?                Spanish,
                                   string?                Swedish,
                                   string?                Thai,
                                   RecordID<ResxRowTable> ID,
                                   RecordID<UserRecord>?  CreatedBy,
                                   Guid?                  OwnerUserID,
                                   DateTimeOffset         DateCreated,
                                   DateTimeOffset?        LastModified = default
) : TableRecord<ResxRowTable>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified )
{
    public ResxRowTable( string key, long keyID, UserRecord? caller = default ) : this( key, keyID, string.Empty, caller ) { }
    public ResxRowTable( string key, long keyID, string neutral, UserRecord? caller = default ) : this( key,
                                                                                                        keyID,
                                                                                                        neutral,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        string.Empty,
                                                                                                        caller ) { }
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
    ) : this( keyID,
              key,
              neutral,
              arabic,
              chinese,
              czech,
              dutch,
              english,
              french,
              german,
              japanese,
              korean,
              polish,
              portuguese,
              spanish,
              swedish,
              thai,
              RecordID<ResxRowTable>.New(),
              caller?.ID,
              caller?.UserID,
              DateTimeOffset.UtcNow ) { }


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
                                                                   SupportedLanguage.Unspecified => throw new OutOfRangeException( nameof(language), language ),
                                                                   _                             => throw new OutOfRangeException( nameof(language), language ),
                                                               } ??
                                                               Neutral;
    public override bool Equals( ResxRowTable? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Neutral == other.Neutral;
    }
}
