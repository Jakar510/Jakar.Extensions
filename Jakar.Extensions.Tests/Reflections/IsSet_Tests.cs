using System.Collections.Concurrent;
using System.Collections.Generic;



namespace Jakar.Extensions.Tests.Reflections;


[TestFixture][TestOf(typeof(Types))]

// ReSharper disable once InconsistentNaming
public class IsSet_Tests : Assert
{
    [Test][TestCase(typeof(string),          false)][TestCase(typeof(string[]),           false)][TestCase(typeof(Dictionary<string, object>), false)][TestCase(typeof(Dictionary<string, string>), false)][TestCase(typeof(ConcurrentBag<string>), false)][TestCase(typeof(ObservableCollection<string>), false)][TestCase(typeof(HashSet<string>), true)][TestCase(typeof(HashSet<int>), true)][TestCase(      typeof(HashSet<double>), true)][TestCase( typeof(List<List<string>>), false)][TestCase(typeof(List<Guid>),                 false)]
    public void IsSet_Test( Type objType, bool expected )
    {
        // TODO: objType.GetInterfaces().PrintJson(JakarExtensionsContext.Default.TypeArray).WriteToConsole();
        That(objType.IsSet(), Is.EqualTo(expected));
    }


    [Test][TestCase(typeof(string), null)][TestCase(typeof(Dictionary<string, object>), null)][TestCase(typeof(Dictionary<string, string>), null)][TestCase(typeof(ConcurrentBag<string>), null)][TestCase(typeof(HashSet<bool>), typeof(bool))][TestCase(typeof(HashSet<string>), typeof(string))][TestCase(typeof(List<List<string>>), null)][TestCase(typeof(List<Guid>), null)]
    public void IsSet_Args_Test( Type objType, Type? expected )
    {
        if ( objType.IsSet(out Type? itemType) )
        {
            That(itemType, Is.EqualTo(expected));
            return;
        }

        That(expected, Is.Null);
    }
}
