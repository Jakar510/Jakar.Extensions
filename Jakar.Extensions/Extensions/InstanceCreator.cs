namespace Jakar.Extensions;


internal static class InstanceCreator
{
    public static NullReferenceException CreateException( params Type[] args ) => new($"constructor not found. Requested arg types: {args}");
}



public static class InstanceCreator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Type Type { get; } = typeof(TInstance);


    public static TInstance Create()                                                                                                                  => (TInstance)( Activator.CreateInstance(typeof(TInstance))                                                           ?? throw new InvalidOperationException() );
    public static TInstance Create( params object[] args )                                                                                            => (TInstance)( Activator.CreateInstance(typeof(TInstance), args)                                                     ?? throw new InvalidOperationException() );
    public static TInstance Create( bool            nonPublic )                                                                                       => (TInstance)( Activator.CreateInstance(typeof(TInstance), nonPublic)                                                ?? throw new InvalidOperationException() );
    public static TInstance Create( object[]        args,        object[] activationAttributes )                                                      => (TInstance)( Activator.CreateInstance(typeof(TInstance), args,        activationAttributes)                        ?? throw new InvalidOperationException() );
    public static TInstance Create( BindingFlags    bindingAttr, Binder   binder, object[] args, CultureInfo culture )                                => (TInstance)( Activator.CreateInstance(typeof(TInstance), bindingAttr, binder, args, culture)                       ?? throw new InvalidOperationException() );
    public static TInstance Create( BindingFlags    bindingAttr, Binder   binder, object[] args, CultureInfo culture, object[] activationAttributes ) => (TInstance)( Activator.CreateInstance(typeof(TInstance), bindingAttr, binder, args, culture, activationAttributes) ?? throw new InvalidOperationException() );
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, T2, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, T2, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1), typeof(T2)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, T2, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, T2, T3, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, T2, T3, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1), typeof(T2), typeof(T3)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, T2, T3, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, T2, T3, T4, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, T2, T3, T4, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, T2, T3, T4, T5, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, T2, T3, T4, T5, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, T5, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, T5, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, T2, T3, T4, T5, T6, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, T2, T3, T4, T5, T6, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, T5, T6, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}



[SuppressMessage("ReSharper", "CoVariantArrayConversion")]
public static class InstanceCreator<T1, T2, T3, T4, T5, T6, T7, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] TInstance>
{
    public static Func<T1, T2, T3, T4, T5, T6, T7, TInstance> Create { get; } = CreateInstance();

    private static Func<T1, T2, T3, T4, T5, T6, T7, TInstance> CreateInstance()
    {
        Type[] argsTypes = [typeof(T1), typeof(T2), typeof(T3), typeof(T4)];

        ConstructorInfo? constructor = typeof(TInstance).GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, argsTypes, null);
        if ( constructor is null ) { throw InstanceCreator.CreateException(argsTypes); }

        ParameterExpression[] args = argsTypes.Select(Expression.Parameter)
                                              .ToArray();


        return Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, TInstance>>(Expression.New(constructor, args), args)
                         .Compile();
    }
}
