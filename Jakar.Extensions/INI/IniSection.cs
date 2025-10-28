namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
public partial class IniConfig
{
    public static readonly int Padding = CLOSE.Length + OPEN.Length + EQUALS_SPACE.Length;



    public sealed class Section( string sectionName ) : IReadOnlyDictionary<string, string?>
    {
        private readonly ConcurrentDictionary<string, string?> __dictionary = new(Environment.ProcessorCount, DEFAULT_CAPACITY, StringComparer.OrdinalIgnoreCase);
        public           int                                   Count => __dictionary.Count;
        public string? this[ string key ] { get => __dictionary[key]; set => __dictionary[key] = value; }
        public   IEnumerable<string>                                                       Keys    => __dictionary.Keys;
        public   int                                                                       Length  => GetLength(out _);
        internal int                                                                       Longest => __dictionary.Keys.Max(static item => item.Length);
        public   ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> Lookup  => __dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
        public   string                                                                    Name    { get; } = sectionName;
        public   IEnumerable<string?>                                                      Values  => __dictionary.Values;


        public static implicit operator Section( string sectionName ) => new(sectionName);


        public static Section Create( string sectionName ) => new(sectionName);


        public override string ToString() => ToString(null, CultureInfo.CurrentCulture);
        public string ToString( string? format, IFormatProvider? formatProvider )
        {
            Span<char> span = stackalloc char[Length + 1];

            if ( TryFormat(span, out int charsWritten, format, formatProvider) )
            {
                return span[..charsWritten]
                   .ToString();
            }

            throw new InvalidOperationException("Cannot convert to string");
        }
        private int GetLength( out int longest )
        {
            int count = __dictionary.Keys.Count;
            longest = Longest;
            int keys   = ( longest + EQUALS_SPACE.Length ) * count;
            int values = __dictionary.Values.Sum(static x => x?.Length ?? 0);
            int result = Padding + keys + values + count;
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)] public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
        {
            int length = GetLength(out int longest);
            Debug.Assert(destination.Length > length);
            charsWritten = 0;

            OPEN.CopyTo(destination[charsWritten..]);
            charsWritten += OPEN.Length;

            Name.CopyTo(destination[charsWritten..]);
            charsWritten += Name.Length;

            CLOSE.CopyTo(destination[charsWritten..]);
            charsWritten += CLOSE.Length;

            destination[charsWritten++] = '\n';


            foreach ( ( string sKey, string? sValue ) in __dictionary )
            {
                ReadOnlySpan<char> sectionValue = sValue;

                foreach ( char c in sKey.PadRight(longest) ) { destination[charsWritten++] = c; }

                foreach ( char c in EQUALS_SPACE ) { destination[charsWritten++] = c; }

                foreach ( char c in sectionValue ) { destination[charsWritten++] = c; }

                destination[charsWritten++] = '\n';
            }

            return true;
        }


        public bool ContainsKey( string key )                    => __dictionary.ContainsKey(key);
        public bool TryGetValue( string key, out string? value ) => __dictionary.TryGetValue(key, out value);
        public bool TryRemove( string   key, out string? value ) => __dictionary.TryRemove(key, out value);
        public void Clear() => __dictionary.Clear();


        public IEnumerator<KeyValuePair<string, string?>> GetEnumerator() => __dictionary.GetEnumerator();
        IEnumerator IEnumerable.                          GetEnumerator() => GetEnumerator();



        #region Gets

        public bool ValueAs<TValue>( string key, [NotNullWhen(true)] out TValue? value )
            where TValue : IJsonModel<TValue>
        {
            string? s = this[key];

            value = string.IsNullOrEmpty(s)
                        ? default
                        : s.FromJson<TValue>();

            return value is not null;
        }
        public bool ValueAs<TValue>( string key, JsonTypeInfo<TValue[]> info, [NotNullWhen(true)] out TValue[]? value )
        {
            value = this[key]
              ?.FromJson(info);

            return value is not null;
        }


        public bool ValueAsNumber<TNumber>( string key, [NotNullWhen(true)] out TNumber? value )
            where TNumber : INumber<TNumber> => TNumber.TryParse(this[key], null, out value);


        public bool ValueAs( string key, [NotNullWhen(true)] out string[]? value )
        {
            value = this[key]
              ?.FromJson<string[]>(JakarExtensionsContext.Default.StringArray);

            return value is not null;
        }
        public bool ValueAs( string key, [NotNullWhen(true)] out string[]? value, char separator )
        {
            string? s = this[key];

            if ( !string.IsNullOrEmpty(s) && s.Contains(separator) )
            {
                value = s.Split(separator);
                return true;
            }

            value = null;
            return false;
        }
        public bool ValueAs( string key, [NotNullWhen(true)] out string[]? value, string separator )
        {
            string? s = this[key];

            if ( !string.IsNullOrEmpty(s) && s.Contains(separator) )
            {
                value = s.Split(separator);
                return true;
            }

            value = null;
            return false;
        }


        public bool ValueAs( string key, [NotNullWhen(true)] out IPAddress?  value )                   => IPAddress.TryParse(this[key], out value);
        public bool ValueAs( string key, out                     TimeSpan    value )                   => TimeSpan.TryParse(this[key], out value);
        public bool ValueAs( string key, out                     TimeSpan    value, CultureInfo info ) => TimeSpan.TryParse(this[key], info, out value);
        public bool ValueAs( string key, out                     DateTime    value )                                          => DateTime.TryParse(this[key], out value);
        public bool ValueAs( string key, out                     DateTime    value, CultureInfo info, DateTimeStyles styles ) => DateTime.TryParse(this[key], info, styles, out value);
        public bool ValueAs( string key, out                     Guid        value ) => Guid.TryParse(this[key], out value);
        public bool ValueAs( string key, out                     bool        value ) => bool.TryParse(this[key], out value);
        public bool ValueAs( string key, [NotNullWhen(true)] out AppVersion? value ) => AppVersion.TryParse(this[key], out value);
        public bool ValueAs( string key, [NotNullWhen(true)] out Version?    value ) => Version.TryParse(this[key], out value);

        #endregion



        #region Adds

        public void AddJson<TValue>( string key, TValue value )
            where TValue : class, IJsonModel<TValue> => this[key] = value.ToJson();
        public void AddJson<TValue>( string key, TValue value, JsonTypeInfo<TValue> info ) => this[key] = value.ToJson(info);
        public void Add<TValue>( string key, params ReadOnlySpan<TValue> values )
            where TValue : IJsonModel<TValue> => this[key] = values.ToJson();
        public void Add<TNumber>( string key, TNumber value )
            where TNumber : INumber<TNumber> => this[key] = value.ToString(null, CultureInfo.CurrentCulture);
        public void Add( string key, IEnumerable<string> values, char   separator ) => this[key] = string.Join(separator, values);
        public void Add( string key, IEnumerable<string> values, string separator ) => this[key] = string.Join(separator, values);
        public void Add( string key, TimeSpan            value ) => this[key] = value.ToString();
        public void Add( string key, DateTime            value ) => this[key] = value.ToString(CultureInfo.CurrentCulture);
        public void Add( string key, DateTimeOffset      value ) => this[key] = value.ToString(CultureInfo.CurrentCulture);
        public void Add( string key, IPAddress           value ) => this[key] = value.ToString();
        public void Add( string key, Guid                value ) => this[key] = value.ToString();
        public void Add( string key, bool                value ) => this[key] = value.ToString();
        public void Add( string key, AppVersion          value ) => this[key] = value.ToString();
        public void Add( string key, Version             value ) => this[key] = value.ToString();
        public void Add( string key, string?             value ) => this[key] = value;


        public void Add( string key, TimeSpan value, string? format, CultureInfo? culture = null ) =>
            this[key] = format is null
                            ? value.ToString()
                            : value.ToString(format, culture ?? CultureInfo.CurrentCulture);

        public void Add( string key, DateTime value, string? format, CultureInfo? culture = null ) =>
            this[key] = format is null
                            ? value.ToString(culture         ?? CultureInfo.CurrentCulture)
                            : value.ToString(format, culture ?? CultureInfo.CurrentCulture);

        public void Add( string key, DateTimeOffset value, string? format, CultureInfo? culture = null ) =>
            this[key] = format is null
                            ? value.ToString(culture         ?? CultureInfo.CurrentCulture)
                            : value.ToString(format, culture ?? CultureInfo.CurrentCulture);

        #endregion
    }
}
