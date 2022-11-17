namespace Jakar.Database.Migrations;


public static class PropertyCache
{
    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _cache    = new();
    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _id_Cache = new();


    public static IEnumerable<PropertyInfo> GetProperties( this object o, bool removeID ) => o.GetType()
                                                                                              .GetProperties( removeID );
    public static IEnumerable<PropertyInfo> GetProperties( this Type type, in bool removeID )
    {
        if ( removeID && _cache.TryGetValue( type, out List<PropertyInfo>? paramNames ) ) { return paramNames; }

        if ( _id_Cache.TryGetValue( type, out paramNames ) ) { return paramNames; }


        PropertyInfo[] properties = type.GetProperties( BindingFlags.Instance | BindingFlags.Public );
        paramNames = new List<PropertyInfo>( properties.Length );

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( PropertyInfo prop in properties.Where( p => p.GetGetMethod( false ) != null ) )
        {
            if ( prop.HasAttribute<DataBaseIgnoreAttribute>( true ) ) { continue; }

            if ( removeID && prop.Name == nameof(IDataBaseID.ID) ) { continue; }

            paramNames.Add( prop );
        }

        if ( removeID ) { _cache[type] = paramNames; }
        else { _id_Cache[type]         = paramNames; }

        return paramNames;
    }

    // public static object GetItem( string name, Type returnType, Module module, params Type[] methodArgs )
    // {
    // 	var method = new DynamicMethod(name, returnType, methodArgs, module);
    // 	ILGenerator il = method.GetILGenerator();
    //
    // 	method.CreateDelegate<>()
    // }


    public static IEnumerable<string> GetParamNames( this object o, in bool removeID )
    {
        if ( o is DynamicParameters parameters ) { return parameters.ParameterNames.ToList(); }

        return o.GetType()
                .GetParamNames( removeID );
    }
    public static IEnumerable<string> GetParamNames( this Type type, in bool removeID ) => type.GetProperties( removeID )
                                                                                               .Select( x => x.Name );
}
