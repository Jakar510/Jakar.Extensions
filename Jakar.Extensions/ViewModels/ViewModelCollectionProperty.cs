// Jakar.Extensions :: Jakar.Extensions
// 06/19/2025  11:30

namespace Jakar.Extensions;


public class ViewModelCollectionProperty<TCommand, TValue>( OneOf<Handler<TValue>, HandlerAsync<TValue>, None> onSelected, TValue value ) : ViewModelProperty<TCommand, TValue>(onSelected, value)
    where TValue : IEquatable<TValue>
    where TCommand : class, ICommand
{
    public ObservableCollection<TValue> Values { get; } = new(DEFAULT_CAPACITY);
    public ViewModelCollectionProperty( TValue value ) : this(Properties.EmptyCommand, value) { }


    public void With( params ReadOnlySpan<TValue> values )
    {
        Values.Clear();
        Values.Add(values);
    }
}



public class StringCollectionProperty<TCommand>( OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : ViewModelCollectionProperty<TCommand, string>(onSelected, value)
    where TCommand : class, ICommand
{
    public StringCollectionProperty( string value ) : this(Properties.EmptyCommand, value) { }
}
