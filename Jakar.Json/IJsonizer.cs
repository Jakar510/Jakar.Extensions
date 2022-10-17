#nullable enable
namespace Jakar.Json;


public interface IJsonizer
{
    public void Serialize( ref           JObject parent );
    protected internal string ToJson( in JWriter writer );
}



public interface IArrayWriter : IJsonizer
{
    public void Serialize( in JArray parent );
}



public interface IDeJsonizer : IJsonizer
{
    protected internal void Deserialize( in JReader writer );
    public void Deserialize( in             JNode   parent );
}
