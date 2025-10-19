// Jakar.Extensions :: Jakar.Database
// 10/18/2025  23:29

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



[AttributeUsage(AttributeTargets.Property)]
public sealed class ColumnMetaDataAttribute : Attribute
{
    internal static readonly ColumnMetaDataAttribute Empty = new();
    public                   ColumnOptions?          Options         { get; set; }
    public                   PostgresType?           DbType          { get; set; }
    public                   PrecisionInfo?          Length          { get; set; }
    public                   string?                 ColumnName      { get; set; }
    public                   string?                 IndexColumnName { get; set; }
    public                   string?                 VariableName    { get; set; }
    public                   string?                 Name            { get; set; }
    public                   string?                 KeyValuePair    { get; set; }
    public                   string?                 ForeignKey      { get; set; }


    public void Deconstruct( out string? columnName, out ColumnOptions options, out PrecisionInfo length, out PostgresType? dbType, out string? foreignKeyName, out string? indexColumnName, out string? variableName, out string? keyValuePair, out string? name )
    {
        columnName      = ColumnName;
        options         = Options ?? ColumnOptions.None;
        length          = Length  ?? PrecisionInfo.Default;
        dbType          = DbType;
        foreignKeyName  = ForeignKey;
        indexColumnName = IndexColumnName;
        variableName    = VariableName;
        keyValuePair    = KeyValuePair;
        name            = Name;
    }
}



public sealed class ColumnMetaData<TSelf>( string               propertyName,
                                           string               columnName,
                                           PostgresType         dbType,
                                           ColumnOptions        options         = ColumnOptions.None,
                                           string?              foreignKeyName  = null,
                                           string?              indexColumnName = null,
                                           PrecisionInfo        length          = default,
                                           ColumnCheckMetaData? checks          = null,
                                           string?              variableName    = null,
                                           string?              name            = null,
                                           string?              keyValuePair    = null )
    where TSelf : ITableRecord<TSelf>
{
    public static readonly ColumnMetaData<TSelf> AdditionalData = new(nameof(IJsonModel.AdditionalData), PostgresType.Json, ColumnOptions.Nullable);
    public static readonly ColumnMetaData<TSelf> CreatedBy      = new(nameof(ICreatedBy.CreatedBy), PostgresType.Guid, ColumnOptions.Nullable, UserRecord.TABLE_NAME);
    public static readonly ColumnMetaData<TSelf> DateCreated    = new(nameof(ICreatedBy.DateCreated), PostgresType.DateTimeOffset, ColumnOptions.Indexed);
    public static readonly ColumnMetaData<TSelf> ID             = new(nameof(ICreatedBy.ID), PostgresType.Guid, ColumnOptions.PrimaryKey                      | ColumnOptions.AlwaysIdentity | ColumnOptions.Unique);
    public static readonly ColumnMetaData<TSelf> LastModified   = new(nameof(ILastModified.LastModified), PostgresType.DateTimeOffset, ColumnOptions.Nullable | ColumnOptions.Indexed);


    public readonly bool                 IsNullable       = options.HasFlagValue(ColumnOptions.Nullable);
    public readonly bool                 IsPrimaryKey     = options.HasFlagValue(ColumnOptions.PrimaryKey);
    public readonly ColumnCheckMetaData? Checks           = checks;
    public readonly ColumnOptions        Options          = options;
    public readonly PrecisionInfo        Length           = length;
    public readonly PostgresType         DbType           = dbType;
    public readonly string               PropertyName     = Validate.ThrowIfNull(propertyName);
    public readonly string               ColumnName       = Validate.ThrowIfNull(columnName);
    public readonly string?              ForeignKeyName   = foreignKeyName?.SqlColumnName();
    public readonly string?              IndexColumnName  = indexColumnName?.SqlColumnName();
    public readonly string               Name             = name         ?? $" {columnName} ";
    public readonly string               VariableName     = variableName ?? $" @{columnName} ";
    public readonly string               KeyValuePair     = keyValuePair ?? $" {columnName} = @{columnName} ";
    public readonly Func<TSelf, object?> GetValueAccessor = GetTablePropertyValueAccessor(propertyName);
    public          string               DataType         = dbType.GetPostgresDataType(in length, in options);

    public bool IsForeignKey { [MemberNotNullWhen(true, nameof(IndexColumnName))] get => !string.IsNullOrWhiteSpace(IndexColumnName); }


    public ColumnMetaData( string propertyName, PostgresType dbType, ColumnOptions options = ColumnOptions.None, string? foreignKeyName = null, string? indexColumnName = null, PrecisionInfo length = default, ColumnCheckMetaData? checks = null, string? variableName = null, string? name = null, string? keyValuePair = null ) : this(propertyName,
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


    public static  string GetColumnName( ColumnMetaData<TSelf>   x )        => x.ColumnName;
    public static  string GetVariableName( ColumnMetaData<TSelf> x )        => x.VariableName;
    public static  string GetKeyValuePair( ColumnMetaData<TSelf> x )        => x.KeyValuePair;
    private static bool   IsDbKey( MemberInfo                    property ) => property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() is not null;


    public static FrozenDictionary<string, ColumnMetaData<TSelf>> Create()
    {
        const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;

        PropertyInfo[] properties = typeof(TSelf).GetProperties(ATTRIBUTES)
                                                 .Where(static x => !x.HasAttribute<DbIgnoreAttribute>())
                                                 .ToArray();

        if ( properties.Length <= 0 ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have any public instance properties that are not marked with the '{nameof(DbIgnoreAttribute)}' attribute."); }

        if ( !properties.Any(IsDbKey) ) { throw new InvalidOperationException($"Type '{typeof(TSelf)}' does not have a property with the '{nameof(System.ComponentModel.DataAnnotations.KeyAttribute)}' attribute."); }

        return properties.ToFrozenDictionary(static x => x.Name, Create);
    }
    public static ColumnMetaData<TSelf> Create( PropertyInfo property ) => Create(property, property.GetCustomAttribute<ColumnMetaDataAttribute>());
    internal static ColumnMetaData<TSelf> Create( PropertyInfo property, ColumnMetaDataAttribute? attribute )
    {
        attribute ??= ColumnMetaDataAttribute.Empty;
        attribute.Deconstruct(out string? columnName, out ColumnOptions options, out PrecisionInfo length, out PostgresType? postgresType, out string? foreignKeyName, out string? indexColumnName, out string? variableName, out string? keyValuePair, out string? name);
        string propertyName = property.Name;
        columnName ??= propertyName.ToSnakeCase();
        PostgresType dbType = PostgresTypes.GetType(property.PropertyType, out bool isNullable, out bool isEnum, ref length);
        name = attribute?.Name ?? columnName;

        if ( postgresType.HasValue ) { dbType = postgresType.Value; }

        if ( isNullable ) { options |= ColumnOptions.Nullable; }

        if ( IsDbKey(property) ) { options |= ColumnOptions.PrimaryKey; }

        return new ColumnMetaData<TSelf>(propertyName,
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
    public static Func<TSelf, object?> GetTablePropertyValueAccessor( string propertyName ) => GetTablePropertyValueAccessor(typeof(TSelf).GetProperty(propertyName) ?? throw new InvalidOperationException($"Property '{propertyName}' not found on type '{typeof(TSelf).FullName}'"));
    private static Func<TSelf, object?> GetTablePropertyValueAccessor( PropertyInfo property )
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
    internal readonly SortedDictionary<string, ColumnMetaData<TSelf>> Columns = new(StringComparer.InvariantCultureIgnoreCase);


    public SqlTable() { }
    public static SqlTable<TSelf> Empty => new();

    public static SqlTable<TSelf> Default => Empty.WithColumn(ColumnMetaData<TSelf>.ID)
                                                  .WithColumn(ColumnMetaData<TSelf>.LastModified)
                                                  .WithColumn(ColumnMetaData<TSelf>.DateCreated);

    public void Dispose() => Columns?.Clear();


    // public SqlTableBuilder<TSelf> WithIndexColumn( string indexColumnName, string columnName ) => WithColumn(ColumnMetaData.Indexed(columnName, indexColumnName));

    public SqlTable<TSelf> With_CreatedBy()      => WithColumn(ColumnMetaData<TSelf>.CreatedBy);
    public SqlTable<TSelf> With_AdditionalData() => WithColumn(ColumnMetaData<TSelf>.AdditionalData);


    public SqlTable<TSelf> WithColumn<TValue>( string propertyName, ColumnOptions options = ColumnOptions.None, PrecisionInfo length = default, ColumnCheckMetaData? checks = null, string? indexColumnName = null )
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

        ColumnMetaData<TSelf> column = new(propertyName, dbType, options, foreignKeyName, indexColumnName, length, checks);
        return WithColumn(column);
    }
    public SqlTable<TSelf> WithForeignKey<TRecord>( string propertyName, ColumnOptions options = ColumnOptions.None, ColumnCheckMetaData? checks = null )
        where TRecord : ITableRecord<TRecord>
    {
        ColumnMetaData<TSelf> column = new(propertyName, PostgresType.Guid, options, TRecord.TableName, checks: checks);
        return WithColumn(column);
    }
    public SqlTable<TSelf> WithColumn( ColumnMetaData<TSelf> column )
    {
        Columns.Add(column.PropertyName, column);
        Columns.Add(column.ColumnName,   column);
        return this;
    }


    public FrozenDictionary<string, ColumnMetaData<TSelf>> Build()
    {
        int check = Columns.Values.Count(static x => x.IsPrimaryKey);
        if ( check != 1 ) { throw new InvalidOperationException($"Must be exactly one primary key defined for {typeof(TSelf).Name}"); }

        if ( TSelf.PropertyCount != Columns.Count ) { throw new InvalidOperationException("Column count mismatch"); }

        return Columns.ToFrozenDictionary(StringComparer.InvariantCultureIgnoreCase);
    }
}
