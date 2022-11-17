using System.Reflection.Emit;



namespace Jakar.Database;


public static class ListExtensions
{
    /// <summary>
    ///     <see href="https://stackoverflow.com/a/17308019/9530917"/>
    /// </summary>
    private static class ArrayAccessor<T>
    {
        static ArrayAccessor()
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
            Getter = (Func<List<T>, T[]>)dm.CreateDelegate( typeof(Func<List<T>, T[]>) );
        }
        public static readonly Func<List<T>, T[]> Getter;
    }



    public static T[] GetInternalArray<T>( this List<T> list ) => ArrayAccessor<T>.Getter( list );
}
