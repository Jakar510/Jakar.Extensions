#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;



namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class IsList_Tests : Assert
{
    [Test]
    [TestCase( typeof(string),                       false )]
    [TestCase( typeof(string[]),                     true )]
    [TestCase( typeof(Dictionary<string, object>),   false )]
    [TestCase( typeof(Dictionary<string, string>),   false )]
    [TestCase( typeof(ConcurrentBag<string>),        false )]
    [TestCase( typeof(ObservableCollection<string>), true )]
    [TestCase( typeof(List<string>),                 true )]
    [TestCase( typeof(List<List<string>>),           true )]
    [TestCase( typeof(List<Guid>),                   true )]
    public void IsList_Test( Type objType, bool expected ) => AreEqual( objType.IsList(), expected );


    [Test]
    [TestCase( typeof(string),                     null )]
    [TestCase( typeof(Dictionary<string, object>), null )]
    [TestCase( typeof(Dictionary<string, string>), null )]
    [TestCase( typeof(ConcurrentBag<string>),      null )]
    [TestCase( typeof(HashSet<string>),            null )]
    [TestCase( typeof(List<string>),               typeof(string) )]
    [TestCase( typeof(List<List<string>>),         typeof(List<string>) )]
    [TestCase( typeof(List<Guid>),                 typeof(Guid) )]
    public void IsList_Args_Test( Type objType, Type? expected )
    {
        if (objType.IsList( out Type? itemType ))
        {
            AreEqual( itemType, expected );
            return;
        }

        IsNull( expected );
    }
}
