// Jakar.AppLogger :: Jakar.AppLogger.Common
// 08/06/2022  7:30 PM

namespace Jakar.AppLogger.Common;


public interface IErrorDetails
{
    public string Platform { get; init; }
}



public abstract class ErrorDetails : BaseJsonModel, IErrorDetails
{
    public string Platform { get; init; }


    protected ErrorDetails() => Platform = GetType().Name;



    [Serializable]
    public class Collection : ObservableCollection<ErrorDetails>
    {
        public Collection() : base() { }
        public Collection( IEnumerable<ErrorDetails> items ) : base(items) { }
    }
}
