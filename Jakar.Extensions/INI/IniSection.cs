#nullable enable
using Org.BouncyCastle.Utilities;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "MemberHidesStaticFromOuterClass" )]
public partial class IniConfig
{
    internal const string OPEN   = "[ ";
    internal const string CLOSE  = " ]";
    internal const string EQUALS = " = ";



    public sealed class Section : ConcurrentDictionary<string, string?>
                              #if NET6_0_OR_GREATER
                                  ,
                                  ISpanFormattable
                              #endif
                              #if NET7_0_OR_GREATER
                                  ,
                                  ISpanParsable<Section>
#endif
    {
        internal int Longest => Keys.Max( item => item.Length );
        public int Length
        {
            get
            {
                int keys   = (Longest + EQUALS.Length) * Keys.Count;
                int values = Values.Sum( x => x?.Length ?? 0 );
                int result = keys + values + Keys.Count;
                return result * 2;
            }
        }


        public Section() : this( StringComparer.OrdinalIgnoreCase ) { }
        public Section( IEqualityComparer<string>                  comparer ) : base( comparer ) { }
        public Section( IDictionary<string, string?>               dictionary ) : this( dictionary, StringComparer.OrdinalIgnoreCase ) { }
        public Section( IDictionary<string, string?>               dictionary, IEqualityComparer<string> comparer ) : base( dictionary, comparer ) { }
        public Section( IEnumerable<KeyValuePair<string, string?>> collection ) : this( collection, StringComparer.OrdinalIgnoreCase ) { }
        public Section( IEnumerable<KeyValuePair<string, string?>> collection, IEqualityComparer<string> comparer ) : base( collection, comparer ) { }



        #region Gets

        public bool ValueAs<T>( string key, [NotNullWhen( true )] out T? value ) where T : notnull
        {
            string? s = this[key];

            value = string.IsNullOrEmpty( s )
                        ? default
                        : s.FromJson<T>();

            return value is not null;
        }
        public bool ValueAs<T>( string key, [NotNullWhen( true )] out T[]? value )
        {
            value = this[key]
              ?.FromJson<T[]>();

            return value is not null;
        }


        public bool ValueAs( string key, [NotNullWhen( true )] out string[]? value )
        {
            value = this[key]
              ?.FromJson<string[]>();

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

        public bool ValueAs( string key, out                       double      value ) => double.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       float       value ) => float.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       long        value ) => long.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       ulong       value ) => ulong.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       int         value ) => int.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       uint        value ) => uint.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       short       value ) => short.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       ushort      value ) => ushort.TryParse( this[key], out value );
        public bool ValueAs( string key, [NotNullWhen( true )] out IPAddress?  value ) => IPAddress.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       TimeSpan    value ) => TimeSpan.TryParse( this[key],                   out value );
        public bool ValueAs( string key, out                       TimeSpan    value, CultureInfo info ) => TimeSpan.TryParse( this[key], info, out value );
        public bool ValueAs( string key, out                       DateTime    value ) => DateTime.TryParse( this[key],                                          out value );
        public bool ValueAs( string key, out                       DateTime    value, CultureInfo info, DateTimeStyles styles ) => DateTime.TryParse( this[key], info, styles, out value );
        public bool ValueAs( string key, out                       Guid        value ) => Guid.TryParse( this[key], out value );
        public bool ValueAs( string key, out                       bool        value ) => bool.TryParse( this[key], out value );
        public bool ValueAs( string key, [NotNullWhen( true )] out AppVersion? value ) => AppVersion.TryParse( this[key], out value );
        public bool ValueAs( string key, [NotNullWhen( true )] out Version?    value ) => Version.TryParse( this[key], out value );

        #endregion



        #region Adds

        public void Add<T>( string key, T                   value ) where T : notnull => this[key] = value?.ToJson();
        public void Add<T>( string key, IEnumerable<T>      values ) => this[key] = values.ToJson();
        public void Add( string    key, IEnumerable<string> values ) => this[key] = values.ToJson();
        public void Add( string    key, IEnumerable<string> values, char   separator ) => this[key] = string.Join( separator, values );
        public void Add( string    key, IEnumerable<string> values, string separator ) => this[key] = string.Join( separator, values );

        public void Add( string key, double    value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, float     value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, long      value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, ulong     value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, int       value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, uint      value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, short     value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, ushort    value ) => this[key] = value.ToString( CultureInfo.InvariantCulture );
        public void Add( string key, IPAddress value ) => this[key] = value.ToString();
        public void Add( string key, Guid      value ) => this[key] = value.ToString();
        public void Add( string key, bool      value ) => this[key] = value.ToString();
        public void Add( string key, AppVersion value )
        {
            string version = value.ToString();
            this[key] = version;

            version.WriteToDebug();

            string.Empty.WriteToConsole();

            this.ToPrettyJson()
                .WriteToDebug();

            string.Empty.WriteToConsole();
        }
        public void Add( string key, Version value ) => this[key] = value.ToString();
        public void Add( string key, string  value ) => this[key] = value;


        public void Add( string key, TimeSpan value, string? format = default, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString()
                            : value.ToString( format, culture ?? CultureInfo.InvariantCulture );

        public void Add( string key, DateTime value, string? format = default, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString( culture ?? CultureInfo.InvariantCulture )
                            : value.ToString( format, culture ?? CultureInfo.InvariantCulture );

        #endregion



        public override string ToString() => ToString( default, CultureInfo.InvariantCulture );
        public string ToString( string? format, IFormatProvider? formatProvider )
        {
            Span<char> span = stackalloc char[Length + 1];

            if ( TryFormat( span, out int charsWritten, format, formatProvider ) )
            {
                return span[..charsWritten]
                   .ToString();
            }

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

            foreach ( (string sKey, string? sValue) in this )
            {
                ReadOnlySpan<char> sectionValue = sValue;
                if ( sKey == nameof(AppVersion) ) { sectionValue.WriteToDebug(); }

                foreach ( char c in sKey.PadRight( longest ) ) { destination[charsWritten++] = c; }

                foreach ( char c in EQUALS ) { destination[charsWritten++] = c; }

                foreach ( char c in sectionValue ) { destination[charsWritten++] = c; }

                destination[charsWritten++] = '\n';
            }

            return true;
        }
        public static Section Parse( string s ) => Parse( s,                                     CultureInfo.InvariantCulture );
        public static Section Parse( string s, IFormatProvider? provider ) => Parse( s.AsSpan(), provider );
        public static bool TryParse( string? s, IFormatProvider? provider, [NotNullWhen( true )] out Section? result )
        {
            result = null;
            return false;
        }
        public static Section Parse( ReadOnlySpan<char> span, IFormatProvider? provider )
        {
            var result = new Section();


            return result;
        }
        public static bool TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, [NotNullWhen( true )] out Section? result )
        {
            result = null;
            return false;
        }
    }
}
