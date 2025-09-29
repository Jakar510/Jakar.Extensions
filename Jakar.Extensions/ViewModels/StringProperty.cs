// Jakar.Extensions :: Jakar.Extensions
// 06/19/2025  10:54

namespace Jakar.Extensions;


public class StringProperty<TCommand>( IEqualityComparer<string?> equalityComparer, OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : ViewModelProperty<TCommand, string>( equalityComparer, onSelected, value )
    where TCommand : class, ICommand
{
    public StringProperty( IEqualityComparer<string?> equalityComparer, string value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }
}



public class EmailProperty<TCommand>( IEqualityComparer<string?> equalityComparer, OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : StringProperty<TCommand>( equalityComparer, onSelected, value )
    where TCommand : class, ICommand
{
    public EmailProperty( IEqualityComparer<string?> equalityComparer, string value, MaskedTextProvider? mask ) : this( equalityComparer, Properties.EmptyCommand, value ) { }


    protected override bool HasValue( [NotNullWhen( true )] string? value ) => base.HasValue( value ) && value.IsEmailAddress();
    protected override string CoerceValue( string? value )
    {
        return string.IsNullOrWhiteSpace( value )
                   ? string.Empty
                   : value;
    }
}



public class PhoneNumberProperty<TCommand>( IEqualityComparer<string?> equalityComparer, OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : StringProperty<TCommand>( equalityComparer, onSelected, value )
    where TCommand : class, ICommand
{
    public MaskedTextProvider Mask { get; init; } = new(@"(XXX) XXX-XXXX")
                                                    {
                                                        IsPassword      = false,
                                                        IncludeLiterals = false,
                                                        ResetOnSpace    = false,
                                                        SkipLiterals    = false
                                                    };

    public PhoneNumberProperty( IEqualityComparer<string?> equalityComparer, string value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }


    protected override string CoerceValue( string? value )
    {
        if ( string.IsNullOrWhiteSpace( value ) ) { return string.Empty; }

        string result = Mask.Set( value )
                            ? Mask.ToString()
                            : value;

        return result;
    }
}



public class PasswordProperty<TCommand>( IEqualityComparer<string?> equalityComparer, OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : StringProperty<TCommand>( equalityComparer, onSelected, value )
    where TCommand : class, ICommand
{
    public MaskedTextProvider Mask { get; init; } = new("*", CultureInfo.CurrentCulture, false, '_', '*', false)
                                                    {
                                                        IsPassword      = true,
                                                        IncludeLiterals = false,
                                                        ResetOnSpace    = false,
                                                        SkipLiterals    = false
                                                    };

    public PasswordProperty( IEqualityComparer<string?> equalityComparer, string value ) : this( equalityComparer, Properties.EmptyCommand, value ) { }


    protected override string CoerceValue( string? value )
    {
        if ( string.IsNullOrWhiteSpace( value ) ) { return string.Empty; }

        string result = Mask.Set( value )
                            ? Mask.ToString()
                            : value;

        return result;
    }
}
