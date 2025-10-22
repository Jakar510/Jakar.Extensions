// Jakar.Extensions :: Jakar.Extensions
// 06/19/2025  10:54

namespace Jakar.Extensions;


public class StringProperty<TCommand>( OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : ViewModelProperty<TCommand, string>(onSelected, value)
    where TCommand : class, ICommand
{
    public StringProperty( string value ) : this(Properties.EmptyCommand, value) { }
}



public class EmailProperty<TCommand>( OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : StringProperty<TCommand>(onSelected, value)
    where TCommand : class, ICommand
{
    public EmailProperty( string value, MaskedTextProvider? mask ) : this(Properties.EmptyCommand, value) { }


    protected override bool HasValue( [NotNullWhen(true)] string? value ) => base.HasValue(value) && value.IsEmailAddress();
    protected override string CoerceValue( string? value )
    {
        return string.IsNullOrWhiteSpace(value)
                   ? EMPTY
                   : value;
    }
}



public class PhoneNumberProperty<TCommand>( OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : StringProperty<TCommand>(onSelected, value)
    where TCommand : class, ICommand
{
    public MaskedTextProvider Mask { get; init; } = new(@"(XXX) XXX-XXXX")
                                                    {
                                                        IsPassword      = false,
                                                        IncludeLiterals = false,
                                                        ResetOnSpace    = false,
                                                        SkipLiterals    = false
                                                    };

    public PhoneNumberProperty( string value ) : this(Properties.EmptyCommand, value) { }


    protected override string CoerceValue( string? value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { return EMPTY; }

        string result = Mask.Set(value)
                            ? Mask.ToString()
                            : value;

        return result;
    }
}



public class PasswordProperty<TCommand>( OneOf<Handler<string>, HandlerAsync<string>, None> onSelected, string value ) : StringProperty<TCommand>(onSelected, value)
    where TCommand : class, ICommand
{
    public MaskedTextProvider Mask { get; init; } = new("*", CultureInfo.CurrentCulture, false, '_', '*', false)
                                                    {
                                                        IsPassword      = true,
                                                        IncludeLiterals = false,
                                                        ResetOnSpace    = false,
                                                        SkipLiterals    = false
                                                    };

    public PasswordProperty( string value ) : this(Properties.EmptyCommand, value) { }


    protected override string CoerceValue( string? value )
    {
        if ( string.IsNullOrWhiteSpace(value) ) { return EMPTY; }

        string result = Mask.Set(value)
                            ? Mask.ToString()
                            : value;

        return result;
    }
}
