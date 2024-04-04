// Jakar.Extensions :: Jakar.SqlBuilder
// 3/1/2024  23:20

using System.Collections.ObjectModel;



namespace Jakar.Database.DbMigrations;


[Flags]
public enum ColumnOptions
{
    AlwaysIdentity  = 1 << 0,
    DefaultIdentity = 1 << 1,
    Indexed         = 1 << 2,
    Unique          = 1 << 3,
    PrimaryKey      = 1 << 4
}



public readonly record struct ColumnPrecisionMetaData( ushort Scope, ushort Precision );



public readonly record struct ColumnCheckMetaData( bool And, params string[] Checks )
{
    public static implicit operator ColumnCheckMetaData( string   check )  => new(true, check);
    public static implicit operator ColumnCheckMetaData( string[] checks ) => new(true, checks);
}



public readonly record struct ColumnMetaData( string Name, DbType DbType, bool IsNullable, OneOf<uint, ColumnPrecisionMetaData> Length = default, ColumnCheckMetaData? Check = default, ColumnOptions Options = 0 )
{
    // public bool    IsForeignKey { get; init; }
    // public bool   IsPrimaryKey   { get; init; }
    // public string PrimaryKeyName { get; init; }

    public string? IndexColumnName { get; init; }

    public static ColumnMetaData Nullable( string    name, DbType dbType, OneOf<uint, ColumnPrecisionMetaData> length = default, ColumnCheckMetaData? check = default, ColumnOptions options = 0 ) => new(name, dbType, true, length, check, options);
    public static ColumnMetaData NotNullable( string name, DbType dbType, OneOf<uint, ColumnPrecisionMetaData> length = default, ColumnCheckMetaData? check = default, ColumnOptions options = 0 ) => new(name, dbType, false, length, check, options);
    public static ColumnMetaData Indexed( string     name, string indexName ) => new(name, DbType.Int64, false) { IndexColumnName = indexName };


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool IsInvalidScopedPrecision() => Length is { IsT1: true, AsT1: { Precision: > SQL.DECIMAL_MAX_PRECISION, Scope: > SQL.DECIMAL_MAX_SCALE } };


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool IsValidLength() =>
        Length.IsT0 &&
        Length.AsT0 switch
        {
            0                                                                                           => false,
            > SQL.ANSI_CAPACITY when DbType is DbType.AnsiString or DbType.AnsiStringFixedLength => false,
            > SQL.UNICODE_CAPACITY when DbType is DbType.String or DbType.StringFixedLength      => false,
            _                                                                                           => true
        };
}



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
       TEXT: A large text data typeof(T), with column.Size limits depending on the DBMS.

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
       UUID/GUID: A special typeof(T) for storing Universally Unique Identifiers.
       JSON: Stores JSON data, allowing for complex data structures within a single database field.
       ARRAY: Supported by some databases like PostgreSQL, allowing storage of arrays.
       XML: For storing XML data, with some DBMS providing additional functions to manipulate XML data.

   Geography and Geometry Types (Spatial Types)

       POINT, LINESTRING, POLYGON, and more: These types are used for storing and manipulating spatial data, such as coordinates and shapes, especially in databases that support GIS (Geographic Information Systems).
 */
public ref struct SqlTableBuilder<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private Buffer<ColumnMetaData> _columns = new();

    public SqlTableBuilder() { }
    public static SqlTableBuilder<TRecord> Create() => new();
    public void Dispose()
    {
        _columns.Dispose();
        this = default;
    }


    public SqlTableBuilder<TRecord> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn( ColumnMetaData.Indexed( columnName, indexColumnName ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public SqlTableBuilder<TRecord> WithColumn<T>( string columnName, OneOf<uint, ColumnPrecisionMetaData> length = default, ColumnCheckMetaData? check = default, ColumnOptions options = 0 )
    {
        DbType         dbType = GetDataType<T>( out bool isNullable, ref length );
        ColumnMetaData column = new(columnName, dbType, isNullable, length, check, options);
        return WithColumn( column );
    }
    public SqlTableBuilder<TRecord> WithColumn( scoped in ColumnMetaData column )
    {
        _columns.Append( column );
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    private static DbType GetDataType<T>( out bool isNullable, ref OneOf<uint, ColumnPrecisionMetaData> length )
    {
        isNullable = typeof(T).IsNullableType() || typeof(T).IsBuiltInNullableType();

        if ( typeof(T) == typeof(byte[]) ) { return DbType.Binary; }

        if ( typeof(JToken).IsAssignableFrom( typeof(T) ) ) { return DbType.Object; }

        if ( typeof(IDictionary<string, JToken>).IsAssignableFrom( typeof(T) ) ) { return DbType.Object; }

        if ( typeof(IDictionary<string, JToken?>).IsAssignableFrom( typeof(T) ) ) { return DbType.Object; }

        if ( typeof(T).IsEnum )
        {
            length = Spans.Max<string, uint>( Enum.GetNames( typeof(T) ), static x => (uint)x.Length, 0 );
            return DbType.StringFixedLength;
        }

        if ( typeof(T) == typeof(string) ) { return DbType.String; }

        if ( typeof(T) == typeof(Int128) ) { return DbType.String; }

        if ( typeof(T) == typeof(Int128?) )
        {
            isNullable = true;
            return DbType.String;
        }

        if ( typeof(T) == typeof(UInt128) ) { return DbType.String; }

        if ( typeof(T) == typeof(UInt128?) )
        {
            isNullable = true;
            return DbType.String;
        }

        if ( typeof(T) == typeof(Guid) || typeof(T) == typeof(RecordID<TRecord>) ) { return DbType.Guid; }

        if ( typeof(T) == typeof(Guid?) || typeof(T) == typeof(RecordID<TRecord>?) )
        {
            isNullable = true;
            return DbType.Guid;
        }

        if ( typeof(T) == typeof(byte) ) { return DbType.Byte; }

        if ( typeof(T) == typeof(byte?) )
        {
            isNullable = true;
            return DbType.Byte;
        }

        if ( typeof(T) == typeof(short) ) { return DbType.Int16; }

        if ( typeof(T) == typeof(short?) )
        {
            isNullable = true;
            return DbType.Int16;
        }

        if ( typeof(T) == typeof(ushort) ) { return DbType.UInt16; }

        if ( typeof(T) == typeof(ushort?) )
        {
            isNullable = true;
            return DbType.UInt16;
        }

        if ( typeof(T) == typeof(int) ) { return DbType.Int32; }

        if ( typeof(T) == typeof(int?) )
        {
            isNullable = true;
            return DbType.Int32;
        }

        if ( typeof(T) == typeof(uint) ) { return DbType.UInt32; }

        if ( typeof(T) == typeof(uint?) )
        {
            isNullable = true;
            return DbType.UInt32;
        }

        if ( typeof(T) == typeof(long) ) { return DbType.Int64; }

        if ( typeof(T) == typeof(long?) )
        {
            isNullable = true;
            return DbType.Int64;
        }

        if ( typeof(T) == typeof(ulong) ) { return DbType.UInt64; }

        if ( typeof(T) == typeof(ulong?) )
        {
            isNullable = true;
            return DbType.UInt64;
        }

        if ( typeof(T) == typeof(float) ) { return DbType.Single; }

        if ( typeof(T) == typeof(float?) )
        {
            isNullable = true;
            return DbType.Single;
        }

        if ( typeof(T) == typeof(double) ) { return DbType.Double; }

        if ( typeof(T) == typeof(double?) )
        {
            isNullable = true;
            return DbType.Double;
        }

        if ( typeof(T) == typeof(decimal) ) { return DbType.Decimal; }

        if ( typeof(T) == typeof(decimal?) )
        {
            isNullable = true;
            return DbType.Decimal;
        }

        if ( typeof(T) == typeof(bool) ) { return DbType.Boolean; }

        if ( typeof(T) == typeof(bool?) )
        {
            isNullable = true;
            return DbType.Boolean;
        }

        if ( typeof(T) == typeof(DateOnly) ) { return DbType.Date; }

        if ( typeof(T) == typeof(DateOnly?) )
        {
            isNullable = true;
            return DbType.Date;
        }

        if ( typeof(T) == typeof(TimeOnly) ) { return DbType.Time; }

        if ( typeof(T) == typeof(TimeOnly?) )
        {
            isNullable = true;
            return DbType.Time;
        }

        if ( typeof(T) == typeof(TimeSpan) ) { return DbType.Time; }

        if ( typeof(T) == typeof(TimeSpan?) )
        {
            isNullable = true;
            return DbType.Time;
        }

        if ( typeof(T) == typeof(DateTime) ) { return DbType.DateTime2; }

        if ( typeof(T) == typeof(DateTime?) )
        {
            isNullable = true;
            return DbType.DateTime2;
        }

        if ( typeof(T) == typeof(DateTimeOffset) ) { return DbType.DateTimeOffset; }

        if ( typeof(T) == typeof(DateTimeOffset?) )
        {
            isNullable = true;
            return DbType.DateTimeOffset;
        }

        throw new ArgumentException( $"Unsupported typeof(T): {typeof(T).Name}" );
    }


    private static string GetDataType( scoped in DbInstance instance, scoped in ColumnMetaData column ) =>
        instance switch
        {
            DbInstance.MsSql    => GetDataTypeSqlServer( column ),
            DbInstance.Postgres => GetDataTypePostgresSql( column ),
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    private static string GetDataTypePostgresSql( scoped in ColumnMetaData column ) =>
        column.DbType switch
        {
            DbType.String or DbType.StringFixedLength when column.IsValidLength() is false                                => throw new OutOfRangeException( nameof(column.Length), column.Length, $"Max length for Unicode strings is {SQL.UNICODE_CAPACITY}" ),
            DbType.AnsiString or DbType.AnsiStringFixedLength when column.IsValidLength() is false                        => throw new OutOfRangeException( nameof(column.Length), column.Length, $"Max length for ANSI strings is {SQL.ANSI_CAPACITY}" ),
            DbType.VarNumeric when column.Length.IsT0 || column.Length.IsT1 is false || column.IsInvalidScopedPrecision() => throw new OutOfRangeException( nameof(column.Length), column.Length, $"Max deciamal scale is {SQL.DECIMAL_MAX_SCALE}. Max deciamal precision is {SQL.DECIMAL_MAX_PRECISION}" ),

            _ => column.DbType switch
                 {
                     DbType.Binary => column.Length.IsT0
                                          ? $"VARBINARY({column.Length.AsT0})"
                                          : "BLOB",
                     DbType.SByte => "bytea",
                     DbType.Byte  => "bytea",
                     DbType.AnsiString => column.Length.IsT0
                                              ? $"varchar({column.Length.AsT0})"
                                              : "varchar(MAX)",
                     DbType.String => column.Length.IsT0
                                          ? $"varchar({column.Length.AsT0})"
                                          : "text",
                     DbType.AnsiStringFixedLength              => $"char({column.Length})",
                     DbType.StringFixedLength                  => $"char({column.Length})",
                     DbType.Guid                               => "uuid",
                     DbType.Int16                              => "smallint",
                     DbType.Int32                              => "integer",
                     DbType.Int64                              => "bigint",
                     DbType.UInt16                             => "smallint",
                     DbType.UInt32                             => "integer",
                     DbType.UInt64                             => "bigint",
                     DbType.Single                             => "float4",
                     DbType.Double                             => "float8",
                     DbType.Decimal                            => "decimal(19, 5)",
                     DbType.VarNumeric when column.Length.IsT1 => $"decimal({column.Length.AsT1.Precision}, {column.Length.AsT1.Scope})",
                     DbType.VarNumeric                         => $"decimal({SQL.DECIMAL_MAX_PRECISION}, {SQL.DECIMAL_MAX_SCALE})",
                     DbType.Boolean                            => "bool",
                     DbType.Date                               => "date",
                     DbType.Time                               => "time",
                     DbType.DateTime                           => "timestamp",
                     DbType.DateTime2                          => "timestamp",
                     DbType.DateTimeOffset                     => "timestamptz",
                     DbType.Currency                           => "money",
                     DbType.Object                             => "json",
                     DbType.Xml                                => "xml",
                     _                                         => throw new OutOfRangeException( nameof(column.DbType), column.DbType )
                 }
        };


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    private static string GetDataTypeSqlServer( scoped in ColumnMetaData column ) =>
        column.DbType switch
        {
            DbType.String or DbType.StringFixedLength when column.Length is { IsT0        : true, AsT0: > SQL.UNICODE_CAPACITY } => throw new OutOfRangeException( nameof(column.Length), column.Length, $"Max length for Unicode strings is {SQL.UNICODE_CAPACITY}" ),
            DbType.AnsiString or DbType.AnsiStringFixedLength when column.Length is { IsT0: true, AsT0: > SQL.ANSI_CAPACITY }    => throw new OutOfRangeException( nameof(column.Length), column.Length, $"Max length for ANSI strings is {SQL.ANSI_CAPACITY}" ),
            DbType.VarNumeric when column.Length.IsT0 || column.Length.IsT1 is false || column.IsInvalidScopedPrecision()               => throw new OutOfRangeException( nameof(column.Length), column.Length, $"Max deciamal scale is {SQL.DECIMAL_MAX_SCALE}. Max deciamal precision is {SQL.DECIMAL_MAX_PRECISION}" ),
            _ => column.DbType switch
                 {
                     DbType.Binary => column.Length.IsT0
                                          ? $"VARBINARY({column.Length.AsT0})"
                                          : "BLOB",
                     DbType.SByte => "TINYINT",
                     DbType.Byte  => "TINYINT",
                     DbType.AnsiString => column.Length.IsT0
                                              ? $"VARCHAR({column.Length.AsT0})"
                                              : "NVARCHAR(MAX)",
                     DbType.String => column.Length.IsT0
                                          ? $"NVARCHAR({column.Length.AsT0})"
                                          : "NVARCHAR(MAX)",
                     DbType.AnsiStringFixedLength              => $"CHAR({column.Length})",
                     DbType.StringFixedLength                  => $"NCHAR({column.Length})",
                     DbType.Guid                               => "GUID",
                     DbType.Int16                              => "SMALLINT",
                     DbType.Int32                              => "INT",
                     DbType.Int64                              => "BIGINT",
                     DbType.UInt16                             => "SMALLINT",
                     DbType.UInt32                             => "INT",
                     DbType.UInt64                             => "BIGINT",
                     DbType.Single                             => "FLOAT(9)",
                     DbType.Double                             => "DOUBLE PRECISION",
                     DbType.Decimal                            => "DECIMAL(19, 5)",
                     DbType.VarNumeric when column.Length.IsT1 => $"DECIMAL({column.Length.AsT1.Precision}, {column.Length.AsT1.Scope})",
                     DbType.VarNumeric                         => $"DECIMAL({SQL.DECIMAL_MAX_PRECISION}, {SQL.DECIMAL_MAX_SCALE})",
                     DbType.Boolean                            => "BOOLEAN",
                     DbType.Date                               => "DATE",
                     DbType.Time                               => "TIME",
                     DbType.DateTime                           => "DATETIME2",
                     DbType.DateTime2                          => "DATETIME2",
                     DbType.DateTimeOffset                     => "TIMESTAMP",
                     DbType.Currency                           => "MONEY",
                     DbType.Object                             => "JSON",
                     DbType.Xml                                => "XML",
                     _                                         => throw new OutOfRangeException( nameof(column.DbType), column.DbType )
                 }
        };


    private string BuildInternal( scoped in DbInstance instance )
    {
        using ValueStringBuilder query = new(10240);

        query.Append( $"CREATE TABLE {TRecord.TableName} (" );

        foreach ( ColumnMetaData column in _columns )
        {
            if ( column.Options.HasFlag( ColumnOptions.Indexed ) )
            {
                query.Append( column.IndexColumnName ?? $"{column.Name}_index" );
                query.Append( " ON " );
                query.Append( TRecord.TableName );
                query.Append( " (" );
                query.Append( column.Name );
                query.Append( ");" );
                continue;
            }

            string dataType = GetDataType( instance, column );

            query.Append( query.Span.EndsWith( '(' )
                              ? "\n    "
                              : ",\n    " );

            query.Append( column.Name );
            query.Append( " " );
            query.Append( dataType );

            if ( column.Check.HasValue )
            {
                query.Append( " CHECK ( " );

                query.AppendJoin( column.Check.Value.And
                                      ? SQL.AND
                                      : SQL.OR,
                                  column.Check.Value.Checks );

                query.Append( " )" );
            }

            if ( column.Options.HasFlag( ColumnOptions.Unique ) ) { query.Append( " UNIQUE" ); }

            query.Append( column.IsNullable
                              ? " NULL"
                              : " NOT NULL" );

            if ( column.Options.HasFlag( ColumnOptions.AlwaysIdentity ) ) { query.Append( " GENERATED ALWAYS AS IDENTITY" ); }

            else if ( column.Options.HasFlag( ColumnOptions.DefaultIdentity ) ) { query.Append( " GENERATED BY DEFAULT AS IDENTITY" ); }

            if ( column.Options.HasFlag( ColumnOptions.PrimaryKey ) ) { query.Append( " PRIMARY KEY" ); }
        }

        // return query.Trim( ' ' ).TrimEnd( ',' ).TrimEnd( '(' ).Append( "\n );" ).ToString();
        return query.ToString();
    }


    public string Build( scoped in DbInstance instance )
    {
        try { return BuildInternal( instance ); }
        finally { Dispose(); }
    }
    public ReadOnlyDictionary<DbInstance, string> Build()
    {
        try
        {
            Dictionary<DbInstance, string> dictionary = new(2)
                                                        {
                                                            { DbInstance.MsSql, BuildInternal( DbInstance.MsSql ) },
                                                            { DbInstance.Postgres, BuildInternal( DbInstance.Postgres ) }
                                                        };

            return new ReadOnlyDictionary<DbInstance, string>( dictionary );
        }
        finally { Dispose(); }
    }
}
