namespace Jakar.Extensions;


public static partial class Validate
{
    [SuppressMessage( "ReSharper", "PartialTypeWithSinglePart" )]
    public static partial class Re
    {
        public const           RegexOptions OPTIONS = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;
        public static readonly TimeSpan     Timeout = TimeSpan.FromMilliseconds( 200 );


        [GeneratedRegex( @"\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$", OPTIONS, 200 )] public static partial Regex DateOnly { get; }


        [GeneratedRegex( @"^(\d{4})-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])T([01]\d|2[0-3]):([0-5]\d):([0-5]\d)(\.\d{1,7})?([+-]([01]\d|2[0-3]):([0-5]\d)|Z)$", OPTIONS, 200 )]
        public static partial Regex DateTimeOffset { get; }


        /// <summary>
        ///     General Email Regex (RFC 5322 Official Standard)
        ///     <para>
        ///         <see href="https://emailregex.com/"/>
        ///     </para>
        /// </summary>
        [GeneratedRegex( """(?:[a-z0-9!#$%&'*+/=>?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=>?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])""", OPTIONS, 200 )]
        public static partial Regex Email { get; }


        /// <summary>
        ///     <para>
        ///         <see href="https://www.regextester.com/22"/>
        ///     </para>
        /// </summary>
        [GeneratedRegex( @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", OPTIONS, 200 )]
        public static partial Regex Ip { get; }


        /// <summary>
        ///     <para>
        ///         <see href="https://www.regextester.com/25"/>
        ///     </para>
        /// </summary>
        [GeneratedRegex( @"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))", OPTIONS, 200 )]
        public static partial Regex IpV6 { get; }


        /// <summary>
        ///     <para>
        ///         <see href="https://urlregex.com/"/>
        ///     </para>
        /// </summary>
        [GeneratedRegex( @"(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])", OPTIONS, 200 )]
        public static partial Regex Url { get; }



        [SuppressMessage( "ReSharper", "PartialTypeWithSinglePart" ), SuppressMessage( "ReSharper", "InconsistentNaming" )]
        public static partial class PhoneNumbers
        {
            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"(^|\()?\s*(\d{2})\s*(\s|\))*(9?\d{4})(\s|-)?(\d{4})($|\n)", OPTIONS, 200 )]
            public static partial Regex Brazil { get; }


            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"^(13[0-9]|14[57]|15[012356789]|17[0678]|18[0-9])[0-9]{8}$", OPTIONS, 200 )]
            public static partial Regex China1 { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"1[34578][012356789]\d{8}|134[012345678]\d{7}", OPTIONS, 200 )]
            public static partial Regex China2 { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://www.regextester.com/1978"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))", OPTIONS, 200 )]
            public static partial Regex Generic { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"/^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{1,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/", OPTIONS, 200 )]
            public static partial Regex Germany { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://www.regextester.com/93470"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$", OPTIONS, 200 )]
            public static partial Regex India { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"\(?(?:\+62|62|0)(?:\d{2,3})?\)?[ .-]?\d{2,4}[ .-]?\d{2,4}[ .-]?\d{2,4}", OPTIONS, 200 )]
            public static partial Regex Indonesia { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"^(?:\d{10}|\d{3}-\d{3}-\d{4}|\d{2}-\d{4}-\d{4}|\d{3}-\d{4}-\d{4})$", OPTIONS, 200 )]
            public static partial Regex Japan { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"^((\+7|7|8)+([0-9]){10})$", OPTIONS, 200 )]
            public static partial Regex Russia { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$", OPTIONS, 200 )]
            public static partial Regex UnitedKingdom { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?", OPTIONS, 200 )]
            public static partial Regex UnitedSatesOrCanada1 { get; }

            /// <summary>
            ///     <para>
            ///         <see href="https://phoneregex.com/"/>
            ///     </para>
            /// </summary>
            [GeneratedRegex( @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", OPTIONS, 200 )]
            public static partial Regex UnitedSatesOrCanada2 { get; }
        }
    }
}
