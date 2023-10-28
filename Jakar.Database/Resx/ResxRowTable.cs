namespace Jakar.Database.Resx;


/// <see cref="LocalizableString"/>
[ Serializable, Table( "Resx" ) ]
public sealed record ResxRowRecord( long                    KeyID,
                                    string                  Key,
                                    string                  Neutral,
                                    string                  Arabic,
                                    string                  Chinese,
                                    string                  Czech,
                                    string                  Dutch,
                                    string                  English,
                                    string                  French,
                                    string                  German,
                                    string                  Japanese,
                                    string                  Korean,
                                    string                  Polish,
                                    string                  Portuguese,
                                    string                  Spanish,
                                    string                  Swedish,
                                    string                  Thai,
                                    RecordID<ResxRowRecord> ID,
                                    DateTimeOffset          DateCreated,
                                    DateTimeOffset?         LastModified = default
) : TableRecord<ResxRowRecord>( ID, DateCreated, LastModified ), IDbReaderMapping<ResxRowRecord>
{
    public static string TableName { get; } = typeof(ResxRowRecord).GetTableName();


    public ResxRowRecord( string key, long keyID ) : this( key, keyID, string.Empty ) { }
    public ResxRowRecord( string key, long keyID, string neutral ) : this( key,
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
                                                                           string.Empty ) { }
    public ResxRowRecord( string key,
                          long   keyID,
                          string neutral,
                          string english,
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
              RecordID<ResxRowRecord>.New(),
              DateTimeOffset.UtcNow ) { }

    public static ResxRowRecord Create( DbDataReader reader )
    {
        long   keyID        = reader.GetFieldValue<long>( nameof(KeyID) );
        string key          = reader.GetString( nameof(Key) );
        string neutral      = reader.GetString( nameof(Neutral) );
        string english      = reader.GetString( nameof(English) );
        string spanish      = reader.GetString( nameof(Spanish) );
        string french       = reader.GetString( nameof(French) );
        string swedish      = reader.GetString( nameof(Swedish) );
        string german       = reader.GetString( nameof(German) );
        string chinese      = reader.GetString( nameof(Chinese) );
        string polish       = reader.GetString( nameof(Polish) );
        string thai         = reader.GetString( nameof(Thai) );
        string japanese     = reader.GetString( nameof(Japanese) );
        string czech        = reader.GetString( nameof(Czech) );
        string portuguese   = reader.GetString( nameof(Portuguese) );
        string dutch        = reader.GetString( nameof(Dutch) );
        string korean       = reader.GetString( nameof(Korean) );
        string arabic       = reader.GetString( nameof(Arabic) );
        var    dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var    lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var    id           = new RecordID<ResxRowRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

        return new ResxRowRecord( keyID,
                                  key,
                                  neutral,
                                  english,
                                  spanish,
                                  french,
                                  swedish,
                                  german,
                                  chinese,
                                  polish,
                                  thai,
                                  japanese,
                                  czech,
                                  portuguese,
                                  dutch,
                                  korean,
                                  arabic,
                                  id,
                                  dateCreated,
                                  lastModified );
    }
    public static async IAsyncEnumerable<ResxRowRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public override int CompareTo( ResxRowRecord? other )
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


    public ResxRowRecord With( string english,
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
    ) => this with
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
             LastModified = DateTimeOffset.UtcNow
         };

    public ResxString ToResxString() => new(Neutral,
                                            Arabic,
                                            Chinese,
                                            Czech,
                                            Dutch,
                                            English,
                                            French,
                                            German,
                                            Japanese,
                                            Korean,
                                            Polish,
                                            Portuguese,
                                            Spanish,
                                            Swedish,
                                            Thai);

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
                                                                   _                             => throw new OutOfRangeException( nameof(language), language )
                                                               } ??
                                                               Neutral;
    public bool Equals( ResxRowRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Neutral == other.Neutral || ID == other.ID;
    }
}
