using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;



namespace Jakar.Xml.Serialization;


public readonly ref struct XWriter
{
    private readonly XContext      _context;
    private readonly StringBuilder _sb = new();

    public XWriter( in XContext context ) => _context = context;

    public override string ToString() => _sb.ToString();

    // public void Dispose() => _context.Dispose();
}
