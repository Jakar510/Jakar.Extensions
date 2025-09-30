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



public enum DbPropertyType
{
    Guid,
    Boolean,
    Byte,
    SByte,
    Int16,
    Int32,
    Int64,
    Int128,
    UInt16,
    UInt32,
    UInt64,
    UInt128,
    Single,
    Double,
    Decimal,
    Currency,
    Time,
    Date,
    DateTime,
    DateTimeOffset,
    Binary,
    String,
    Xml,
    Json,
    Object,
    Enum,
    Set,
    Polygon,
    Linestring,
    Point
}



[Flags]
public enum ColumnOptions
{
    AlwaysIdentity  = 1 << 0,
    DefaultIdentity = 1 << 1,
    Indexed         = 1 << 2,
    Unique          = 1 << 3,
    PrimaryKey      = 1 << 4
}



public readonly record struct ColumnPrecisionMetaData( int Length, int Scope, int Precision )
{
    public readonly                 int Length    = Length;
    public readonly                 int Precision = Precision;
    public readonly                 int Scope     = Scope;
    public static implicit operator ColumnPrecisionMetaData( int                        value ) => new(value, -1, -1);
    public static implicit operator ColumnPrecisionMetaData( (int Precision, int Scope) value ) => new(-1, value.Precision, value.Scope);
}



public readonly record struct ColumnCheckMetaData( bool And, params string[] Checks )
{
    public readonly                 bool     And    = And;
    public readonly                 string[] Checks = Checks;
    public static implicit operator ColumnCheckMetaData( string   check )  => new(true, check);
    public static implicit operator ColumnCheckMetaData( string[] checks ) => new(true, checks);
}



public readonly record struct ColumnMetaData( string                   ColumnName,
                                              DbPropertyType           DbType,
                                              bool                     IsNullable,
                                              bool                     IsPrimaryKey,
                                              string?                  ForeignKeyName,
                                              string?                  IndexColumnName = null,
                                              ColumnPrecisionMetaData? Length          = null,
                                              ColumnCheckMetaData?     Check           = null,
                                              ColumnOptions            Options         = 0 )
{
    public readonly bool                     IsForeignKey    = !string.IsNullOrWhiteSpace(IndexColumnName);
    public readonly bool                     IsNullable      = IsNullable;
    public readonly bool                     IsPrimaryKey    = IsPrimaryKey;
    public readonly ColumnCheckMetaData?     Check           = Check;
    public readonly ColumnOptions            Options         = Options;
    public readonly ColumnPrecisionMetaData? Length          = Length;
    public readonly DbPropertyType           DbType          = DbType;
    public readonly string                   ColumnName      = ColumnName;
    public readonly string?                  ForeignKeyName  = ForeignKeyName;
    public readonly string?                  IndexColumnName = IndexColumnName;


    public static ColumnMetaData Nullable( string    columnName, DbPropertyType dbType, string? foreignKeyName = null, string? indexColumnName = null, ColumnPrecisionMetaData? length = null, ColumnCheckMetaData? check = null, ColumnOptions options = 0 )                            => new(columnName, dbType, true, false, foreignKeyName, indexColumnName, length, check, options);
    public static ColumnMetaData NotNullable( string columnName, DbPropertyType dbType, string? foreignKeyName = null, string? indexColumnName = null, ColumnPrecisionMetaData? length = null, ColumnCheckMetaData? check = null, ColumnOptions options = 0, bool isPrimaryKey = false ) => new(columnName, dbType, false, isPrimaryKey, foreignKeyName, indexColumnName, length, check, options);

    // public static ColumnMetaData Indexed( string     columnName, string         indexColumnName ) => default;


    public bool IsInvalidScopedPrecision() => Length is { Precision: > Constants.DECIMAL_MAX_PRECISION, Scope: > Constants.DECIMAL_MAX_SCALE };

    public bool IsValidLength() =>
        Length?.Scope switch
        {
            0                                                                 => false,
            > Constants.ANSI_CAPACITY when DbType is DbPropertyType.String    => false,
            > Constants.UNICODE_CAPACITY when DbType is DbPropertyType.String => false,
            _                                                                 => true
        };


    public string GetDataTypePostgresSql() =>
        DbType switch
        {
            DbPropertyType.String when !IsValidLength() => throw new OutOfRangeException(Length, $"Max length for Unicode strings is {Constants.UNICODE_CAPACITY}"),
            DbPropertyType.String when !IsValidLength() => throw new OutOfRangeException(Length, $"Max length for ANSI strings is {Constants.ANSI_CAPACITY}"),

            // DbPropertyType.VarNumeric when Length.IsT0 || !Length.IsT1 || IsInvalidScopedPrecision()
            // => throw new OutOfRangeException(Length, $"Max decimal scale is {Constants.DECIMAL_MAX_SCALE}. Max decimal precision is {Constants.DECIMAL_MAX_PRECISION}"),

            _ => DbType switch
                 {
                     DbPropertyType.Binary => Length.HasValue
                                                  ? $"VARBINARY({Length.Value.Scope})"
                                                  : "BLOB",
                     DbPropertyType.SByte => "bytea",
                     DbPropertyType.Byte  => "bytea",
                     DbPropertyType.String => Length.HasValue
                                                  ? Length.Value.Scope > Constants.ANSI_CAPACITY
                                                        ? "text"
                                                        : $"varchar({Length.Value.Scope})"
                                                  : "varchar(MAX)",
                     DbPropertyType.Guid           => "uuid",
                     DbPropertyType.Int16          => "smallint",
                     DbPropertyType.Int32          => "integer",
                     DbPropertyType.Int64          => "bigint",
                     DbPropertyType.UInt16         => "smallint",
                     DbPropertyType.UInt32         => "integer",
                     DbPropertyType.UInt64         => "bigint",
                     DbPropertyType.Single         => "float4",
                     DbPropertyType.Double         => "float8",
                     DbPropertyType.Decimal        => "decimal(19, 5)",
                     DbPropertyType.Boolean        => "bool",
                     DbPropertyType.Date           => "date",
                     DbPropertyType.Time           => "time",
                     DbPropertyType.DateTime       => "timestamp",
                     DbPropertyType.DateTimeOffset => @"timestamptz",
                     DbPropertyType.Currency       => "money",
                     DbPropertyType.Object         => "json",
                     DbPropertyType.Json           => "json",
                     DbPropertyType.Xml            => "xml",
                     DbPropertyType.Enum           => "enum",
                     DbPropertyType.Set            => "set",
                     DbPropertyType.Polygon        => "polygon",
                     DbPropertyType.Linestring     => "linestring",
                     DbPropertyType.Point          => "point",

                     // DbPropertyType.VarNumeric when Length.IsT1 => $"decimal({Length.AsT1.Precision}, {Length.AsT1.Scope})",
                     // DbPropertyType.VarNumeric                  => $"decimal({Constants.DECIMAL_MAX_PRECISION}, {Constants.DECIMAL_MAX_SCALE})",
                     _ => throw new OutOfRangeException(DbType)
                 }
        };
}



[Experimental("SqlTableBuilder")]
public ref struct SqlTableBuilder<TClass> : IDisposable
    where TClass : class, ITableRecord<TClass>
{
    private Buffer<ColumnMetaData> __columns = new(Buffers.DEFAULT_CAPACITY);
    public SqlTableBuilder() { }
    public static SqlTableBuilder<TClass> Create() => new();
    public void Dispose()
    {
        __columns.Dispose();
        this = default;
    }


    public static string FromTable()
    {
        SqlTableBuilder<TClass> builder = new();
        foreach ( ColumnMetaData column in TClass.Properties.Values ) { builder.WithColumn(column); }

        return builder.Build();
    }

    // public SqlTableBuilder<TClass> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn(ColumnMetaData.Indexed(columnName, indexColumnName));
    public SqlTableBuilder<TClass> WithColumn<TValue>( string columnName, string? indexColumnName = null, ColumnPrecisionMetaData? length = null, ColumnCheckMetaData? check = null, ColumnOptions options = 0, bool isPrimaryKey = false )
    {
        DbPropertyType dbType = GetDataType<TValue>(out bool isNullable, ref length);
        ColumnMetaData column = new(columnName, dbType, isNullable, isPrimaryKey, null, indexColumnName, length, check, options);
        return WithColumn(column);
    }
    public SqlTableBuilder<TClass> WithColumn<TValue, TRecord>( string columnName, string? indexColumnName = null, ColumnPrecisionMetaData? length = null, ColumnCheckMetaData? check = null, ColumnOptions options = 0 )
        where TRecord : ITableRecord<TRecord>
    {
        DbPropertyType dbType = GetDataType<TValue>(out bool isNullable, ref length);
        ColumnMetaData column = new(columnName, dbType, isNullable, false, TRecord.TableName, indexColumnName, length, check, options);
        return WithColumn(column);
    }
    public SqlTableBuilder<TClass> WithColumn( ColumnMetaData column )
    {
        __columns.Add(column);
        return this;
    }


    private static DbPropertyType GetDataType<TValue>( out bool isNullable, ref ColumnPrecisionMetaData? length )
    {
        isNullable = typeof(TValue).IsNullableType() || typeof(TValue).IsBuiltInNullableType();

        if ( typeof(TValue) == typeof(byte[]) || typeof(TValue) == typeof(ReadOnlyMemory<byte>) || typeof(TValue) == typeof(ImmutableArray<byte>) ) { return DbPropertyType.Binary; }

        if ( typeof(JsonNode).IsAssignableFrom(typeof(TValue)) ) { return DbPropertyType.Object; }


        if ( typeof(TValue).IsEnum )
        {
            length = Enum.GetNames(typeof(TValue)).AsValueEnumerable().Max(static x => x.Length);
            return DbPropertyType.String;
        }

        if ( typeof(TValue) == typeof(string) ) { return DbPropertyType.String; }


        if ( typeof(TValue) == typeof(Int128) ) { return DbPropertyType.String; }

        if ( typeof(TValue) == typeof(Int128?) )
        {
            isNullable = true;
            return DbPropertyType.Int128;
        }

        if ( typeof(TValue) == typeof(UInt128) ) { return DbPropertyType.String; }

        if ( typeof(TValue) == typeof(UInt128?) )
        {
            isNullable = true;
            return DbPropertyType.UInt128;
        }

        if ( typeof(TValue) == typeof(Guid) || typeof(TValue) == typeof(RecordID<TClass>) ) { return DbPropertyType.Guid; }

        if ( typeof(TValue) == typeof(Guid?) || typeof(TValue) == typeof(RecordID<TClass>?) )
        {
            isNullable = true;
            return DbPropertyType.Guid;
        }

        if ( typeof(TValue) == typeof(byte) ) { return DbPropertyType.Byte; }

        if ( typeof(TValue) == typeof(byte?) )
        {
            isNullable = true;
            return DbPropertyType.Byte;
        }

        if ( typeof(TValue) == typeof(short) ) { return DbPropertyType.Int16; }

        if ( typeof(TValue) == typeof(short?) )
        {
            isNullable = true;
            return DbPropertyType.Int16;
        }

        if ( typeof(TValue) == typeof(ushort) ) { return DbPropertyType.UInt16; }

        if ( typeof(TValue) == typeof(ushort?) )
        {
            isNullable = true;
            return DbPropertyType.UInt16;
        }

        if ( typeof(TValue) == typeof(int) ) { return DbPropertyType.Int32; }

        if ( typeof(TValue) == typeof(int?) )
        {
            isNullable = true;
            return DbPropertyType.Int32;
        }

        if ( typeof(TValue) == typeof(uint) ) { return DbPropertyType.UInt32; }

        if ( typeof(TValue) == typeof(uint?) )
        {
            isNullable = true;
            return DbPropertyType.UInt32;
        }

        if ( typeof(TValue) == typeof(long) ) { return DbPropertyType.Int64; }

        if ( typeof(TValue) == typeof(long?) )
        {
            isNullable = true;
            return DbPropertyType.Int64;
        }

        if ( typeof(TValue) == typeof(ulong) ) { return DbPropertyType.UInt64; }

        if ( typeof(TValue) == typeof(ulong?) )
        {
            isNullable = true;
            return DbPropertyType.UInt64;
        }

        if ( typeof(TValue) == typeof(float) ) { return DbPropertyType.Single; }

        if ( typeof(TValue) == typeof(float?) )
        {
            isNullable = true;
            return DbPropertyType.Single;
        }

        if ( typeof(TValue) == typeof(double) ) { return DbPropertyType.Double; }

        if ( typeof(TValue) == typeof(double?) )
        {
            isNullable = true;
            return DbPropertyType.Double;
        }

        if ( typeof(TValue) == typeof(decimal) ) { return DbPropertyType.Decimal; }

        if ( typeof(TValue) == typeof(decimal?) )
        {
            isNullable = true;
            return DbPropertyType.Decimal;
        }

        if ( typeof(TValue) == typeof(bool) ) { return DbPropertyType.Boolean; }

        if ( typeof(TValue) == typeof(bool?) )
        {
            isNullable = true;
            return DbPropertyType.Boolean;
        }

        if ( typeof(TValue) == typeof(DateOnly) ) { return DbPropertyType.Date; }

        if ( typeof(TValue) == typeof(DateOnly?) )
        {
            isNullable = true;
            return DbPropertyType.Date;
        }

        if ( typeof(TValue) == typeof(TimeOnly) ) { return DbPropertyType.Time; }

        if ( typeof(TValue) == typeof(TimeOnly?) )
        {
            isNullable = true;
            return DbPropertyType.Time;
        }

        if ( typeof(TValue) == typeof(TimeSpan) ) { return DbPropertyType.Time; }

        if ( typeof(TValue) == typeof(TimeSpan?) )
        {
            isNullable = true;
            return DbPropertyType.Time;
        }

        if ( typeof(TValue) == typeof(DateTime) ) { return DbPropertyType.DateTime; }

        if ( typeof(TValue) == typeof(DateTime?) )
        {
            isNullable = true;
            return DbPropertyType.DateTime;
        }

        if ( typeof(TValue) == typeof(DateTimeOffset) ) { return DbPropertyType.DateTimeOffset; }

        if ( typeof(TValue) == typeof(DateTimeOffset?) )
        {
            isNullable = true;
            return DbPropertyType.DateTimeOffset;
        }

        throw new ArgumentException($"Unsupported typeof(TValue): {typeof(TValue).Name}");
    }


    private string BuildInternal()
    {
        ReadOnlySpan<ColumnMetaData> columns = __columns.Values;
        StringBuilder                query   = new(10240);

        query.Append("CREATE TABLE ");
        query.Append(TClass.TableName);
        query.Append(" (");

        foreach ( ref readonly ColumnMetaData column in columns )
        {
            if ( column.Options.HasFlag(ColumnOptions.Indexed) )
            {
                query.Append(column.IndexColumnName ?? $"{column.ColumnName}_index");
                query.Append(" ON ");
                query.Append(TClass.TableName);
                query.Append(" (");
                query.Append(column.ColumnName);
                query.Append(");");
                continue;
            }

            string dataType = column.GetDataTypePostgresSql();

            query.Append(query[^1] == '('
                             ? "\n    "
                             : ",\n    ");

            query.Append(column.ColumnName);
            query.Append(' ');
            query.Append(dataType);

            if ( column.Check.HasValue )
            {
                query.Append(" CHECK ( ");

                query.AppendJoin(column.Check.Value.And
                                     ? Constants.AND
                                     : Constants.OR,
                                 column.Check.Value.Checks);

                query.Append(" )");
            }

            if ( HasFlag(column.Options, ColumnOptions.Unique) ) { query.Append(" UNIQUE"); }

            query.Append(column.IsNullable
                             ? " NULL"
                             : " NOT NULL");

            if ( HasFlag(column.Options, ColumnOptions.AlwaysIdentity) ) { query.Append(" GENERATED ALWAYS AS IDENTITY"); }

            else if ( HasFlag(column.Options, ColumnOptions.DefaultIdentity) ) { query.Append(" GENERATED BY DEFAULT AS IDENTITY"); }

            if ( HasFlag(column.Options, ColumnOptions.PrimaryKey) ) { query.Append(" PRIMARY KEY"); }
        }

        return query.ToString();
    }
    public static bool HasFlag( ColumnOptions options, ColumnOptions flag ) => ( options & flag ) != 0;


    public string Build()
    {
        try { return BuildInternal(); }
        finally { Dispose(); }
    }
}
