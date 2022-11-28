// ReSharper disable once CheckNamespace

#nullable enable


using System.ComponentModel;



// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;


#if NETSTANDARD



/// <summary>
///     Reserved to be used by the compiler for tracking metadata.
///     This class should not be used by developers in source code.
/// </summary>
[EditorBrowsable( EditorBrowsableState.Never )]
internal class IsExternalInit { }



#endif
