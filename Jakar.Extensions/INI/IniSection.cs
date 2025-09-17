namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
public partial class IniConfig
{
    internal const         string CLOSE   = " ]";
    internal const         string EQUALS  = " = ";
    internal const         string OPEN    = "[ ";
    public static readonly int    Padding = CLOSE.Length + OPEN.Length + EQUALS.Length;



    public sealed class Section( string sectionName ) : IReadOnlyDictionary<string, string?>
    {
        private readonly ConcurrentDictionary<string, string?>                                     __dictionary = new(Environment.ProcessorCount, DEFAULT_CAPACITY, StringComparer.OrdinalIgnoreCase);
        public           int                                                                       Length  => GetLength(out _);
        internal         int                                                                       Longest => __dictionary.Keys.Max(static item => item.Length);
        public           ConcurrentDictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> Lookup  => __dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
        public           string                                                                    Name    { get; } = sectionName;
        public           int                                                                       Count   => __dictionary.Count;
        public string? this[ string key ] { get => __dictionary[key]; set => __dictionary[key] = value; }
        public IEnumerable<string>  Keys   => __dictionary.Keys;
        public IEnumerable<string?> Values => __dictionary.Values;


        public static implicit operator Section( string sectionName ) => new(sectionName);


        public static Section Create( string sectionName ) => new(sectionName);


        public override string ToString() => ToString(null, CultureInfo.CurrentCulture);
        public string ToString( string? format, IFormatProvider? formatProvider )
        {
            Span<char> span = stackalloc char[Length + 1];
            if ( TryFormat(span, out int charsWritten, format, formatProvider) ) { return span[..charsWritten].ToString(); }

            throw new InvalidOperationException("Cannot convert to string");
        }
        private int GetLength( out int longest )
        {
            int count = __dictionary.Keys.Count;
            longest = Longest;
            int keys   = ( longest + EQUALS.Length ) * count;
            int values = __dictionary.Values.Sum(static x => x?.Length ?? 0);
            int result = Padding + keys + values + count;
            return result;
        }


        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
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

                foreach ( char c in EQUALS ) { destination[charsWritten++] = c; }

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

        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
        public bool ValueAs<TValue>( string key, [NotNullWhen(true)] out TValue? value )
        {
            string? s = this[key];

            value = string.IsNullOrEmpty(s)
                        ? default
                        : s.FromJson<TValue>();

            return value is not null;
        }
        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
        public bool ValueAs<TValue>( string key, [NotNullWhen(true)] out TValue[]? value )
        {
            value = this[key]?.FromJson<TValue[]>();

            return value is not null;
        }


        public bool ValueAsNumber<TNumber>( string key, [NotNullWhen(true)] out TNumber? value )
            where TNumber : INumber<TNumber> => TNumber.TryParse(this[key], null, out value);


        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
        public bool ValueAs( string key, [NotNullWhen(true)] out string[]? value )
        {
            value = this[key]?.FromJson<string[]>();

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

        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
        public void AddJson<TValue>( string key, TValue value )
            where TValue : class => this[key] = value.ToJson();
        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public void Add<TValue>( string key, params ReadOnlySpan<TValue> values ) => this[key] = values.ToJson();
        public void Add<TNumber>( string key, TNumber value )
            where TNumber : INumber<TNumber> => this[key] = value.ToString(null, CultureInfo.CurrentCulture);
        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public void Add<TValue>( string key, IEnumerable<TValue> values )                   => this[key] = values.ToJson();
        [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public void Add( string         key, IEnumerable<string> values )                   => this[key] = values.ToJson();
        public                                                                                                  void Add( string         key, IEnumerable<string> values, char   separator ) => this[key] = string.Join(separator, values);
        public                                                                                                  void Add( string         key, IEnumerable<string> values, string separator ) => this[key] = string.Join(separator, values);
        public                                                                                                  void Add( string         key, TimeSpan            value ) => this[key] = value.ToString();
        public                                                                                                  void Add( string         key, DateTime            value ) => this[key] = value.ToString(CultureInfo.CurrentCulture);
        public                                                                                                  void Add( string         key, DateTimeOffset      value ) => this[key] = value.ToString(CultureInfo.CurrentCulture);
        public                                                                                                  void Add( string         key, IPAddress           value ) => this[key] = value.ToString();
        public                                                                                                  void Add( string         key, Guid                value ) => this[key] = value.ToString();
        public                                                                                                  void Add( string         key, bool                value ) => this[key] = value.ToString();
        public                                                                                                  void Add( string         key, AppVersion          value ) => this[key] = value.ToString();
        public                                                                                                  void Add( string         key, Version             value ) => this[key] = value.ToString();
        public                                                                                                  void Add( string         key, string?             value ) => this[key] = value;


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
