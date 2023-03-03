using System.Reflection.Emit;



namespace Jakar.Extensions;


public static class ListExtensions
{
    /// <summary>
    ///     <see href="https://stackoverflow.com/a/17308019/9530917"/>
    /// </summary>
    internal static class ArrayAccessor<T>
    {
        static ArrayAccessor()
        {
            Getter           = CreateGetter();
            CollectionGetter = GetCollectionGetter();
        }
        private static Func<List<T>, T[]> CreateGetter()
        {
            FieldInfo field = typeof(List<T>).GetField( "_items", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException();

            var dm = new DynamicMethod( "get",
                                        MethodAttributes.Static | MethodAttributes.Public,
                                        CallingConventions.Standard,
                                        typeof(T[]),
                                        new[]
                                        {
                                            typeof(List<T>),
                                        },
                                        typeof(ArrayAccessor<T>),
                                        true );

            ILGenerator il = dm.GetILGenerator();
            il.Emit( OpCodes.Ldarg_0 );      // Load List<T> argument
            il.Emit( OpCodes.Ldfld, field ); // Replace argument by field
            il.Emit( OpCodes.Ret );          // Return field

            return (Func<List<T>, T[]>)dm.CreateDelegate( typeof(Func<List<T>, T[]>) );
        }
        private static Func<Collection<T>, List<T>> GetCollectionGetter()
        {
            FieldInfo field = typeof(Collection<T>).GetField( "items", BindingFlags.NonPublic | BindingFlags.Instance ) ?? throw new InvalidOperationException();

            var dm = new DynamicMethod( "get",
                                        MethodAttributes.Static | MethodAttributes.Public,
                                        CallingConventions.Standard,
                                        typeof(List<T>),
                                        new[]
                                        {
                                            typeof(Collection<T>),
                                        },
                                        typeof(ArrayAccessor<T>),
                                        true );

            ILGenerator il = dm.GetILGenerator();
            il.Emit( OpCodes.Ldarg_0 );      // Load List<T> argument
            il.Emit( OpCodes.Ldfld, field ); // Replace argument by field
            il.Emit( OpCodes.Ret );          // Return field

            return (Func<Collection<T>, List<T>>)dm.CreateDelegate( typeof(Func<Collection<T>, List<T>>) );
        }


        internal static readonly Func<List<T>, T[]>           Getter;
        internal static readonly Func<Collection<T>, List<T>> CollectionGetter;
    }



    public static T[] GetInternalArray<T>( this List<T> list ) => ArrayAccessor<T>.Getter( list );
    public static T[] GetInternalArray<T>( this Collection<T> list ) => ArrayAccessor<T>.CollectionGetter( list )
                                                                                        .GetInternalArray();
}
