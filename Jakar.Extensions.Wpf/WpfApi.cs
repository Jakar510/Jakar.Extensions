namespace Jakar.Extensions.Wpf;


public static partial class WpfApi
{
    public static BitmapImage ConvertImage( this LocalFile file ) => new(file.ToUri());



    extension( DispatcherObject self )
    {
        public DispatcherOperation CallAsync( Action func, CancellationToken token = default ) => self.Dispatcher.CallAsync(func, token);
        public DispatcherOperation CallAsync<T1>( Action<T1> func, T1 arg1, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, token);
        public DispatcherOperation CallAsync<T1, T2>( Action<T1, T2> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, token);
        public DispatcherOperation CallAsync<T1, T2, T3>( Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, token);
        public DispatcherOperation CallAsync<T1, T2, T3, T4>( Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, arg4, token);
        public DispatcherOperation CallAsync<T1, T2, T3, T4, T5>( Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, arg4, arg5, token);
    }



    extension( Dispatcher self )
    {
        public DispatcherOperation CallAsync( Action func, CancellationToken token = default ) => self.BeginInvoke(func, DispatcherPriority.Normal, token);
        public DispatcherOperation CallAsync<T1>( Action<T1> func, T1 arg1, CancellationToken token = default ) =>
            self.InvokeAsync(() => func(arg1), DispatcherPriority.Normal, token);
        public DispatcherOperation CallAsync<T1, T2>( Action<T1, T2> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
            self.InvokeAsync(() => func(arg1, arg2), DispatcherPriority.Normal, token);
        public DispatcherOperation CallAsync<T1, T2, T3>( Action<T1, T2, T3> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
            self.InvokeAsync(() => func(arg1, arg2, arg3), DispatcherPriority.Normal, token);
        public DispatcherOperation CallAsync<T1, T2, T3, T4>( Action<T1, T2, T3, T4> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
            self.InvokeAsync(() => func(arg1, arg2, arg3, arg4), DispatcherPriority.Normal, token);
        public DispatcherOperation CallAsync<T1, T2, T3, T4, T5>( Action<T1, T2, T3, T4, T5> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
            self.InvokeAsync(() => func(arg1, arg2, arg3, arg4, arg5), DispatcherPriority.Normal, token);
    }



    extension( DispatcherObject self )
    {
        public DispatcherOperation<Task> CallAsync( Func<Task>         func, CancellationToken token                        = default ) => self.Dispatcher.CallAsync(func, token);
        public DispatcherOperation<Task> CallAsync<T1>( Func<T1, Task> func, T1                arg, CancellationToken token = default ) => self.Dispatcher.CallAsync(func, arg, token);
        public DispatcherOperation<Task> CallAsync<T1, T2>( Func<T1, T2, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3>( Func<T1, T2, T3, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( Func<T1, T2, T3, T4, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, arg4, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( Func<T1, T2, T3, T4, T5, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, arg4, arg5, token);
        public DispatcherOperation<Task> CallAsync<T1>( Func<T1, CancellationToken, Task> func, T1 arg, CancellationToken token = default ) => self.Dispatcher.CallAsync(func, arg, token);
        public DispatcherOperation<Task> CallAsync<T1, T2>( Func<T1, T2, CancellationToken, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3>( Func<T1, T2, T3, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( Func<T1, T2, T3, T4, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, arg4, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( Func<T1, T2, T3, T4, T5, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
            self.Dispatcher.CallAsync(func, arg1, arg2, arg3, arg4, arg5, token);
    }



    extension( Dispatcher self )
    {
        public DispatcherOperation<Task> CallAsync( Func<Task>         func, CancellationToken token                        = default ) => self.InvokeAsync(func,                        DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1>( Func<T1, Task> func, T1                arg, CancellationToken token = default ) => self.InvokeAsync(async () => await func(arg), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2>( Func<T1, T2, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3>( Func<T1, T2, T3, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, arg3), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( Func<T1, T2, T3, T4, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, arg3, arg4), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( Func<T1, T2, T3, T4, T5, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, arg3, arg4, arg5), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1>( Func<T1, CancellationToken, Task> func, T1 arg, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg, token), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2>( Func<T1, T2, CancellationToken, Task> func, T1 arg1, T2 arg2, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, token), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3>( Func<T1, T2, T3, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, arg3, token), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4>( Func<T1, T2, T3, T4, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, arg3, arg4, token), DispatcherPriority.Normal, token);
        public DispatcherOperation<Task> CallAsync<T1, T2, T3, T4, T5>( Func<T1, T2, T3, T4, T5, CancellationToken, Task> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken token = default ) =>
            self.InvokeAsync(async () => await func(arg1, arg2, arg3, arg4, arg5, token), DispatcherPriority.Normal, token);
    }



    /// <summary>
    ///     <see href="https://stackoverflow.com/a/41579163/9530917"> Convert drawing.bitmap to windows.controls.image </see>
    /// </summary>
    /// <param name="image"> </param>
    /// <returns>
    ///     <see cref="ImageSource"/>
    /// </returns>
    public static ImageSource ConvertImage( this Bitmap image )
    {
        using MemoryStream stream = new();
        image.Save(stream, ImageFormat.Png);

        BitmapImage photo = new();

        photo.BeginInit();
        photo.CacheOption  = BitmapCacheOption.OnLoad;
        photo.StreamSource = stream;
        photo.EndInit();

        return photo;
    }


    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------



    extension<TValue>( ConcurrentObservableCollection<TValue> self )
        where TValue : IEquatable<TValue>
    {
        public ListCollectionView ToCollectionView()
        {
            BindingOperations.EnableCollectionSynchronization(self, self.Lock);
            ListCollectionView collection = new(self);
            return collection;
        }
        public ListCollectionView ToCollectionView<TComparer>( TComparer comparer )
            where TComparer : IComparer<TValue>, IComparer
        {
            BindingOperations.EnableCollectionSynchronization(self, self.Lock);

            ListCollectionView collection = new(self) { CustomSort = comparer };

            return collection;
        }
        public ListCollectionView ToCollectionView<TComparer>( TComparer comparer, Func<TValue, bool> filter )
            where TComparer : IComparer<TValue>, IComparer
        {
            BindingOperations.EnableCollectionSynchronization(self, self.Lock);

            ListCollectionView collection = new(self)
                                            {
                                                Filter     = doFilter,
                                                CustomSort = comparer
                                            };

            return collection;
            bool doFilter( object item ) => item is TValue value && filter(value);
        }
    }



    // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/65200533/9530917"/>
    /// </summary>
    /// <param name="title"> </param>
    /// <returns> </returns>
    public static LocalDirectory? PickFolder( string title )
    {
        using FolderBrowserDialog dialog = new()
                                           {
                                               Description            = title,
                                               UseDescriptionForTitle = true,
                                               SelectedPath           = LocalDirectory.CurrentDirectory.FullPath,
                                               ShowNewFolderButton    = true
                                           };

        DialogResult result = dialog.ShowDialog();

        return result is DialogResult.OK or DialogResult.Yes
                   ? new LocalDirectory(dialog.SelectedPath)
                   : null;
    }

    public static LocalFile? PickFile( string title, params string[] filters )
    {
        OpenFileDialog file = new()
                              {
                                  Title            = title,
                                  Multiselect      = false,
                                  AddExtension     = true,
                                  CheckFileExists  = true,
                                  CheckPathExists  = true,
                                  InitialDirectory = LocalDirectory.CurrentDirectory.FullPath,
                                  Filter           = @"Files|" + string.Join(';', filters)
                              };

        DialogResult result = file.ShowDialog();

        return result is DialogResult.OK or DialogResult.Yes
                   ? new LocalFile(file.FileName)
                   : null;
    }
    public static void SetContent( this ContentControl control, object? value )
    {
        object old = control.Content;
        control.Content = value;

        ContentControl.ContentProperty.GetMetadata(control)
                      .PropertyChangedCallback.Invoke(control, new DependencyPropertyChangedEventArgs(ContentControl.ContentProperty, old, value));
    }
}
