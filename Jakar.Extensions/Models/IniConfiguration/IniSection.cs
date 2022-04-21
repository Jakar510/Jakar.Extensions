namespace Jakar.Extensions.Models.IniConfiguration;


public partial class IniConfig
{
    public class Section : ConcurrentDictionary<string, string>
    {
        public Section() : this(StringComparer.OrdinalIgnoreCase) { }
        public Section( IEqualityComparer<string>                 comparer ) : base(comparer) { }
        public Section( IDictionary<string, string>               dictionary ) : this(dictionary, StringComparer.OrdinalIgnoreCase) { }
        public Section( IDictionary<string, string>               dictionary, IEqualityComparer<string> comparer ) : base(dictionary, comparer) { }
        public Section( IEnumerable<KeyValuePair<string, string>> collection ) : this(collection, StringComparer.OrdinalIgnoreCase) { }
        public Section( IEnumerable<KeyValuePair<string, string>> collection, IEqualityComparer<string> comparer ) : base(collection, comparer) { }



        #region Gets

        public bool ValueAs( in string key, [NotNullWhen(true)] out IEnumerable<string>? value, in char separator )
        {
            string s = this[key];

            if ( s.Contains(separator) )
            {
                value = s.Split(separator);
                return true;
            }

            value = default;
            return false;
        }
        public bool ValueAs( in string key, [NotNullWhen(true)] out IEnumerable<string>? value, in string separator )
        {
            string s = this[key];

            if ( s.Contains(separator) )
            {
                value = s.Split(separator);
                return true;
            }

            value = default;
            return false;
        }

        public bool ValueAs( in string key, out double value )
        {
            string s = this[key];
            return double.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out float value )
        {
            string s = this[key];
            return float.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out long value )
        {
            string s = this[key];
            return long.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out ulong value )
        {
            string s = this[key];
            return ulong.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out int value )
        {
            string s = this[key];
            return int.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out uint value )
        {
            string s = this[key];
            return uint.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out short value )
        {
            string s = this[key];
            return short.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out ushort value )
        {
            string s = this[key];
            return ushort.TryParse(s, out value);
        }

        public bool ValueAs( in string key, [NotNullWhen(true)] out IPAddress? value )
        {
            string s = this[key];
            return IPAddress.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out TimeSpan value )
        {
            string s = this[key];
            return TimeSpan.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out TimeSpan value, CultureInfo info )
        {
            string s = this[key];
            return TimeSpan.TryParse(s, info, out value);
        }

        public bool ValueAs( in string key, out DateTime value )
        {
            string s = this[key];
            return DateTime.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out DateTime value, CultureInfo info, DateTimeStyles styles )
        {
            string s = this[key];
            return DateTime.TryParse(s, info, styles, out value);
        }

        public bool ValueAs( in string key, out Guid value )
        {
            string s = this[key];
            return Guid.TryParse(s, out value);
        }

        public bool ValueAs( in string key, out bool value )
        {
            string s = this[key];
            return bool.TryParse(s, out value);
        }

        public bool ValueAs( in string key, [NotNullWhen(true)] out AppVersion? value )
        {
            string s = this[key];
            return AppVersion.TryParse(s, out value);
        }

        public bool ValueAs( in string key, [NotNullWhen(true)] out Version? value )
        {
            string s = this[key];
            return Version.TryParse(s, out value);
        }

        #endregion



        #region Adds

        public void Add( in string key, in IEnumerable<string> values, string separator = "," ) => this[key] = string.Join(separator, values);

        public void Add( in string key, in double     value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in float      value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in long       value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in ulong      value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in int        value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in uint       value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in short      value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in ushort     value ) => this[key] = value.ToString(CultureInfo.InvariantCulture);
        public void Add( in string key, in IPAddress  value ) => this[key] = value.ToString();
        public void Add( in string key, in Guid       value ) => this[key] = value.ToString();
        public void Add( in string key, in bool       value ) => this[key] = value.ToString();
        public void Add( in string key, in AppVersion value ) => this[key] = value.ToString();
        public void Add( in string key, in Version    value ) => this[key] = value.ToString();
        public void Add( in string key, in string     value ) => this[key] = value;


        public void Add( in string key, in TimeSpan value, string? format = default, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString()
                            : value.ToString(format, culture ?? CultureInfo.InvariantCulture);

        public void Add( in string key, in DateTime value, string? format = default, CultureInfo? culture = default ) =>
            this[key] = format is null
                            ? value.ToString(culture ?? CultureInfo.InvariantCulture)
                            : value.ToString(format, culture ?? CultureInfo.InvariantCulture);

        #endregion
    }
}
