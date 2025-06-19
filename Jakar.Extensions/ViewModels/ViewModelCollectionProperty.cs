// Jakar.Extensions :: Jakar.Extensions
// 06/19/2025  11:30

namespace Jakar.Extensions;


public class ViewModelCollectionProperty<TCommand, TValue>( IEqualityComparer<TValue?> equalityComparer, OneOf<Handler<TValue>, HandlerAsync<TValue>, None> onSelected, TValue value ) : ViewModelProperty<TCommand, TValue>( equalityComparer, onSelected, value )
    where TValue : IEquatable<TValue>
    where TCommand : class, ICommand
{
    public ObservableCollection<TValue> Values { get; } = new(DEFAULT_CAPACITY);
    public ViewModelCollectionProperty( IEqualityComparer<TValue?> equalityComparer, TValue value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }
    
    
    public void With( params ReadOnlySpan<TValue> values )
    {
        Values.Clear();
        Values.Add( values );
    }
}



public class StringCollectionProperty<TCommand>( IEqualityComparer<string?> equalityComparer, OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : ViewModelCollectionProperty<TCommand, string>( equalityComparer, onSelected, value )
    where TCommand : class, ICommand
{
    public StringCollectionProperty( IEqualityComparer<string?> equalityComparer, string value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }
}
