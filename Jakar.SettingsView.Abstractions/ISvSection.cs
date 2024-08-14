// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/13/2024  20:08

using System.Runtime.CompilerServices;



namespace Jakar.SettingsView.Abstractions;


public interface ISvSection : IValidator, IDisposable
{
    public ISectionBorder Header { get; }
    public ISectionBorder Footer { get; }
}



public interface ISvSection<T> : ISvSection
    where T : ISvCellTitle
{
    public ObservableCollection<T> Cells { get; }
}



public static class SvSectionExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool AreCellsValid<T>( this ISvSection<T> section )
        where T : ISvCellTitle => section.Cells.AreCellsValid();


    public static bool AreCellsValid<T>( this IEnumerable<T> cells )
        where T : ISvCellTitle
    {
        foreach ( T cell in cells )
        {
            if ( cell is not IValidator validator ) { continue; }

            if ( validator.IsValid is false ) { return false; }
        }

        return true;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Add<T>( this ISvSection<T> section, T cell )
        where T : ISvCellTitle
    {
        section.Cells.Add( cell );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Add<T>( this ISvSection<T> section, IEnumerable<T> cells )
        where T : ISvCellTitle
    {
        foreach ( T cell in cells ) { section.Cells.Add( cell ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Add<T>( this ISvSection<T> section, params T[] cells )
        where T : ISvCellTitle
    {
        section.Add( cells.AsEnumerable() );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Remove<T>( this ISvSection<T> section, T cell )
        where T : ISvCellTitle
    {
        section.Cells.Remove( cell );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Remove<T>( this ISvSection<T> section, IEnumerable<T> cells )
        where T : ISvCellTitle
    {
        foreach ( T cell in cells ) { section.Cells.Remove( cell ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Remove<T>( this ISvSection<T> section, params T[] cells )
        where T : ISvCellTitle
    {
        section.Remove( cells.AsEnumerable() );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Clear<T>( this ISvSection<T> section )
        where T : ISvCellTitle
    {
        section.Cells.Clear();
    }
}
