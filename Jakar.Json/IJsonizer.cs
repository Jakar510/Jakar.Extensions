namespace Jakar.Json;


public interface IJsonizer
{
    internal string ToJson( in    JWriter writer );
    internal string Serialize( in JObject context );
    internal string Serialize( in JArray  context );


    internal string Deserialize( in JReader writer );
    internal string Deserialize( in JNode   context );
}
