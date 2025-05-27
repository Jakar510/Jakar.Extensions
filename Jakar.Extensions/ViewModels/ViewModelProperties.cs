// Jakar.Extensions :: Jakar.Extensions
// 03/10/2025  15:03


namespace Jakar.Extensions;


public static class Properties
{
    public static ViewModelProperty<bool>    Bool( bool      value )        => new(ValueEqualizer<bool>.Default, value);
    public static ViewModelProperty<string?> Email( string?  value = null ) => new(StringComparer.Ordinal, value) { CheckValue = CheckIsEmail };
    public static ViewModelProperty<string?> String( string? value = null ) => new(StringComparer.Ordinal, value) { CheckValue = HasValue };
    public static ViewModelProperty<string?> PhoneNumber( string? value = null, string mask = @"(XXX) XXX-XXXX" )
    {
        MaskedTextProvider phoneMask = new(mask)
                                       {
                                           IsPassword      = false,
                                           IncludeLiterals = false,
                                           ResetOnSpace    = false,
                                           SkipLiterals    = false
                                       };

        ViewModelProperty<string?> property = new(StringComparer.Ordinal, value)
                                              {
                                                  CheckValue  = HasValue,
                                                  CoerceValue = CoercePhoneNumber
                                              };

        return property;

        string CoercePhoneNumber( string? x )
        {
            if ( string.IsNullOrWhiteSpace( x ) ) { return string.Empty; }

            phoneMask.Set( x );
            x = phoneMask.ToString();
            return x;
        }
    }


    public static CollectionViewModelProperty<string> Strings( string value ) => new(StringComparer.Ordinal, value);
    public static CollectionViewModelProperty<TValue> Collection<TValue>()
        where TValue : IEquatable<TValue> => new(EqualityComparer<TValue?>.Default, new None());
    public static CollectionViewModelProperty<TValue> Collection<TValue>( Handler<TValue> onSelected )
        where TValue : IEquatable<TValue> => new(EqualityComparer<TValue?>.Default, onSelected);
    public static CollectionViewModelProperty<TValue> Collection<TValue>( HandlerAsync<TValue> onSelected )
        where TValue : IEquatable<TValue> => new(EqualityComparer<TValue?>.Default, onSelected);


    public static TValue NoCoerceValue<TValue>( TValue                   value ) => value;
    public static bool   HasValue( [NotNullWhen(         true )] string? value ) => string.IsNullOrWhiteSpace( value ) is false;
    public static bool   HasValue<TValue>( [NotNullWhen( true )] TValue? value ) => value is not null;
    public static bool   CheckIsEmail( [NotNullWhen(     true )] string? value ) => HasValue( value ) && value.IsEmailAddress();



    public delegate TValue? Coercer<TValue>( TValue? value );



    public delegate bool Checker<in TValue>( TValue? value );



    public delegate void Handler<in TValue>( TValue? value );



    public delegate Task HandlerAsync<in TValue>( TValue? value, CancellationToken token = default );
}



public class ViewModelProperty() : ObservableClass()
{
    private bool      _isEnabled = true;
    private bool      _isVisible = true;
    private ICommand? _command;
    private object?   _commandParameter;
    private string?   _description;
    private string?   _hint;
    private string?   _placeholder;
    private string?   _title;


    public ICommand? Command          { get => _command;          set => SetProperty( ref _command,          value ); }
    public object?   CommandParameter { get => _commandParameter; set => SetProperty( ref _commandParameter, value ); }
    public string?   Description      { get => _description;      set => SetProperty( ref _description,      value ); }
    public string?   Hint             { get => _hint;             set => SetProperty( ref _hint,             value ); }
    public bool      IsEnabled        { get => _isEnabled;        set => SetProperty( ref _isEnabled,        value ); }
    public bool      IsVisible        { get => _isVisible;        set => SetProperty( ref _isVisible,        value ); }
    public string?   Placeholder      { get => _placeholder;      set => SetProperty( ref _placeholder,      value ); }
    public string?   Title            { get => _title;            set => SetProperty( ref _title,            value ); }
}



public class ViewModelProperty<TValue> : ViewModelProperty, IValidator
{
    private readonly IEqualityComparer<TValue?> _equalityComparer;
    protected        Properties.Checker<TValue> _checkValue  = Properties.HasValue;
    protected        Properties.Coercer<TValue> _coerceValue = Properties.NoCoerceValue;
    private          TValue?                    _value;


    public static Func<Properties.Handler<TValue>, ICommand?>      ActionToCommand { get;                 set; } = static x => null;
    public static Func<Properties.HandlerAsync<TValue>, ICommand?> FuncToCommand   { get;                 set; } = static x => null;
    public        Properties.Checker<TValue>                       CheckValue      { get => _checkValue;  set => _checkValue = value; }
    public        Properties.Coercer<TValue>                       CoerceValue     { get => _coerceValue; set => _coerceValue = value; }
    public        bool                                             IsValid         => _checkValue( _value );
    public        TValue?                                          Value           { get => _value; set => SetValue( ref _value, value ); }


    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, TValue? value = default ) : this( equalityComparer, new None(), value ) { }
    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, OneOf<Properties.Handler<TValue>, Properties.HandlerAsync<TValue>, None> onSelected, TValue? value = default ) : base()
    {
        _equalityComparer = equalityComparer;
        _value            = value;
        Command           = onSelected.Match( ActionToCommand, FuncToCommand, none => null );
    }

    public static implicit operator TValue?( ViewModelProperty<TValue> property ) => property.Value;


    protected virtual bool? SetValue( ref TValue? field, TValue? value, [CallerMemberName] string propertyName = EMPTY )
    {
        value = _coerceValue( value );

        return SetProperty( ref field, value, _equalityComparer, propertyName )
                   ? _checkValue( value )
                   : null;
    }
}



public class CollectionViewModelProperty<TValue>( IEqualityComparer<TValue?> equalityComparer, OneOf<Properties.Handler<TValue>, Properties.HandlerAsync<TValue>, None> onSelected, TValue? value = default ) : ViewModelProperty<TValue>( equalityComparer, onSelected, value )
    where TValue : IEquatable<TValue>
{
    public ObservableCollection<TValue> Values { get; } = new(DEFAULT_CAPACITY);
    public CollectionViewModelProperty( IEqualityComparer<TValue?> equalityComparer, TValue? value = default ) : this( equalityComparer, new None(), value ) { }
    public CollectionViewModelProperty<TValue> With( params ReadOnlySpan<TValue> values )
    {
        Values.Clear();
        Values.Add( values );
        return this;
    }
}
