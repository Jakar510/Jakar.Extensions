// Jakar.Extensions :: Jakar.Database
// 10/13/2025  22:56

using System.Xml;
using Jakar.Shapes;



namespace Jakar.Database.DbMigrations;


public enum PostgresType
{
    NotSet,

    Enum,

    /// <summary>
    /// Corresponds to the PostgreSQL "boolean" type.
    /// <para> <see cref="bool"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-boolean.html"/>
    /// </remarks>
    Boolean,

    /// <summary>
    /// Corresponds to the PostgreSQL 2-byte "smallint" type.
    /// <para> <see cref="short"/> </para>
    /// <para> <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/> </para>
    /// </summary>
    Short,

    /// <summary>
    /// Corresponds to the PostgreSQL 2-byte "smallint" type.
    /// <para> <see cref="short"/> </para>
    /// <para> <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/> </para>
    /// </summary>
    UShort,

    /// <summary>
    /// Corresponds to the PostgreSQL "integer" type.
    /// <para> <see cref="int"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Int,

    /// <summary>
    /// Corresponds to the PostgreSQL "integer" type.
    /// <para> <see cref="int"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    UInt,

    /// <summary>
    /// Corresponds to the PostgreSQL "bigint" type.
    /// <para> <see cref="long"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Long,

    /// <summary>
    /// Corresponds to the PostgreSQL "bigint" type.
    /// <para> <see cref="long"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    ULong,

    /// <summary>
    /// Corresponds to the PostgreSQL "bigint" type.
    /// <para> <see cref="long"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Int128,

    /// <summary>
    /// Corresponds to the PostgreSQL "bigint" type.
    /// <para> <see cref="long"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    UInt128,

    /// <summary>
    /// Corresponds to the PostgreSQL "float" type.
    /// <para> <see cref="double"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Single,

    /// <summary>
    /// Corresponds to the PostgreSQL "double" type.
    /// <para> <see cref="double"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Double,

    /// <summary>
    /// Represents the PostgreSQL arbitrary-precision "numeric" type.
    /// </summary>
    /// <remarks> For more information, see the PostgreSQL documentation at <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/> </remarks>
    Numeric,

    /// <summary>
    /// Represents the PostgreSQL floating-point "real" data type.
    /// <para> <see cref="decimal"/> </para>
    /// </summary>
    /// <remarks>
    /// The "real" type in PostgreSQL is a single-precision, 4-byte floating-point number.
    /// For more information, see the PostgreSQL documentation at <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>.
    /// </remarks>
    Decimal,

    /// <summary>
    /// Corresponds to the PostgreSQL "money" type.
    /// <para> <see cref="Decimal"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-money.html"/>
    /// </remarks>
    Money,

    /// <summary>
    /// Corresponds to the PostgreSQL "box" type.
    /// <para> <see cref="ReadOnlyRectangle"/> </para>
    /// <para> <see cref="ReadOnlyRectangleF"/> </para>
    /// <para> <see cref="MutableRectangle"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Box,

    /// <summary>
    /// Corresponds to the PostgreSQL "circle" type.
    /// <para> <see cref="Circle"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Circle,

    /// <summary>
    /// Corresponds to the PostgreSQL "line" type.
    /// <para> <see cref="ReadOnlyPoint"/> </para>
    /// <para> <see cref="ReadOnlyPointF"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Line,

    /// <summary>
    /// Corresponds to the PostgreSQL "lseg" type.
    /// <para> <see cref="ReadOnlyLine"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    LineSegment,

    /// <summary>
    /// Corresponds to the PostgreSQL "path" type.
    /// <para> <see cref="Spline"/> </para>
    /// <para> <see cref="CalculatedLine"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Path,

    /// <summary>
    /// Corresponds to the PostgreSQL "point" type.
    /// <para> <see cref="ReadOnlyPoint"/> </para>
    /// <para> <see cref="ReadOnlyPointF"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Point,

    /// <summary>
    /// Corresponds to the PostgreSQL "polygon" type.
    /// <para> <see cref="ReadOnlyPoint"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Polygon,

    /// <summary>
    /// Corresponds to the PostgreSQL "char" type.
    /// <para> <see cref="char"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-character.html"/>
    /// </remarks>
    Char,

    /// <summary>
    /// Corresponds to the PostgreSQL "text" type.
    /// <para> <see cref="string"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-character.html"/>
    /// </remarks>
    String,

    /// <summary>
    /// Corresponds to the PostgreSQL "citext" type.
    /// <para> <see cref="string"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/citext.html"/>
    /// </remarks>
    CiText,

    /// <summary>
    /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
    /// <para> <see cref="byte[]"/> </para>
    /// <para> <see cref="Memory{byte}"/> </para>
    /// <para> <see cref="ReadOnlyMemory{byte}"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-binary.html"/>
    /// </remarks>
    Binary,

    /// <summary>
    /// Corresponds to the PostgreSQL "date" type.
    /// <para> <see cref="DateOnly"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    Date,

    /// <summary>
    /// Corresponds to the PostgreSQL "time" type.
    /// <para> <see cref="TimeOnly"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    Time,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp" type.
    /// <para> <see cref="DateTime"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    DateTime,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestampz" type.
    /// <para> <see cref="DateTimeOffset"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    DateTimeOffset,

    /// <summary>
    /// Corresponds to the PostgreSQL "interval" type.
    /// <para> <see cref="System.TimeSpan"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    TimeSpan,

    /// <summary>
    /// Corresponds to the PostgreSQL "time with time zone" type.
    /// <para> <see cref="DateTime"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    TimeTz,

    /// <summary>
    /// Corresponds to the PostgreSQL "inet" type.
    /// <para> <see cref="IPAddress"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    Inet,

    /// <summary>
    /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
    /// <para> <see cref="MacAddr"/> </para>
    /// <para> <see cref="MacAddr8"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    Cidr,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
    /// <para> <see cref="MacAddr"/> </para> 
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    MacAddr,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
    /// <para> <see cref="MacAddr8"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    MacAddr8,

    /// <summary>
    /// Corresponds to the PostgreSQL "bit" type.
    /// <para> <see cref="byte"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-bit.html"/>
    /// </remarks>
    Bit,

    /// <summary>
    /// Corresponds to the PostgreSQL "bit" type.
    /// <para> <see cref="byte"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-bit.html"/>
    /// </remarks>
    Byte,

    /// <summary>
    /// Corresponds to the PostgreSQL "bit" type.
    /// <para> <see cref="byte"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-bit.html"/>
    /// </remarks>
    SByte,

    /// <summary>
    /// Corresponds to the PostgreSQL "varbit" type.
    /// <para> <see cref="byte"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-bit.html"/>
    /// </remarks>
    VarBit,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsvector" type.
    /// <para> <see cref="Regex"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-textsearch.html"/>
    /// </remarks>
    TsVector,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsquery" type.
    /// <para> <see cref="Regex"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-textsearch.html"/>
    /// </remarks>
    TsQuery,

    /// <summary>
    /// Corresponds to the PostgreSQL "regconfig" type.
    /// <para> <see cref="Regex"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-textsearch.html"/>
    /// </remarks>
    RegConfig,

    /// <summary>
    /// Corresponds to the PostgreSQL "uuid" type.
    /// <para> <see cref="Guid"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-uuid.html"/>
    /// </remarks>
    Guid,

    /// <summary>
    /// Corresponds to the PostgreSQL "xml" type.
    /// <para> <see cref="XmlDocument"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-xml.html"/>
    /// </remarks>
    Xml,

    /// <summary>
    /// Corresponds to the PostgreSQL "json" type.
    /// <para> <see cref="JsonNode"/> </para>
    /// <para> <see cref="JsonObject"/> </para>
    /// <para> <see cref="JsonArray"/> </para>
    /// <para> <see cref="JsonValue"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-json.html"/>
    /// </remarks>
    Json,

    /// <summary>
    /// Corresponds to the PostgreSQL "jsonb" type.
    /// <para> <see cref="JsonNode"/> </para>
    /// <para> <see cref="JsonObject"/> </para>
    /// <para> <see cref="JsonArray"/> </para>
    /// <para> <see cref="JsonValue"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-json.html"/>
    /// </remarks>
    Jsonb,

    /// <summary>
    /// Corresponds to the PostgreSQL "jsonpath" type.
    /// <para> <see cref="JsonNode"/> </para>
    /// <para> <see cref="JsonObject"/> </para>
    /// <para> <see cref="JsonArray"/> </para>
    /// <para> <see cref="JsonValue"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-json.html#DATATYPE-JSONPATH"/>
    /// </remarks>
    JsonPath,

    /// <summary>
    /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value
    /// <para> <see cref="Dictionary{string,string}"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/hstore.html"/>
    /// </remarks>
    Hstore,

    /// <summary>
    /// Corresponds to the PostgreSQL "refcursor" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-refcursor.html"/>
    /// </remarks>
    RefCursor,

    /// <summary>
    /// Corresponds to the PostgreSQL "oidvector" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    OidVector,

    /// <summary>
    /// Corresponds to the PostgreSQL "oid" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Oid,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Xid,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid8" type, an internal transaction identifier.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Xid8,

    /// <summary>
    /// Corresponds to the PostgreSQL "cid" type, an internal transaction identifier.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Cid,

    /// <summary>
    /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    RegType,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
    /// </summary>
    Tid,

    /// <summary>
    ///     Corresponds to the PostgreSQL "pg_lsn" type, which can be used to store LSN (Log Sequence Number) data which is a pointer to a location in the WAL.
    /// </summary>
    /// <remarks>
    /// <see cref="https://www.postgresql.org/docs/current/datatype-pg-lsn.html"/> and <see cref="https://git.postgresql.org/gitweb/?p=postgresql.git;a=commit;h=7d03a83f4d0736ba869fa6f93973f7623a27038a"/>
    /// </remarks>
    PgLsn,

    /// <summary>
    ///     The geometry type for PostgreSQL spatial extension PostGIS.
    /// </summary>
    Geometry,

    /// <summary>
    ///     The geography (geodetic) type for PostgreSQL spatial extension PostGIS.
    /// </summary>
    Geography,

    /// <summary>
    ///     The PostgreSQL ltree type, each value is a label path "a.label.tree.value", forming a tree in a set.
    /// </summary> 
    ///  <remarks> See https://www.postgresql.org/docs/current/static/ltree.html </remarks>
    LTree,

    /// <summary>
    ///     The PostgreSQL lquery type for PostgreSQL extension ltree
    /// </summary>
    ///  <remarks>  See https://www.postgresql.org/docs/current/static/ltree.html </remarks>
    LQuery,

    /// <summary>
    ///     The PostgreSQL ltxtquery type for PostgreSQL extension ltree
    /// </summary>
    ///  <remarks>    See https://www.postgresql.org/docs/current/static/ltree.html </remarks>
    LTxtQuery,


    /// <summary>
    /// Corresponds to the PostgreSQL "integer[]" type.
    /// <para> <see cref="int[]"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-array.html"/>
    /// </remarks>
    IntVector,

    /// <summary>
    /// Corresponds to the PostgreSQL "bigint[]" type.
    /// <para> <see cref="long[]"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-array.html"/>
    /// </remarks>
    LongVector,

    /// <summary>
    /// Corresponds to the PostgreSQL "float[]" type.
    /// <para> <see cref="float[]"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-array.html"/>
    /// </remarks>
    FloatVector,

    /// <summary>
    /// Corresponds to the PostgreSQL "double[]" type.
    /// <para> <see cref="double[]"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-array.html"/>
    /// </remarks>
    DoubleVector,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int4range" type.
    /// </summary>
    IntegerRange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int8range" type.
    /// </summary>
    BigIntRange,

    /// <summary>
    /// Represents the PostgreSQL "numrange" type, which is a range of numeric values.    Corresponds to the PostgreSQL "numrange" type.
    /// </summary>
    /// <remarks>The "numrange" type in PostgreSQL is used to define a range of numeric values,  such as
    /// integers or decimals. This can be useful for representing intervals or  ranges in database queries and
    /// operations.</remarks>
    NumericRange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tsrange" type.
    /// </summary>
    TimestampRange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tstzrange" type.
    /// </summary>
    DateTimeOffsetRange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "daterange" type.
    /// </summary>
    DateRange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int4multirange" type.
    /// </summary>
    IntMultirange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int8multirange" type.
    /// </summary>
    LongMultirange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "nummultirange" type.
    /// </summary>
    NumericMultirange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tsmultirange" type.
    /// </summary>
    TimestampMultirange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tstzmultirange" type.
    /// </summary>
    DateTimeOffsetMultirange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "datemultirange" type.
    /// </summary>
    DateMultirange
}



public static class PostgresTypes
{
    private static bool TryGetUnderlyingType( Type type, [NotNullWhen(true)] out Type? result )
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
    public static PostgresType GetType<TValue>( out bool isNullable, out bool isEnum, ref SizeInfo length ) => GetType(typeof(TValue), out isNullable, out isEnum, ref length);
    public static PostgresType GetType( in Type type, out bool isNullable, out bool isEnum, ref SizeInfo length )
    {
        isEnum     = type.IsEnum           || TryGetUnderlyingType(type, out Type? underlyingType) && underlyingType.IsEnum;
        isNullable = type.IsNullableType() || type.IsBuiltInNullableType();

        if ( type == typeof(byte[]) || type == typeof(Memory<byte>) || type == typeof(ReadOnlyMemory<byte>) || type == typeof(ImmutableArray<byte>) ) { return PostgresType.Binary; }

        if ( typeof(JsonNode).IsAssignableFrom(type) || type == typeof(JsonDocument) || type == typeof(JsonElement) ) { return PostgresType.Json; }

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


    public static string GetPostgresDataType( this PostgresType dbType, ref readonly SizeInfo info, in ColumnOptions options = 0 )
    {
        LengthInfo    length    = info.Length;
        PrecisionInfo precision = info.Precision;

        return dbType switch
               {
                   // !IsValidLength() => throw new OutOfRangeException(Length, $"Max length for Unicode strings is {Constants.UNICODE_CAPACITY}"),
                   PostgresType.Binary => length.IsValid
                                              ? @$"varbit({precision.Scope})"
                                              : "bytea",
                   PostgresType.String => length.IsValid
                                              ? length.Value < MAX_FIXED && options.HasFlagValue(ColumnOptions.Fixed)
                                                    ? $"character({length.Value})"
                                                    : $"varchar({length.Value})"
                                              : "text",
                   PostgresType.Byte => length.IsValid
                                            ? $"bit({length.Value})"
                                            : "bit(8)",
                   PostgresType.SByte => length.IsValid
                                             ? $"bit({length.Value})"
                                             : "bit(8)",
                   PostgresType.Bit => length.IsValid
                                           ? $"bit({length.Value})"
                                           : "bit(8)",
                   PostgresType.VarBit => length.IsValid
                                              ? $"bit varying({length.Value})"
                                              : "bit varying(8)",
                   PostgresType.Short                    => "smallint",
                   PostgresType.UShort                   => "smallint",
                   PostgresType.Int                      => "integer",
                   PostgresType.UInt                     => "integer",
                   PostgresType.Long                     => "bigint",
                   PostgresType.ULong                    => "bigint",
                   PostgresType.Single                   => "float4",
                   PostgresType.Double                   => "double precision",
                   PostgresType.Decimal                  => "numeric(28, 28)",
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
                   PostgresType.Int128                   => "decimal(128, 0)",
                   PostgresType.UInt128                  => "decimal(128, 0)",
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
                   PostgresType.IntMultirange            => "int4multirange",
                   PostgresType.LongMultirange           => "int8multirange",
                   PostgresType.NumericMultirange        => @"nummultirange",
                   PostgresType.TimestampMultirange      => @"tsmultirange",
                   PostgresType.DateTimeOffsetMultirange => @"tstzmultirange",
                   PostgresType.DateMultirange           => @"datemultirange",
                   PostgresType.NotSet                   => throw new OutOfRangeException(dbType),
                   _                                     => throw new OutOfRangeException(dbType)
               };
    }


    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")] public static NpgsqlDbType ToNpgsqlDbType( this PostgresType type )
    {
        return type switch
               {
                   PostgresType.Enum                     => NpgsqlDbType.Bigint,
                   PostgresType.Boolean                  => NpgsqlDbType.Boolean,
                   PostgresType.Short                    => NpgsqlDbType.Smallint,
                   PostgresType.UShort                   => NpgsqlDbType.Smallint,
                   PostgresType.Int                      => NpgsqlDbType.Integer,
                   PostgresType.UInt                     => NpgsqlDbType.Integer,
                   PostgresType.Long                     => NpgsqlDbType.Bigint,
                   PostgresType.ULong                    => NpgsqlDbType.Bigint,
                   PostgresType.Int128                   => NpgsqlDbType.Numeric,
                   PostgresType.UInt128                  => NpgsqlDbType.Numeric,
                   PostgresType.Single                   => NpgsqlDbType.Double,
                   PostgresType.Double                   => NpgsqlDbType.Double,
                   PostgresType.Numeric                  => NpgsqlDbType.Numeric,
                   PostgresType.Decimal                  => NpgsqlDbType.Numeric,
                   PostgresType.Money                    => NpgsqlDbType.Money,
                   PostgresType.Box                      => NpgsqlDbType.Box,
                   PostgresType.Circle                   => NpgsqlDbType.Circle,
                   PostgresType.Line                     => NpgsqlDbType.Line,
                   PostgresType.LineSegment              => NpgsqlDbType.LSeg,
                   PostgresType.Path                     => NpgsqlDbType.Path,
                   PostgresType.Point                    => NpgsqlDbType.Point,
                   PostgresType.Polygon                  => NpgsqlDbType.Polygon,
                   PostgresType.Char                     => NpgsqlDbType.Char,
                   PostgresType.String                   => NpgsqlDbType.Text,
                   PostgresType.CiText                   => NpgsqlDbType.Citext,
                   PostgresType.Binary                   => NpgsqlDbType.Bytea,
                   PostgresType.Date                     => NpgsqlDbType.Date,
                   PostgresType.Time                     => NpgsqlDbType.Time,
                   PostgresType.DateTime                 => NpgsqlDbType.DateRange,
                   PostgresType.DateTimeOffset           => NpgsqlDbType.TimestampTz,
                   PostgresType.TimeSpan                 => NpgsqlDbType.Integer,
                   PostgresType.TimeTz                   => NpgsqlDbType.TimeTz,
                   PostgresType.Inet                     => NpgsqlDbType.Inet,
                   PostgresType.Cidr                     => NpgsqlDbType.Cidr,
                   PostgresType.MacAddr                  => NpgsqlDbType.MacAddr,
                   PostgresType.MacAddr8                 => NpgsqlDbType.MacAddr8,
                   PostgresType.Bit                      => NpgsqlDbType.Bit,
                   PostgresType.Byte                     => NpgsqlDbType.Bit,
                   PostgresType.SByte                    => NpgsqlDbType.Bit,
                   PostgresType.VarBit                   => NpgsqlDbType.Bit,
                   PostgresType.TsVector                 => NpgsqlDbType.TsVector,
                   PostgresType.TsQuery                  => NpgsqlDbType.TsQuery,
                   PostgresType.RegConfig                => NpgsqlDbType.Regconfig,
                   PostgresType.Guid                     => NpgsqlDbType.Uuid,
                   PostgresType.Xml                      => NpgsqlDbType.Xml,
                   PostgresType.Json                     => NpgsqlDbType.Json,
                   PostgresType.Jsonb                    => NpgsqlDbType.Jsonb,
                   PostgresType.JsonPath                 => NpgsqlDbType.JsonPath,
                   PostgresType.Hstore                   => NpgsqlDbType.Hstore,
                   PostgresType.RefCursor                => NpgsqlDbType.Refcursor,
                   PostgresType.OidVector                => NpgsqlDbType.Oidvector,
                   PostgresType.Oid                      => NpgsqlDbType.Oid,
                   PostgresType.Xid                      => NpgsqlDbType.Xid,
                   PostgresType.Xid8                     => NpgsqlDbType.Xid8,
                   PostgresType.Cid                      => NpgsqlDbType.Cid,
                   PostgresType.RegType                  => NpgsqlDbType.Regtype,
                   PostgresType.Tid                      => NpgsqlDbType.Tid,
                   PostgresType.PgLsn                    => NpgsqlDbType.PgLsn,
                   PostgresType.Geometry                 => NpgsqlDbType.Geometry,
                   PostgresType.Geography                => NpgsqlDbType.Geography,
                   PostgresType.LTree                    => NpgsqlDbType.LTree,
                   PostgresType.LQuery                   => NpgsqlDbType.LQuery,
                   PostgresType.LTxtQuery                => NpgsqlDbType.LTxtQuery,
                   PostgresType.IntVector                => NpgsqlDbType.Int2Vector,
                   PostgresType.LongVector               => NpgsqlDbType.BigIntRange,
                   PostgresType.FloatVector              => NpgsqlDbType.Double | NpgsqlDbType.Array,
                   PostgresType.DoubleVector             => NpgsqlDbType.Double | NpgsqlDbType.Array,
                   PostgresType.IntegerRange             => NpgsqlDbType.IntegerRange,
                   PostgresType.BigIntRange              => NpgsqlDbType.BigIntRange,
                   PostgresType.NumericRange             => NpgsqlDbType.NumericRange,
                   PostgresType.TimestampRange           => NpgsqlDbType.TimestampRange,
                   PostgresType.DateTimeOffsetRange      => NpgsqlDbType.TimestampTzRange,
                   PostgresType.DateRange                => NpgsqlDbType.DateRange,
                   PostgresType.IntMultirange            => NpgsqlDbType.IntegerMultirange,
                   PostgresType.LongMultirange           => NpgsqlDbType.BigIntMultirange,
                   PostgresType.NumericMultirange        => NpgsqlDbType.NumericMultirange,
                   PostgresType.TimestampMultirange      => NpgsqlDbType.TimestampMultirange,
                   PostgresType.DateTimeOffsetMultirange => NpgsqlDbType.TimestampTzMultirange,
                   PostgresType.DateMultirange           => NpgsqlDbType.DateMultirange,
                   PostgresType.NotSet                   => throw new OutOfRangeException(type),
                   _                                     => throw new OutOfRangeException(type)
               };
    }
}
