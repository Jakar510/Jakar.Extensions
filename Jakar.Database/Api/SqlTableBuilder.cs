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
public enum ColumnOptions : ulong
{
    None            = 0,
    PrimaryKey      = 1 << 0,
    Unique          = 1 << 1,
    Nullable        = 1 << 2,
    Indexed         = 1 << 3,
    Fixed           = 1 << 4,
    AlwaysIdentity  = 1 << 5,
    DefaultIdentity = 1 << 6,
    All             = ~0UL,
}



[DefaultValue(nameof(Default))]
public readonly record struct PrecisionInfo( int Value, int Scope, int Precision )
{
    public static readonly          PrecisionInfo Default   = new(-1, -1, -1);
    public readonly                 int           Value     = Value;
    public readonly                 int           Precision = Precision;
    public readonly                 int           Scope     = Scope;
    public readonly                 bool          IsValid   = Value >= 0 || Scope >= 0 && Precision >= 0;
    public static implicit operator PrecisionInfo( int                        value ) => new(value, -1, -1);
    public static implicit operator PrecisionInfo( (int Precision, int Scope) value ) => Create(value.Precision, value.Scope);


    public static PrecisionInfo Create( int scope, int precision )
    {
        if ( precision > DECIMAL_MAX_PRECISION ) { throw new OutOfRangeException(precision); }

        if ( scope > DECIMAL_MAX_SCALE ) { throw new OutOfRangeException(scope); }

        return new PrecisionInfo(-1, scope, precision);
    }
}



public sealed record ColumnCheckMetaData( bool And, params string[] Checks )
{
    public readonly                 bool     And    = And;
    public readonly                 string[] Checks = Checks;
    public static implicit operator ColumnCheckMetaData( string   check )  => new(true, check);
    public static implicit operator ColumnCheckMetaData( string[] checks ) => new(true, checks);
}



public sealed record ColumnMetaData( string ColumnName, PostgresType DbType, ColumnOptions Options = ColumnOptions.None, string? ForeignKeyName = null, string? IndexColumnName = null, PrecisionInfo Length = default, ColumnCheckMetaData? Checks = null )
{
    public static readonly ColumnMetaData       AdditionalData  = new(nameof(IJsonModel.AdditionalData), PostgresType.Json, ColumnOptions.Nullable);
    public static readonly ColumnMetaData       CreatedBy       = new(nameof(ICreatedBy.CreatedBy), PostgresType.Guid, ColumnOptions.Nullable, UserRecord.TABLE_NAME);
    public static readonly ColumnMetaData       DateCreated     = new(nameof(ICreatedBy.DateCreated), PostgresType.DateTimeOffset, ColumnOptions.Indexed);
    public static readonly ColumnMetaData       ID              = new(nameof(ICreatedBy.ID), PostgresType.Guid, ColumnOptions.PrimaryKey                      | ColumnOptions.AlwaysIdentity | ColumnOptions.Unique);
    public static readonly ColumnMetaData       LastModified    = new(nameof(ILastModified.LastModified), PostgresType.DateTimeOffset, ColumnOptions.Nullable | ColumnOptions.Indexed);
    public readonly        bool                 IsForeignKey    = !string.IsNullOrWhiteSpace(IndexColumnName);
    public readonly        bool                 IsNullable      = Options.HasFlagValue(ColumnOptions.Nullable);
    public readonly        bool                 IsPrimaryKey    = Options.HasFlagValue(ColumnOptions.PrimaryKey);
    public readonly        ColumnCheckMetaData? Checks          = Checks;
    public readonly        ColumnOptions        Options         = Options;
    public readonly        PrecisionInfo        Length          = Length;
    public readonly        PostgresType         DbType          = DbType;
    public readonly        string               ColumnName      = ColumnName.SqlColumnName();
    public readonly        string?              ForeignKeyName  = ForeignKeyName?.SqlColumnName();
    public readonly        string?              IndexColumnName = IndexColumnName?.SqlColumnName();


    public string GetDataType() => DbType.GetPostgresDataType(in Length, in Options);


    public NpgsqlParameter ToParameter<T>( T value, [CallerArgumentExpression(nameof(value))] string parameterName = EMPTY, ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        NpgsqlParameter parameter = new(parameterName, DbType.ToNpgsqlDbType())
                                    {
                                        SourceColumn  = ColumnName,
                                        IsNullable    = IsNullable,
                                        SourceVersion = sourceVersion,
                                        Direction     = direction,
                                        Value         = value,
                                    };

        return parameter;
    }


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


    public SqlTable<TSelf> WithColumn<TValue>( string columnName, ColumnOptions options = ColumnOptions.None, PrecisionInfo length = default, ColumnCheckMetaData? checks = null, string? indexColumnName = null )
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
            dbType = PostgresTypes.GetType<TValue>(out bool checkIsNullable, out isEnum, ref length);
            if ( checkIsNullable != options.HasFlagValue(ColumnOptions.Nullable) ) { throw new InvalidOperationException($"{nameof(options)} mismatch. Type: {typeof(TValue).Name} Nullable: {checkIsNullable}"); }
        }

        string? foreignKeyName = isEnum
                                     ? typeof(TValue).Name
                                     : null;

        ColumnMetaData column = new(columnName, dbType, options, foreignKeyName, indexColumnName, length, checks);
        return WithColumn(column);
    }
    public SqlTable<TSelf> WithForeignKey<TRecord>( string columnName, ColumnOptions options = ColumnOptions.None, ColumnCheckMetaData? checks = null )
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
