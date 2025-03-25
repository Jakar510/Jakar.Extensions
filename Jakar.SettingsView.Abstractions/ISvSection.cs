// Jakar.Extensions :: Jakar.SettingsView.Abstractions
// 08/13/2024  20:08

using System.Runtime.CompilerServices;



namespace Jakar.SettingsView.Abstractions;


public interface ISvSection : IValidator, IDisposable
{
    public ISectionBorder Header { get; }
    public ISectionBorder Footer { get; }
}



public interface ISvSection<TValue> : ISvSection
    where TValue : ISvCellTitle, IEquatable<TValue>
{
    public ObservableCollection<TValue> Cells { get; }
}



public static class SvSectionExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool AreCellsValid<TValue>( this ISvSection<TValue> section )
        where TValue : ISvCellTitle, IEquatable<TValue> => section.Cells.AreCellsValid();


    public static bool AreCellsValid<TValue>( this IEnumerable<TValue> cells )
        where TValue : ISvCellTitle
    {
        foreach ( TValue cell in cells )
        {
            if ( cell is not IValidator validator ) { continue; }

            if ( validator.IsValid is false ) { return false; }
        }

        return true;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Add<TValue>( this ISvSection<TValue> section, TValue cell )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        section.Cells.Add( cell );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Add<TValue>( this ISvSection<TValue> section, IEnumerable<TValue> cells )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        foreach ( TValue cell in cells ) { section.Cells.Add( cell ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Add<TValue>( this ISvSection<TValue> section, params TValue[] cells )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        section.Add( cells.AsEnumerable() );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Remove<TValue>( this ISvSection<TValue> section, TValue cell )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        section.Cells.Remove( cell );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Remove<TValue>( this ISvSection<TValue> section, IEnumerable<TValue> cells )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        foreach ( TValue cell in cells ) { section.Cells.Remove( cell ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Remove<TValue>( this ISvSection<TValue> section, params TValue[] cells )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        section.Remove( cells.AsEnumerable() );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void Clear<TValue>( this ISvSection<TValue> section )
        where TValue : ISvCellTitle, IEquatable<TValue>
    {
        section.Cells.Clear();
    }
}
