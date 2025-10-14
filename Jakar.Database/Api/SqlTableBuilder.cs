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



[Flags]
public enum ColumnOptions
{
    AlwaysIdentity  = 1 << 0,
    DefaultIdentity = 1 << 1,
    Indexed         = 1 << 2,
    Unique          = 1 << 3,
    PrimaryKey      = 1 << 4,
    Nullable        = 1 << 5,
    Fixed           = 1 << 6
}



public sealed record ColumnPrecisionMetaData( int Value, int Scope, int Precision )
{
    public readonly                 int Value     = Value;
    public readonly                 int Precision = Precision;
    public readonly                 int Scope     = Scope;
    public static implicit operator ColumnPrecisionMetaData( int                        value ) => new(value, -1, -1);
    public static implicit operator ColumnPrecisionMetaData( (int Precision, int Scope) value ) => Create(value.Precision, value.Scope);


    public static ColumnPrecisionMetaData Create( int Scope, int Precision )
    {
        if ( Precision > DECIMAL_MAX_PRECISION ) { throw new OutOfRangeException(Precision); }

        if ( Scope > DECIMAL_MAX_SCALE ) { throw new OutOfRangeException(Scope); }

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



public sealed record ColumnMetaData( string ColumnName, PostgresType DbType, ColumnOptions Options = 0, string? ForeignKeyName = null, string? IndexColumnName = null, ColumnPrecisionMetaData? Length = null, ColumnCheckMetaData? Checks = null )
{
    public static readonly ColumnMetaData           AdditionalData  = new(nameof(IJsonModel.AdditionalData), PostgresType.Json, ColumnOptions.Nullable);
    public static readonly ColumnMetaData           CreatedBy       = new(nameof(ICreatedBy.CreatedBy), PostgresType.Guid, ColumnOptions.Nullable, UserRecord.TABLE_NAME);
    public static readonly ColumnMetaData           DateCreated     = new(nameof(ICreatedBy.DateCreated), PostgresType.DateTimeOffset, ColumnOptions.Indexed);
    public static readonly ColumnMetaData           ID              = new(nameof(ICreatedBy.ID), PostgresType.Guid, ColumnOptions.PrimaryKey                      | ColumnOptions.AlwaysIdentity | ColumnOptions.Unique);
    public static readonly ColumnMetaData           LastModified    = new(nameof(ILastModified.LastModified), PostgresType.DateTimeOffset, ColumnOptions.Nullable | ColumnOptions.Indexed);
    public readonly        bool                     IsForeignKey    = !string.IsNullOrWhiteSpace(IndexColumnName);
    public readonly        bool                     IsNullable      = Options.HasFlagValue(ColumnOptions.Nullable);
    public readonly        bool                     IsPrimaryKey    = Options.HasFlagValue(ColumnOptions.PrimaryKey);
    public readonly        ColumnCheckMetaData?     Checks          = Checks;
    public readonly        ColumnOptions            Options         = Options;
    public readonly        ColumnPrecisionMetaData? Length          = Length;
    public readonly        PostgresType             DbType          = DbType;
    public readonly        string                   ColumnName      = ColumnName.SqlColumnName();
    public readonly        string?                  ForeignKeyName  = ForeignKeyName?.SqlColumnName();
    public readonly        string?                  IndexColumnName = IndexColumnName?.SqlColumnName();


    public string GetDataType() =>
        DbType switch
        {
            // !IsValidLength() => throw new OutOfRangeException(Length, $"Max length for Unicode strings is {Constants.UNICODE_CAPACITY}"),
            PostgresType.Binary => Length is not null
                                       ? @$"varbit({Length.Scope})"
                                       : "bytea",
            PostgresType.String => Length is null || Length.Scope > MAX_VARIABLE
                                       ? "text"
                                       : Length.Value < MAX_FIXED && Options.HasFlagValue(ColumnOptions.Fixed)
                                           ? $"character({Length.Value})"
                                           : $"varchar({Length.Value})",
            PostgresType.Byte => Length is null
                                     ? "bit(8)"
                                     : $"bit({Length.Value})",
            PostgresType.SByte => Length is null
                                      ? "bit(8)"
                                      : $"bit({Length.Value})",
            PostgresType.Short                    => "smallint",
            PostgresType.UShort                   => "smallint",
            PostgresType.Int                      => "integer",
            PostgresType.UInt                     => "integer",
            PostgresType.Long                     => "bigint",
            PostgresType.ULong                    => "bigint",
            PostgresType.Single                   => "float4",
            PostgresType.Double                   => "float8",
            PostgresType.Decimal                  => "decimal(19, 5)",
            PostgresType.Boolean                  => "bool",
            PostgresType.Date                     => "date",
            PostgresType.Time                     => "time",
            PostgresType.DateTime                 => "timestamp",
            PostgresType.DateTimeOffset           => @"timestamptz",
            PostgresType.Money                    => "money",
            PostgresType.Guid                     => "uuid",
            PostgresType.Json                     => "json",
            PostgresType.Xml                      => "xml",
            PostgresType.Enum                     => "enum",
            PostgresType.Polygon                  => "polygon",
            PostgresType.LineSegment              => @"lseg",
            PostgresType.Point                    => "point",
            PostgresType.Int128                   => "decimal(19, 5)",
            PostgresType.UInt128                  => "decimal(19, 5)",
            PostgresType.Numeric                  => "numeric",
            PostgresType.Box                      => "box",
            PostgresType.Circle                   => "circle",
            PostgresType.Line                     => "line",
            PostgresType.Path                     => "path",
            PostgresType.Char                     => "char",
            PostgresType.CiText                   => @"citext",
            PostgresType.TimeSpan                 => "interval",
            PostgresType.TimeTz                   => "time with time zone",
            PostgresType.Inet                     => "inet",
            PostgresType.Cidr                     => "cidr",
            PostgresType.MacAddr                  => @"macaddr",
            PostgresType.MacAddr8                 => "macaddr8",
            PostgresType.Bit                      => $"bit({Validate.ThrowIfNull(Length).Value})",
            PostgresType.VarBit                   => $"bit varying({Validate.ThrowIfNull(Length).Value})",
            PostgresType.TsVector                 => @"tsvector",
            PostgresType.TsQuery                  => @"tsquery",
            PostgresType.RegConfig                => @"regconfig",
            PostgresType.Jsonb                    => "jsonb",
            PostgresType.JsonPath                 => "jsonpath",
            PostgresType.Hstore                   => "hstore",
            PostgresType.RefCursor                => @"refcursor",
            PostgresType.OidVector                => @"oidvector",
            PostgresType.Oid                      => "oid",
            PostgresType.Xid                      => "xid",
            PostgresType.Xid8                     => "xid8",
            PostgresType.Cid                      => "cit",
            PostgresType.RegType                  => @"regtype",
            PostgresType.Tid                      => "tid",
            PostgresType.PgLsn                    => @"pglsn",
            PostgresType.Geometry                 => "geometry",
            PostgresType.Geography                => "geodetic",
            PostgresType.LTree                    => @"ltree",
            PostgresType.LQuery                   => @"lquery",
            PostgresType.LTxtQuery                => @"ltxtquery",
            PostgresType.IntVector                => "integer[]",
            PostgresType.LongVector               => "bigint[]",
            PostgresType.FloatVector              => "float[]",
            PostgresType.DoubleVector             => "double[]",
            PostgresType.IntegerRange             => "int4range",
            PostgresType.BigIntRange              => "int8range",
            PostgresType.NumericRange             => @"numrange",
            PostgresType.TimestampRange           => @"tsrange",
            PostgresType.DateTimeOffsetRange      => @"tstzrange",
            PostgresType.DateRange                => @"daterange",
            PostgresType.IntegerMultirange        => "int4multirange",
            PostgresType.BigIntMultirange         => "int8multirange",
            PostgresType.NumericMultirange        => @"nummultirange",
            PostgresType.TimestampMultirange      => @"tsmultirange",
            PostgresType.DateTimeOffsetMultirange => @"tstzmultirange",
            PostgresType.DateMultirange           => @"datemultirange",
            PostgresType.NotSet                   => throw new OutOfRangeException(DbType),
            _                                     => throw new OutOfRangeException(DbType)
        };


    /*
    public static ImmutableDictionary<string, ColumnMetaData> FromType<TSelf>()
        where TSelf : class
    {
        ImmutableDictionary<string, ColumnMetaData>.Builder builder = ImmutableDictionary.CreateBuilder<string, ColumnMetaData>();

        foreach ( PropertyInfo info in typeof(TSelf).GetProperties(BindingFlags.Instance | BindingFlags.Public).AsValueEnumerable() )
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



public readonly ref struct SqlTable<TSelf> : IDisposable
    where TSelf : class, ITableRecord<TSelf>
{
    internal readonly SortedDictionary<string, ColumnMetaData> Columns = new(StringComparer.InvariantCultureIgnoreCase);


    public SqlTable() { }
    public static SqlTable<TSelf> Empty() => new();
    public static SqlTable<TSelf> Create() => Empty()
                                             .WithColumn(ColumnMetaData.ID)
                                             .WithColumn(ColumnMetaData.LastModified)
                                             .WithColumn(ColumnMetaData.DateCreated);
    public void Dispose() => Columns?.Clear();


    // public SqlTableBuilder<TSelf> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn(ColumnMetaData.Indexed(columnName, indexColumnName));


    public SqlTable<TSelf> With_CreatedBy()      => WithColumn(ColumnMetaData.CreatedBy);
    public SqlTable<TSelf> With_AdditionalData() => WithColumn(ColumnMetaData.AdditionalData);


    public SqlTable<TSelf> WithColumn<TValue>( string columnName, ColumnOptions options = 0, ColumnPrecisionMetaData? length = null, ColumnCheckMetaData? checks = null, string? indexColumnName = null )
    {
        bool         isEnum = false;
        PostgresType dbType;

        if ( ( typeof(TValue) ) == typeof(RecordID<TSelf>) || ( typeof(TValue) ) == typeof(RecordID<TSelf>?) )
        {
            dbType  =  PostgresType.Guid;
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
    public SqlTable<TSelf> WithForeignKey<TRecord>( string columnName, ColumnOptions options = 0, ColumnCheckMetaData? checks = null )
        where TRecord : ITableRecord<TRecord>
    {
        ColumnMetaData column = new(columnName, PostgresType.Guid, options, TRecord.TableName, Checks: checks);
        return WithColumn(column);
    }
    public SqlTable<TSelf> WithColumn( ColumnMetaData column )
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
    public static PostgresType GetDataType<TValue>( out bool isNullable, out bool isEnum, ref ColumnPrecisionMetaData? length )
    {
        Type type = typeof(TValue);
        isEnum     = type.IsEnum           || TryGetUnderlyingType(type, out Type? underlyingType) && underlyingType.IsEnum;
        isNullable = type.IsNullableType() || type.IsBuiltInNullableType();

        if ( type == typeof(byte[]) || type == typeof(Memory<byte>) || type == typeof(ReadOnlyMemory<byte>) || type == typeof(ImmutableArray<byte>) ) { return PostgresType.Binary; }

        if ( typeof(JsonNode).IsAssignableFrom(type) ) { return PostgresType.Json; }

        if ( typeof(XmlNode).IsAssignableFrom(type) ) { return PostgresType.Xml; }


        if ( type == typeof(Guid) ) { return PostgresType.Guid; }

        if ( type == typeof(Guid?) )
        {
            isNullable = true;
            return PostgresType.Guid;
        }


        if ( isEnum )
        {
            length = Enum.GetNames(type)
                         .AsValueEnumerable()
                         .Max(static x => x.Length);

            return PostgresType.String;
        }

        if ( type == typeof(string) ) { return PostgresType.String; }


        if ( type == typeof(Int128) ) { return PostgresType.String; }

        if ( type == typeof(Int128?) )
        {
            isNullable = true;
            return PostgresType.Int128;
        }

        if ( type == typeof(UInt128) ) { return PostgresType.String; }

        if ( type == typeof(UInt128?) )
        {
            isNullable = true;
            return PostgresType.UInt128;
        }

        if ( type == typeof(byte) ) { return PostgresType.Bit; }

        if ( type == typeof(byte?) )
        {
            isNullable = true;
            return PostgresType.Bit;
        }

        if ( type == typeof(short) ) { return PostgresType.Short; }

        if ( type == typeof(short?) )
        {
            isNullable = true;
            return PostgresType.Short;
        }

        if ( type == typeof(ushort) ) { return PostgresType.Short; }

        if ( type == typeof(ushort?) )
        {
            isNullable = true;
            return PostgresType.Short;
        }

        if ( type == typeof(int) ) { return PostgresType.Int; }

        if ( type == typeof(int?) )
        {
            isNullable = true;
            return PostgresType.Int;
        }

        if ( type == typeof(uint) ) { return PostgresType.Int; }

        if ( type == typeof(uint?) )
        {
            isNullable = true;
            return PostgresType.Int;
        }

        if ( type == typeof(long) ) { return PostgresType.Long; }

        if ( type == typeof(long?) )
        {
            isNullable = true;
            return PostgresType.Long;
        }

        if ( type == typeof(ulong) ) { return PostgresType.Long; }

        if ( type == typeof(ulong?) )
        {
            isNullable = true;
            return PostgresType.Long;
        }

        if ( type == typeof(float) ) { return PostgresType.Single; }

        if ( type == typeof(float?) )
        {
            isNullable = true;
            return PostgresType.Single;
        }

        if ( type == typeof(double) ) { return PostgresType.Double; }

        if ( type == typeof(double?) )
        {
            isNullable = true;
            return PostgresType.Double;
        }

        if ( type == typeof(decimal) ) { return PostgresType.Decimal; }

        if ( type == typeof(decimal?) )
        {
            isNullable = true;
            return PostgresType.Decimal;
        }

        if ( type == typeof(bool) ) { return PostgresType.Boolean; }

        if ( type == typeof(bool?) )
        {
            isNullable = true;
            return PostgresType.Boolean;
        }

        if ( type == typeof(DateOnly) ) { return PostgresType.Date; }

        if ( type == typeof(DateOnly?) )
        {
            isNullable = true;
            return PostgresType.Date;
        }

        if ( type == typeof(TimeOnly) ) { return PostgresType.Time; }

        if ( type == typeof(TimeOnly?) )
        {
            isNullable = true;
            return PostgresType.Time;
        }

        if ( type == typeof(TimeSpan) ) { return PostgresType.Time; }

        if ( type == typeof(TimeSpan?) )
        {
            isNullable = true;
            return PostgresType.Time;
        }

        if ( type == typeof(DateTime) ) { return PostgresType.DateTime; }

        if ( type == typeof(DateTime?) )
        {
            isNullable = true;
            return PostgresType.DateTime;
        }

        if ( type == typeof(DateTimeOffset) ) { return PostgresType.DateTimeOffset; }

        if ( type == typeof(DateTimeOffset?) )
        {
            isNullable = true;
            return PostgresType.DateTimeOffset;
        }

        throw new ArgumentException($"Unsupported type: {type.Name}");
    }


    public ImmutableDictionary<string, ColumnMetaData> Build()
    {
        int check = Columns.Values.Count(static x => x.IsPrimaryKey);
        if ( check != 1 ) { throw new InvalidOperationException($"Must be exactly one primary key defined for {typeof(TSelf).Name}"); }

        if ( TSelf.PropertyCount != Columns.Count ) { throw new InvalidOperationException("Column count mismatch"); }

        return Columns.ToImmutableDictionary(StringComparer.InvariantCultureIgnoreCase);
    }
}



public readonly ref struct SqlTableBuilder<TSelf>( ImmutableDictionary<string, ColumnMetaData> columns )
    where TSelf : class, ITableRecord<TSelf>
{
    private readonly ImmutableDictionary<string, ColumnMetaData> __columns = columns;
    public static    SqlTableBuilder<TSelf>                      Create()                                              => new(TSelf.PropertyMetaData);
    public static    bool                                        HasValue( ColumnOptions options, ColumnOptions flag ) => ( options & flag ) != 0;


    public string Build()
    {
        StringBuilder query     = new(10240);
        string        tableName = TSelf.TableName.ToSnakeCase();

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
                                     ? AND
                                     : OR,
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
