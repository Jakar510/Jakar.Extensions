#nullable enable
namespace Jakar.Extensions;


public static partial class Validate
{
    public static class Re
    {
        private static readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(200);
        private static          Regex?   _email;
        private static          Regex?   _url;
        private static          Regex?   _ip;


        /// <summary>
        ///     General Email Regex (RFC 5322 Official Standard)
        ///     <para>
        ///         <see href = "https://emailregex.com/" />
        ///     </para>
        /// </summary>
        public static Regex Email =>
            _email ??=
                new
                    Regex(@"(?:[a-z0-9!#$%&'*+/=>?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=>?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
                          RegexOptions.Compiled,
                          _timeout);

        /// <summary>
        /// 
        ///     <para>
        ///         <see href = "https://urlregex.com/" />
        ///     </para>
        /// </summary>
        public static Regex Url => _url ??= new Regex(@"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$", RegexOptions.Compiled, _timeout);

        /// <summary>
        ///     <para>
        ///         <see href = "https://ipregex.com/" />
        ///     </para>
        /// </summary>
        public static Regex Ip => _ip ??= new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.Compiled, _timeout);



        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static class PhoneNumbers
        {
            private static Regex? _generic;
            private static Regex? _usCanada1;
            private static Regex? _usCanada2;
            private static Regex? _uk;
            private static Regex? _germany;
            private static Regex? _china1;
            private static Regex? _china2;
            private static Regex? _russia;
            private static Regex? _japan;
            private static Regex? _indiaOld;
            private static Regex? _indiaNew;
            private static Regex? _indonesia;
            private static Regex? _brazil;


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex Generic => _generic ??= new Regex(@"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$",
                                                                  RegexOptions.Compiled,
                                                                  _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex UnitedSatesOrCanada1 => _usCanada1 ??= new Regex(@"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?", RegexOptions.Compiled, _timeout);

            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex UnitedSatesOrCanada2 =>
                _usCanada2 ??=
                    new
                        Regex(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$",
                              RegexOptions.Compiled,
                              _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex UnitedKingdom =>
                _uk ??=
                    new
                        Regex(@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$",
                              RegexOptions.Compiled,
                              _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex Germany => _germany ??= new Regex(@"/^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{1,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/", RegexOptions.Compiled, _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex China1 => _china1 ??= new Regex(@"^(13[0-9]|14[57]|15[012356789]|17[0678]|18[0-9])[0-9]{8}$", RegexOptions.Compiled, _timeout);

            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex China2 => _china2 ??= new Regex(@"1[34578][012356789]\d{8}|134[012345678]\d{7}", RegexOptions.Compiled, _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex Russia => _russia ??= new Regex(@"^((\+7|7|8)+([0-9]){10})$", RegexOptions.Compiled, _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex Japan => _japan ??= new Regex(@"^(?:\d{10}|\d{3}-\d{3}-\d{4}|\d{2}-\d{4}-\d{4}|\d{3}-\d{4}-\d{4})$", RegexOptions.Compiled, _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex IndiaOld => _indiaOld ??= new Regex(@"((\+*)(0*|(0 )*|(0-)*|(91 )*)(\d{12}+|\d{10}+))|\d{5}([- ]*)\d{6}", RegexOptions.Compiled, _timeout);

            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex IndiaNew => _indiaNew ??= new Regex(@"((\+*)((0[ -]+)*|(91 )*)(\d{12}+|\d{10}+))|\d{5}([- ]*)\d{6}", RegexOptions.Compiled, _timeout);


            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex Indonesia => _indonesia ??= new Regex(@"\(?(?:\+62|62|0)(?:\d{2,3})?\)?[ .-]?\d{2,4}[ .-]?\d{2,4}[ .-]?\d{2,4}", RegexOptions.Compiled, _timeout);

            /// <summary>
            ///     <para>
            ///         <see href = "https://phoneregex.com/" />
            ///     </para>
            /// </summary>
            public static Regex Brazil => _brazil ??= new Regex(@"(^|\()?\s*(\d{2})\s*(\s|\))*(9?\d{4})(\s|-)?(\d{4})($|\n)", RegexOptions.Compiled, _timeout);
        }
    }
}
