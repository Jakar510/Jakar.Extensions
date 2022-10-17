// TrueLogic :: TrueKeep.Client
// 01/25/2022  12:40 PM


#nullable enable
namespace Jakar.Extensions.Wpf;


public static partial class WpfApi
{
    public static DispatcherOperation CallAsync( this DispatcherObject view, Action func, CancellationToken token = default ) => view.Dispatcher.CallAsync( func, token );
    public static DispatcherOperation CallAsync<T1>( this DispatcherObject view, Action<T1> func, T1 arg1, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, token );
    public static DispatcherOperation CallAsync<T1, T2>( this DispatcherObject view, Action<T1, T2> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, token );
    public static DispatcherOperation CallAsync<T1, T2, T3>( this DispatcherObject view, Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, token );
    public static DispatcherOperation CallAsync<T1, T2, T3, T4>( this DispatcherObject view, Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, arg4, token );
    public static DispatcherOperation CallAsync<T1, T2, T3, T4, T5>( this DispatcherObject view, Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, arg4, arg5, token );


    public static DispatcherOperation CallAsync( this Dispatcher dispatcher, Action func, CancellationToken token = default ) => dispatcher.BeginInvoke( func, DispatcherPriority.Normal, token );
    public static DispatcherOperation CallAsync<T1>( this Dispatcher dispatcher, Action<T1> func, T1 arg1, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( () => func( arg1 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation CallAsync<T1, T2>( this Dispatcher dispatcher, Action<T1, T2> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( () => func( arg1, arg2 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation CallAsync<T1, T2, T3>( this Dispatcher dispatcher, Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( () => func( arg1, arg2, arg3 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation CallAsync<T1, T2, T3, T4>( this Dispatcher dispatcher, Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( () => func( arg1, arg2, arg3, arg4 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation CallAsync<T1, T2, T3, T4, T5>( this Dispatcher dispatcher, Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( () => func( arg1, arg2, arg3, arg4, arg5 ), DispatcherPriority.Normal, token );


    public static DispatcherOperation<Task> CallAsync( this     DispatcherObject view, Func<Task>     func, CancellationToken token                        = default ) => view.Dispatcher.CallAsync( func, token );
    public static DispatcherOperation<Task> CallAsync<T1>( this DispatcherObject view, Func<T1, Task> func, T1                arg, CancellationToken token = default ) => view.Dispatcher.CallAsync( func, arg, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2>( this DispatcherObject view, Func<T1, T2, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3>( this DispatcherObject view, Func<T1, T2, T3, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( this DispatcherObject view, Func<T1, T2, T3, T4, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, arg4, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( this DispatcherObject view, Func<T1, T2, T3, T4, T5, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, arg4, arg5, token );


    public static DispatcherOperation<Task> CallAsync<T1>( this DispatcherObject view, Func<T1, CancellationToken, Task> func, T1 arg, CancellationToken token = default ) => view.Dispatcher.CallAsync( func, arg, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2>( this DispatcherObject view, Func<T1, T2, CancellationToken, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3>( this DispatcherObject view, Func<T1, T2, T3, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( this DispatcherObject view, Func<T1, T2, T3, T4, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, arg4, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( this DispatcherObject view, Func<T1, T2, T3, T4, T5, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
        view.Dispatcher.CallAsync( func, arg1, arg2, arg3, arg4, arg5, token );


    public static DispatcherOperation<Task> CallAsync( this     Dispatcher dispatcher, Func<Task>     func, CancellationToken token = default ) => dispatcher.InvokeAsync( func, DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1>( this Dispatcher dispatcher, Func<T1, Task> func, T1 arg, CancellationToken token = default ) => dispatcher.InvokeAsync( async () => await func( arg ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2>( this Dispatcher dispatcher, Func<T1, T2, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3>( this Dispatcher dispatcher, Func<T1, T2, T3, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, arg3 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( this Dispatcher dispatcher, Func<T1, T2, T3, T4, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, arg3, arg4 ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( this Dispatcher dispatcher, Func<T1, T2, T3, T4, T5, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, arg3, arg4, arg5 ), DispatcherPriority.Normal, token );


    public static DispatcherOperation<Task> CallAsync<T1>( this Dispatcher dispatcher, Func<T1, CancellationToken, Task> func, T1 arg, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg, token ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2>( this Dispatcher dispatcher, Func<T1, T2, CancellationToken, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, token ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3>( this Dispatcher dispatcher, Func<T1, T2, T3, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, arg3, token ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( this Dispatcher dispatcher, Func<T1, T2, T3, T4, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, arg3, arg4, token ), DispatcherPriority.Normal, token );
    public static DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( this Dispatcher dispatcher, Func<T1, T2, T3, T4, T5, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
        dispatcher.InvokeAsync( async () => await func( arg1, arg2, arg3, arg4, arg5, token ), DispatcherPriority.Normal, token );


    public static BitmapImage ConvertImage( this LocalFile file ) => new(file.ToUri());


    /// <summary>
    ///     <see href = "https://stackoverflow.com/a/41579163/9530917" > Convert drawing.bitmap to windows.controls.image </see>
    /// </summary>
    /// <param name = "image" > </param>
    /// <returns>
    ///     <see cref = "ImageSource" />
    /// </returns>
    public static ImageSource ConvertImage( this Bitmap image )
    {
        using var stream = new MemoryStream();
        image.Save( stream, ImageFormat.Png );

        var photo = new BitmapImage();

        photo.BeginInit();
        photo.CacheOption  = BitmapCacheOption.OnLoad;
        photo.StreamSource = stream;
        photo.EndInit();

        return photo;
    }

    public static LocalFile? PickFile( string title, params string[] filters )
    {
        var file = new OpenFileDialog
                   {
                       Title            = title,
                       Multiselect      = false,
                       AddExtension     = true,
                       CheckFileExists  = true,
                       CheckPathExists  = true,
                       InitialDirectory = LocalDirectory.CurrentDirectory.FullPath,
                       Filter           = @"Files|" + string.Join( ';', filters )
                   };

        DialogResult result = file.ShowDialog();

        return result is DialogResult.OK or DialogResult.Yes
                   ? new LocalFile( file.FileName )
                   : default;
    }


    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    ///     <see href = "https://stackoverflow.com/a/65200533/9530917" />
    /// </summary>
    /// <param name = "title" > </param>
    /// <returns> </returns>
    public static LocalDirectory? PickFolder( string title )
    {
        using var dialog = new FolderBrowserDialog
                           {
                               Description            = title,
                               UseDescriptionForTitle = true,
                               SelectedPath           = LocalDirectory.CurrentDirectory.FullPath,
                               ShowNewFolderButton    = true
                           };

        DialogResult result = dialog.ShowDialog();

        return result is DialogResult.OK or DialogResult.Yes
                   ? new LocalDirectory( dialog.SelectedPath )
                   : default;
    }
    public static void SetContent( this ContentControl control, object? value )
    {
        object old = control.Content;
        control.Content = value;

        ContentControl.ContentProperty.GetMetadata( control )
                      .PropertyChangedCallback.Invoke( control, new DependencyPropertyChangedEventArgs( ContentControl.ContentProperty, old, value ) );
    }


    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    public static ListCollectionView ToCollectionView<T>( this ConcurrentObservableCollection<T> list )
    {
        BindingOperations.EnableCollectionSynchronization( list, list.Lock );
        var collection = new ListCollectionView( list );
        return collection;
    }
    public static ListCollectionView ToCollectionView<T, TComparer>( this ConcurrentObservableCollection<T> list, TComparer comparer ) where TComparer : IComparer<T>, IComparer
    {
        BindingOperations.EnableCollectionSynchronization( list, list.Lock );

        var collection = new ListCollectionView( list )
                         {
                             CustomSort = comparer
                         };

        return collection;
    }
    public static ListCollectionView ToCollectionView<T, TComparer>( this ConcurrentObservableCollection<T> list, TComparer comparer, Predicate<T> filter ) where TComparer : IComparer<T>, IComparer
    {
        bool Filter( object item ) { return item is T value && filter( value ); }

        BindingOperations.EnableCollectionSynchronization( list, list.Lock );

        var collection = new ListCollectionView( list )
                         {
                             Filter     = Filter,
                             CustomSort = comparer
                         };

        return collection;
    }
}
