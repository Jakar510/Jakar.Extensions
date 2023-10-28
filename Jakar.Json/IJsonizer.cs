namespace Jakar.Json;


public interface IJsonizer
{
    protected internal int    JsonSize();
    public             string ToJson( ref      JWriter writer );
    public             void   Serialize( ref   JObject parent );
    public             void   Serialize( ref   JArray  parent );
    public             void   Deserialize( ref JReader writer );
    public             void   Deserialize( ref JNode   parent );
}
