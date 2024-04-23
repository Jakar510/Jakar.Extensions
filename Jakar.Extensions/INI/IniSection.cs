using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "MemberHidesStaticFromOuterClass" )]
public partial class IniConfig
{
    internal const string CLOSE  = " ]";
    internal const string EQUALS = " = ";
    internal const string OPEN   = "[ ";



    public sealed class Section : ConcurrentDictionary<string, string?>
    {
        public int Length
        {
            get
            {
                int title  = Name.Length + OPEN.Length + CLOSE.Length;
                int keys   = (Longest + EQUALS.Length) * Keys.Count;
                int values = Values.Sum( x => x?.Length ?? 0 );
                int result = title + keys + values + Keys.Count;
                return result;
            }
        }
        internal int    Longest => Keys.Max( item => item.Length );
        public   string Name    { get; }


        public Section( string sectionName ) : this( sectionName, StringComparer.OrdinalIgnoreCase ) { }
        public Section( string sectionName, IEqualityComparer<string>                  comparer ) : base( comparer ) => Name = sectionName;
        public Section( string sectionName, IDictionary<string, string?>               dictionary ) : this( sectionName, dictionary, StringComparer.OrdinalIgnoreCase ) { }
        public Section( string sectionName, IDictionary<string, string?>               dictionary, IEqualityComparer<string> comparer ) : base( dictionary, comparer ) => Name = sectionName;
        public Section( string sectionName, IEnumerable<KeyValuePair<string, string?>> collection ) : this( sectionName, collection, StringComparer.OrdinalIgnoreCase ) { }
        public Section( string sectionName, IEnumerable<KeyValuePair<string, string?>> collection, IEqualityComparer<string> comparer ) : base( collection, comparer ) => Name = sectionName;

        public static implicit operator Section( string sectionName ) => new(sectionName);


        public override string ToString() => ToString( default, CultureInfo.CurrentCulture );
        public string ToString( string? format, IFormatProvider? formatProvider )
        {
            Span<char> span = stackalloc char[Length + 1];

            if ( TryFormat( span, out int charsWritten, format, formatProvider ) ) { return span[..charsWritten].ToString(); }

            throw new InvalidOperationException( "Cannot convert to string" );
        }


    #if NET6_0_OR_GREATER
        [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    #endif
        public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
        {
            Debug.Assert( destination.Length > Length );
            charsWritten = 0;
            int longest = Longest;

            foreach ( char t in OPEN ) { destination[charsWritten++] = t; }

            foreach ( char t in Name ) { destination[charsWritten++] = t; }

            foreach ( char t in CLOSE ) { destination[charsWritten++] = t; }

            destination[charsWritten++] = '\n';


            foreach ( (string sKey, string? sValue) in this )
            {
                ReadOnlySpan<char> sectionValue = sValue;

                foreach ( char c in sKey.PadRight( longest ) ) { destination[charsWritten++] = c; }

                foreach ( char c in EQUALS ) { destination[charsWritten++] = c; }

                foreach ( char c in sectionValue ) { destination[charsWritten++] = c; }

                destination[charsWritten++] = '\n';
            }

            return true;
        }



        #region Gets

        public bool ValueAs<T>( string key, [NotNullWhen( true )] out T? value )
            where T : notnull
        {
            string? s = this[key];

            value = string.IsNullOrEmpty( s )
                        ? default
                        : s.FromJson<T>();

            return value is not null;
        }
        public bool ValueAs<T>( string key, [NotNullWhen( true )] out T[]? value )
        {
            value = this[key]?.FromJson<T[]>();

            return value is not null;
        }


        public bool ValueAs( string key, [NotNullWhen( true )] out string[]? value )
        {
            value = this[key]?.FromJson<string[]>();

            return value is not null;
        }
        public bool ValueAs( string key, [NotNullWhen( true )] out string[]? value, char separator )
        {
            string? s = this[key];

            if ( !string.IsNullOrEmpty( s ) && s.Contains( separator ) )
            {
                value = s.Split( separator );
                return true;
            }

            value = default;
            return false;
        }
        public bool ValueAs( string key, [NotNullWhen( true )] out string[]? value, string separator )
        {
            string? s = this[key];

            if ( !string.IsNullOrEmpty( s ) && s.Contains( separator ) )
            {
                value = s.Split( separator );
                return true;
            }

            value = default;
            return false;
        }

        public bool ValueAs( string key, out                       double      value )                   => double.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       float       value )                   => float.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       long        value )                   => long.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       ulong       value )                   => ulong.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       int         value )                   => int.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       uint        value )                   => uint.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       short       value )                   => short.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       ushort      value )                   => ushort.TryParse( this[key], out value );
        public bool ValueAs( string key, [NotNullWhen( true )] out IPAddress?  value )                   => IPAddress.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       TimeSpan    value )                   => TimeSpan.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       TimeSpan    value, CultureInfo info ) => TimeSpan.TryParse( this[key], info, out value );
        public bool ValueAs( string key, out                       DateTime    value )                                          => DateTime.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       DateTime    value, CultureInfo info, DateTimeStyles styles ) => DateTime.TryParse( this[key], info, styles, out value );
        public bool ValueAs( string key, out                       Guid        value ) => Guid.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       bool        value ) => bool.TryParse( this[key], out value );
        public bool ValueAs( string key, [NotNullWhen( true )] out AppVersion? value ) => AppVersion.TryParse( this[key], out value );
        public bool ValueAs( string key, [NotNullWhen( true )] out Version?    value ) => Version.TryParse( this[key], out value );

        #endregion



        #region Adds

        public void Add<T>( string key, T value )
            where T : notnull => this[key] = value.ToJson();
        public void Add<T>( string key, IEnumerable<T>      values )                   => this[key] = values.ToJson();
        public void Add( string    key, IEnumerable<string> values )                   => this[key] = values.ToJson();
        public void Add( string    key, IEnumerable<string> values, char   separator ) => this[key] = string.Join( separator, values );
        public void Add( string    key, IEnumerable<string> values, string separator ) => this[key] = string.Join( separator, values );

        public void Add( string key, double         value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, float          value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, long           value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, ulong          value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, int            value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, uint           value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, short          value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, ushort         value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, TimeSpan       value ) => this[key] = value.ToString();
        public void Add( string key, DateTime       value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, DateTimeOffset value ) => this[key] = value.ToString( CultureInfo.CurrentCulture );
        public void Add( string key, IPAddress      value ) => this[key] = value.ToString();
        public void Add( string key, Guid           value ) => this[key] = value.ToString();
        public void Add( string key, bool           value ) => this[key] = value.ToString();
        public void Add( string key, AppVersion value )
        {
            string version = value.ToString();
            this[key] = version;

            version.WriteToDebug();

            string.Empty.WriteToConsole();

            this.ToPrettyJson().WriteToDebug();

            string.Empty.WriteToConsole();
        }
        public void Add( string key, Version value ) => this[key] = value.ToString();
        public void Add( string key, string? value ) => this[key] = value;


        public void Add( string key, TimeSpan value, string? format, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString()
                            : value.ToString( format, culture ?? CultureInfo.CurrentCulture );

        public void Add( string key, DateTime value, string? format, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString( culture         ?? CultureInfo.CurrentCulture )
                            : value.ToString( format, culture ?? CultureInfo.CurrentCulture );

        public void Add( string key, DateTimeOffset value, string? format, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString( culture         ?? CultureInfo.CurrentCulture )
                            : value.ToString( format, culture ?? CultureInfo.CurrentCulture );

        #endregion
    }
}
