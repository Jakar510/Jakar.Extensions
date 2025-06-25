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


    public static TValue NoCoerceValue<TValue>( TValue                   value ) => value;
    public static bool   HasValue( [NotNullWhen(         true )] string? value ) => string.IsNullOrWhiteSpace( value ) is false;
    public static bool   HasValue<TValue>( [NotNullWhen( true )] TValue? value ) => value is not null;
    public static bool   CheckIsEmail( [NotNullWhen(     true )] string? value ) => HasValue( value ) && value.IsEmailAddress();
}



public class ViewModelProperty<TCommand>() : ObservableClass()
    where TCommand : class, ICommand
{
    private bool      _isEnabled = true;
    private bool      _isVisible = true;
    private object?   _commandParameter;
    private string?   _description;
    private string?   _hint;
    private string?   _placeholder;
    private string?   _title;
    private TCommand? _command;


    public TCommand? Command          { get => _command;          set => SetProperty( ref _command,          value ); }
    public object?   CommandParameter { get => _commandParameter; set => SetProperty( ref _commandParameter, value ); }
    public string?   Description      { get => _description;      set => SetProperty( ref _description,      value ); }
    public string?   Hint             { get => _hint;             set => SetProperty( ref _hint,             value ); }
    public bool      IsEnabled        { get => _isEnabled;        set => SetProperty( ref _isEnabled,        value ); }
    public bool      IsVisible        { get => _isVisible;        set => SetProperty( ref _isVisible,        value ); }
    public string?   Placeholder      { get => _placeholder;      set => SetProperty( ref _placeholder,      value ); }
    public string?   Title            { get => _title;            set => SetProperty( ref _title,            value ); }
}



public class ViewModelProperty<TCommand, TValue> : ViewModelProperty<TCommand>, IValidator
    where TValue : IEquatable<TValue>
    where TCommand : class, ICommand
{
    private readonly IEqualityComparer<TValue?> _equalityComparer;
    private          TValue?                    _value;


    public static Func<Handler<TValue>, TCommand?>      ActionToCommand { get; set; } = static x => null;
    public static Func<HandlerAsync<TValue>, TCommand?> FuncToCommand   { get; set; } = static x => null;
    public        bool                                  IsValid         => HasValue( _value );
    public        TValue?                               Value           { get => _value; set => SetValue( value ); }


    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, TValue value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }
    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, OneOf<Handler<TValue>, HandlerAsync<TValue>, None> onSelected, TValue value ) : base()
    {
        _equalityComparer = equalityComparer;
        _value            = value;
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

            return SetProperty( ref _value, value, _equalityComparer, propertyName )
                       ? IsValid
                       : null;
        }
        finally { OnPropertyChanged( nameof(IsValid) ); }
    }
}
