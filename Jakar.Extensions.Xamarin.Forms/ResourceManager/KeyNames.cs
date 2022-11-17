﻿#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


[SuppressMessage( "ReSharper", "MemberHidesStaticFromOuterClass" )]
public class KeyNames
{
    public enum Button
    {
        BorderColor,
        TextColor,
        BackgroundColor,
        FontSize,
        FontAttributes,
        FontFamily,
    }



    public enum Entry
    {
        AccentColor,
        TextColor,
        PlaceholderColor,
        BackgroundColor,
        FontSize,
        FontAttributes,
        FontFamily,
    }



    public enum FontSize
    {
        PageTitle,
        Header,
        Title,
        Description,
        Hint,
        Value,
        Misc,
    }



    public enum Footer
    {
        TextColor,
        BackgroundColor,
        FontSize,
        FontAttributes,
        FontFamily,
    }



    public enum Header
    {
        TextColor,
        BackgroundColor,
        FontSize,
        FontAttributes,
        FontFamily,
    }



    public enum Label
    {
        TextColor,
        BackgroundColor,
        FontSize,
        FontAttributes,
        FontFamily,
    }



    public enum ListView
    {
        BackgroundColor,
        SeparatorColor, // i.e. ListView
    }



    public enum ShellColor
    {
        Foreground,
        Background,
        NavigationBar,
        Disabled,
        Unselected,
        FlyOutBorder,
    }



    public enum ThemedColor
    {
        Text,
        Background,
        PageBackground,
        Accent, // i.e. CheckBox
        Valid,
        Invalid,
        InFocus,
    }
}
