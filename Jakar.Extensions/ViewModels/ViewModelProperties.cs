// Jakar.Extensions :: Jakar.Extensions
// 03/10/2025  15:03


namespace Jakar.Extensions;


public delegate void Handler<in TValue>( TValue? value );



public delegate Task HandlerAsync<in TValue>( TValue? value, CancellationToken token = default );



public static class Properties
{
    public static readonly None EmptyCommand = new();


    public static ViewModelProperty<TCommand, bool> Bool<TCommand>( bool value )
        where TCommand : class, ICommand => new(value);
    public static EmailProperty<TCommand> Email<TCommand>( MaskedTextProvider? mask, string value = EMPTY )
        where TCommand : class, ICommand => new(value, mask);
    public static StringProperty<TCommand> String<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(value);
    public static PhoneNumberProperty<TCommand> PhoneNumber<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(value);
    public static PasswordProperty<TCommand> Password<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(value);


    public static StringCollectionProperty<TCommand> Strings<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(value);
    public static ViewModelCollectionProperty<TCommand, TValue> Collection<TCommand, TValue>( TValue value )
        where TValue : IEquatable<TValue>
        where TCommand : class, ICommand => new(EmptyCommand, value);
    public static ViewModelCollectionProperty<TCommand, TValue> Collection<TCommand, TValue>( Handler<TValue> onSelected, TValue value )
        where TValue : IEquatable<TValue>
        where TCommand : class, ICommand => new(onSelected, value);
    public static ViewModelCollectionProperty<TCommand, TValue> Collection<TCommand, TValue>( HandlerAsync<TValue> onSelected, TValue value )
        where TValue : IEquatable<TValue>
        where TCommand : class, ICommand => new(onSelected, value);


    public static TValue NoCoerceValue<TValue>( TValue                 value ) => value;
    public static bool   HasValue( [NotNullWhen(        true)] string? value ) => !string.IsNullOrWhiteSpace(value);
    public static bool   HasValue<TValue>( [NotNullWhen(true)] TValue? value ) => value is not null;
    public static bool   CheckIsEmail( [NotNullWhen(    true)] string? value ) => HasValue(value) && value.IsEmailAddress();
}



public class ViewModelProperty<TCommand>() : BaseClass()
    where TCommand : class, ICommand
{
    public TCommand? Command          { get; set => SetProperty(ref field, value); }
    public object?   CommandParameter { get; set => SetProperty(ref field, value); }
    public string?   Description      { get; set => SetProperty(ref field, value); }
    public string?   Hint             { get; set => SetProperty(ref field, value); }
    public bool      IsEnabled        { get; set => SetProperty(ref field, value); } = true;
    public bool      IsVisible        { get; set => SetProperty(ref field, value); } = true;
    public string?   Placeholder      { get; set => SetProperty(ref field, value); }
    public string?   Title            { get; set => SetProperty(ref field, value); }
}



public class ViewModelProperty<TCommand, TValue> : ViewModelProperty<TCommand>, IValidator
    where TValue : IEquatable<TValue>
    where TCommand : class, ICommand
{
    private TValue? __value;


    public static Func<Handler<TValue>, TCommand?>      ActionToCommand { get; set; } = static x => null;
    public static Func<HandlerAsync<TValue>, TCommand?> FuncToCommand   { get; set; } = static x => null;
    public        bool                                  IsValid         => HasValue(__value);
    public        TValue?                               Value           { get => __value; set => SetValue(value); }


    public ViewModelProperty( TValue value ) : this(Properties.EmptyCommand, value) { }
    public ViewModelProperty( OneOf<Handler<TValue>, HandlerAsync<TValue>, None> onSelected, TValue value ) : base()
    {
        __value = value;
        Command = onSelected.Match(ActionToCommand, FuncToCommand, none => null);
    }
    public static implicit operator TValue?( ViewModelProperty<TCommand, TValue> property ) => property.Value;


    protected virtual bool    HasValue( [NotNullWhen(true)]                  TValue? value ) => value is not null;
    protected virtual TValue? CoerceValue( [NotNullIfNotNull(nameof(value))] TValue? value ) => value;
    protected virtual bool? SetValue( TValue? value, [CallerMemberName] string propertyName = EMPTY )
    {
        try
        {
            value = CoerceValue(value);

            return SetProperty(ref __value, value, propertyName)
                       ? IsValid
                       : null;
        }
        finally { OnPropertyChanged(nameof(IsValid)); }
    }
}
