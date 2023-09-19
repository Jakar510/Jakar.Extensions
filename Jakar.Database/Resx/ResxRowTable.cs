namespace Jakar.Database.Resx;


/// <see cref="LocalizableString"/>
[ Serializable, Table( "Resx" ) ]
public sealed partial record ResxRowRecord( long                    KeyID,
                                            string                  Key,
                                            string                  Neutral,
                                            string?                 Arabic,
                                            string?                 Chinese,
                                            string?                 Czech,
                                            string?                 Dutch,
                                            string?                 English,
                                            string?                 French,
                                            string?                 German,
                                            string?                 Japanese,
                                            string?                 Korean,
                                            string?                 Polish,
                                            string?                 Portuguese,
                                            string?                 Spanish,
                                            string?                 Swedish,
                                            string?                 Thai,
                                            RecordID<ResxRowRecord> ID,
                                            RecordID<UserRecord>?   CreatedBy,
                                            Guid?                   OwnerUserID,
                                            DateTimeOffset          DateCreated,
                                            DateTimeOffset?         LastModified = default
) : TableRecord<ResxRowRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<ResxRowRecord>
{
    public ResxRowRecord( string key, long keyID, UserRecord? caller = default ) : this( key, keyID, string.Empty, caller ) { }
    public ResxRowRecord( string key, long keyID, string neutral, UserRecord? caller = default ) : this( key,
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
    public ResxRowRecord( string      key,
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
              RecordID<ResxRowRecord>.New(),
              caller?.ID,
              caller?.UserID,
              DateTimeOffset.UtcNow ) { }

    public static ResxRowRecord Create( DbDataReader reader )
    {
        var keyID        = reader.GetFieldValue<long>( nameof(KeyID) );
        var key          = reader.GetString( nameof(Key) );
        var neutral      = reader.GetString( nameof(Neutral) );
        var english      = reader.GetString( nameof(English) );
        var spanish      = reader.GetString( nameof(Spanish) );
        var french       = reader.GetString( nameof(French) );
        var swedish      = reader.GetString( nameof(Swedish) );
        var german       = reader.GetString( nameof(German) );
        var chinese      = reader.GetString( nameof(Chinese) );
        var polish       = reader.GetString( nameof(Polish) );
        var thai         = reader.GetString( nameof(Thai) );
        var japanese     = reader.GetString( nameof(Japanese) );
        var czech        = reader.GetString( nameof(Czech) );
        var portuguese   = reader.GetString( nameof(Portuguese) );
        var dutch        = reader.GetString( nameof(Dutch) );
        var korean       = reader.GetString( nameof(Korean) );
        var arabic       = reader.GetString( nameof(Arabic) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id           = new RecordID<ResxRowRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );

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
                                  createdBy,
                                  ownerUserID,
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


    public ResxRowRecord Update( string english,
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
    public bool Equals( ResxRowRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Neutral == other.Neutral || ID == other.ID;
    }
}
