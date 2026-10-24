// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

namespace Jakar.Extensions.Tests;


[JsonModel]
public sealed partial class TestPreferences : PreferenceFile<TestPreferences>, IEqualComparable<TestPreferences>
{
    public GeneralSection General { get; set => SetProperty(ref field, value); } = new();
    public ServerSection  Server  { get; set => SetProperty(ref field, value); } = new();


    protected override void CopyFrom( TestPreferences loaded )
    {
        General = loaded.General;
        Server  = loaded.Server;
    }


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



    [JsonModel]
    public sealed partial class GeneralSection
    {
        public string Theme   { get; set; } = "Dark";
        public bool   Enabled { get; set; }
    }



    [JsonModel]
    public sealed partial class ServerSection
    {
        public string Host { get; set; } = "localhost";
        public int    Port { get; set; } = 80;
    }
}



/// <summary> A small [JsonModel] type for the value-helper tests. </summary>
[JsonModel]
public sealed partial record Point( int X, int Y );
