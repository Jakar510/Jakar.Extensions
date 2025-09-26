using System.Collections.Concurrent;
using System.Collections.Generic;



namespace Jakar.Extensions.Tests.Reflections;


[TestFixture][TestOf( typeof(Types) )]

// ReSharper disable once InconsistentNaming
public class IsDictionary_Tests : Assert
{
    [Test][TestCase( typeof(string), false )][TestCase( typeof(MultiDict), true )][TestCase( typeof(Dictionary<string, object>), true )][TestCase( typeof(Dictionary<string, string>), true )][TestCase( typeof(ConcurrentDictionary<string, string>), true )][TestCase( typeof(ObservableDictionary<string, string>), true )][TestCase( typeof(List<string>), false )][TestCase( typeof(List<Guid>), false )]
    public void IsDictionary_Test( Type objType, bool expected ) => this.AreEqual( objType.IsDictionary(), expected );


    [Test][TestCase(typeof(string),                         null )][TestCase(             typeof(Dictionary<string, object>), typeof(object) )][TestCase(typeof(Dictionary<string, string>), typeof(string) )][TestCase( typeof(Dictionary<string, int>), typeof(int) )][TestCase( typeof(Dictionary<string, int?>), typeof(int?) )][TestCase( typeof(Dictionary<string, List<int>>), typeof(List<int>) )][TestCase(      typeof(Dictionary<string, List<int>?>), typeof(List<int>) )][TestCase(typeof(MultiDict),                  typeof(object) )][TestCase(typeof(ObservableDictionary<string, string>), typeof(string) )][TestCase( typeof(List<string>), null )][TestCase( typeof(List<Guid>), null )]
    public void IsDictionary_Args_Test( Type objType, Type? expected )
    {
        if ( objType.IsDictionary( out Type? _, out Type? valueType ) )
        {
            this.AreEqual( valueType, expected );
            return;
        }

        this.IsNull( expected );
    }
}
