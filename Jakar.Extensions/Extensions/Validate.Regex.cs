namespace Jakar.Extensions;


public static partial class Validate
{
    [SuppressMessage( "ReSharper", "PartialTypeWithSinglePart" )]
    public static partial class Re
    {
        private static          Regex?       _email;
        private static          Regex?       _ip;
        private static          Regex?       _ipv6;
        private static          Regex?       _url;
        private const           RegexOptions OPTIONS  = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;
        private static readonly TimeSpan     _timeout = TimeSpan.FromMilliseconds( 200 );


    #if NET7_0_OR_GREATER
        /// <summary>
        ///     General Email Regex (RFC 5322 Official Standard)
        ///     <para>
        ///         <see href="https://emailregex.com/"/>
        ///     </para>
        /// </summary>
        public static Regex Email => _email ??= GetEmail();


        /// <summary>
        ///     <para>
        ///         <see href="https://www.regextester.com/22"/>
        ///     </para>
        /// </summary>
        public static Regex Ip => _ip ??= GetIp();


        /// <summary>
        ///     <para>
        ///         <see href="https://www.regextester.com/25"/>
        ///     </para>
        /// </summary>
        public static Regex IpV6 => _ipv6 ??= GetIpV6();


        /// <summary>
        ///     <para>
        ///         <see href="https://urlregex.com/"/>
        ///     </para>
        /// </summary>
        public static Regex Url => _url ??= GetUrl();


        [GeneratedRegex( @"(?:[a-z0-9!#$%&'*+/=>?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=>?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
                         OPTIONS,
                         200 )]
        private static partial Regex GetEmail();


        [GeneratedRegex( @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", OPTIONS, 200 )]
        private static partial Regex GetIp();


        [GeneratedRegex( @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))",
                         OPTIONS,
                         200 )]
        private static partial Regex GetIpV6();

        [GeneratedRegex( @"(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])", OPTIONS, 200 )]
        private static partial Regex GetUrl();
    #else
        /// <summary>
        ///     General Email Regex (RFC 5322 Official Standard)
        ///     <para>
        ///         <see href="https://emailregex.com/"/>
        ///     </para>
        /// </summary>
        public static Regex Email =>
            _email ??=
                new
                    Regex( @"(?:[a-z0-9!#$%&'*+/=>?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=>?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
                           OPTIONS,
                           _timeout );

        /// <summary>
        ///     <para>
        ///         <see href="https://www.regextester.com/22"/>
        ///     </para>
        /// </summary>
        public static Regex Ip => _ip ??= new Regex( @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", OPTIONS, _timeout );

        /// <summary>
        ///     <para>
        ///         <see href="https://www.regextester.com/22"/>
        ///     </para>
        /// </summary>
        public static Regex IpV6 =>
            _ipv6 ??=
                new
                    Regex( @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))",
                           OPTIONS,
                           _timeout );

        /// <summary>
        ///     <para>
        ///         <see href="https://urlregex.com/"/>
        ///     </para>
        /// </summary>
        public static Regex Url => _url ??= new Regex( @"(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])", OPTIONS, _timeout );

    #endif



        [SuppressMessage( "ReSharper", "PartialTypeWithSinglePart" ), SuppressMessage( "ReSharper", "InconsistentNaming" )]
        public static partial class PhoneNumbers
        {
            private static Regex? _brazil;
            private static Regex? _china1;
            private static Regex? _china2;
            private static Regex? _generic;
            private static Regex? _germany;
            private static Regex? _india;
            private static Regex? _indonesia;
            private static Regex? _japan;
            private static Regex? _russia;
            private static Regex? _uk;
            private static Regex? _usCanada1;
            private static Regex? _usCanada2;


        #if NET7_0_OR_GREATER
            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Brazil => _brazil ??= GetBrazil();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex China1 => _china1 ??= GetChina1();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex China2 => _china2 ??= GetChina2();

            /// <summary>
            ///     <para>
            ///         <see href="https://www.regextester.com/1978"/>
            ///     </para>
            /// </summary>
            public static Regex Generic => _generic ??= GetGeneric();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Germany => _germany ??= GetGermany();

            /// <summary>
            ///     <para>
            ///         <see href="https://www.regextester.com/93470"/>
            ///     </para>
            /// </summary>
            public static Regex India => _india ??= GetIndia();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Indonesia => _indonesia ??= GetIndonesia();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Japan => _japan ??= GetJapan();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Russia => _russia ??= GetRussia();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex UnitedKingdom => _uk ??= GetUnitedKingdom();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex UnitedSatesOrCanada1 => _usCanada1 ??= GetUnitedSatesOrCanada1();

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex UnitedSatesOrCanada2 => _usCanada2 ??= GetUnitedSatesOrCanada2();


            [GeneratedRegex( @"(^|\()?\s*(\d{2})\s*(\s|\))*(9?\d{4})(\s|-)?(\d{4})($|\n)", OPTIONS, 200 )] private static partial Regex GetBrazil();

            [GeneratedRegex( @"^(13[0-9]|14[57]|15[012356789]|17[0678]|18[0-9])[0-9]{8}$", OPTIONS, 200 )] private static partial Regex GetChina1();

            [GeneratedRegex( @"1[34578][012356789]\d{8}|134[012345678]\d{7}", OPTIONS, 200 )] private static partial Regex GetChina2();


            [GeneratedRegex( @"((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))", OPTIONS, 200 )]
            private static partial Regex GetGeneric();


            [GeneratedRegex( @"/^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{1,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/", OPTIONS, 200 )]
            private static partial Regex GetGermany();


            [GeneratedRegex( @"^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$", OPTIONS, 200 )]
            private static partial Regex GetIndia();


            [GeneratedRegex( @"\(?(?:\+62|62|0)(?:\d{2,3})?\)?[ .-]?\d{2,4}[ .-]?\d{2,4}[ .-]?\d{2,4}", OPTIONS, 200 )]
            private static partial Regex GetIndonesia();


            [GeneratedRegex( @"^(?:\d{10}|\d{3}-\d{3}-\d{4}|\d{2}-\d{4}-\d{4}|\d{3}-\d{4}-\d{4})$", OPTIONS, 200 )]
            private static partial Regex GetJapan();


            [GeneratedRegex( @"^((\+7|7|8)+([0-9]){10})$", OPTIONS, 200 )] private static partial Regex GetRussia();


            [GeneratedRegex( @"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$", OPTIONS, 200 )]
            private static partial Regex GetUnitedKingdom();


            [GeneratedRegex( @"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?", OPTIONS, 200 )]
            private static partial Regex GetUnitedSatesOrCanada1();

            [GeneratedRegex( @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", OPTIONS, 200 )]
            private static partial Regex GetUnitedSatesOrCanada2();
        #else
            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Brazil => _brazil ??= new Regex( @"(^|\()?\s*(\d{2})\s*(\s|\))*(9?\d{4})(\s|-)?(\d{4})($|\n)", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex China1 => _china1 ??= new Regex( @"^(13[0-9]|14[57]|15[012356789]|17[0678]|18[0-9])[0-9]{8}$", OPTIONS, _timeout );

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex China2 => _china2 ??= new Regex( @"1[34578][012356789]\d{8}|134[012345678]\d{7}", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://www.regextester.com/1978"/>
            ///     </para>
            /// </summary>
            public static Regex Generic =>
                _generic ??= new Regex( @"((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Germany => _germany ??= new Regex( @"/^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{1,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/", OPTIONS, _timeout );

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex India => _india ??= new Regex( @"^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Indonesia => _indonesia ??= new Regex( @"\(?(?:\+62|62|0)(?:\d{2,3})?\)?[ .-]?\d{2,4}[ .-]?\d{2,4}[ .-]?\d{2,4}", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Japan => _japan ??= new Regex( @"^(?:\d{10}|\d{3}-\d{3}-\d{4}|\d{2}-\d{4}-\d{4}|\d{3}-\d{4}-\d{4})$", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex Russia => _russia ??= new Regex( @"^((\+7|7|8)+([0-9]){10})$", OPTIONS, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex UnitedKingdom =>
                _uk ??= new Regex( @"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$", RegexOptions.Compiled, _timeout );


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex UnitedSatesOrCanada1 => _usCanada1 ??= new Regex( @"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?", OPTIONS, _timeout );

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            public static Regex UnitedSatesOrCanada2 =>
                _usCanada2 ??= new Regex( @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", OPTIONS, _timeout );
        #endif
        }
    }
}
