// Jakar.Extensions :: Jakar.SqlBuilder
// 3/1/2024  23:20

namespace Jakar.Database.DbMigrations;


/*
 Numeric Types

       INTEGER: A whole number, varying in column.Size from SMALLINT to BIGINT depending on the storage column.Size and range.
       DECIMAL(Precision, Scale): A fixed-point number with a specified number of digits before and after the decimal point.
       NUMERIC(Precision, Scale): Similar to DECIMAL, often used interchangeably.
       FLOAT(Precision): A floating-point number with machine-dependent precision.
       REAL and DOUBLE PRECISION: Floating-point numbers with more precision than FLOAT.

   String Types

       CHAR(Size): A fixed-length character string, space-padded.
       VARCHAR(Size): A variable-length character string.
       TEXT: A large text data typeof(TValue), with column.Size limits depending on the DBMS.

   Date and Time Types

       DATE: Stores a date (year, month, day).
       TIME: Stores a time of day without a date.
       DATETIME: Stores both date and time.
       TIMESTAMP: Stores a timestamp, often used for recording when a row was last updated.

   Binary Types

       BINARY(Size): Similar to CHAR, but stores binary bytes.
       VARBINARY(Size): Similar to VARCHAR, but for binary bytes.
       BLOB: Binary Large Object, a large piece of binary data, such as images or audio.

   Boolean Type

       BOOLEAN: Stores TRUE or FALSE.

   Specialized Types

       ENUM: A string object that can have only one value chosen from a list of predefined values.
       SET: Similar to ENUM, but can store multiple values from a predefined list.
       UUID/GUID: A special typeof(TValue) for storing Universally Unique Identifiers.
       JSON: Stores JSON data, allowing for complex data structures within a single database field.
       ARRAY: Supported by some databases like PostgreSQL, allowing storage of arrays.
       XML: For storing XML data, with some DBMS providing additional functions to manipulate XML data.

   Geography and Geometry Types (Spatial Types)

       POINT, LINESTRING, POLYGON, and more: These types are used for storing and manipulating spatial data, such as coordinates and shapes, especially in databases that support GIS (Geographic Information Systems).
 */



[Experimental( "SqlTableBuilder" )]
[Flags]
public enum ColumnOptions
{
    AlwaysIdentity  = 1 << 0,
    DefaultIdentity = 1 << 1,
    Indexed         = 1 << 2,
    Unique          = 1 << 3,
    PrimaryKey      = 1 << 4
}



[Experimental( "SqlTableBuilder" )] public readonly record struct ColumnPrecisionMetaData( ushort Scope, ushort Precision );



[Experimental( "SqlTableBuilder" )]
public readonly record struct ColumnCheckMetaData( bool And, params string[] Checks )
{
    public static implicit operator ColumnCheckMetaData( string   check )  => new(true, check);
    public static implicit operator ColumnCheckMetaData( string[] checks ) => new(true, checks);
}



[Experimental( "SqlTableBuilder" )]
public readonly record struct ColumnMetaData( string Name, DbType DbType, bool IsNullable, OneOf<uint, ColumnPrecisionMetaData> Length = default, ColumnCheckMetaData? Check = null, ColumnOptions Options = 0 )
{
    // public bool    IsForeignKey { get; init; }
    // public bool   IsPrimaryKey   { get; init; }
    // public string PrimaryKeyName { get; init; }

    public string? IndexColumnName { get; init; }

    public static ColumnMetaData Nullable( string    name, DbType dbType, OneOf<uint, ColumnPrecisionMetaData> length = default, ColumnCheckMetaData? check = null, ColumnOptions options = 0 ) => new(name, dbType, true, length, check, options);
    public static ColumnMetaData NotNullable( string name, DbType dbType, OneOf<uint, ColumnPrecisionMetaData> length = default, ColumnCheckMetaData? check = null, ColumnOptions options = 0 ) => new(name, dbType, false, length, check, options);
    public static ColumnMetaData Indexed( string     name, string indexName ) => new(name, DbType.Int64, false) { IndexColumnName = indexName };


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool IsInvalidScopedPrecision() => Length is { IsT1: true, AsT1: { Precision: > Constants.DECIMAL_MAX_PRECISION, Scope: > Constants.DECIMAL_MAX_SCALE } };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool IsValidLength() =>
        Length.IsT0 &&
        Length.AsT0 switch
        {
            0                                                                                    => false,
            > Constants.ANSI_CAPACITY when DbType is DbType.AnsiString or DbType.AnsiStringFixedLength => false,
            > Constants.UNICODE_CAPACITY when DbType is DbType.String or DbType.StringFixedLength      => false,
            _                                                                                    => true
        };


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    public string GetDataTypeSqlServer() =>
        DbType switch
        {
            DbType.String or DbType.StringFixedLength when Length is { IsT0        : true, AsT0: > Constants.UNICODE_CAPACITY } => throw new OutOfRangeException( Length, $"Max length for Unicode strings is {Constants.UNICODE_CAPACITY}" ),
            DbType.AnsiString or DbType.AnsiStringFixedLength when Length is { IsT0: true, AsT0: > Constants.ANSI_CAPACITY }    => throw new OutOfRangeException( Length, $"Max length for ANSI strings is {Constants.ANSI_CAPACITY}" ),
            DbType.VarNumeric when Length.IsT0 || Length.IsT1 is false || IsInvalidScopedPrecision()                      => throw new OutOfRangeException( Length, $"Max deciamal scale is {Constants.DECIMAL_MAX_SCALE}. Max deciamal precision is {Constants.DECIMAL_MAX_PRECISION}" ),
            _ => DbType switch
                 {
                     DbType.Binary => Length.IsT0
                                          ? $"VARBINARY({Length.AsT0})"
                                          : "BLOB",
                     DbType.SByte => "TINYINT",
                     DbType.Byte  => "TINYINT",
                     DbType.AnsiString => Length.IsT0
                                              ? $"VARCHAR({Length.AsT0})"
                                              : "NVARCHAR(MAX)",
                     DbType.String => Length.IsT0
                                          ? $"NVARCHAR({Length.AsT0})"
                                          : "NVARCHAR(MAX)",
                     DbType.AnsiStringFixedLength       => $"CHAR({Length})",
                     DbType.StringFixedLength           => $"NCHAR({Length})",
                     DbType.Guid                        => "GUID",
                     DbType.Int16                       => "SMALLINT",
                     DbType.Int32                       => "INT",
                     DbType.Int64                       => "BIGINT",
                     DbType.UInt16                      => "SMALLINT",
                     DbType.UInt32                      => "INT",
                     DbType.UInt64                      => "BIGINT",
                     DbType.Single                      => "FLOAT(9)",
                     DbType.Double                      => "DOUBLE PRECISION",
                     DbType.Decimal                     => "DECIMAL(19, 5)",
                     DbType.VarNumeric when Length.IsT1 => $"DECIMAL({Length.AsT1.Precision}, {Length.AsT1.Scope})",
                     DbType.VarNumeric                  => $"DECIMAL({Constants.DECIMAL_MAX_PRECISION}, {Constants.DECIMAL_MAX_SCALE})",
                     DbType.Boolean                     => "BOOLEAN",
                     DbType.Date                        => "DATE",
                     DbType.Time                        => "TIME",
                     DbType.DateTime                    => "DATETIME2",
                     DbType.DateTime2                   => "DATETIME2",
                     DbType.DateTimeOffset              => "TIMESTAMP",
                     DbType.Currency                    => "MONEY",
                     DbType.Object                      => "JSON",
                     DbType.Xml                         => "XML",
                     _                                  => throw new OutOfRangeException( DbType )
                 }
        };


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    public string GetDataTypePostgresSql() =>
        DbType switch
        {
            DbType.String or DbType.StringFixedLength when IsValidLength() is false                  => throw new OutOfRangeException( Length, $"Max length for Unicode strings is {Constants.UNICODE_CAPACITY}" ),
            DbType.AnsiString or DbType.AnsiStringFixedLength when IsValidLength() is false          => throw new OutOfRangeException( Length, $"Max length for ANSI strings is {Constants.ANSI_CAPACITY}" ),
            DbType.VarNumeric when Length.IsT0 || Length.IsT1 is false || IsInvalidScopedPrecision() => throw new OutOfRangeException( Length, $"Max deciamal scale is {Constants.DECIMAL_MAX_SCALE}. Max deciamal precision is {Constants.DECIMAL_MAX_PRECISION}" ),

            _ => DbType switch
                 {
                     DbType.Binary => Length.IsT0
                                          ? $"VARBINARY({Length.AsT0})"
                                          : "BLOB",
                     DbType.SByte => "bytea",
                     DbType.Byte  => "bytea",
                     DbType.AnsiString => Length.IsT0
                                              ? $"varchar({Length.AsT0})"
                                              : "varchar(MAX)",
                     DbType.String => Length.IsT0
                                          ? $"varchar({Length.AsT0})"
                                          : "text",
                     DbType.AnsiStringFixedLength       => $"char({Length})",
                     DbType.StringFixedLength           => $"char({Length})",
                     DbType.Guid                        => "uuid",
                     DbType.Int16                       => "smallint",
                     DbType.Int32                       => "integer",
                     DbType.Int64                       => "bigint",
                     DbType.UInt16                      => "smallint",
                     DbType.UInt32                      => "integer",
                     DbType.UInt64                      => "bigint",
                     DbType.Single                      => "float4",
                     DbType.Double                      => "float8",
                     DbType.Decimal                     => "decimal(19, 5)",
                     DbType.VarNumeric when Length.IsT1 => $"decimal({Length.AsT1.Precision}, {Length.AsT1.Scope})",
                     DbType.VarNumeric                  => $"decimal({Constants.DECIMAL_MAX_PRECISION}, {Constants.DECIMAL_MAX_SCALE})",
                     DbType.Boolean                     => "bool",
                     DbType.Date                        => "date",
                     DbType.Time                        => "time",
                     DbType.DateTime                    => "timestamp",
                     DbType.DateTime2                   => "timestamp",
                     DbType.DateTimeOffset              => "timestamptz",
                     DbType.Currency                    => "money",
                     DbType.Object                      => "json",
                     DbType.Xml                         => "xml",
                     _                                  => throw new OutOfRangeException( DbType )
                 }
        };
}



[Experimental( "SqlTableBuilder" )]
public ref struct SqlTableBuilder<TClass>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    private Buffer<ColumnMetaData> _columns = new();

    public SqlTableBuilder() { }
    public static SqlTableBuilder<TClass> Create() => new();
    public void Dispose()
    {
        _columns.Dispose();
        this = default;
    }


    public SqlTableBuilder<TClass> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn( ColumnMetaData.Indexed( columnName, indexColumnName ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public SqlTableBuilder<TClass> WithColumn<TValue>( string columnName, OneOf<uint, ColumnPrecisionMetaData> length = default, ColumnCheckMetaData? check = null, ColumnOptions options = 0 )
    {
        DbType         dbType = GetDataType<TValue>( out bool isNullable, ref length );
        ColumnMetaData column = new(columnName, dbType, isNullable, length, check, options);
        return WithColumn( column );
    }
    public SqlTableBuilder<TClass> WithColumn( ColumnMetaData column )
    {
        _columns.Add( column );
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    private static DbType GetDataType<TValue>( out bool isNullable, ref OneOf<uint, ColumnPrecisionMetaData> length )
    {
        isNullable = typeof(TValue).IsNullableType() || typeof(TValue).IsBuiltInNullableType();

        if ( typeof(TValue) == typeof(byte[]) ) { return DbType.Binary; }

        if ( typeof(JToken).IsAssignableFrom( typeof(TValue) ) ) { return DbType.Object; }

        if ( typeof(IDictionary<string, JToken>).IsAssignableFrom( typeof(TValue) ) ) { return DbType.Object; }

        if ( typeof(IDictionary<string, JToken?>).IsAssignableFrom( typeof(TValue) ) ) { return DbType.Object; }

        if ( typeof(TValue).IsEnum )
        {
            length = Spans.Max<string, uint>( Enum.GetNames( typeof(TValue) ), static x => (uint)x.Length, 0 );
            return DbType.StringFixedLength;
        }

        if ( typeof(TValue) == typeof(string) ) { return DbType.String; }

        if ( typeof(TValue) == typeof(Int128) ) { return DbType.String; }

        if ( typeof(TValue) == typeof(Int128?) )
        {
            isNullable = true;
            return DbType.String;
        }

        if ( typeof(TValue) == typeof(UInt128) ) { return DbType.String; }

        if ( typeof(TValue) == typeof(UInt128?) )
        {
            isNullable = true;
            return DbType.String;
        }

        if ( typeof(TValue) == typeof(Guid) || typeof(TValue) == typeof(RecordID<TClass>) ) { return DbType.Guid; }

        if ( typeof(TValue) == typeof(Guid?) || typeof(TValue) == typeof(RecordID<TClass>?) )
        {
            isNullable = true;
            return DbType.Guid;
        }

        if ( typeof(TValue) == typeof(byte) ) { return DbType.Byte; }

        if ( typeof(TValue) == typeof(byte?) )
        {
            isNullable = true;
            return DbType.Byte;
        }

        if ( typeof(TValue) == typeof(short) ) { return DbType.Int16; }

        if ( typeof(TValue) == typeof(short?) )
        {
            isNullable = true;
            return DbType.Int16;
        }

        if ( typeof(TValue) == typeof(ushort) ) { return DbType.UInt16; }

        if ( typeof(TValue) == typeof(ushort?) )
        {
            isNullable = true;
            return DbType.UInt16;
        }

        if ( typeof(TValue) == typeof(int) ) { return DbType.Int32; }

        if ( typeof(TValue) == typeof(int?) )
        {
            isNullable = true;
            return DbType.Int32;
        }

        if ( typeof(TValue) == typeof(uint) ) { return DbType.UInt32; }

        if ( typeof(TValue) == typeof(uint?) )
        {
            isNullable = true;
            return DbType.UInt32;
        }

        if ( typeof(TValue) == typeof(long) ) { return DbType.Int64; }

        if ( typeof(TValue) == typeof(long?) )
        {
            isNullable = true;
            return DbType.Int64;
        }

        if ( typeof(TValue) == typeof(ulong) ) { return DbType.UInt64; }

        if ( typeof(TValue) == typeof(ulong?) )
        {
            isNullable = true;
            return DbType.UInt64;
        }

        if ( typeof(TValue) == typeof(float) ) { return DbType.Single; }

        if ( typeof(TValue) == typeof(float?) )
        {
            isNullable = true;
            return DbType.Single;
        }

        if ( typeof(TValue) == typeof(double) ) { return DbType.Double; }

        if ( typeof(TValue) == typeof(double?) )
        {
            isNullable = true;
            return DbType.Double;
        }

        if ( typeof(TValue) == typeof(decimal) ) { return DbType.Decimal; }

        if ( typeof(TValue) == typeof(decimal?) )
        {
            isNullable = true;
            return DbType.Decimal;
        }

        if ( typeof(TValue) == typeof(bool) ) { return DbType.Boolean; }

        if ( typeof(TValue) == typeof(bool?) )
        {
            isNullable = true;
            return DbType.Boolean;
        }

        if ( typeof(TValue) == typeof(DateOnly) ) { return DbType.Date; }

        if ( typeof(TValue) == typeof(DateOnly?) )
        {
            isNullable = true;
            return DbType.Date;
        }

        if ( typeof(TValue) == typeof(TimeOnly) ) { return DbType.Time; }

        if ( typeof(TValue) == typeof(TimeOnly?) )
        {
            isNullable = true;
            return DbType.Time;
        }

        if ( typeof(TValue) == typeof(TimeSpan) ) { return DbType.Time; }

        if ( typeof(TValue) == typeof(TimeSpan?) )
        {
            isNullable = true;
            return DbType.Time;
        }

        if ( typeof(TValue) == typeof(DateTime) ) { return DbType.DateTime2; }

        if ( typeof(TValue) == typeof(DateTime?) )
        {
            isNullable = true;
            return DbType.DateTime2;
        }

        if ( typeof(TValue) == typeof(DateTimeOffset) ) { return DbType.DateTimeOffset; }

        if ( typeof(TValue) == typeof(DateTimeOffset?) )
        {
            isNullable = true;
            return DbType.DateTimeOffset;
        }

        throw new ArgumentException( $"Unsupported typeof(TValue): {typeof(TValue).Name}" );
    }


    private string BuildInternal()
    {
        ReadOnlySpan<ColumnMetaData> columns = _columns.Values;
        StringBuilder                query   = new(10240);

        query.Append( "CREATE TABLE " );
        query.Append( TClass.TableName );
        query.Append( " (" );

        foreach ( ColumnMetaData column in columns )
        {
            if ( column.Options.HasFlag( ColumnOptions.Indexed ) )
            {
                query.Append( column.IndexColumnName ?? $"{column.Name}_index" );
                query.Append( " ON " );
                query.Append( TClass.TableName );
                query.Append( " (" );
                query.Append( column.Name );
                query.Append( ");" );
                continue;
            }

            string dataType = column.GetDataTypePostgresSql();

            query.Append( query[^1] == '('
                              ? "\n    "
                              : ",\n    " );

            query.Append( column.Name );
            query.Append( ' ' );
            query.Append( dataType );

            if ( column.Check.HasValue )
            {
                query.Append( " CHECK ( " );

                query.AppendJoin( column.Check.Value.And
                                      ? Constants.AND
                                      : Constants.OR,
                                  column.Check.Value.Checks );

                query.Append( " )" );
            }

            if ( HasFlag( column.Options, ColumnOptions.Unique ) ) { query.Append( " UNIQUE" ); }

            query.Append( column.IsNullable
                              ? " NULL"
                              : " NOT NULL" );

            if ( HasFlag( column.Options, ColumnOptions.AlwaysIdentity ) ) { query.Append( " GENERATED ALWAYS AS IDENTITY" ); }

            else if ( HasFlag( column.Options, ColumnOptions.DefaultIdentity ) ) { query.Append( " GENERATED BY DEFAULT AS IDENTITY" ); }

            if ( HasFlag( column.Options, ColumnOptions.PrimaryKey ) ) { query.Append( " PRIMARY KEY" ); }
        }

        // return query.Trim( ' ' ).TrimEnd( ',' ).TrimEnd( '(' ).Append( "\n );" ).ToString();
        return query.ToString();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool HasFlag( ColumnOptions options, ColumnOptions flag ) => (options & flag) != 0;


    public string Build()
    {
        try { return BuildInternal(); }
        finally { Dispose(); }
    }
}
