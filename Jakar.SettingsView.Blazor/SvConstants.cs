// Jakar.Extensions :: Jakar.SettingsView.Blazor
// 08/05/2024  22:08

namespace Jakar.SettingsView.Blazor;


public static class SvConstants
{
    public const string ASPECT_FIT                       = "aspect-fit";
    public const string CLASS                            = "class";
    public const string IMAGE                            = "img";
    public const string ON_CLICK                         = "onclick";
    public const string SPAN                             = "span";
    public const string SV_ACTIVE                        = "sv-active";
    public const string SV_BUTTON                        = "sv-button";
    public const string SV_LABEL                         = "sv-label";
    public const string SV_CELL                          = "sv-cell";
    public const string SV_CELL_BODY                     = "sv-cell-body";
    public const string SV_CELL_BODY_DESCRIPTION         = "sv-body-description";
    public const string SV_CELL_BODY_HINT                = "sv-body-hint";
    public const string SV_CELL_BODY_TITLE               = "sv-body-title";
    public const string SV_CELL_BODY_VALUE               = "sv-body-value";
    public const string SV_CELL_BODY_VALUE_ENTRY         = "sv-body-value-entry";
    public const string SV_DISABLED                      = "sv-disabled";
    public const string SV_ROOT                          = "sv-root";
    public const string SV_ROOT_LIST                          = "sv-root-list";
    public const string SV_SECTION                       = "sv-section";
    public const string SV_SECTION_BODY                  = "sv-section-body";
    public const string SV_SECTION_FOOTER                = "sv-section-footer";
    public const string SV_SECTION_HEADER                = "sv-section-header";
    public const string SV_SECTION_HEADER_ICON           = "sv-section-header-icon";
    public const string SV_SECTION_HEADER_ICON_COLLAPSED = "sv-section-header-icon-collapsed";
    public const string SV_SECTION_HEADER_ICON_EXPANDED  = "sv-section-header-icon-expanded";


    public static async ValueTask Execute<T>( this EventCallback<T> callback, T? args = default )
    {
        if ( args is null ) { return; }

        if ( callback.HasDelegate ) { await callback.InvokeAsync( args ); }
    }
    public static string ToggleClass( this string classValue, bool isToggle )
    {
        if ( isToggle is false ) { return classValue; }

        const string ACTIVE = $" {SV_ACTIVE}";

        return classValue.Contains( SV_ACTIVE, StringComparison.Ordinal ) is false
                   ? classValue + ACTIVE
                   : classValue.Replace( ACTIVE, string.Empty, StringComparison.Ordinal );
    }
}
