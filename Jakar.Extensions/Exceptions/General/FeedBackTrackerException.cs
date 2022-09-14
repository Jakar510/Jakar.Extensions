#nullable enable
namespace Jakar.Extensions;


public class FeedBackTrackerException : Exception
{
    //var e = new FeedBackTrackerException(JsonConvert.SerializeObject(dict));
    public FeedBackTrackerException() { }
    public FeedBackTrackerException( string message ) : base(message) { }
    public FeedBackTrackerException( string message, Exception inner ) : base(message, inner) { }

    public FeedBackTrackerException( object dict ) : this(dict.ToPrettyJson()) { }
    public FeedBackTrackerException( object dict, Exception inner ) : this(dict.ToPrettyJson(), inner) { }

    protected void Update( object value )
    {
        switch ( value )
        {
            case IDictionary dict:
            {
                foreach ( DictionaryEntry pair in dict ) { Data[pair.Key.ToString() ?? throw new NullReferenceException(nameof(pair.Key))] = pair.Value?.ToString(); }

                break;
            }

            case IList list:
            {
                foreach ( ( int index, object? item ) in list.Enumerate(0) ) { Data[index.ToString()] = item?.ToString(); }

                break;
            }

            default:
            {
                Data[nameof(value)] = value.ToString();
                break;
            }
        }
    }
}
