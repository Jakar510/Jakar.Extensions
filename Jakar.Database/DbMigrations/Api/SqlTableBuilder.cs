// Jakar.Extensions :: Jakar.SqlBuilder
// 3/1/2024  23:20

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
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private static readonly string             NULL       = "NULL";
    private static readonly string             TABLE_NAME = TRecord.TableName;
    private                 ValueStringBuilder _query     = new(10240);


    public SqlTableBuilder() { }
    public SqlTableBuilder<TRecord> Create( string tableName )
    {
        _query.Append( $"CREATE TABLE {tableName} (" );
        return this;
    }

    public SqlTableBuilder<TRecord> WithColumn<T>( string columnName, int? size = null, bool isPrimaryKey = false )
    {
        string dataType = GetDataType<T>( out bool isNullable, size );

        _query.Append( _query.Span.EndsWith( "(" )
                           ? $"{columnName} {dataType}"
                           : $", {columnName} {dataType}" );

        if ( isPrimaryKey ) { _query.Append( " PRIMARY KEY" ); }

        if ( isNullable ) { }

        return this;
    }
    private static string GetDataType<T>( out bool isNullable, int? size = null )
    {
        Type type = typeof(T);
        isNullable = type.IsNullableType() || type.IsBuiltInNullableType();

        if ( type.IsEnum ) { return "CHAR(255)"; }

        if ( type == typeof(string) )
        {
            return size.HasValue
                       ? $"VARCHAR({size})"
                       : "CHAR(256)";
        }

        if ( type == typeof(byte[]) )
        {
            return size.HasValue
                       ? $"VARBINARY({size})"
                       : "BLOB";
        }

        if ( typeof(JToken).IsAssignableFrom( type ) || type == typeof(IDictionary<string, JToken?>) ) { return "JSON"; }

        if ( type == typeof(Guid) || type == typeof(RecordID<TRecord>) ) { return "GUID"; }

        if ( type == typeof(byte) ) { return "TINYINT"; }

        if ( type == typeof(short) ) { return "SMALLINT"; }

        if ( type == typeof(int) ) { return "INT"; }

        if ( type == typeof(long) ) { return "BIGINT"; }

        if ( type == typeof(float) ) { return "FLOAT(9)"; }

        if ( type == typeof(double) ) { return "REAL"; }

        if ( type == typeof(decimal) ) { return "DECIMAL(29, 29)"; }

        if ( type == typeof(bool) ) { return "BOOLEAN"; }

        if ( type == typeof(DateOnly) ) { return "DATE"; }

        if ( type == typeof(TimeOnly) ) { return "TIME"; }

        if ( type == typeof(TimeSpan) ) { return "DATETIME"; }

        if ( type == typeof(DateTime) ) { return "DATETIME"; }

        if ( type == typeof(DateTimeOffset) ) { return "TIMESTAMP"; }


        throw new ArgumentException( $"Unsupported type: {type.Name}" );
    }

    public SqlTableBuilder<TRecord> Modify( string tableName )
    {
        _query.Reset();
        _query.Append( $"ALTER TABLE {tableName} " );
        return this;
    }
    public SqlTableBuilder<TRecord> AddColumn( string columnName, string dataType )
    {
        _query.Append( $"ADD COLUMN {columnName} {dataType}, " );
        return this;
    }
    public SqlTableBuilder<TRecord> DropColumn( string columnName )
    {
        _query.Append( $"DROP COLUMN {columnName}, " );
        return this;
    }

    public string Build()
    {
        string query = _query.ToString().TrimEnd( ' ', ',' );
        if ( query.EndsWith( "(" ) ) { query = query.Remove( query.Length - 1 ); }

        query += ");";
        return query;
    }
}
