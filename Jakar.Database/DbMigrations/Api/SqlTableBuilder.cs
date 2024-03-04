// Jakar.Extensions :: Jakar.SqlBuilder
// 3/1/2024  23:20

using FluentMigrator.Model;



namespace Jakar.Database.DbMigrations;


/*
 Numeric Types

       INTEGER: A whole number, varying in size from SMALLINT to BIGINT depending on the storage size and range.
       DECIMAL(Precision, Scale): A fixed-point number with a specified number of digits before and after the decimal point.
       NUMERIC(Precision, Scale): Similar to DECIMAL, often used interchangeably.
       FLOAT(Precision): A floating-point number with machine-dependent precision.
       REAL and DOUBLE PRECISION: Floating-point numbers with more precision than FLOAT.

   String Types

       CHAR(Size): A fixed-length character string, space-padded.
       VARCHAR(Size): A variable-length character string.
       TEXT: A large text data type, with size limits depending on the DBMS.

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
    private SqlTableBuilder( DbInstance instance, string firstLine )
    {
        _instance = instance;
        _query.Append( firstLine );
    }
    public static SqlTableBuilder<TRecord> Create( DbInstance instance ) => new(instance, $"CREATE TABLE {TRecord.TableName} (");

    /*
    public static SqlTableBuilder<TRecord> Modify( DbInstance instance ) => new(instance, $"MODIFY TABLE {TRecord.TableName} (");
    public static SqlTableBuilder<TRecord> Drop( DbInstance   instance ) => new(instance, $"DROP TABLE {TRecord.TableName} (");
    public SqlTableBuilder<TRecord> Modify( string tableName )
    {
        _query.Reset();
        _query.Append( $"ALTER TABLE {tableName} " );
        return this;
    }
    public SqlTableBuilder<TRecord> DropColumn( string columnName )
    {
        _query.Reset();
        _query.Append( $"DROP COLUMN {columnName}, " );
        return this;
    }
    */

    public SqlTableBuilder<TRecord> WithColumn<T>( string columnName, int? size = null, bool isPrimaryKey = false )
    {
        DbType dbType = GetDataType<T>( out bool isNullable, ref size );
        return WithColumn( columnName, dbType, isNullable, size, isPrimaryKey );
    }
    public SqlTableBuilder<TRecord> WithColumn( string columnName, DbType dbType, bool isNullable, int? size = null, bool isPrimaryKey = false )
    {
        ColumnDefinition definition = new();
        string           dataType   = GetDataType( _instance, dbType, size );

        _query.Append( _query.Span.EndsWith( "(" )
                           ? $" {columnName} {dataType}"
                           : $", {columnName} {dataType}" );

        if ( isPrimaryKey ) { _query.Append( " PRIMARY KEY" ); }

        if ( isNullable ) { _query.Append( " NOT NULL" ); }

        _query.Append( '\n' );
        return this;
    }


    private static DbType GetDataType<T>( out bool isNullable, ref int? size )
    {
        Type type = typeof(T);
        isNullable = type.IsNullableType() || type.IsBuiltInNullableType();

        if ( type.IsEnum )
        {
            size = Spans.Max<string, int>( Enum.GetNames( typeof(T) ), static x => x.Length, 0 );
            return DbType.StringFixedLength;
        }

        if ( type == typeof(string) ) { return DbType.String; }

        if ( type == typeof(byte[]) ) { return DbType.Binary; }

        if ( typeof(JToken).IsAssignableFrom( type ) || typeof(IDictionary<string, JToken?>).IsAssignableFrom( type ) ) { return DbType.Object; }

        if ( type == typeof(Guid) || type == typeof(RecordID<TRecord>) ) { return DbType.Guid; }

        if ( type == typeof(byte) ) { return DbType.Byte; }

        if ( type == typeof(short) ) { return DbType.Int16; }

        if ( type == typeof(ushort) ) { return DbType.UInt16; }

        if ( type == typeof(int) ) { return DbType.Int32; }

        if ( type == typeof(uint) ) { return DbType.UInt32; }

        if ( type == typeof(long) ) { return DbType.Int64; }

        if ( type == typeof(ulong) ) { return DbType.UInt64; }

        if ( type == typeof(float) ) { return DbType.Single; }

        if ( type == typeof(double) ) { return DbType.Double; }

        if ( type == typeof(decimal) ) { return DbType.Decimal; }

        if ( type == typeof(bool) ) { return DbType.Boolean; }

        if ( type == typeof(DateOnly) ) { return DbType.Date; }

        if ( type == typeof(TimeOnly) ) { return DbType.Time; }

        if ( type == typeof(TimeSpan) ) { return DbType.Time; }

        if ( type == typeof(DateTime) ) { return DbType.DateTime2; }

        if ( type == typeof(DateTimeOffset) ) { return DbType.DateTimeOffset; }

        throw new ArgumentException( $"Unsupported type: {type.Name}" );
    }


    private static string GetDataType( in DbInstance instance, in DbType dbType, in int? size = null )
    {
        return instance switch
               {
                   DbInstance.MsSql    => GetDataTypeSqlServer( dbType, size ),
                   DbInstance.Postgres => GetDataTypePostgresSql( dbType, size ),
                   _                   => throw new OutOfRangeException( nameof(instance), instance )
               };
    }


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    private static string GetDataTypePostgresSql( in DbType dbType, in int? size )
    {
        return dbType switch
               {
                   DbType.Binary => size.HasValue
                                        ? $"VARBINARY({size})"
                                        : "BLOB",
                   DbType.SByte => "bytea",
                   DbType.Byte  => "bytea",
                   DbType.AnsiString => size.HasValue
                                            ? $"varchar({size})"
                                            : "varchar(MAX)",
                   DbType.String => size.HasValue
                                        ? $"text({size})"
                                        : "text(MAX)",
                   DbType.AnsiStringFixedLength => $"char({size})",
                   DbType.StringFixedLength     => $"char({size})",
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
                   DbType.VarNumeric            => "decimal(19, 5)",
                   DbType.Boolean               => "bool",
                   DbType.Date                  => "date",
                   DbType.Time                  => "time",
                   DbType.DateTime              => "timestamp",
                   DbType.DateTime2             => "timestamp",
                   DbType.DateTimeOffset        => "timestamptz",
                   DbType.Currency              => "money",
                   DbType.Object                => "json",
                   DbType.Xml                   => "xml",
                   _                            => throw new OutOfRangeException( nameof(dbType), dbType )
               };
    }


    [SuppressMessage( "ReSharper", "StringLiteralTypo" )]
    private static string GetDataTypeSqlServer( in DbType dbType, int? size )
    {
        return dbType switch
               {
                   DbType.Binary => size.HasValue
                                        ? $"VARBINARY({size})"
                                        : "BLOB",
                   DbType.SByte => "TINYINT",
                   DbType.Byte  => "TINYINT",
                   DbType.AnsiString => size.HasValue
                                            ? $"NVARCHAR({size})"
                                            : "NVARCHAR(MAX)",
                   DbType.String => size.HasValue
                                        ? $"NTEXT({size})"
                                        : "NTEXT(MAX)",
                   DbType.AnsiStringFixedLength => $"CHAR({size})",
                   DbType.StringFixedLength     => $"NCHAR({size})",
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
                   DbType.VarNumeric            => "DECIMAL(19, 5)",
                   DbType.Boolean               => "BOOLEAN",
                   DbType.Date                  => "DATE",
                   DbType.Time                  => "TIME",
                   DbType.DateTime              => "DATETIME2",
                   DbType.DateTime2             => "DATETIME2",
                   DbType.DateTimeOffset        => "TIMESTAMP",
                   DbType.Currency              => "MONEY",
                   DbType.Object                => "JSON",
                   DbType.Xml                   => "XML",
                   _                            => throw new OutOfRangeException( nameof(dbType), dbType )
               };
    }


    public string Build()
    {
        string query = _query.ToString().TrimEnd( ' ', ',' );
        if ( query.EndsWith( '(' ) ) { query = query.Remove( query.Length - 1 ); }

        query += " );";
        return query;
    }
}
