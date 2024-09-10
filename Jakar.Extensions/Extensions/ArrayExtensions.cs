namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameterInConstructor" )]
public static class ArrayExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining ), RequiresDynamicCode( "Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()" ), SuppressMessage( "ReSharper", "InvokeAsExtensionMethod" )]
    public static ReadOnlySpan<TElement> GetInternalArray<TElement>( this IEnumerable<TElement> values ) => values switch
                                                                                                            {
                                                                                                                TElement[] array                          => array,
                                                                                                                List<TElement> list                       => GetInternalArray( list ),
                                                                                                                ObservableCollection<TElement> collection => collection.AsSpan(),
                                                                                                                Collection<TElement> collection           => GetInternalArray( collection ),
                                                                                                                IList<TElement> collection                => collection.ToArray( collection.Count ),
                                                                                                                IReadOnlyList<TElement> collection        => collection.ToArray( collection.Count ),
                                                                                                                ICollection<TElement> collection          => collection.ToArray( collection.Count ),
                                                                                                                _                                         => values.ToArray()
                                                                                                            };

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ReadOnlySpan<TElement> GetInternalArray<TElement>( this List<TElement> list ) => CollectionsMarshal.AsSpan( list );


    [RequiresDynamicCode( "Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()" ), MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ReadOnlySpan<TElement> GetInternalArray<TElement>( this Collection<TElement> list ) => ArrayAccessor<TElement>.CollectionGetter( list ).GetInternalArray();



    /// <summary> <see href="https://stackoverflow.com/a/17308019/9530917"/> </summary>
    internal static class ArrayAccessor<TElement>
    {
        private static Func<Collection<TElement>, List<TElement>>? _collectionGetter;
        private static Func<List<TElement>, TElement[]>?           _getter;


        internal static Func<Collection<TElement>, List<TElement>> CollectionGetter
        {
            [RequiresDynamicCode( "Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()" )]
            get { return _collectionGetter ??= GetCollectionGetter(); }
        }


        internal static Func<List<TElement>, TElement[]> Getter { [RequiresDynamicCode( "Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.CreateGetter()" )] get { return _getter ??= CreateGetter(); } }


        [RequiresDynamicCode( "System.Reflection.Emit.DynamicMethod.DynamicMethod(String, MethodAttributes, CallingConventions, Type, Type[], Type, Boolean)" )]
        private static Func<List<TElement>, TElement[]> CreateGetter()
        {
            FieldInfo field = typeof(List<TElement>).GetField( "_items", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException();

            DynamicMethod dm = new DynamicMethod( "get", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(TElement[]), [typeof(List<TElement>)], typeof(ArrayAccessor<TElement>), true );

            ILGenerator il = dm.GetILGenerator();
            il.Emit( OpCodes.Ldarg_0 );      // Load List<TElement> argument
            il.Emit( OpCodes.Ldfld, field ); // Replace argument by field
            il.Emit( OpCodes.Ret );          // Return field

            return (Func<List<TElement>, TElement[]>)dm.CreateDelegate( typeof(Func<List<TElement>, TElement[]>) );
        }


        [RequiresDynamicCode( "System.Reflection.Emit.DynamicMethod.DynamicMethod(String, MethodAttributes, CallingConventions, Type, Type[], Type, Boolean)" )]
        private static Func<Collection<TElement>, List<TElement>> GetCollectionGetter()
        {
            FieldInfo field = typeof(Collection<TElement>).GetField( "items", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException();

            DynamicMethod dm = new DynamicMethod( "get", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(List<TElement>), [typeof(Collection<TElement>)], typeof(ArrayAccessor<TElement>), true );

            ILGenerator il = dm.GetILGenerator();
            il.Emit( OpCodes.Ldarg_0 );      // Load List<TElement> argument
            il.Emit( OpCodes.Ldfld, field ); // Replace argument by field
            il.Emit( OpCodes.Ret );          // Return field

            return (Func<Collection<TElement>, List<TElement>>)dm.CreateDelegate( typeof(Func<Collection<TElement>, List<TElement>>) );
        }
    }
}
