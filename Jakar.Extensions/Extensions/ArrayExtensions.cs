﻿namespace Jakar.Extensions;


public static class ArrayExtensions
{
    [ MethodImpl( MethodImplOptions.AggressiveInlining ), SuppressMessage( "ReSharper", "InvokeAsExtensionMethod" ) ]
    public static ReadOnlySpan<TElement> GetInternalArray<TElement>( this IEnumerable<TElement> values ) => values switch
                                                                                                            {
                                                                                                                TElement[] array                   => array,
                                                                                                                List<TElement> list                => list.GetInternalArray(),
                                                                                                                Collection<TElement> collection    => collection.GetInternalArray(),
                                                                                                                IReadOnlyList<TElement> collection => collection.ToArray(),
                                                                                                                _                                  => values.ToArray()
                                                                                                            };


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static ReadOnlySpan<TElement> GetInternalArray<TElement>( this List<TElement>       list ) => new(ArrayAccessor<TElement>.Getter( list ), 0, list.Count);
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static ReadOnlySpan<TElement> GetInternalArray<TElement>( this Collection<TElement> list ) => ArrayAccessor<TElement>.CollectionGetter( list ).GetInternalArray();



    /// <summary> <see href="https://stackoverflow.com/a/17308019/9530917"/> </summary>
    internal static class ArrayAccessor<TElement>
    {
        internal static readonly Func<Collection<TElement>, List<TElement>> CollectionGetter;
        internal static readonly Func<List<TElement>, TElement[]>           Getter;


        static ArrayAccessor()
        {
            Getter           = CreateGetter();
            CollectionGetter = GetCollectionGetter();
        }
        private static Func<List<TElement>, TElement[]> CreateGetter()
        {
            FieldInfo field = typeof(List<TElement>).GetField( "_items", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException();

            var dm = new DynamicMethod( "get",
                                        MethodAttributes.Static | MethodAttributes.Public,
                                        CallingConventions.Standard,
                                        typeof(TElement[]),
                                        new[]
                                        {
                                            typeof(List<TElement>)
                                        },
                                        typeof(ArrayAccessor<TElement>),
                                        true );

            ILGenerator il = dm.GetILGenerator();
            il.Emit( OpCodes.Ldarg_0 );      // Load List<TElement> argument
            il.Emit( OpCodes.Ldfld, field ); // Replace argument by field
            il.Emit( OpCodes.Ret );          // Return field

            return (Func<List<TElement>, TElement[]>)dm.CreateDelegate( typeof(Func<List<TElement>, TElement[]>) );
        }
        private static Func<Collection<TElement>, List<TElement>> GetCollectionGetter()
        {
            FieldInfo field = typeof(Collection<TElement>).GetField( "items", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException();

            var dm = new DynamicMethod( "get",
                                        MethodAttributes.Static | MethodAttributes.Public,
                                        CallingConventions.Standard,
                                        typeof(List<TElement>),
                                        new[]
                                        {
                                            typeof(Collection<TElement>)
                                        },
                                        typeof(ArrayAccessor<TElement>),
                                        true );

            ILGenerator il = dm.GetILGenerator();
            il.Emit( OpCodes.Ldarg_0 );      // Load List<TElement> argument
            il.Emit( OpCodes.Ldfld, field ); // Replace argument by field
            il.Emit( OpCodes.Ret );          // Return field

            return (Func<Collection<TElement>, List<TElement>>)dm.CreateDelegate( typeof(Func<Collection<TElement>, List<TElement>>) );
        }
    }
}
