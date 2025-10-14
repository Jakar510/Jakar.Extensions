// Jakar.Extensions :: Jakar.Database
// 10/13/2025  22:56

using System.Xml;
using Jakar.Shapes;



namespace Jakar.Database.DbMigrations;


public enum PostgresType
{
    Guid,
    Byte,
    SByte,
    Int128,
    UInt128,
    Single,
    Decimal,
    Currency,
    Binary,
    String,
    Object,
    Enum,
    Linestring,

    /// <summary>
    /// Corresponds to the PostgreSQL "bigint" type.
    /// <para> <see cref="long"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Bigint = 1,

    /// <summary>
    /// Corresponds to the PostgreSQL "double" type.
    /// <para> <see cref="double"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Double = 8,

    /// <summary>
    /// Corresponds to the PostgreSQL "integer" type.
    /// <para> <see cref="int"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>
    /// </remarks>
    Integer = 9,

    /// <summary>
    /// Represents the PostgreSQL arbitrary-precision "numeric" type.
    /// </summary>
    /// <remarks> For more information, see the PostgreSQL documentation at <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/> </remarks>
    Numeric = 13,

    /// <summary>
    /// Represents the PostgreSQL floating-point "real" data type.
    /// <para> <see cref="decimal"/> </para>
    /// </summary>
    /// <remarks>
    /// The "real" type in PostgreSQL is a single-precision, 4-byte floating-point number.
    /// For more information, see the PostgreSQL documentation at <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/>.
    /// </remarks>
    Real = 17,

    /// <summary>
    /// Corresponds to the PostgreSQL 2-byte "smallint" type.
    /// <para> <see cref="short"/> </para>
    /// <para> <see href="https://www.postgresql.org/docs/current/static/datatype-numeric.html"/> </para>
    /// </summary>
    Smallint = 18,

    /// <summary>
    /// Corresponds to the PostgreSQL "money" type.
    /// <para> <see cref="Decimal"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-money.html"/>
    /// </remarks>
    Money = 12,

    /// <summary>
    /// Corresponds to the PostgreSQL "boolean" type.
    /// <para> <see cref="bool"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-boolean.html"/>
    /// </remarks>
    Boolean = 2,

    /// <summary>
    /// Corresponds to the PostgreSQL "box" type.
    /// <para> <see cref="ReadOnlyRectangle"/> </para>
    /// <para> <see cref="ReadOnlyRectangleF"/> </para>
    /// <para> <see cref="MutableRectangle"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Box = 3,

    /// <summary>
    /// Corresponds to the PostgreSQL "circle" type.
    /// <para> <see cref="Circle"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Circle = 5,

    /// <summary>
    /// Corresponds to the PostgreSQL "line" type.
    /// <para> <see cref="ReadOnlyPoint"/> </para>
    /// <para> <see cref="ReadOnlyPointF"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Line = 10,

    /// <summary>
    /// Corresponds to the PostgreSQL "lseg" type.
    /// <para> <see cref="ReadOnlyLine"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    LineSegment = 11,

    /// <summary>
    /// Corresponds to the PostgreSQL "path" type.
    /// <para> <see cref="Spline"/> </para>
    /// <para> <see cref="CalculatedLine"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Path = 14,

    /// <summary>
    /// Corresponds to the PostgreSQL "point" type.
    /// <para> <see cref="ReadOnlyPoint"/> </para>
    /// <para> <see cref="ReadOnlyPointF"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Point = 15,

    /// <summary>
    /// Corresponds to the PostgreSQL "polygon" type.
    /// <para> <see cref="ReadOnlyPoint"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-geometric.html"/>
    /// </remarks>
    Polygon = 16,

    /// <summary>
    /// Corresponds to the PostgreSQL "char" type.
    /// <para> <see cref="char"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-character.html"/>
    /// </remarks>
    Char = 6,

    /// <summary>
    /// Corresponds to the PostgreSQL "text" type.
    /// <para> <see cref="string"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-character.html"/>
    /// </remarks>
    Text = 19,

    /// <summary>
    /// Corresponds to the PostgreSQL "varchar" type.
    /// <para> <see cref="string"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-character.html"/>
    /// </remarks>
    Varchar = 22,

    /// <summary>
    /// Corresponds to the PostgreSQL "name" type.
    /// <para> <see cref="string"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-character.html"/>
    /// </remarks>
    Name = 32,

    /// <summary>
    /// Corresponds to the PostgreSQL "citext" type.
    /// <para> <see cref="string"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/citext.html"/>
    /// </remarks>
    CiText = 51,

    /// <summary>
    /// Corresponds to the PostgreSQL "bytea" type, holding a raw byte string.
    /// <para> <see cref="byte[]"/> </para>
    /// <para> <see cref="Memory{byte}"/> </para>
    /// <para> <see cref="ReadOnlyMemory{byte}"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-binary.html"/>
    /// </remarks>
    Bytea = 4,

    /// <summary>
    /// Corresponds to the PostgreSQL "date" type.
    /// <para> <see cref="DateOnly"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    Date = 7,

    /// <summary>
    /// Corresponds to the PostgreSQL "time" type.
    /// <para> <see cref="TimeOnly"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    Time = 20,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestamp" type.
    /// <para> <see cref="DateTime"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    DateTime = 21,

    /// <summary>
    /// Corresponds to the PostgreSQL "timestampz" type.
    /// <para> <see cref="DateTimeOffset"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    DateTimeOffset = 26,

    /// <summary>
    /// Corresponds to the PostgreSQL "interval" type.
    /// <para> <see cref="TimeSpan"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    Interval = 30,

    /// <summary>
    /// Corresponds to the PostgreSQL "time with time zone" type.
    /// <para> <see cref="DateTime"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-datetime.html"/>
    /// </remarks>
    TimeTz = 31,

    /// <summary>
    /// Corresponds to the PostgreSQL "inet" type.
    /// <para> <see cref="IPAddress"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    Inet = 24,

    /// <summary>
    /// Corresponds to the PostgreSQL "cidr" type, a field storing an IPv4 or IPv6 network.
    /// <para> <see cref="MacAddr"/> </para>
    /// <para> <see cref="MacAddr8"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    Cidr = 44,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr" type, a field storing a 6-byte physical address.
    /// <para> <see cref="MacAddr"/> </para> 
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    MacAddr = 34,

    /// <summary>
    /// Corresponds to the PostgreSQL "macaddr8" type, a field storing a 6-byte or 8-byte physical address.
    /// <para> <see cref="MacAddr8"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-net-types.html"/>
    /// </remarks>
    MacAddr8 = 54,

    /// <summary>
    /// Corresponds to the PostgreSQL "bit" type.
    /// <para> <see cref="byte"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-bit.html"/>
    /// </remarks>
    Bit = 25,

    /// <summary>
    /// Corresponds to the PostgreSQL "varbit" type.
    /// <para> <see cref="byte"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-bit.html"/>
    /// </remarks>
    VarBit = 39,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsvector" type.
    /// <para> <see cref="Regex"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-textsearch.html"/>
    /// </remarks>
    TsVector = 45,

    /// <summary>
    /// Corresponds to the PostgreSQL "tsquery" type.
    /// <para> <see cref="Regex"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-textsearch.html"/>
    /// </remarks>
    TsQuery = 46,

    /// <summary>
    /// Corresponds to the PostgreSQL "regconfig" type.
    /// <para> <see cref="Regex"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-textsearch.html"/>
    /// </remarks>
    RegConfig = 56,

    /// <summary>
    /// Corresponds to the PostgreSQL "uuid" type.
    /// <para> <see cref="Guid"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-uuid.html"/>
    /// </remarks>
    Uuid = 27,

    /// <summary>
    /// Corresponds to the PostgreSQL "xml" type.
    /// <para> <see cref="XmlDocument"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-xml.html"/>
    /// </remarks>
    Xml = 28,

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
    Json = 35,

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
    Jsonb = 36,

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
    JsonPath = 57,

    /// <summary>
    /// Corresponds to the PostgreSQL "hstore" type, a dictionary of string key-value
    /// <para> <see cref="Dictionary{string,string}"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/hstore.html"/>
    /// </remarks>
    Hstore = 37,

    /// <summary>
    /// Corresponds to the PostgreSQL "refcursor" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-refcursor.html"/>
    /// </remarks>
    RefCursor = 23,

    /// <summary>
    /// Corresponds to the PostgreSQL "oidvector" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    OidVector = 29,

    /// <summary>
    /// Corresponds to the PostgreSQL "int2vector" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-int2vector.html"/>
    /// </remarks>
    Int2Vector = 52,

    /// <summary>
    /// Corresponds to the PostgreSQL "oid" type.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Oid = 41,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid" type, an internal transaction identifier.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Xid = 42,

    /// <summary>
    /// Corresponds to the PostgreSQL "xid8" type, an internal transaction identifier.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Xid8 = 64,

    /// <summary>
    /// Corresponds to the PostgreSQL "cid" type, an internal transaction identifier.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    Cid = 43,

    /// <summary>
    /// Corresponds to the PostgreSQL "regtype" type, a numeric (OID) ID of a type in the pg_type table.
    /// <para> <see cref="object"/> </para>
    /// </summary>
    /// <remarks>
    ///  For more information, see the PostgreSQL documentation at <see cref="https://www.postgresql.org/docs/current/static/datatype-oid.html"/>
    /// </remarks>
    RegType = 49,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tid" type, a tuple id identifying the physical location of a row within its table.
    /// </summary>
    Tid = 53,

    /// <summary>
    ///     Corresponds to the PostgreSQL "pg_lsn" type, which can be used to store LSN (Log Sequence Number) data which is a pointer to a location in the WAL.
    /// </summary>
    /// <remarks>
    /// <see cref="https://www.postgresql.org/docs/current/datatype-pg-lsn.html"/> and <see cref="https://git.postgresql.org/gitweb/?p=postgresql.git;a=commit;h=7d03a83f4d0736ba869fa6f93973f7623a27038a"/>
    /// </remarks>
    PgLsn = 59,

    /// <summary>
    ///     A special value that can be used to send parameter values to the database without specifying their type, allowing the database to cast them to another value based on context. The value will be converted to a string and send as text.
    /// </summary>
    /// <remarks> This value shouldn't ordinarily be used, and makes sense only when sending a data type unsupported by Npgsql. </remarks>
    Unknown = 40,

    /// <summary>
    ///     The geometry type for PostgreSQL spatial extension PostGIS.
    /// </summary>
    Geometry = 50,

    /// <summary>
    ///     The geography (geodetic) type for PostgreSQL spatial extension PostGIS.
    /// </summary>
    Geography = 55,
    
    /// <summary>
    ///     The PostgreSQL ltree type, each value is a label path "a.label.tree.value", forming a tree in a set.
    /// </summary> 
    ///  <remarks> See https://www.postgresql.org/docs/current/static/ltree.html </remarks>
    LTree = 60,

    /// <summary>
    ///     The PostgreSQL lquery type for PostgreSQL extension ltree
    /// </summary>
    ///  <remarks>  See https://www.postgresql.org/docs/current/static/ltree.html </remarks>
    LQuery = 61,

    /// <summary>
    ///     The PostgreSQL ltxtquery type for PostgreSQL extension ltree
    /// </summary>
    ///  <remarks>    See https://www.postgresql.org/docs/current/static/ltree.html </remarks>
    LTxtQuery = 62,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int4range" type.
    /// </summary>
    IntegerRange = 1073741833,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int8range" type.
    /// </summary>
    BigIntRange = 1073741825,

    /// <summary>
    /// Represents the PostgreSQL "numrange" type, which is a range of numeric values.    Corresponds to the PostgreSQL "numrange" type.
    /// </summary>
    /// <remarks>The "numrange" type in PostgreSQL is used to define a range of numeric values,  such as
    /// integers or decimals. This can be useful for representing intervals or  ranges in database queries and
    /// operations.</remarks>
    NumericRange = 1073741837,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tsrange" type.
    /// </summary>
    TimestampRange = 1073741845,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tstzrange" type.
    /// </summary>
    DateTimeOffsetRange = 1073741850,

    /// <summary>
    ///     Corresponds to the PostgreSQL "daterange" type.
    /// </summary>
    DateRange = 1073741831,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int4multirange" type.
    /// </summary>
    IntegerMultirange = 536870921,

    /// <summary>
    ///     Corresponds to the PostgreSQL "int8multirange" type.
    /// </summary>
    BigIntMultirange = 536870913,

    /// <summary>
    ///     Corresponds to the PostgreSQL "nummultirange" type.
    /// </summary>
    NumericMultirange = 536870925,
    
    /// <summary>
    ///     Corresponds to the PostgreSQL "tsmultirange" type.
    /// </summary>
    TimestampMultirange = 536870933,

    /// <summary>
    ///     Corresponds to the PostgreSQL "tstzmultirange" type.
    /// </summary>
    DateTimeOffsetMultirange = 536870938,

    /// <summary>
    ///     Corresponds to the PostgreSQL "datemultirange" type.
    /// </summary>
    DateMultirange = 536870919,

    /// <summary>
    ///     Corresponds to the PostgreSQL "array" type, a variable-length multidimensional array of another type. This value must be combined with another value from NpgsqlTypes.NpgsqlDbType via a bit OR (e.g. NpgsqlDbType.Array | NpgsqlDbType.Integer)
    /// </summary>
    /// <remarks><see cref="https://www.postgresql.org/docs/current/static/arrays.html"/></remarks>
    Array = int.MinValue,

    /// <summary>
    ///  Corresponds to the PostgreSQL "range" type, continuous range of values of specific type. This value must be combined with another value from NpgsqlTypes.NpgsqlDbType via a bit OR (e.g. NpgsqlDbType.Range | NpgsqlDbType.Integer)
    /// </summary>
    /// <remarks>Supported since PostgreSQL 9.2. See <see cref="https://www.postgresql.org/docs/current/static/rangetypes.html"/></remarks>
    Range = 1073741824,

    /// <summary>
    ///     Corresponds to the PostgreSQL "multirange" type, continuous range of values of specific type. This value must be combined with another value from NpgsqlTypes.NpgsqlDbType via a bit OR (e.g. NpgsqlDbType.Multirange | NpgsqlDbType.Integer)
    /// </summary>
    /// <remarks>Supported since PostgreSQL 14. See <see cref="https://www.postgresql.org/docs/current/static/rangetypes.html"/></remarks>
    Multirange = 536870912
}
