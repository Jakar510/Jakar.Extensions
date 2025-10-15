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
    IntegerMultirange,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int8multirange" type.
    /// </summary>
    BigIntMultirange,

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
