using System.Linq;
using ZLinq;



namespace Jakar.Extensions;


public static partial class Types
{
    public static ParameterDetails GetParameterInfo( this ParameterInfo parameter ) => new(parameter);



    extension( MethodBase self )
    {
        public string MethodSignature()
        {
            return self.Name.AppendJoin(self.GetParameters()
                                            .AsValueEnumerable()
                                            .Select(static x => x.ParameterType.FullName ?? x.ParameterType.Name))
                       .ToString();
        }

        public string MethodName() => self.Name;

        public string? MethodClass() => self.DeclaringType?.FullName;

        public MethodDetails MethodInfo() => new(self);
    }
}
