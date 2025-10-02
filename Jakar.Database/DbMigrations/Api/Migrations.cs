namespace Jakar.Database.DbMigrations;


[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static partial class Migrations
{
    internal static readonly ConcurrentDictionary<string, ConcurrentDictionary<ulong, MigrationRecord>> migrations = new();
    public static void AddMigrations<TClass>()
        where TClass : ITableRecord<TClass>
    {
        ConcurrentDictionary<ulong, MigrationRecord> list = migrations.GetOrAdd(TClass.TableName, static _ => new ConcurrentDictionary<ulong, MigrationRecord>());

        foreach ( MigrationRecord record in MigrationRecord.Migrations )
        {
            if ( list.TryGetValue(record.MigrationID, out _) ) { throw new InvalidOperationException($"Duplicate migration version {record.MigrationID} for {typeof(TClass).Name}"); }

            list[record.MigrationID] = record;
        }
    }
    public static async ValueTask ApplyMigrations( this WebApplication app )
    {
        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        Database                      db    = scope.ServiceProvider.GetRequiredService<Database>();

        await foreach ( MigrationRecord record in db.Migrations.All() )
        {
            if ( !migrations.TryGetValue(record.TableID, out var list) ) { continue; }

            if ( !list.TryGetValue(record.MigrationID, out _) ) { continue; }


        }
    }


    public static DbPropType ToDbPropertyType( this DbType type ) => type switch
                                                                     {
                                                                         DbType.AnsiString            => DbPropType.String,
                                                                         DbType.Binary                => DbPropType.Binary,
                                                                         DbType.Byte                  => DbPropType.Byte,
                                                                         DbType.Boolean               => DbPropType.Boolean,
                                                                         DbType.Currency              => DbPropType.Decimal,
                                                                         DbType.Date                  => DbPropType.Date,
                                                                         DbType.Decimal               => DbPropType.Decimal,
                                                                         DbType.Double                => DbPropType.Double,
                                                                         DbType.Guid                  => DbPropType.Guid,
                                                                         DbType.Int16                 => DbPropType.Int16,
                                                                         DbType.Int32                 => DbPropType.Int32,
                                                                         DbType.Int64                 => DbPropType.Int64,
                                                                         DbType.SByte                 => DbPropType.SByte,
                                                                         DbType.Single                => DbPropType.Double,
                                                                         DbType.String                => DbPropType.String,
                                                                         DbType.StringFixedLength     => DbPropType.String,
                                                                         DbType.Time                  => DbPropType.Time,
                                                                         DbType.UInt16                => DbPropType.UInt16,
                                                                         DbType.UInt32                => DbPropType.UInt32,
                                                                         DbType.UInt64                => DbPropType.UInt64,
                                                                         DbType.VarNumeric            => DbPropType.Decimal,
                                                                         DbType.Xml                   => DbPropType.Xml,
                                                                         DbType.AnsiStringFixedLength => DbPropType.String,
                                                                         DbType.DateTime              => DbPropType.DateTime,
                                                                         DbType.DateTime2             => DbPropType.DateTime,
                                                                         DbType.DateTimeOffset        => DbPropType.DateTimeOffset,
                                                                         _                            => throw new OutOfRangeException(type)
                                                                     };
    public static DbPropType? ToDbPropertyType( this DbType? type ) => type?.ToDbPropertyType();


    /// <summary>
    ///     <see href="https://github.com/fluentmigrator/fluentmigrator/issues/1038"/>
    /// </summary>
    public static bool CreateColumn_Enum( this ICreateTableColumnAsTypeSyntax col, PropertyInfo propertyInfo, Type propertyType )
    {
        if ( !propertyType.TryGetUnderlyingEnumType(out Type? enumType) ) { return false; }

        ICreateTableColumnOptionOrWithColumnSyntax item;

        DbPropType? type = propertyInfo.GetCustomAttribute<DataBaseTypeAttribute>()
                                      ?.Type.ToDbPropertyType();

        if ( type.HasValue )
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch ( type )
            {
                case DbPropType.Byte:
                    item = col.AsByte();
                    return item.SetNullable(propertyInfo);

                case DbPropType.Int16:
                case DbPropType.SByte:
                    item = col.AsInt16();
                    return item.SetNullable(propertyInfo);

                case DbPropType.UInt16:
                case DbPropType.UInt32:
                case DbPropType.Int32:
                    item = col.AsInt32();
                    return item.SetNullable(propertyInfo);

                case DbPropType.UInt64:
                case DbPropType.Int64:
                    item = col.AsInt64();
                    return item.SetNullable(propertyInfo);
            }
        }


        if ( enumType == typeof(byte) ) { item = col.AsByte(); }

        else if ( enumType.IsOneOf(typeof(sbyte), typeof(short)) ) { item = col.AsInt16(); }

        else if ( enumType.IsOneOf(typeof(ushort), typeof(int)) ) { item = col.AsInt32(); }

        else if ( enumType.IsOneOf(typeof(uint), typeof(long), typeof(ulong)) ) { item = col.AsInt64(); }

        else { throw new ExpectedValueTypeException(nameof(enumType), enumType, typeof(byte), typeof(sbyte), typeof(ushort), typeof(short), typeof(int), typeof(uint), typeof(long), typeof(ulong)); }


        // migration.CreateEnumTable(propertyType);
        // return col.CreateColumn_Reference(propertyType);
        // ICreateTableColumnOptionOrWithColumnSyntax item = col.AsInt64().ForeignKey(propertyType.AppName, nameof(IDataBaseID.ID));

        return item.SetNullable(propertyInfo);
    }


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/4963190/9530917"/>
    /// </summary>
    /// <param name="col"> </param>
    /// <param name="propertyType"> </param>
    /// <returns> </returns>
    public static bool CreateColumn_Reference( this ICreateTableColumnAsTypeSyntax col, Type propertyType )
    {
        ICreateTableColumnOptionOrWithColumnSyntax reference = col.AsInt64();

        reference.ForeignKey(propertyType.GetTableName(), nameof(IUniqueID<Guid>.ID));

        // return reference.SetNullable(false);
        return true;
    }


    public static bool IsInitOnly( this PropertyInfo propertyInfo )
    {
        MethodInfo? setMethod = propertyInfo.SetMethod;

        if ( setMethod is null ) { return false; }

        Type isExternalInitType = typeof(IsExternalInit);

        return setMethod.ReturnParameter.GetRequiredCustomModifiers()
                        .ToList()
                        .Contains(isExternalInitType);
    }


    public static bool SetNullable<TNext, TNextFk>( this IColumnOptionSyntax<TNext, TNextFk> item, PropertyInfo propInfo )
        where TNext : IFluentSyntax
        where TNextFk : IFluentSyntax
    {
        if ( propInfo.Name == "DeviceIDs" )
        {
            propInfo.Name.WriteToConsole();
            "------------------------------------------------------------------------".WriteToConsole();
        }

        if ( propInfo.PropertyType.IsNullableType() || propInfo.IsNullable() ) { item.Nullable(); }
        else { item.NotNullable(); }

        return true;
    }

    public static bool ShouldIgnore( this PropertyInfo propInfo )
    {
        if ( propInfo.HasAttribute<DataBaseIgnoreAttribute>() || propInfo.IsDefined(typeof(JsonIgnoreAttribute)) ) { return true; }

        return false;
    }


    public static bool TryGetUnderlyingEnumType( this Type propertyType, [NotNullWhen(true)] out DbPropType? dbType )
    {
        if ( propertyType.TryGetUnderlyingEnumType(out Type? type) )
        {
            dbType = type.GetDbType();
            return true;
        }

        dbType = null;
        return false;
    }


    public static bool TryHandleOtherTypes( this ICreateTableColumnAsTypeSyntax col, PropertyInfo propInfo, Type propertyType )
    {
        if ( col.CreateColumn_Enum(propInfo, propertyType) ) { return true; }

        if ( propertyType == typeof(JsonObject) || propertyType == typeof(JsonNode) || propertyType == typeof(List<JsonObject>) || propertyType == typeof(List<JsonObject?>) || propertyType == typeof(JsonObject[]) || propertyType == typeof(JsonObject) )
        {
            return col.AsXml(int.MaxValue)
                      .SetNullable(propInfo);
        }

        if ( ( propertyType.IsGenericType && propertyType.IsList() ) || propertyType.IsSet() || propertyType.IsCollection() )
        {
            return col.AsXml(int.MaxValue)
                      .SetNullable(propInfo);
        }

        if ( propInfo.GetCustomAttribute<DataBaseTypeAttribute>()
                    ?.Type is DbType.Xml )
        {
            return col.AsXml(int.MaxValue)
                      .SetNullable(propInfo);
        }

        if ( propertyType.HasInterface<IUniqueID<int>>() ) { return col.CreateColumn_Reference(propertyType); }

        if ( propertyType.HasInterface<IUniqueID<long>>() ) { return col.CreateColumn_Reference(propertyType); }

        if ( propertyType.HasInterface<IUniqueID<Guid>>() ) { return col.CreateColumn_Reference(propertyType); }

        return false;
    }


    public static DbPropType GetDbType( this PropertyInfo propertyInfo ) => propertyInfo.PropertyType.GetDbType();
    public static DbPropType GetDbType( this Type propertyType )
    {
        if ( propertyType.TryGetUnderlyingEnumType(out DbPropType? type) ) { return type.Value; }


        if ( propertyType.IsEqualType(typeof(string)) ) { return DbPropType.String; }


        if ( propertyType.IsOneOf(typeof(bool), typeof(bool?)) ) { return DbPropType.Boolean; }


        if ( propertyType.IsOneOf(typeof(byte), typeof(byte?)) ) { return DbPropType.Byte; }


        if ( propertyType.IsOneOf(typeof(short), typeof(short?)) ) { return DbPropType.Int16; }


        if ( propertyType.IsOneOf(typeof(ushort), typeof(ushort?)) ) { return DbPropType.UInt16; }


        if ( propertyType.IsOneOf(typeof(int), typeof(int?)) ) { return DbPropType.Int32; }


        if ( propertyType.IsOneOf(typeof(uint), typeof(uint?)) ) { return DbPropType.UInt32; }


        if ( propertyType.IsOneOf(typeof(long), typeof(long?)) ) { return DbPropType.Int64; }


        if ( propertyType.IsOneOf(typeof(ulong), typeof(ulong?)) ) { return DbPropType.UInt64; }


        if ( propertyType.IsOneOf(typeof(float), typeof(float?), typeof(double), typeof(double?)) ) { return DbPropType.Double; }


        if ( propertyType.IsOneOf(typeof(decimal), typeof(decimal?)) ) { return DbPropType.Decimal; }


        if ( propertyType.IsOneOf(typeof(byte[]), typeof(ReadOnlySpan<byte>)) ) { return DbPropType.Binary; }


        if ( propertyType.IsOneOf(typeof(Guid), typeof(Guid?)) ) { return DbPropType.Guid; }


        if ( propertyType.IsOneOf(typeof(TimeSpan), typeof(TimeSpan?)) ) { return DbPropType.Time; }


        if ( propertyType.IsOneOf(typeof(DateTime), typeof(DateTime?)) ) { return DbPropType.DateTime; }


        if ( propertyType.IsOneOf(typeof(DateTimeOffset), typeof(DateTimeOffset?)) ) { return DbPropType.DateTimeOffset; }


        throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, "Can't discern DbType");
    }


    public static IInsertDataSyntax AddRow<TValue>( this IInsertDataSyntax insert, TValue context )
        where TValue : BaseRecord
    {
        PropertyInfo[]              properties = typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        Dictionary<string, object?> columns    = new(properties.Length);

        foreach ( PropertyInfo property in properties )

        {
            object? value = property.GetValue(context);

            columns[property.Name] = value switch
                                     {
                                         Enum => Convert.ChangeType(value, Enum.GetUnderlyingType(property.PropertyType)),
                                         _    => value
                                     };
        }

        return insert.Row(columns);
    }


    public static string ColumnName( this PropertyInfo prop ) => prop.GetCustomAttribute<ColumnAttribute>()
                                                                    ?.Name ??
                                                                 prop.Name;


    public static string GetMappingTableName( this Type parent, PropertyInfo propertyInfo )
    {
        if ( !propertyInfo.PropertyType.IsList(out Type? itemType) ) { throw new ExpectedValueTypeException(nameof(propertyInfo), propertyInfo.PropertyType, typeof(IList<>)); }

        return parent.GetMappingTableName(propertyInfo, itemType);
    }


    public static string GetMappingTableName<TValue>( this Type   parent, string       propertyName, IEnumerable<TValue> items ) => parent.GetMappingTableName(propertyName,      typeof(TValue));
    public static string GetMappingTableName( this         Type   parent, PropertyInfo propertyInfo, Type                other ) => parent.GetMappingTableName(propertyInfo.Name, other);
    public static string GetMappingTableName( this         Type   parent, string       propertyName, Type                other ) => parent.Name.GetMappingTableName(propertyName, other.Name);
    public static string GetMappingTableName( this         string parent, string       propertyName, string              other ) => $"{parent}_{other}_{propertyName}_mapping";


    public static TNext? TryBuiltInValueTypes<TNext>( this IColumnTypeSyntax<TNext> col, PropertyInfo propInfo, Type propertyType )
        where TNext : IFluentSyntax
    {
        if ( propertyType.TryGetUnderlyingEnumType(out Type? _) )
        {
            // return table.TryCreateBuiltInValueTypes(ref col, propInfo, type);
            return default;
        }


        if ( propertyType.IsEqualType(typeof(string)) )
        {
            return col.AsString(propInfo.GetCustomAttribute<StringLengthAttribute>()
                                       ?.MaximumLength ??
                                throw new InvalidOperationException($"{propertyType.DeclaringType?.Name}.{propertyType.Name}.{propInfo.Name}"));
        }


        if ( propertyType.IsOneOf(typeof(bool), typeof(bool?)) ) { return col.AsBoolean(); }

        if ( propertyType.IsOneOf(typeof(byte), typeof(byte?)) ) { return col.AsByte(); }

        if ( propertyType.IsOneOf(typeof(short), typeof(short?)) ) { return col.AsInt16(); }

        if ( propertyType.IsOneOf(typeof(int), typeof(int?)) ) { return col.AsInt32(); }

        if ( propertyType.IsOneOf(typeof(long), typeof(long?)) ) { return col.AsInt64(); }

        if ( propertyType.IsOneOf(typeof(float), typeof(float?), typeof(double), typeof(double?)) ) { return col.AsDouble(); }

        if ( propertyType.IsOneOf(typeof(decimal), typeof(decimal?)) ) { return col.AsDecimal(); }

        if ( propertyType.IsOneOf(typeof(byte[]), typeof(ReadOnlyMemory<byte>)) ) { return col.AsBinary(); }

        if ( propertyType.IsOneOf(typeof(Guid), typeof(Guid?)) ) { return col.AsGuid(); }

        if ( propertyType.IsOneOf(typeof(TimeSpan), typeof(TimeSpan?), typeof(TimeOnly), typeof(TimeOnly?)) ) { return col.AsTime(); }

        if ( propertyType.IsOneOf(typeof(DateOnly), typeof(DateOnly?)) ) { return col.AsDate(); }

        if ( propertyType.IsOneOf(typeof(DateTime), typeof(DateTime?)) ) { return col.AsDateTime2(); }

        if ( propertyType.IsOneOf(typeof(DateTimeOffset), typeof(DateTimeOffset?)) ) { return col.AsDateTimeOffset(); }


        return default;
    }


    public static async ValueTask MigrateDown( this IHost app )
    {
        await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
        IMigrationRunner              runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(0);
    }
    public static async ValueTask MigrateDown( this IHost app, string key )
    {
        IConfiguration config = app.Services.GetRequiredService<IConfiguration>();
        if ( !config.GetValue(key, false) ) { return; }

        await app.MigrateDown();
    }
    public static async ValueTask MigrateUp( this IHost app, long? version = null )
    {
        await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
        IMigrationRunner              runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();

        if ( version.HasValue )
        {
            if ( runner.HasMigrationsToApplyUp(version.Value) ) { runner.MigrateUp(version.Value); }
        }
        else if ( runner.HasMigrationsToApplyUp() ) { runner.MigrateUp(); }
    }


    public static void AsGuidKey( this ICreateTableColumnAsTypeSyntax col ) =>
        col.AsGuid()
           .NotNullable()
           .PrimaryKey()
           .Identity();
    public static void AsIntKey( this ICreateTableColumnAsTypeSyntax col ) =>
        col.AsInt32()
           .NotNullable()
           .PrimaryKey()
           .Identity();
    public static void AsLongKey( this ICreateTableColumnAsTypeSyntax col ) =>
        col.AsInt64()
           .NotNullable()
           .PrimaryKey()
           .Identity();
    public static void AsStringKey( this ICreateTableColumnAsTypeSyntax col ) =>
        col.AsString(int.MaxValue)
           .NotNullable()
           .PrimaryKey()
           .Identity();


    public static void Throw( this Type classType, PropertyInfo prop )
    {
        string key = $"{classType.FullName}.{prop.Name}";

        _ = prop.PropertyType.TryGetUnderlyingEnumType(out Type? _);

        throw new ExpectedValueTypeException(key,
                                             prop.PropertyType,
                                             typeof(string),
                                             typeof(bool),
                                             typeof(byte),
                                             typeof(byte[]),
                                             typeof(ReadOnlyMemory<byte>),
                                             typeof(short),
                                             typeof(int),
                                             typeof(long),
                                             typeof(double),
                                             typeof(decimal),
                                             typeof(Guid),
                                             typeof(DateTime),
                                             typeof(DateTimeOffset),
                                             typeof(TimeSpan),
                                             typeof(Enum),
                                             typeof(JsonObject),
                                             typeof(JsonNode),
                                             typeof(List<JsonObject>),
                                             typeof(List<JsonObject?>),
                                             typeof(JsonObject),
                                             typeof(JsonObject),
                                             typeof(IDictionary),
                                             typeof(IList));
    }


    public static WebApplicationBuilder AddFluentMigrator( this WebApplicationBuilder builder, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> configureRunner ) =>
        builder.AddFluentMigrator(configureRunner, ConfigureScanIn, GetConnectionString);
    public static WebApplicationBuilder AddFluentMigrator( this WebApplicationBuilder builder, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> configureRunner, Func<IMigrationRunnerBuilder, IMigrationRunnerBuilder> configureScanIn, Func<IServiceProvider, string> getConnectionString )
    {
        builder.Services.AddFluentMigratorCore()
               .ConfigureRunner(runner =>
                                {
                                    configureRunner(runner);

                                    runner.WithGlobalConnectionString(getConnectionString);

                                    configureScanIn(runner);
                                });

        return builder;
    }
    public static string GetConnectionString( IServiceProvider provider ) => GetConnectionString(provider, "DEFAULT");
    public static string GetConnectionString( IServiceProvider provider, string name ) => provider.GetRequiredService<IConfiguration>()
                                                                                                  .GetConnectionString(name) ??
                                                                                          throw new KeyNotFoundException(name);


    public static IMigrationRunnerBuilder ConfigureScanIn( IMigrationRunnerBuilder runner ) =>
        runner.ScanIn(Assembly.GetEntryAssembly(), typeof(UserRecord).Assembly)
              .For.All();


    //
//		/// <summary>
//		/// junction table / XML
//		/// </summary>
//		/// <param name="migration"></param>
//		/// <param name="classType"></param>
//		/// <param name="col"></param>
//		/// <param name="propInfo"></param>
//		/// <param name="propertyType"></param>
//		/// <param name="isList"></param>
//		/// <returns></returns>
//		public static bool TryCreateMapping( this CreateTable                    migration,
//											      Type                           classType,
//											   ref  ICreateTableColumnAsTypeSyntax col,
//											      PropertyInfo                   propInfo,
//											      Type                           propertyType
//		) => col.CreateColumn_XML(propertyType);
}
