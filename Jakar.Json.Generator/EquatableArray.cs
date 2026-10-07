// Jakar.Json.Generator
// 10/02/2026

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;



namespace Jakar.Json.Generator;


/// <summary> An immutable array with value equality, so pipeline models compare equal across runs and the incremental generator can cache them. </summary>
internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    public static readonly EquatableArray<T> Empty = new([]);

    private readonly T[]? _array;


    public EquatableArray( T[]            array ) => _array = array;
    public EquatableArray( IEnumerable<T> values ) => _array = [.. values];


    public int Count => _array?.Length ?? 0;
    public T this[ int index ] => _array![index];


    public bool Equals( EquatableArray<T> other )
    {
        T[] left  = _array       ?? [];
        T[] right = other._array ?? [];
        if ( left.Length != right.Length ) { return false; }

        for ( int i = 0; i < left.Length; i++ )
        {
            if ( !left[i].Equals(right[i]) ) { return false; }
        }

        return true;
    }

    public override bool Equals( object? obj ) => obj is EquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        int hash = 17;

        foreach ( T value in _array ?? [] ) { hash = unchecked (hash * 31 + value.GetHashCode()); }

        return hash;
    }

    public static bool operator ==( EquatableArray<T> left, EquatableArray<T> right ) => left.Equals(right);
    public static bool operator !=( EquatableArray<T> left, EquatableArray<T> right ) => !left.Equals(right);


    public IEnumerator<T>   GetEnumerator() => ( (IEnumerable<T>)( _array ?? [] ) ).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


    public ImmutableArray<T> AsImmutableArray() => _array is null
                                                       ? ImmutableArray<T>.Empty
                                                       : [.. _array];
}
