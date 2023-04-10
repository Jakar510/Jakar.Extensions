// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/26/2022  2:48 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


public abstract record LoggerTable<TRecord> : TableRecord<TRecord>, JsonModels.IJsonStringModel where TRecord : LoggerTable<TRecord>
{
    private string? _additionalData;


    [MaxLength( int.MaxValue )]
    public string? AdditionalData
    {
        get => _additionalData;
        set => SetProperty( ref _additionalData, value );
    }


    public   bool IsActive    { get; init; } = true;
    internal bool IsNotActive => !IsActive;


    protected LoggerTable() : base() { }
    protected LoggerTable( Guid id, UserRecord? caller = default ) : base( id, caller ) { }
}
