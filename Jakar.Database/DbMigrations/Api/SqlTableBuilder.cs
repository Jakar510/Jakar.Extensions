// Jakar.Extensions :: Jakar.SqlBuilder
// 3/1/2024  23:20

using System.Formats.Asn1;
using System.Xml;



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
       TEXT: A large text data type, with column.Size limits depending on the DBMS.

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
       UUID/GUID: A special type for storing Universally Unique Identifiers.
       JSON: Stores JSON data, allowing for complex data structures within a single database field.
       ARRAY: Supported by some databases like PostgreSQL, allowing storage of arrays.
       XML: For storing XML data, with some DBMS providing additional functions to manipulate XML data.

   Geography and Geometry Types (Spatial Types)

       POINT, LINESTRING, POLYGON, and more: These types are used for storing and manipulating spatial data, such as coordinates and shapes, especially in databases that support GIS (Geographic Information Systems).
 */



public enum DbPropType
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
    PrimaryKey      = 1 << 4,
    Nullable        = 1 << 5
}



public sealed record ColumnPrecisionMetaData( int Length, int Scope, int Precision )
{
    public readonly                 int Length    = Length;
    public readonly                 int Precision = Precision;
    public readonly                 int Scope     = Scope;
    public static implicit operator ColumnPrecisionMetaData( int                        value ) => new(value, -1, -1);
    public static implicit operator ColumnPrecisionMetaData( (int Precision, int Scope) value ) => Create(value.Precision, value.Scope);


    public static ColumnPrecisionMetaData Create( int Scope, int Precision )
    {
        if ( Precision > Constants.DECIMAL_MAX_PRECISION ) { throw new OutOfRangeException(Precision); }

        if ( Scope > Constants.DECIMAL_MAX_SCALE ) { throw new OutOfRangeException(Scope); }

        return new ColumnPrecisionMetaData(-1, Scope, Precision);
    }
}



public sealed record ColumnCheckMetaData( bool And, params string[] Checks )
{
    public readonly                 bool     And    = And;
    public readonly                 string[] Checks = Checks;
    public static implicit operator ColumnCheckMetaData( string   check )  => new(true, check);
    public static implicit operator ColumnCheckMetaData( string[] checks ) => new(true, checks);
}



public sealed record ColumnMetaData( string ColumnName, DbPropType DbType, ColumnOptions Options = 0, string? ForeignKeyName = null, string? IndexColumnName = null, ColumnPrecisionMetaData? Length = null, ColumnCheckMetaData? Checks = null )
{
    public static readonly ColumnMetaData           AdditionalData  = new(nameof(IJsonModel.AdditionalData), DbPropType.Json, ColumnOptions.Nullable);
    public static readonly ColumnMetaData           CreatedBy       = new(nameof(ICreatedBy.CreatedBy), DbPropType.Guid, ColumnOptions.Nullable, UserRecord.TABLE_NAME);
    public static readonly ColumnMetaData           DateCreated     = new(nameof(ICreatedBy.DateCreated), DbPropType.DateTimeOffset, ColumnOptions.Indexed);
    public static readonly ColumnMetaData           ID              = new(nameof(ICreatedBy.ID), DbPropType.Guid, ColumnOptions.PrimaryKey                   | ColumnOptions.AlwaysIdentity | ColumnOptions.Unique);
    public static readonly ColumnMetaData           LastModified    = new(nameof(ILastModified.LastModified), DbPropType.DateTimeOffset, ColumnOptions.Nullable | ColumnOptions.Indexed);
    public readonly        bool                     IsForeignKey    = !string.IsNullOrWhiteSpace(IndexColumnName);
    public readonly        bool                     IsNullable      = Options.HasFlagValue(ColumnOptions.Nullable);
    public readonly        bool                     IsPrimaryKey    = Options.HasFlagValue(ColumnOptions.PrimaryKey);
    public readonly        ColumnCheckMetaData?     Checks          = Checks;
    public readonly        ColumnOptions            Options         = Options;
    public readonly        ColumnPrecisionMetaData? Length          = Length;
    public readonly        DbPropType               DbType          = DbType;
    public readonly        string                   ColumnName      = ColumnName.ToSnakeCase();
    public readonly        string?                  ForeignKeyName  = ForeignKeyName?.ToSnakeCase();
    public readonly        string?                  IndexColumnName = IndexColumnName?.ToSnakeCase();


    public bool IsValidLength() =>
        Length?.Scope switch
        {
            0                                                             => false,
            > Constants.ANSI_CAPACITY when DbType is DbPropType.String    => false,
            > Constants.UNICODE_CAPACITY when DbType is DbPropType.String => false,
            _                                                             => true
        };

    public string GetDataType() =>
        DbType switch
        {
            DbPropType.String when !IsValidLength() => throw new OutOfRangeException(Length, $"Max length for Unicode strings is {Constants.UNICODE_CAPACITY}"),
            DbPropType.String when !IsValidLength() => throw new OutOfRangeException(Length, $"Max length for ANSI strings is {Constants.ANSI_CAPACITY}"),

            // DbPropertyType.VarNumeric when Length.IsT0 || !Length.IsT1 || IsInvalidScopedPrecision()
            // => throw new OutOfRangeException(Length, $"Max decimal scale is {Constants.DECIMAL_MAX_SCALE}. Max decimal precision is {Constants.DECIMAL_MAX_PRECISION}"),

            _ => DbType switch
                 {
                     DbPropType.Binary => Length is not null
                                              ? $"VARBINARY({Length.Scope})"
                                              : "BLOB",
                     DbPropType.SByte => "bytea",
                     DbPropType.Byte  => "bytea",
                     DbPropType.String => Length is not null
                                              ? Length.Scope > Constants.ANSI_CAPACITY
                                                    ? "text"
                                                    : $"varchar({Length.Scope})"
                                              : "varchar(MAX)",
                     DbPropType.Guid           => "uuid",
                     DbPropType.Int16          => "smallint",
                     DbPropType.Int32          => "integer",
                     DbPropType.Int64          => "bigint",
                     DbPropType.UInt16         => "smallint",
                     DbPropType.UInt32         => "integer",
                     DbPropType.UInt64         => "bigint",
                     DbPropType.Single         => "float4",
                     DbPropType.Double         => "float8",
                     DbPropType.Decimal        => "decimal(19, 5)",
                     DbPropType.Boolean        => "bool",
                     DbPropType.Date           => "date",
                     DbPropType.Time           => "time",
                     DbPropType.DateTime       => "timestamp",
                     DbPropType.DateTimeOffset => @"timestamptz",
                     DbPropType.Currency       => "money",
                     DbPropType.Object         => "json",
                     DbPropType.Json           => "json",
                     DbPropType.Xml            => "xml",
                     DbPropType.Enum           => "enum",
                     DbPropType.Set            => "set",
                     DbPropType.Polygon        => "polygon",
                     DbPropType.Linestring     => "linestring",
                     DbPropType.Point          => "point",
                     DbPropType.Int128         => "decimal(19, 5)",
                     DbPropType.UInt128        => "decimal(19, 5)",

                     // DbPropertyType.VarNumeric when Length.IsT1 => $"decimal({Length.AsT1.Precision}, {Length.AsT1.Scope})",
                     // DbPropertyType.VarNumeric                  => $"decimal({Constants.DECIMAL_MAX_PRECISION}, {Constants.DECIMAL_MAX_SCALE})",
                     _ => throw new OutOfRangeException(DbType)
                 }
        };


    /*
    public static ImmutableDictionary<string, ColumnMetaData> FromType<TClass>()
        where TClass : class
    {
        ImmutableDictionary<string, ColumnMetaData>.Builder builder = ImmutableDictionary.CreateBuilder<string, ColumnMetaData>();

        foreach ( PropertyInfo info in typeof(TClass).GetProperties(BindingFlags.Instance | BindingFlags.Public).AsValueEnumerable() )
        {
            Type   type       = info.PropertyType;
            string columnName = info.Name;
            bool   isNullable = type.IsNullableType() || type.IsBuiltInNullableType();

            ColumnMetaData value = new(columnName,);
            builder.Add(columnName, value);
        }


        return builder.ToImmutable();
    }
    */
}



public readonly ref struct SqlTable<TClass> : IDisposable
    where TClass : class, ITableRecord<TClass>
{
    internal readonly SortedDictionary<string, ColumnMetaData> Columns = new(StringComparer.InvariantCultureIgnoreCase);


    public SqlTable() { }
    public static SqlTable<TClass> Empty() => new();
    public static SqlTable<TClass> Create() => Empty()
                                              .WithColumn(ColumnMetaData.ID)
                                              .WithColumn(ColumnMetaData.LastModified)
                                              .WithColumn(ColumnMetaData.DateCreated);
    public void Dispose() => Columns?.Clear();


    // public SqlTableBuilder<TClass> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn(ColumnMetaData.Indexed(columnName, indexColumnName));


    public SqlTable<TClass> With_CreatedBy()      => WithColumn(ColumnMetaData.CreatedBy);
    public SqlTable<TClass> With_AdditionalData() => WithColumn(ColumnMetaData.AdditionalData);


    public SqlTable<TClass> WithColumn<TValue>( string columnName, ColumnOptions options = 0, ColumnPrecisionMetaData? length = null, ColumnCheckMetaData? checks = null, string? indexColumnName = null )
    {
        bool       isEnum = false;
        DbPropType dbType;

        if ( ( typeof(TValue) ) == typeof(RecordID<TClass>) || ( typeof(TValue) ) == typeof(RecordID<TClass>?) )
        {
            dbType  =  DbPropType.Guid;
            options |= ColumnOptions.PrimaryKey;
        }
        else
        {
            dbType = GetDataType<TValue>(out bool checkIsNullable, out isEnum, ref length);
            if ( checkIsNullable != options.HasFlagValue(ColumnOptions.Nullable) ) { throw new InvalidOperationException($"{nameof(options)} mismatch. Type: {typeof(TValue).Name} Nullable: {checkIsNullable}"); }
        }


        string? foreignKeyName = isEnum
                                     ? typeof(TValue).Name
                                     : null;

        ColumnMetaData column = new(columnName, dbType, options, foreignKeyName, indexColumnName, length, checks);
        return WithColumn(column);
    }
    public SqlTable<TClass> WithForeignKey<TRecord>( string columnName, ColumnOptions options = 0, ColumnCheckMetaData? checks = null )
        where TRecord : ITableRecord<TRecord>
    {
        ColumnMetaData column = new(columnName, DbPropType.Guid, options, TRecord.TableName, Checks: checks);
        return WithColumn(column);
    }
    public SqlTable<TClass> WithColumn( ColumnMetaData column )
    {
        Columns.Add(column.ColumnName, column);
        return this;
    }


    public static bool TryGetUnderlyingType( Type type, [NotNullWhen(true)] out Type? result )
    {
        if ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) )
        {
            foreach ( Type argument in type.GenericTypeArguments.AsSpan() )
            {
                result = argument;
                return true;
            }
        }

        result = null;
        return false;
    }
    public static DbPropType GetDataType<TValue>( out bool isNullable, out bool isEnum, ref ColumnPrecisionMetaData? length )
    {
        Type type = typeof(TValue);
        isEnum     = type.IsEnum           || TryGetUnderlyingType(type, out Type? underlyingType) && underlyingType.IsEnum;
        isNullable = type.IsNullableType() || type.IsBuiltInNullableType();


        if ( type == typeof(byte[]) || type == typeof(Memory<byte>) || type == typeof(ReadOnlyMemory<byte>) || type == typeof(ImmutableArray<byte>) ) { return DbPropType.Binary; }

        if ( typeof(JsonNode).IsAssignableFrom(type) ) { return DbPropType.Json; }

        if ( typeof(XmlNode).IsAssignableFrom(type) ) { return DbPropType.Xml; }


        if ( type == typeof(Guid) ) { return DbPropType.Guid; }

        if ( type == typeof(Guid?) )
        {
            isNullable = true;
            return DbPropType.Guid;
        }


        if ( isEnum )
        {
            length = Enum.GetNames(type)
                         .AsValueEnumerable()
                         .Max(static x => x.Length);

            return DbPropType.String;
        }

        if ( type == typeof(string) ) { return DbPropType.String; }


        if ( type == typeof(Int128) ) { return DbPropType.String; }

        if ( type == typeof(Int128?) )
        {
            isNullable = true;
            return DbPropType.Int128;
        }

        if ( type == typeof(UInt128) ) { return DbPropType.String; }

        if ( type == typeof(UInt128?) )
        {
            isNullable = true;
            return DbPropType.UInt128;
        }

        if ( type == typeof(byte) ) { return DbPropType.Byte; }

        if ( type == typeof(byte?) )
        {
            isNullable = true;
            return DbPropType.Byte;
        }

        if ( type == typeof(short) ) { return DbPropType.Int16; }

        if ( type == typeof(short?) )
        {
            isNullable = true;
            return DbPropType.Int16;
        }

        if ( type == typeof(ushort) ) { return DbPropType.UInt16; }

        if ( type == typeof(ushort?) )
        {
            isNullable = true;
            return DbPropType.UInt16;
        }

        if ( type == typeof(int) ) { return DbPropType.Int32; }

        if ( type == typeof(int?) )
        {
            isNullable = true;
            return DbPropType.Int32;
        }

        if ( type == typeof(uint) ) { return DbPropType.UInt32; }

        if ( type == typeof(uint?) )
        {
            isNullable = true;
            return DbPropType.UInt32;
        }

        if ( type == typeof(long) ) { return DbPropType.Int64; }

        if ( type == typeof(long?) )
        {
            isNullable = true;
            return DbPropType.Int64;
        }

        if ( type == typeof(ulong) ) { return DbPropType.UInt64; }

        if ( type == typeof(ulong?) )
        {
            isNullable = true;
            return DbPropType.UInt64;
        }

        if ( type == typeof(float) ) { return DbPropType.Single; }

        if ( type == typeof(float?) )
        {
            isNullable = true;
            return DbPropType.Single;
        }

        if ( type == typeof(double) ) { return DbPropType.Double; }

        if ( type == typeof(double?) )
        {
            isNullable = true;
            return DbPropType.Double;
        }

        if ( type == typeof(decimal) ) { return DbPropType.Decimal; }

        if ( type == typeof(decimal?) )
        {
            isNullable = true;
            return DbPropType.Decimal;
        }

        if ( type == typeof(bool) ) { return DbPropType.Boolean; }

        if ( type == typeof(bool?) )
        {
            isNullable = true;
            return DbPropType.Boolean;
        }

        if ( type == typeof(DateOnly) ) { return DbPropType.Date; }

        if ( type == typeof(DateOnly?) )
        {
            isNullable = true;
            return DbPropType.Date;
        }

        if ( type == typeof(TimeOnly) ) { return DbPropType.Time; }

        if ( type == typeof(TimeOnly?) )
        {
            isNullable = true;
            return DbPropType.Time;
        }

        if ( type == typeof(TimeSpan) ) { return DbPropType.Time; }

        if ( type == typeof(TimeSpan?) )
        {
            isNullable = true;
            return DbPropType.Time;
        }

        if ( type == typeof(DateTime) ) { return DbPropType.DateTime; }

        if ( type == typeof(DateTime?) )
        {
            isNullable = true;
            return DbPropType.DateTime;
        }

        if ( type == typeof(DateTimeOffset) ) { return DbPropType.DateTimeOffset; }

        if ( type == typeof(DateTimeOffset?) )
        {
            isNullable = true;
            return DbPropType.DateTimeOffset;
        }

        throw new ArgumentException($"Unsupported type: {type.Name}");
    }


    public ImmutableDictionary<string, ColumnMetaData> Build()
    {
        int check = Columns.Values.Count(static x => x.IsPrimaryKey);
        if ( check != 1 ) { throw new InvalidOperationException($"Must be exactly one primary key defined for {typeof(TClass).Name}"); }

        if ( TClass.PropertyCount != Columns.Count ) { throw new InvalidOperationException("Column count mismatch"); }

        return Columns.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase);
    }
}



public readonly ref struct SqlTableBuilder<TClass>( ImmutableDictionary<string, ColumnMetaData> columns )
    where TClass : class, ITableRecord<TClass>
{
    private readonly ImmutableDictionary<string, ColumnMetaData> __columns = columns;
    public static    SqlTableBuilder<TClass>                     Create()                                              => new(TClass.PropertyMetaData);
    public static    bool                                        HasValue( ColumnOptions options, ColumnOptions flag ) => ( options & flag ) != 0;


    public string Build()
    {
        StringBuilder query     = new(10240);
        string        tableName = TClass.TableName.ToSnakeCase();

        query.Append("CREATE TABLE ");
        query.Append(tableName);
        query.Append(" (");

        foreach ( ( string columnName, ColumnMetaData column ) in __columns )
        {
            ColumnOptions options = column.Options;

            if ( options.HasFlagValue(ColumnOptions.Indexed) )
            {
                query.Append(column.IndexColumnName ?? $"{column.ColumnName}_index");
                query.Append(" ON ");
                query.Append(tableName);
                query.Append(" (");
                query.Append(columnName);
                query.Append(");");
                continue;
            }

            string dataType = column.GetDataType();

            query.Append(query[^1] == '('
                             ? "\n    "
                             : ",\n    ");

            query.Append(columnName);
            query.Append(' ');
            query.Append(dataType);

            if ( column.Checks is not null )
            {
                query.Append(" CHECK ( ");

                query.AppendJoin(column.Checks.And
                                     ? Constants.AND
                                     : Constants.OR,
                                 column.Checks.Checks);

                query.Append(" )");
            }

            if ( options.HasFlagValue(ColumnOptions.Unique) ) { query.Append(" UNIQUE"); }

            query.Append(column.IsNullable
                             ? " NULL"
                             : " NOT NULL");

            if ( options.HasFlagValue(ColumnOptions.AlwaysIdentity) ) { query.Append(" GENERATED ALWAYS AS IDENTITY"); }

            else if ( options.HasFlagValue(ColumnOptions.DefaultIdentity) ) { query.Append(" GENERATED BY DEFAULT AS IDENTITY"); }

            if ( options.HasFlagValue(ColumnOptions.PrimaryKey) ) { query.Append(" PRIMARY KEY"); }
        }

        return query.ToString();
    }
}
