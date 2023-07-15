#nullable enable
namespace Jakar.Json;


public interface IJsonizer
{
    protected internal int JsonSize();
    public string ToJson( in    JWriter writer );
    public void Serialize( ref  JObject parent );
    public void Serialize( in   JArray  parent );
    public void Deserialize( in JReader writer );
    public void Deserialize( in JNode   parent );
}
