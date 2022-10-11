#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;



namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class IsCollection_Tests : Assert
{
    [Test]
    [TestCase( typeof(string),                       false )]
    [TestCase( typeof(string[]),                     true )]
    [TestCase( typeof(Dictionary<string, object>),   true )]
    [TestCase( typeof(Dictionary<string, string>),   true )]
    [TestCase( typeof(ConcurrentBag<string>),        true )]
    [TestCase( typeof(ObservableCollection<string>), true )]
    [TestCase( typeof(List<string>),                 true )]
    [TestCase( typeof(List<List<string>>),           true )]
    [TestCase( typeof(List<Guid>),                   true )]
    public void IsCollection_Test( Type objType, bool expected ) => AreEqual( objType.IsCollection(), expected );


    [Test]
    [TestCase( typeof(string),                     null )]
    [TestCase( typeof(Dictionary<string, object>), typeof(string) )]
    [TestCase( typeof(Dictionary<string, string>), typeof(string) )]
    [TestCase( typeof(ConcurrentBag<string>),      typeof(string) )]
    [TestCase( typeof(HashSet<string>),            typeof(string) )]
    [TestCase( typeof(List<string>),               typeof(string) )]
    [TestCase( typeof(List<List<string>>),         typeof(List<string>) )]
    [TestCase( typeof(List<Guid>),                 typeof(Guid) )]
    public void IsCollection_Args_Test( Type objType, Type? expected )
    {
        if (objType.IsCollection( out Type? itemType ))
        {
            AreEqual( itemType, expected );
            return;
        }

        IsNull( expected );
    }
}
