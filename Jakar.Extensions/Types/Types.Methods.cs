using System.Linq;
using ZLinq;



namespace Jakar.Extensions;


public static partial class Types
{
    public static ParameterDetails GetParameterInfo( this ParameterInfo parameter ) => new(parameter);



    extension( MethodBase self )
    {
        public string MethodName() => self.Name;
        public string MethodSignature()
        {
            StringBuilder sb = new();
            sb.Append(self.Name);
            sb.Append("( ");

            using PooledArray<string> strings = self.GetParameters()
                                              .AsValueEnumerable()
                                              .Select(static x => x.ParameterType.FullName ?? x.ParameterType.Name)
                                              .ToArrayPool();

            sb.AppendJoin(", ", strings.Span);

            sb.Append(" )");
            return sb.ToString();
        }
        public string?       MethodClass() => self.DeclaringType?.FullName;
        public MethodDetails MethodInfo()  => new(self);
    }
}
