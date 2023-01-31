// unset

#nullable enable
using Plugin.Media.Abstractions;



namespace Jakar.Extensions.Xamarin.Forms;


public static class FileExtensions
{
    public static FileInfo ToFileInfo( this MediaFile file ) => new(file.Path ?? throw new ArgumentNullException( nameof(file) ));


    public static async ValueTask<IEnumerable<LocalFile>?> PickMultiple( PickOptions? options = null )
    {
        IEnumerable<FileResult>? items = await FilePicker.PickMultipleAsync( options );
        return items?.Select( item => new LocalFile( item.FullPath ) );
    }
    public static async ValueTask<LocalFile?> Pick( PickOptions? options = null )
    {
        FileResult? result = await FilePicker.PickAsync( options );
        return new LocalFile( result.FullPath );
    }
}
