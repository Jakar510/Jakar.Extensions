// Jakar.Extensions :: Jakar.SettingsView.Maui
// 08/13/2024  09:08

namespace Jakar.SettingsView.Maui.Cells;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class SettingsView : ContentView, IDisposable
{
    public static readonly BindableProperty SectionsProperty = BindableProperty.Create( nameof(Sections), typeof(ObservableCollection<SvSection>), typeof(SettingsView), defaultValueCreator: static bindable => new ObservableCollection<CellBase>() );


    public ObservableCollection<SvSection> Sections { get => (ObservableCollection<SvSection>)GetValue( SectionsProperty ); set => SetValue( SectionsProperty, value ); }


    public virtual void Dispose()
    {
        foreach ( SvSection section in Sections ) { section.Dispose(); }

        GC.SuppressFinalize( this );
    }
}
