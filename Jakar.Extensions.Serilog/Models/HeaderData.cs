// Jakar.Extensions :: Jakar.Extensions.Serilog
// 01/19/2025  14:01

using Newtonsoft.Json;



namespace Jakar.Extensions.Serilog;


public interface IHeaderCells : JsonModels.IJsonModel, IHeader
{
    HeaderDataCell[]? HeaderCells { get; init; }
}



[Serializable]
public readonly record struct HeaderDataCell( string? Title, string? Value )
{
    public static implicit operator HeaderDataCell( (string? Title, string? Value) tuple ) => new(tuple.Title, tuple.Value);
    public static implicit operator HeaderDataCell( KeyValuePair<string, string>   tuple ) => new(tuple.Key, tuple.Value);



    public sealed class Collection() : ObservableCollection<HeaderDataCell>( Buffers.DEFAULT_CAPACITY )
    {
        public Collection( IEnumerable<HeaderDataCell> enumerable ) : this() => Add( enumerable );
    }
}



[Serializable]
[method: JsonConstructor]
public sealed class HeaderData( string? title, TextFontAttributes titleAttributes, params HeaderDataCell[]? cells )
{
    public static HeaderData         Empty           => new(null, TextFontAttributes.None, null);
    public        HeaderDataCell[]?  Cells           { get; init; } = cells;
    public        bool               IsEmpty         => !IsNotEmpty;
    public        bool               IsNotEmpty      => Cells?.Length is > 0 || !string.IsNullOrEmpty( Title );
    public        string?            Title           { get; init; } = title;
    public        TextFontAttributes TitleAttributes { get; init; } = titleAttributes;


    public bool GetCells( out ReadOnlySpan<HeaderDataCell> cells )
    {
        if ( Cells?.Length is > 0 )
        {
            cells = Cells;
            return true;
        }

        cells = ReadOnlySpan<HeaderDataCell>.Empty;
        return false;
    }


    public static HeaderData? TryCreate<TValue>( [NotNullIfNotNull( nameof(heading) )] TValue? heading )
        where TValue : IHeaderCells => heading is not null
                                      ? Create( heading )
                                      : null;


    public static HeaderData Create<TValue>( TValue heading )
        where TValue : IHeaderCells => new(heading.Title,
                                      heading.IsTitleBold
                                          ? TextFontAttributes.Bold
                                          : TextFontAttributes.None,
                                      heading.HeaderCells);
}
