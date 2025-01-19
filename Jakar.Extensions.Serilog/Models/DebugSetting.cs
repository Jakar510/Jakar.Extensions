// Jakar.Extensions :: Jakar.Extensions
// 01/02/2025  12:01

namespace Jakar.Extensions.Serilog;


public class DebugSetting( bool value, Func<bool, string> hinter, Action<bool>? action = null ) : ObservableClass
{
    private readonly Action<bool>?      _action      = action;
    private readonly Func<bool, string> _hinter      = hinter;
    private          bool               _value       = value;
    private          string             _description = string.Empty;
    private          string             _title       = string.Empty;
    private          bool               _isTitleBold;


    public string Description { get => _description; set => SetProperty( ref _description, value ); }
    public string Hint        => _hinter( Value );
    public bool   IsTitleBold { get => _isTitleBold; set => SetProperty( ref _isTitleBold, value ); }
    public string Title       { get => _title;       set => SetProperty( ref _title,       value ); }
    public bool Value
    {
        get => _value;
        set
        {
            if ( SetProperty( ref _value, value ) is false ) { return; }

            OnPropertyChanged( nameof(Hint) );
            _action?.Invoke( value );
        }
    }
}
