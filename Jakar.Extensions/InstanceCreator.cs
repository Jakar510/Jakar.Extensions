using System.Linq.Expressions;



#nullable enable
namespace Jakar.Extensions;


internal static class InstanceCreator
{
    public static NullReferenceException CreateException( params Type[] args ) => new($"constructor not found. Requested arg types: {args}");
}



public static class InstanceCreator<TItem>
{
    private static Type _Type => typeof(TItem);


    public static TItem Create() => (TItem)Activator.CreateInstance(_Type);
    public static TItem Create( params object[] args ) => (TItem)Activator.CreateInstance(_Type,                                                                                            args);
    public static TItem Create( bool            nonPublic ) => (TItem)Activator.CreateInstance(_Type,                                                                                       nonPublic);
    public static TItem Create( object[]        args,        object[] activationAttributes ) => (TItem)Activator.CreateInstance(_Type,                                                      args,        activationAttributes);
    public static TItem Create( BindingFlags    bindingAttr, Binder   binder, object[] args, CultureInfo culture ) => (TItem)Activator.CreateInstance(_Type,                                bindingAttr, binder, args, culture);
    public static TItem Create( BindingFlags    bindingAttr, Binder   binder, object[] args, CultureInfo culture, object[] activationAttributes ) => (TItem)Activator.CreateInstance(_Type, bindingAttr, binder, args, culture, activationAttributes);
}



public static class InstanceCreator<T1, TInstance>
{
    public static Func<T1, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1)
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}



public static class InstanceCreator<T1, T2, TInstance>
{
    public static Func<T1, T2, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1),
            typeof(T2)
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, T2, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}



public static class InstanceCreator<T1, T2, T3, TInstance>
{
    public static Func<T1, T2, T3, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1),
            typeof(T2),
            typeof(T3)
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, T2, T3, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}



public static class InstanceCreator<T1, T2, T3, T4, TInstance>
{
    public static Func<T1, T2, T3, T4, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}



public static class InstanceCreator<T1, T2, T3, T4, T5, TInstance>
{
    public static Func<T1, T2, T3, T4, T5, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, T5, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, T5, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}



public static class InstanceCreator<T1, T2, T3, T4, T5, T6, TInstance>
{
    public static Func<T1, T2, T3, T4, T5, T6, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, T5, T6, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}



public static class InstanceCreator<T1, T2, T3, T4, T5, T6, T7, TInstance>
{
    public static Func<T1, T2, T3, T4, T5, T6, T7, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, T5, T6, T7, TInstance> CreateInstance()
    {
        Type[] argsTypes =
        {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
        };

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter).ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, TInstance>>(Expression.New(constructor, args), args).Compile();
    }
}
