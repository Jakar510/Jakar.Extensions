using System.Collections;
using System.Reflection;
using System.Xml;
using Jakar.Extensions.Exceptions.General;
using Jakar.Extensions.Models.Base;
using Jakar.Extensions.Strings;
using Jakar.Extensions.Types;
using Jakar.Json.Serialization;



namespace Jakar.Json;


public interface IJsonizer
{
    internal string ToJson( in    JWriter writer );
    internal string Serialize( in JObject context );
    internal string Serialize( in JArray  context );
}
