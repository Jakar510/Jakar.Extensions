// Jakar.Extensions :: Jakar.Extensions
// 03/10/2025  15:03


namespace Jakar.Extensions;


public delegate void Handler<in TValue>( TValue? value );



public delegate Task HandlerAsync<in TValue>( TValue? value, CancellationToken token = default );



public static class Properties
{
    public static readonly None EmptyCommand = new();


    public static ViewModelProperty<TCommand, bool> Bool<TCommand>( bool value )
        where TCommand : class, ICommand => new(ValueSorter<bool>.Default, value);
    public static EmailProperty<TCommand> Email<TCommand>( MaskedTextProvider? mask, string value = EMPTY )
        where TCommand : class, ICommand => new(StringComparer.Ordinal, value, mask);
    public static StringProperty<TCommand> String<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(StringComparer.Ordinal, value);
    public static PhoneNumberProperty<TCommand> PhoneNumber<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(StringComparer.Ordinal, value);
    public static PasswordProperty<TCommand> Password<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(StringComparer.Ordinal, value);


    public static StringCollectionProperty<TCommand> Strings<TCommand>( string value = EMPTY )
        where TCommand : class, ICommand => new(StringComparer.Ordinal, value);
    public static ViewModelCollectionProperty<TCommand, TValue> Collection<TCommand, TValue>( TValue value )
        where TValue : IEquatable<TValue>
        where TCommand : class, ICommand => new(EqualityComparer<TValue?>.Default, EmptyCommand, value);
    public static ViewModelCollectionProperty<TCommand, TValue> Collection<TCommand, TValue>( Handler<TValue> onSelected, TValue value )
        where TValue : IEquatable<TValue>
        where TCommand : class, ICommand => new(EqualityComparer<TValue?>.Default, onSelected, value);
    public static ViewModelCollectionProperty<TCommand, TValue> Collection<TCommand, TValue>( HandlerAsync<TValue> onSelected, TValue value )
        where TValue : IEquatable<TValue>
        where TCommand : class, ICommand => new(EqualityComparer<TValue?>.Default, onSelected, value);


    public static TValue NoCoerceValue<TValue>( TValue                  value ) => value;
    public static bool   HasValue( [NotNullWhen(        true )] string? value ) => !string.IsNullOrWhiteSpace( value );
    public static bool   HasValue<TValue>( [NotNullWhen(true )] TValue? value ) => value is not null;
    public static bool   CheckIsEmail( [NotNullWhen(    true )] string? value ) => HasValue( value ) && value.IsEmailAddress();
}



public class ViewModelProperty<TCommand>() : ObservableClass()
    where TCommand : class, ICommand
{
    private bool      __isEnabled = true;
    private bool      __isVisible = true;
    private object?   __commandParameter;
    private string?   __description;
    private string?   __hint;
    private string?   __placeholder;
    private string?   __title;
    private TCommand? __command;


    public TCommand? Command          { get => __command;          set => SetProperty( ref __command,          value ); }
    public object?   CommandParameter { get => __commandParameter; set => SetProperty( ref __commandParameter, value ); }
    public string?   Description      { get => __description;      set => SetProperty( ref __description,      value ); }
    public string?   Hint             { get => __hint;             set => SetProperty( ref __hint,             value ); }
    public bool      IsEnabled        { get => __isEnabled;        set => SetProperty( ref __isEnabled,        value ); }
    public bool      IsVisible        { get => __isVisible;        set => SetProperty( ref __isVisible,        value ); }
    public string?   Placeholder      { get => __placeholder;      set => SetProperty( ref __placeholder,      value ); }
    public string?   Title            { get => __title;            set => SetProperty( ref __title,            value ); }
}



public class ViewModelProperty<TCommand, TValue> : ViewModelProperty<TCommand>, IValidator
    where TValue : IEquatable<TValue>
    where TCommand : class, ICommand
{
    private readonly IEqualityComparer<TValue?> __equalityComparer;
    private          TValue?                    __value;


    public static Func<Handler<TValue>, TCommand?>      ActionToCommand { get; set; } = static x => null;
    public static Func<HandlerAsync<TValue>, TCommand?> FuncToCommand   { get; set; } = static x => null;
    public        bool                                  IsValid         => HasValue( __value );
    public        TValue?                               Value           { get => __value; set => SetValue( value ); }


    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, TValue value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }
    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, OneOf<Handler<TValue>, HandlerAsync<TValue>, None> onSelected, TValue value ) : base()
    {
        __equalityComparer = equalityComparer;
        __value            = value;
        Command           = onSelected.Match( ActionToCommand, FuncToCommand, none => null );
    }
    public static implicit operator TValue?( ViewModelProperty<TCommand, TValue> property ) => property.Value;


    protected virtual bool    HasValue( [NotNullWhen( true )]                  TValue? value ) => value is not null;
    protected virtual TValue? CoerceValue( [NotNullIfNotNull( nameof(value) )] TValue? value ) => value;
    protected virtual bool? SetValue( TValue? value, [CallerMemberName] string propertyName = EMPTY )
    {
        try
        {
            value = CoerceValue( value );

            return SetProperty( ref __value, value, __equalityComparer, propertyName )
                       ? IsValid
                       : null;
        }
        finally { OnPropertyChanged( nameof(IsValid) ); }
    }
}
