// Jakar.Extensions :: Jakar.Extensions
// 03/10/2025  15:03

namespace Jakar.Extensions;


public static class Properties
{
    public static ViewModelProperty<bool>    Bool( bool      value )        => new(ValueEqualizer<bool>.Default, value);
    public static ViewModelProperty<string?> Email( string?  value = null ) => new(StringComparer.Ordinal, value) { CheckValue = CheckIsEmail };
    public static ViewModelProperty<string?> String( string? value = null ) => new(StringComparer.Ordinal, value) { CheckValue = CheckIsValid };
    public static ViewModelProperty<string?> PhoneNumber( string? value = null )
    {
        MaskedTextProvider phoneMask = new(@"(XXX) XXX-XXXX")
                                       {
                                           IsPassword      = false,
                                           IncludeLiterals = false,
                                           ResetOnSpace    = false,
                                           SkipLiterals    = false
                                       };

        ViewModelProperty<string?> property = new(StringComparer.Ordinal, value)
                                              {
                                                  CheckValue  = CheckIsValid,
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
    public static CollectionViewModelProperty<TValue> Collection<TValue>( Action<TValue?> onSelected )
        where TValue : IEquatable<TValue> => new(EqualityComparer<TValue?>.Default, onSelected);
    public static CollectionViewModelProperty<TValue> Collection<TValue>( Func<TValue?, CancellationToken, Task> onSelected )
        where TValue : IEquatable<TValue> => new(EqualityComparer<TValue?>.Default, onSelected);


    public static TValue NoCoerceValue<TValue>( TValue               value ) => value;
    public static bool   HasValue( string?                           value ) => value?.Length is > 0;
    public static bool   HasValue<TValue>( TValue?                   value ) => value is not null;
    public static bool   CheckIsValid( [NotNullWhen( true )] string? value ) => string.IsNullOrWhiteSpace( value ) is false;
    public static bool   CheckIsEmail( [NotNullWhen( true )] string? value ) => CheckIsValid( value ) && value.IsEmailAddress();
}



public class ViewModelProperty() : ObservableClass
{
    private bool      _isEnabled = true;
    private bool      _isVisible = true;
    private string?   _description;
    private string?   _hint;
    private string?   _placeholder;
    private string?   _title;
    private ICommand? _command;
    private object?   _commandParameter;


    public string?   Description      { get => _description;      set => SetProperty( ref _description,      value ); }
    public string?   Hint             { get => _hint;             set => SetProperty( ref _hint,             value ); }
    public bool      IsEnabled        { get => _isEnabled;        set => SetProperty( ref _isEnabled,        value ); }
    public bool      IsVisible        { get => _isVisible;        set => SetProperty( ref _isVisible,        value ); }
    public string?   Placeholder      { get => _placeholder;      set => SetProperty( ref _placeholder,      value ); }
    public string?   Title            { get => _title;            set => SetProperty( ref _title,            value ); }
    public ICommand? Command          { get => _command;          set => SetProperty( ref _command,          value ); }
    public object?   CommandParameter { get => _commandParameter; set => SetProperty( ref _commandParameter, value ); }
}



public class ViewModelProperty<TValue> : ViewModelProperty, IValidator
{
    private readonly IEqualityComparer<TValue?> _equalityComparer;
    private          TValue?                    _value;
    protected        Func<TValue?, TValue?>     _coerceValue = Properties.NoCoerceValue;
    protected        Func<TValue?, bool>        _checkValue  = Properties.HasValue;


    public static Func<Action<TValue?>, ICommand?>                        ActionToCommand { get;                 set; } = static x => null;
    public static Func<Func<TValue?, CancellationToken, Task>, ICommand?> FuncToCommand   { get;                 set; } = static x => null;
    public        Func<TValue?, bool>                                     CheckValue      { get => _checkValue;  set => _checkValue = value; }
    public        Func<TValue?, TValue?>                                  CoerceValue     { get => _coerceValue; set => _coerceValue = value; }
    public        TValue?                                                 Value           { get => _value;       set => SetValue( ref _value, value ); }
    public        bool                                                    IsValid         => CheckValue( _value );


    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, TValue? value = default ) : this( equalityComparer, new None(), value ) { }
    public ViewModelProperty( IEqualityComparer<TValue?> equalityComparer, OneOf<Action<TValue?>, Func<TValue?, CancellationToken, Task>, None> onSelected, TValue? value = default ) : base()
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



public class CollectionViewModelProperty<TValue>( IEqualityComparer<TValue?> equalityComparer, OneOf<Action<TValue?>, Func<TValue?, CancellationToken, Task>, None> onSelected, TValue? value = default ) : ViewModelProperty<TValue>( equalityComparer, onSelected, value )
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
