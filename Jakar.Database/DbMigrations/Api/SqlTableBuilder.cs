// Jakar.Extensions :: Jakar.SqlBuilder
// 3/1/2024  23:20

namespace Jakar.Database.DbMigrations;


public enum ColumnIdentityOptions
{
    Always,
    Default
}



public readonly record struct ColumnCheckMetaData( bool And, params string[] Checks )
{
    internal void Write( ref ValueStringBuilder query )
    {
        query.Append( " CHECK ( " );

        query.AppendJoin( And
                              ? SQL.AND
                              : SQL.OR,
                          Checks );

        query.Append( $" )" );
    }
}



public readonly record struct ColumnMetaData( string                 Name,
                                              DbType                 DbType,
                                              bool                   IsNullable,
                                              int?                   Size         = default,
                                              int?                   Precision    = default,
                                              ColumnCheckMetaData?   Check        = default,
                                              ColumnIdentityOptions? Identity     = default,
                                              bool                   IsIndexed    = false,
                                              bool                   IsUnique     = false,
                                              bool                   IsPrimaryKey = false )
{
    // public bool    IsForeignKey { get; init; }
    // public bool   IsPrimaryKey   { get; init; }
    // public string PrimaryKeyName { get; init; }
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
    public const     int                ANSI_STRING_CAPACITY    = 8000;
    public const     int                ANSI_TEXT_CAPACITY      = 2147483647;
    public const     int                UNICODE_STRING_CAPACITY = 4000;
    public const     int                UNICODE_TEXT_CAPACITY   = 1073741823;
    public const     int                IMAGE_CAPACITY          = 2147483647;
    public const     int                DECIMAL_CAPACITY        = 38;
    private readonly DbInstance         _instance;
    private          ValueStringBuilder _query = new(10240);


    public SqlTableBuilder() => throw new NotImplementedException();
    private SqlTableBuilder( scoped in DbInstance instance, scoped in ReadOnlySpan<char> firstLine )
    {
        _instance = instance;
        _query.Append( firstLine );
    }
    public static SqlTableBuilder<TRecord> Create( DbInstance instance ) => new(instance, $"CREATE TABLE {TRecord.TableName} (");


    public SqlTableBuilder<TRecord> WithColumn<T>( string columnName, ColumnCheckMetaData? check = default, int? size = default, int? precision = default, ColumnIdentityOptions? identity = default, bool isIndexed = false, bool isUnique = false, bool isPrimaryKey = false )
    {
        DbType dbType = GetDataType<T>( out bool isNullable, ref size );
        return WithColumn( columnName, dbType, isNullable, check, size, precision, identity, isIndexed, isUnique, isPrimaryKey );
    }
    public SqlTableBuilder<TRecord> WithColumn( string columnName, DbType dbType, bool isNullable, ColumnCheckMetaData? check = default, int? size = default, int? precision = default, ColumnIdentityOptions? identity = default, bool isIndexed = false, bool isUnique = false, bool isPrimaryKey = false )
    {
        ColumnMetaData column = new(columnName, dbType, isNullable, size, precision, check, identity, isIndexed, isUnique, isPrimaryKey);
        return WithColumn( column );
    }
    public SqlTableBuilder<TRecord> WithIndexColumn( string indexColumnName, string columnName )
    {
        _query.Append( $"CREATE INDEX {indexColumnName}_index ON {TRecord.TableName} ({columnName});" );
        return this;
    }
    public SqlTableBuilder<TRecord> WithColumn( scoped in ColumnMetaData column )
    {
        if ( column.IsIndexed ) { return WithIndexColumn( $"{column.Name}_index", column.Name ); }

        string dataType = GetDataType( _instance, column );

        _query.Append( _query.Span.EndsWith( "(" )
                           ? $" {column.Name} {dataType}"
                           : $", {column.Name} {dataType}" );

        column.Check?.Write( ref _query );

        if ( column.IsUnique ) { _query.Append( " UNIQUE" ); }

        _query.Append( column.IsNullable
                           ? " NOT NULL"
                           : " NULL" );

        if ( column.Identity.HasValue )
        {
            _query.Append( column.Identity.Value switch
                           {
                               ColumnIdentityOptions.Always  => " GENERATED ALWAYS AS IDENTITY",
                               ColumnIdentityOptions.Default => " GENERATED BY DEFAULT AS IDENTITY",
                               _                             => throw new OutOfRangeException( nameof(column.Identity), column.Identity )
                           } );
        }

        if ( column.IsPrimaryKey ) { _query.Append( " PRIMARY KEY" ); }

        _query.Append( ",\n" );
        return this;
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    private static DbType GetDataType<T>( out bool isNullable, ref int? size )
    {
        isNullable = typeof(T).IsNullableType() || typeof(T).IsBuiltInNullableType();

        if ( typeof(T) == typeof(byte[]) ) { return DbType.Binary; }

        if ( typeof(JToken).IsAssignableFrom( typeof(T) ) ) { return DbType.Object; }

        if ( typeof(IDictionary<string, JToken>).IsAssignableFrom( typeof(T) ) ) { return DbType.Object; }

        if ( typeof(IDictionary<string, JToken?>).IsAssignableFrom( typeof(T) ) ) { return DbType.Object; }

        if ( typeof(T).IsEnum )
        {
            size = Spans.Max<string, int>( Enum.GetNames( typeof(T) ), static x => x.Length, 0 );
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


    private static string GetDataType( in DbInstance instance, in ColumnMetaData column ) =>
        instance switch
        {
            DbInstance.MsSql    => GetDataTypeSqlServer( column ),
            DbInstance.Postgres => GetDataTypePostgresSql( column ),
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    private static string GetDataTypePostgresSql( in ColumnMetaData column ) =>
        column.DbType switch
        {
            DbType.Binary => column.Size.HasValue
                                 ? $"VARBINARY({column.Size})"
                                 : "BLOB",
            DbType.SByte => "bytea",
            DbType.Byte  => "bytea",
            DbType.AnsiString => column.Size.HasValue
                                     ? $"varchar({column.Size})"
                                     : "varchar(MAX)",
            DbType.String => column.Size.HasValue
                                 ? $"text({column.Size})"
                                 : "text(MAX)",
            DbType.AnsiStringFixedLength => $"char({column.Size})",
            DbType.StringFixedLength     => $"char({column.Size})",
            DbType.Guid                  => "uuid",
            DbType.Int16                 => "smallint",
            DbType.Int32                 => "integer",
            DbType.Int64                 => "bigint",
            DbType.UInt16                => "smallint",
            DbType.UInt32                => "integer",
            DbType.UInt64                => "bigint",
            DbType.Single                => "float4",
            DbType.Double                => "float8",
            DbType.Decimal               => "decimal(19, 5)",
            DbType.VarNumeric            => $"decimal({column.Size}, {column.Precision})",
            DbType.Boolean               => "bool",
            DbType.Date                  => "date",
            DbType.Time                  => "time",
            DbType.DateTime              => "timestamp",
            DbType.DateTime2             => "timestamp",
            DbType.DateTimeOffset        => "timestamptz",
            DbType.Currency              => "money",
            DbType.Object                => "json",
            DbType.Xml                   => "xml",
            _                            => throw new OutOfRangeException( nameof(column.DbType), column.DbType )
        };


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    private static string GetDataTypeSqlServer( in ColumnMetaData column ) =>
        column.DbType switch
        {
            DbType.Binary => column.Size.HasValue
                                 ? $"VARBINARY({column.Size})"
                                 : "BLOB",
            DbType.SByte => "TINYINT",
            DbType.Byte  => "TINYINT",
            DbType.AnsiString => column.Size.HasValue
                                     ? $"NVARCHAR({column.Size})"
                                     : "NVARCHAR(MAX)",
            DbType.String => column.Size.HasValue
                                 ? $"NTEXT({column.Size})"
                                 : "NTEXT(MAX)",
            DbType.AnsiStringFixedLength => $"CHAR({column.Size})",
            DbType.StringFixedLength     => $"NCHAR({column.Size})",
            DbType.Guid                  => "GUID",
            DbType.Int16                 => "SMALLINT",
            DbType.Int32                 => "INT",
            DbType.Int64                 => "BIGINT",
            DbType.UInt16                => "SMALLINT",
            DbType.UInt32                => "INT",
            DbType.UInt64                => "BIGINT",
            DbType.Single                => "FLOAT(9)",
            DbType.Double                => "DOUBLE PRECISION",
            DbType.Decimal               => "DECIMAL(19, 5)",
            DbType.VarNumeric            => $"DECIMAL({column.Size}, {column.Precision})",
            DbType.Boolean               => "BOOLEAN",
            DbType.Date                  => "DATE",
            DbType.Time                  => "TIME",
            DbType.DateTime              => "DATETIME2",
            DbType.DateTime2             => "DATETIME2",
            DbType.DateTimeOffset        => "TIMESTAMP",
            DbType.Currency              => "MONEY",
            DbType.Object                => "JSON",
            DbType.Xml                   => "XML",
            _                            => throw new OutOfRangeException( nameof(column.DbType), column.DbType )
        };


    public string Build()
    {
        string query = _query.ToString().TrimEnd( ' ', ',' );
        if ( query.EndsWith( '(' ) ) { query = query.Remove( query.Length - 1 ); }

        query += " );";
        return query;
    }
}
