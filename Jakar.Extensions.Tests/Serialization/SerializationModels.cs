// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Collections.Generic;
using System.Text.Json.Serialization;



namespace Jakar.Extensions.Tests.Serialization;


[JsonModel(typeof(TestJsonContext))]
public sealed partial class TestPreferences : PreferenceFile<TestPreferences>, IEqualComparable<TestPreferences>
{
    public GeneralSection General { get; set => SetProperty(ref field, value); } = new();
    public ServerSection  Server  { get; set => SetProperty(ref field, value); } = new();


    public override bool Equals( TestPreferences? other ) => ReferenceEquals(this, other);
    public override int CompareTo( TestPreferences? other ) => ReferenceEquals(this, other)
                                                                   ? 0
                                                                   : 1;
    public override bool Equals( object? other ) => ReferenceEquals(this, other);
    public override int  GetHashCode()           => base.GetHashCode();

    public static bool operator ==( TestPreferences? left, TestPreferences? right ) => ReferenceEquals(left, right);
    public static bool operator !=( TestPreferences? left, TestPreferences? right ) => !ReferenceEquals(left, right);
    public static bool operator <( TestPreferences   left, TestPreferences  right ) => false;
    public static bool operator >( TestPreferences   left, TestPreferences  right ) => false;
    public static bool operator <=( TestPreferences  left, TestPreferences  right ) => true;
    public static bool operator >=( TestPreferences  left, TestPreferences  right ) => true;



    public sealed class GeneralSection
    {
        public string Theme   { get; set; } = "Dark";
        public bool   Enabled { get; set; }
    }



    public sealed class ServerSection
    {
        public string Host { get; set; } = "localhost";
        public int    Port { get; set; } = 80;
    }
}



/// <summary> A plain (non-[JsonModel]) type for the value-helper tests. </summary>
public sealed record Point( int X, int Y );
