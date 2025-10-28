// Jakar.Extensions :: Jakar.Extensions
// 01/18/2025  13:01

using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions;


public interface IExceptionHandler<in TFileData, TID, TFileMetaData>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TFileMetaData : class, IFileMetaData<TFileMetaData>
    where TFileData : class, IFileData<TFileData, TID, TFileMetaData>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void      HandleException<TValue>( TValue      _, Exception e );
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void      HandleException<TValue>( TValue      _, Exception e, params TFileData[] attachmentLogs );
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask HandleExceptionAsync<TValue>( TValue _, Exception e );
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask HandleExceptionAsync<TValue>( TValue _, Exception e, params TFileData[] attachmentLogs );
}



public interface IExceptionHandler : IExceptionHandler<FileData, Guid, FileMetaData>;
