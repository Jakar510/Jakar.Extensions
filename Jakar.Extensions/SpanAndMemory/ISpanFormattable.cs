﻿// Jakar.Extensions :: Jakar.Extensions
// 03/29/2023  6:44 PM

#if NETSTANDARD2_1
namespace System;


/// <summary> Provides functionality to format the string representation of an object into a span. </summary>
public interface ISpanFormattable : IFormattable
{
    /// <summary> Tries to format the value of the current instance into the provided span of characters. </summary>
    /// <param name="destination"> When this method returns, this instance's value formatted as a span of characters. </param>
    /// <param name="charsWritten"> When this method returns, the number of characters that were written in <paramref name="destination"/>. </param>
    /// <param name="format"> A span containing the characters that represent a standard or custom format string that defines the acceptable format for <paramref name="destination"/>. </param>
    /// <param name="provider"> An optional object that supplies culture-specific formatting information for <paramref name="destination"/>. </param>
    /// <returns> <see langword="true"/> if the formatting was successful; otherwise, <see langword="false"/>. </returns>
    /// <remarks> An implementation of this interface should produce the same string of characters as an implementation of <see cref="IFormattable.ToString(string?, IFormatProvider?)"/> on the same type. TryFormat should return false only if there is not enough space in the destination buffer. Any other failures should throw an exception. </remarks>
    bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider );
}



#endif
