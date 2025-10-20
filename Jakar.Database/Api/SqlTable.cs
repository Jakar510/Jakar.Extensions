// Jakar.Extensions :: Jakar.Database
// 10/18/2025  23:29

using Org.BouncyCastle.Asn1.Tsp;



namespace Jakar.Database.DbMigrations;


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



/// <summary>
/// <para>
/// <see cref="Scope"/>:  Order of magnitude of representable range (the exponent range).
/// </para>
/// <para>
/// <see cref="Precision"/>: Reliable decimal digits of accuracy.
/// </para>
/// </summary>
/// <param name="Scope"> Order of magnitude of representable range (the exponent range). </param>
/// <param name="Precision"> Reliable decimal digits of accuracy. </param>
[DefaultValue(nameof(Default))]
public readonly record struct PrecisionInfo( int Scope, int Precision )
{
    public static readonly          PrecisionInfo Default   = new(-1, -1);
    public static readonly          PrecisionInfo Float     = new(38, 7);
    public static readonly          PrecisionInfo Double    = new(308, 15);
    public static readonly          PrecisionInfo Decimal   = new(28, 28);
    public static readonly          PrecisionInfo Int128    = new(128, 0);
    public readonly                 int           Scope     = Scope;
    public readonly                 int           Precision = Precision;
    public readonly                 bool          IsValid   = Scope >= 0 && Precision >= 0;
    public static implicit operator PrecisionInfo( (int Precision, int Scope) value ) => Create(value.Precision, value.Scope);

    public override string ToString() => $"{Scope}, {Precision}";
    public static PrecisionInfo Create( int scope, int precision )
    {
        if ( precision > DECIMAL_MAX_PRECISION ) { throw new OutOfRangeException(precision); }

        if ( scope > DECIMAL_MAX_SCALE ) { throw new OutOfRangeException(scope); }

        return new PrecisionInfo(scope, precision);
    }
}



[DefaultValue(nameof(Default))]
public readonly record struct LengthInfo( int Value )
{
    public static readonly          LengthInfo Default = new(-1);
    public readonly                 int        Value   = Value;
    public readonly                 bool       IsValid = Value >= 0;
    public static implicit operator LengthInfo( int value ) => new(value);
}



[DefaultValue(nameof(Default))]
public readonly record struct SizeInfo( LengthInfo Length, PrecisionInfo Precision )
{
    public static readonly SizeInfo      Default   = new(LengthInfo.Default, PrecisionInfo.Default);
    public readonly        LengthInfo    Length    = Length;
    public readonly        PrecisionInfo Precision = Precision;
    public readonly        bool          IsValid   = Length.IsValid || Precision.IsValid;


    public static implicit operator SizeInfo( int                        value ) => new(value, PrecisionInfo.Default);
    public static implicit operator SizeInfo( LengthInfo                 value ) => new(value, PrecisionInfo.Default);
    public static implicit operator SizeInfo( (int Precision, int Scope) value ) => new(LengthInfo.Default, value);
    public static implicit operator SizeInfo( PrecisionInfo              value ) => new(LengthInfo.Default, value);
}



public sealed record ColumnCheckMetaData( bool And, params string[] Checks )
{
    public readonly                 bool     And    = And;
    public readonly                 string[] Checks = Checks;
    public static implicit operator ColumnCheckMetaData( string   check )  => new(true, check);
    public static implicit operator ColumnCheckMetaData( string[] checks ) => new(true, checks);
}



[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnMetaDataAttribute : Attribute
{
    internal static readonly ColumnMetaDataAttribute Empty = new();
    public                   ColumnOptions?          Options         { get; set; }
    public                   PostgresType?           DbType          { get; set; }
    public                   SizeInfo?               Length          { get; set; }
    public                   string?                 ColumnName      { get; set; }
    public                   string?                 IndexColumnName { get; set; }
    public                   string?                 VariableName    { get; set; }
    public                   string?                 Name            { get; set; }
    public                   string?                 KeyValuePair    { get; set; }
    public                   string?                 ForeignKey      { get; set; }


    public void Deconstruct( out string? columnName, out ColumnOptions options, out SizeInfo length, out PostgresType? dbType, out string? foreignKeyName, out string? indexColumnName, out string? variableName, out string? keyValuePair, out string? name )
    {
        columnName      = ColumnName;
        options         = Options ?? ColumnOptions.None;
        length          = Length  ?? SizeInfo.Default;
        dbType          = DbType;
        foreignKeyName  = ForeignKey;
        indexColumnName = IndexColumnName;
        variableName    = VariableName;
        keyValuePair    = KeyValuePair;
        name            = Name;
    }
}



public sealed class ColumnMetaData( string               propertyName,
                                    string               columnName,
                                    PostgresType         dbType,
                                    ColumnOptions        options         = ColumnOptions.None,
                                    string?              foreignKeyName  = null,
                                    string?              indexColumnName = null,
                                    SizeInfo             length          = default,
                                    ColumnCheckMetaData? checks          = null,
                                    string?              variableName    = null,
                                    string?              name            = null,
                                    string?              keyValuePair    = null )
{
    public static readonly ColumnMetaData AdditionalData = new(nameof(IJsonModel.AdditionalData), PostgresType.Json, ColumnOptions.Nullable);
    public static readonly ColumnMetaData CreatedBy      = new(nameof(ICreatedBy.CreatedBy), PostgresType.Guid, ColumnOptions.Nullable, UserRecord.TABLE_NAME);
    public static readonly ColumnMetaData DateCreated    = new(nameof(ICreatedBy.DateCreated), PostgresType.DateTimeOffset, ColumnOptions.Indexed);
    public static readonly ColumnMetaData ID             = new(nameof(ICreatedBy.ID), PostgresType.Guid, ColumnOptions.PrimaryKey                      | ColumnOptions.AlwaysIdentity | ColumnOptions.Unique);
    public static readonly ColumnMetaData LastModified   = new(nameof(ILastModified.LastModified), PostgresType.DateTimeOffset, ColumnOptions.Nullable | ColumnOptions.Indexed);


    public readonly bool                 IsNullable      = options.HasFlagValue(ColumnOptions.Nullable);
    public readonly bool                 IsPrimaryKey    = options.HasFlagValue(ColumnOptions.PrimaryKey);
    public readonly ColumnCheckMetaData? Checks          = checks;
    public readonly ColumnOptions        Options         = options;
    public readonly SizeInfo             Length          = length;
    public readonly PostgresType         DbType          = dbType;
    public readonly string               PropertyName    = Validate.ThrowIfNull(propertyName);
    public readonly string               ColumnName      = Validate.ThrowIfNull(columnName);
    public readonly string?              ForeignKeyName  = foreignKeyName?.SqlColumnName();
    public readonly string?              IndexColumnName = indexColumnName?.SqlColumnName();
    public readonly string               Name            = name         ?? $" {columnName} ";
    public readonly string               VariableName    = variableName ?? $" @{columnName} ";
    public readonly string               KeyValuePair    = keyValuePair ?? $" {columnName} = @{columnName} ";
    public          string               DataType        = dbType.GetPostgresDataType(in length, in options);


    public bool IsForeignKey { [MemberNotNullWhen(true, nameof(IndexColumnName))] get => !string.IsNullOrWhiteSpace(IndexColumnName); }


    public ColumnMetaData( string propertyName, PostgresType dbType, ColumnOptions options = ColumnOptions.None, string? foreignKeyName = null, string? indexColumnName = null, SizeInfo length = default, ColumnCheckMetaData? checks = null, string? variableName = null, string? name = null, string? keyValuePair = null ) : this(propertyName,
                                                                                                                                                                                                                                                                                                                                      propertyName.SqlColumnName(),
                                                                                                                                                                                                                                                                                                                                      dbType,
                                                                                                                                                                                                                                                                                                                                      options,
                                                                                                                                                                                                                                                                                                                                      foreignKeyName,
                                                                                                                                                                                                                                                                                                                                      indexColumnName,
                                                                                                                                                                                                                                                                                                                                      length,
                                                                                                                                                                                                                                                                                                                                      checks,
                                                                                                                                                                                                                                                                                                                                      variableName,
                                                                                                                                                                                                                                                                                                                                      name,
                                                                                                                                                                                                                                                                                                                                      keyValuePair) { }


    public static  string GetColumnName( ColumnMetaData   x )        => x.ColumnName;
    public static  string GetVariableName( ColumnMetaData x )        => x.VariableName;
    public static  string GetKeyValuePair( ColumnMetaData x )        => x.KeyValuePair;
    private static bool   IsDbKey( MemberInfo             property ) => property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;


    public static FrozenDictionary<string, ColumnMetaData> Create<TSelf>()
        where TSelf : ITableRecord<TSelf>
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = typeof(TSelf).GetProperties(ATTRIBUTES)
                                                 .Where(static x => !x.HasAttribute<DbIgnoreAttribute>())
                                                 .ToArray();

        if ( properties.Length <= 0 ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have any public instance properties that are not marked with the '{nameof(DbIgnoreAttribute)}' attribute."); }

        if ( !properties.Any(IsDbKey) ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have a property with the '{nameof(System.ComponentModel.DataAnnotations.KeyAttribute)}' attribute."); }

        return properties.ToFrozenDictionary(static x => x.Name, Create);
    }
    public static ColumnMetaData Create( PropertyInfo property ) => Create(property, property.GetCustomAttribute<ColumnMetaDataAttribute>(), property.GetCustomAttribute<MaxLengthAttribute>(), property.GetCustomAttribute<LengthAttribute>());
    internal static ColumnMetaData Create( PropertyInfo property, ColumnMetaDataAttribute? attribute, MaxLengthAttribute? stringLength, LengthAttribute? maxLength )
    {
        attribute ??= ColumnMetaDataAttribute.Empty;
        attribute.Deconstruct(out string? columnName, out ColumnOptions options, out SizeInfo length, out PostgresType? postgresType, out string? foreignKeyName, out string? indexColumnName, out string? variableName, out string? keyValuePair, out string? name);
        string propertyName = property.Name;
        columnName ??= propertyName.ToSnakeCase();
        PostgresType dbType = PostgresTypes.GetType(property.PropertyType, out bool isNullable, out bool isEnum, ref length);
        name = attribute?.Name ?? columnName;


        if ( postgresType.HasValue ) { dbType = postgresType.Value; }

        if ( isNullable ) { options |= ColumnOptions.Nullable; }

        if ( IsDbKey(property) ) { options |= ColumnOptions.PrimaryKey; }

        return new ColumnMetaData(propertyName,
                                  columnName,
                                  dbType,
                                  options,
                                  foreignKeyName,
                                  indexColumnName,
                                  length,
                                  null,
                                  variableName,
                                  name,
                                  keyValuePair);
    }


    public static Func<TSelf, object?> GetTablePropertyValueAccessor<TSelf>( string propertyName ) => GetTablePropertyValueAccessor<TSelf>(typeof(TSelf).GetProperty(propertyName) ?? throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(TSelf).FullName}'"));
    private static Func<TSelf, object?> GetTablePropertyValueAccessor<TSelf>( PropertyInfo property )
    {
        // Validate getter and declaring type once
        MethodInfo? getter = property.GetMethod;
        if ( getter is null ) { throw new InvalidOperationException($"Property '{property.Name}' does not have a getter."); }

        Type? declaringType = getter.DeclaringType;
        if ( declaringType is null ) { throw new InvalidOperationException($"Getter for property '{property.Name}' has no declaring type."); }

        // Clear, per-property name helps when inspecting emitted methods
        string methodName = string.Concat(declaringType.FullName, ".__get_", property.Name);

        // Create Emit for the delegate type we want: Func<TSelf, object?>
        Emit<Func<TSelf, object?>> emit = Emit<Func<TSelf, object?>>.NewDynamicMethod(declaringType, methodName);

        // Load the instance argument
        emit.LoadArgument(0);

        // Only cast when the getter's declaring type differs from TSelf
        if ( declaringType != typeof(TSelf) ) { emit.CastClass(declaringType); }

        // Call the getter
        emit.Call(getter);

        // Box value types (reference types require no boxing)
        if ( property.PropertyType.IsValueType ) { emit.Box(property.PropertyType); }

        // Return and create delegate
        emit.Return();
        return emit.CreateDelegate();
    }
}



public readonly ref struct SqlTable<TSelf> : IDisposable
    where TSelf : class, ITableRecord<TSelf>
{
    internal readonly SortedDictionary<string, ColumnMetaData> Columns = new(StringComparer.InvariantCultureIgnoreCase);


    public SqlTable() { }
    public static SqlTable<TSelf> Empty => new();

    public static SqlTable<TSelf> Default => Empty.WithColumn(ColumnMetaData.ID)
                                                  .WithColumn(ColumnMetaData.LastModified)
                                                  .WithColumn(ColumnMetaData.DateCreated);

    public void Dispose() => Columns?.Clear();


    // public SqlTableBuilder<TSelf> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn(ColumnMetaData.Indexed(columnName, indexColumnName));

    public SqlTable<TSelf> With_CreatedBy()      => WithColumn(ColumnMetaData.CreatedBy);
    public SqlTable<TSelf> With_AdditionalData() => WithColumn(ColumnMetaData.AdditionalData);


    public SqlTable<TSelf> WithColumn<TValue>( string propertyName, ColumnOptions options = ColumnOptions.None, SizeInfo length = default, ColumnCheckMetaData? checks = null, string? indexColumnName = null )
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

        ColumnMetaData column = new(propertyName, dbType, options, foreignKeyName, indexColumnName, length, checks);
        return WithColumn(column);
    }
    public SqlTable<TSelf> WithForeignKey<TRecord>( string propertyName, ColumnOptions options = ColumnOptions.None, ColumnCheckMetaData? checks = null )
        where TRecord : ITableRecord<TRecord>
    {
        ColumnMetaData column = new(propertyName, PostgresType.Guid, options, TRecord.TableName, checks: checks);
        return WithColumn(column);
    }
    public SqlTable<TSelf> WithColumn( ColumnMetaData column )
    {
        Columns.Add(column.PropertyName, column);
        Columns.Add(column.ColumnName,   column);
        return this;
    }


    public FrozenDictionary<string, ColumnMetaData> Build()
    {
        int check = Columns.Values.Count(static x => x.IsPrimaryKey);
        if ( check != 1 ) { throw new InvalidOperationException($"Must be exactly one primary key defined for {typeof(TSelf).Name}"); }

        if ( TSelf.PropertyCount != Columns.Count ) { throw new InvalidOperationException("Column count mismatch"); }

        return Columns.ToFrozenDictionary(StringComparer.InvariantCultureIgnoreCase);
    }
}
