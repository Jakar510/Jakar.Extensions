// Jakar.Extensions :: Jakar.Extensions
// 03/10/2025  15:03

namespace Jakar.Extensions;


public static class ViewModelProperties
{
    public static ViewModelProperty<TColor, bool>    Bool<TColor>( IAppColors<TColor>   app, bool    value )        => new(app, ValueEqualizer<bool>.Default, value);
    public static ViewModelProperty<TColor, string?> Email<TColor>( IAppColors<TColor>  app, string? value = null ) => new(app, StringComparer.Ordinal, value) { CheckValue = CheckIsEmail };
    public static ViewModelProperty<TColor, string?> String<TColor>( IAppColors<TColor> app, string? value = null ) => new(app, StringComparer.Ordinal, value) { CheckValue = CheckIsValid };
    public static ViewModelProperty<TColor, string?> PhoneNumber<TColor>( IAppColors<TColor> app, string? value = null )
    {
        MaskedTextProvider phoneMask = new(@"(XXX) XXX-XXXX")
                                       {
                                           IsPassword      = false,
                                           IncludeLiterals = false,
                                           ResetOnSpace    = false,
                                           SkipLiterals    = false
                                       };

        ViewModelProperty<TColor, string?> property = new(app, StringComparer.Ordinal, value)
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


    public static CollectionViewModelProperty<TColor, string> Strings<TColor>( IAppColors<TColor> app, string value ) => new(app, StringComparer.Ordinal, value);
    public static CollectionViewModelProperty<TColor, TValue> Collection<TColor, TValue>( IAppColors<TColor> app )
        where TValue : IEquatable<TValue> => new(app, EqualityComparer<TValue?>.Default, new None());
    public static CollectionViewModelProperty<TColor, TValue> Collection<TColor, TValue>( IAppColors<TColor> app, Action<TValue?> onSelected )
        where TValue : IEquatable<TValue> => new(app, EqualityComparer<TValue?>.Default, onSelected);
    public static CollectionViewModelProperty<TColor, TValue> Collection<TColor, TValue>( IAppColors<TColor> app, Func<TValue?, CancellationToken, Task> onSelected )
        where TValue : IEquatable<TValue> => new(app, EqualityComparer<TValue?>.Default, onSelected);


    public static TValue NoCoerceValue<TValue>( TValue               value ) => value;
    public static bool   HasValue( string?                           value ) => value?.Length is > 0;
    public static bool   HasValue<TValue>( TValue?                   value ) => value is not null;
    public static bool   CheckIsValid( [NotNullWhen( true )] string? value ) => string.IsNullOrWhiteSpace( value ) is false;
    public static bool   CheckIsEmail( [NotNullWhen( true )] string? value ) => CheckIsValid( value ) && value.IsEmailAddress();
}



public class ViewModelProperty<TColor>( IAppColors<TColor> app ) : ObservableClass
{
    protected readonly IAppColors<TColor> _app             = app;
    private            bool               _isEnabled       = true;
    private            bool               _isVisible       = true;
    private            TColor             _backgroundColor = app.Transparent;
    private            TColor             _textColor       = app.TextColor;
    private            string?            _description;
    private            string?            _hint;
    private            string?            _placeholder;
    private            string?            _title;
    private            ICommand?          _command;
    private            object?            _commandParameter;


    public TColor    BackgroundColor  { get => _backgroundColor;  set => SetProperty( ref _backgroundColor,  value ); }
    public string?   Description      { get => _description;      set => SetProperty( ref _description,      value ); }
    public string?   Hint             { get => _hint;             set => SetProperty( ref _hint,             value ); }
    public bool      IsEnabled        { get => _isEnabled;        set => SetProperty( ref _isEnabled,        value ); }
    public bool      IsVisible        { get => _isVisible;        set => SetProperty( ref _isVisible,        value ); }
    public string?   Placeholder      { get => _placeholder;      set => SetProperty( ref _placeholder,      value ); }
    public TColor    TextColor        { get => _textColor;        set => SetProperty( ref _textColor,        value ); }
    public string?   Title            { get => _title;            set => SetProperty( ref _title,            value ); }
    public ICommand? Command          { get => _command;          set => SetProperty( ref _command,          value ); }
    public object?   CommandParameter { get => _commandParameter; set => SetProperty( ref _commandParameter, value ); }
}



public interface IAppColors<out TColor>
{
    TColor Transparent  { get; }
    TColor TextColor    { get; }
    TColor InvalidColor { get; }
}



public class ViewModelProperty<TColor, TValue> : ViewModelProperty<TColor>, IValidator
{
    private readonly IEqualityComparer<TValue?> _equalityComparer;
    private          TValue?                    _value;
    protected        Func<TValue?, TValue?>     _coerceValue = ViewModelProperties.NoCoerceValue;
    protected        Func<TValue?, bool>        _checkValue  = ViewModelProperties.HasValue;


    public static Func<Action<TValue?>, ICommand?>                        ActionToCommand { get;                 set; } = static x => null;
    public static Func<Func<TValue?, CancellationToken, Task>, ICommand?> FuncToCommand   { get;                 set; } = static x => null;
    public        Func<TValue?, bool>                                     CheckValue      { get => _checkValue;  set => _checkValue = value; }
    public        Func<TValue?, TValue?>                                  CoerceValue     { get => _coerceValue; set => _coerceValue = value; }
    public        TValue?                                                 Value           { get => _value;       set => SetValue( ref _value, value ); }
    public        bool                                                    IsValid         => CheckValue( _value );


    public ViewModelProperty( IAppColors<TColor> app, IEqualityComparer<TValue?> equalityComparer, TValue? value = default ) : this( app, equalityComparer, new None(), value ) { }
    public ViewModelProperty( IAppColors<TColor> app, IEqualityComparer<TValue?> equalityComparer, OneOf<Action<TValue?>, Func<TValue?, CancellationToken, Task>, None> onSelected, TValue? value = default ) : base( app )
    {
        _equalityComparer = equalityComparer;
        _value            = value;
        Command           = onSelected.Match( ActionToCommand, FuncToCommand, none => null );
    }

    public static implicit operator TValue?( ViewModelProperty<TColor, TValue> property ) => property.Value;


    private void SetValue( ref TValue? field, TValue? value, [CallerMemberName] string propertyName = EMPTY )
    {
        value = _coerceValue( value );

        if ( SetProperty( ref field, value, _equalityComparer, propertyName ) )
        {
            TextColor = _checkValue( value )
                            ? _app.TextColor
                            : _app.InvalidColor;
        }
    }
}



public class CollectionViewModelProperty<TColor, TValue>( IAppColors<TColor> app, IEqualityComparer<TValue?> equalityComparer, OneOf<Action<TValue?>, Func<TValue?, CancellationToken, Task>, None> onSelected, TValue? value = default ) : ViewModelProperty<TColor, TValue>( app, equalityComparer, onSelected, value )
    where TValue : IEquatable<TValue>
{
    public ObservableCollection<TValue> Values { get; } = new(DEFAULT_CAPACITY);


    public CollectionViewModelProperty( IAppColors<TColor> app, IEqualityComparer<TValue?> equalityComparer, TValue? value = default ) : this( app, equalityComparer, new None(), value ) { }

    public CollectionViewModelProperty<TColor, TValue> With( params ReadOnlySpan<TValue> values )
    {
        Values.Clear();
        Values.Add( values );
        return this;
    }
}
