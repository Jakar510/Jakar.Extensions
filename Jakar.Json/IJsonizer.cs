namespace Jakar.Json;


public interface IJsonizer
{
    protected internal int  JsonSize();
    public             void Serialize( ref   JWriter    writer );
    public             void Serialize( ref   JsonObject parent );
    public             void Serialize( ref   JsonArray  parent );
    public             void Deserialize( ref JReader    writer );
    public             void Deserialize( ref JNode      parent );
}
