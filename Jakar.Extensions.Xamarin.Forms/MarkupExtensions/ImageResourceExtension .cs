#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.MarkupExtensions;


[ContentProperty(nameof(Source))]
public abstract class ImageResourceExtension : IMarkupExtension
{
    public string? Source { get; set; }

    protected abstract string GetPath( string fileName ); // Path.to.file.in.assembly

    public object? ProvideValue( IServiceProvider serviceProvider )
    {
        if ( Source is null ) { return null; }

        // Do your translation lookup here, using whatever method you require
        ImageSource imageSource = ImageSource.FromResource(GetPath(Source), typeof(ImageResourceExtension).GetTypeInfo().Assembly);

        return imageSource;
    }
}
