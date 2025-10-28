namespace Jakar.Shapes;


public delegate TValue RefSelect<TValue>( ref readonly TValue value );



public delegate TOutput RefSelect<TInput, out TOutput>( ref readonly TInput value );
