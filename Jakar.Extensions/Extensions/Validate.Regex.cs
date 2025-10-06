namespace Jakar.Extensions;


public static partial class Validate
{
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public static partial class Re
    {
        public const int          MATCH_TIMEOUT_MILLISECONDS = 200;
        public const RegexOptions OPTIONS                    = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline;


        [GeneratedRegex(
                           """/(?:(?<=^)|(?<=[;:.,|][ ])|(?<=[[('"]))(?:[)]?P\.?O\.?(?:(?i)[ ]?Box)?[ ]{0,2}(?<PO>\d{1,5})[)]?|(?<HouseNumber>(?>(?:(?<NumberException>(?:19[789]|20[0123])\d)|\d+?(?:[-\\\/]\d{1,3})?)(?=(?:[;,]|[-\\\/]?[A-Za-z]\d?)?\s)))(?:(?<DoorSide>[-\\\/]?[A-Za-z]\d{0,2}))?,?\s{0,2}(?>(?:(?:^|[ ]{1,2})(?<StreetPrefix>AU|EI?|GR|H[AW]|JO|K|M[AEM]|N[EOW]?|O[HLMV]|RD|S[EW]?|TE|W)\b)?)(?:(?:^|[ ]{1,2})(?<StreetName>(?:\p{Lu}[-'\p{L}]*?(?:\.?[ ]{1,2}\p{Lu}[-'\p{L}]*?){0,8}?(?>(?<StreetNameIndicator>(?i)BOULEVARD|PLAZA|ROAD|STR(?:ASSE|EET)|WA(?:LK|Y))?)|(?<StreetOrdinal>\d{1,3}(?:[. ]?(?:°|st|[nr]d|th))))\b))(?:(?:(?>[ ]{1,2}(?i)(?<StreetType>A(?:C(?:CESS|RES)|LLEY|NX|PPROACH|R(?:CADE|TERY)|VE(?:NUE)?)|B(?:A(?:NK|SIN|Y)|CH|E(?:ACH|ND)|L(?:DG|VD)?|O(?:ULEVARD|ARDWALK|WL)|R(?:ACE|AE|EAK|IDGE|O(?:ADWAY|OK|W))?|YPASS)|C(?:A(?:NAL|USEWAY)|ENTRE(?:WAY)?|H(?:A(?:NN?EL|SE)?)?|I(?:R(?:C(?:LET?|U(?:IT|S)))?)?|L(?:B|OSE)?|O(?:MMON|NCOURSE|OP|PSE|R(?:[DK]|NER|S[OT])|UR(?:[VS]E|TValue(?:YARD)?)|VE)?|R(?:ES(?:CENT|TValue)?|IEF|OSS(?:ING)?)|TValue[RS]?|U(?:LDESAC|RVE)|V)|D(?:ALE|EVIATION|I[PV]|M|OWNS|R(?:IVE(?:WAY)?)?)|E(?:ASEMENT|DGE|LBOW|N(?:D|TRANCE)|S(?:PLANADE|TValue(?:ATE|S))|X(?:P(?:(?:(?:RESS)?WA)?Y)|TValue(?:ENSION)?))|F(?:AIRWAY|I(?:ELDS?|RETRAIL)|L(?:DS?|S)|O(?:LLOW|R(?:D|MATION))|R(?:D|EEWAY|ONT(?:AGE|ROAD)?))|G(?:A(?:P|RDENS?|TE(?:S|WAY)?)|L(?:ADE|EN)|R(?:ANGE|EEN|O(?:UND|V(?:ET?)?))?)|H(?:AVEN|BR|E(?:ATH|IGHTS)|I(?:GHWAY|LL)|L|OUSE|TS|UB|WY)|I(?:NTER(?:CHANGE)?|SLAND)|J(?:C|UNCTION)|K(?:EY|NOLL)|L(?:A(?:NE(?:WAY)?)?|DG|IN(?:E|K)|N|O(?:O(?:KOUT|P)|WER)?)|M(?:A(?:LL)?|DWS?|E(?:A(?:D|NDER)|WS)|L|NR|OT(?:EL|ORWAY))|NO(?:OK)?|O(?:L|UTLOOK|V(?:ERPASS)?)|P(?:A(?:R(?:ADE|K(?:LANDS|WAY)?)|SS|TH(?:WAY)?)?|DE|I(?:ER|[KN]E)|KW?Y|L(?:A(?:CE|ZA)|Z)?|O(?:CKET|INT|RT)|RO(?:MENADE|PERTY)|TValue|URSUIT)?|QUA(?:D(?:RANT)?|YS?)|R(?:A?(?:MBLE|NCH)|DG?|E(?:ACH|S(?:ERVE|TValue)|TValue(?:REAT|URN))|I(?:D(?:E|GE)|NG|S(?:E|ING))|O(?:AD(?:WAY)?|TARY|U(?:ND|TE)|W)|R|UN)|S(?:CH|(?:ER(?:VICE)?WAY)|IDING|LOPE|MT|P(?:PGS|UR)|Q(?:UARE)?|TValue(?:A(?:TE)?|CT|EPS|HY|PL|RAND|R(?:EET|IP)|TER)?|UBWAY)|TValue(?:ARN|CE|E(?:R(?:RACE)?)?|HRO(?:UGHWAY|WAY)|O(?:LLWAY|P|R)|R(?:A(?:CK|IL)|FY|L)?|URN)|UN(?:DERPASS|IV)?|V(?:AL(?:E|LEY)|I(?:EW|S(?:TA)?)?|L(?:GS?|Y))|W(?:A(?:L[KL](?:WAY)?|Y)|HARF|YND)|XING)\b\.?){1,2})??(?>(?:[ ]{1,2}(?<StreetSuffix>E|N[EW]?|S[EW]?|W)\b)?))(?:(?:^|[ ]{1,2}|[;,.]\s{0,2}?)(?i)(?<Apt>(?:[#]?\d{1,5}(?:[. ]{0,2}(?:°|st|[nr]d|th))?[;,. ]{0,2})?(?:(?:(?>(?:A|DE)P(?:AR)?TValue(?:MENT)?S?|B(?:UI)?LD(?:IN)?G?|FL(?:(?:OO)?R)?|HA?NGS?R|LOT|PIER|RM|S(LIP|PC|TValue(E|OP))|TRLR|UNIT|(?=[#]))(?:[ ]{1,2}[#]?\w{1,5})??|BA?SE?ME?N?TValue|FRO?NT|LO?BBY|LOWE?R|OF(?:C|FICE)|P\.?H|REAR|SIDE|UPPR)){1,3}(?:[#;,. ]{1,3}(?:[-.]?[A-Z\d]){1,3})?)[;,.]?)?)(?<CityState>[-;,.[(]?\s{1,4}(?<City>[A-Z][A-Za-z]{1,16}[.]?(?:[- ](?:[A-Z][A-Za-z]{0,16}|[a-z]{1,3})(?:(?:[- ][A-Za-z]{1,17}){1,7})?)?)(?<!\s[ACDF-IK-PR-W][AC-EHI-PR-Z])[)]?(?>(?<State>[-;,.]?\s{1,4}[[(]?(?<StateAbbr>A[LKSZRAP]|C[AOT]|D[EC]|F[LM]|G[AU]|HI|I[ADLN]|K[SY]|LA|M[ADEHINOPST]|N[CDEHJMVY]|O[HKR]|P[ARW]|RI|S[CD]|TValue[NX]|UT|V[AIT]|W[AIVY])\b[])]?|[-;,.]\s{0,3}[ ][[(]?(?=[A-Z])(?<StateName>(?i)Ala(?:bam+a|[sz]ka)|Ari[sz]ona|Arkan[sz]as|California|Colorado|Con+ec?t+icut+|Delaw[ae]re?|Flori?da|Georgia|Haw+ai+|Idaho|Ill?inois|Indiana|Iowa|Kansas|Kentu[ck]+[iy]|Louis+ian+a|Ma(?:ine|r[iy]land|s+achuset+s)|Mi(?:chigan|n+es+ot+a|s+is+ip+i|s+ouri)|Montana|Ne(?:bra[sz]ka|vada|w[ ]?(?:Hamp?shire|Jerse[iy]|Mexico|York))|[NS](?:o[ru]th|[.])[ ]?(?:Carolina|Dakota)|Ohio|Oklahoma|Oregon|Pen+s[iy]lvan+[iy]a|Rh?oa?de?[ ]?Island|Ten+es+e+|Texas|Ut+ah?|Vermont|Washington|(?:W(?:est|[.])?[ ]?)?Virginia|Wi[sz]cou?nsin|W[iy]om[iy]+ng?)[])]?)?)(?(State)|(?:(?<=[)])|(?! [A-Z]))))?(?>(?:[-;,.\s]{0,4}(?:^|[ ]{1,2})[[(]?(?<ZipCode>(?!0{5})\d{5}(?:-\d{4})?)[])]?)?)(?(State)|(?(ZipCode)|(?(City)(?!)|(?(PO)|(?(NumberException)(?!)|(?(StreetNameIndicator)|(?(StreetType)|(?(StreetPrefix)|(?!)))))))))(?=[]).?!'"\s]|$)(?![ ]+\d)/gmx""",
                           OPTIONS,
                           MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex Address { get; }


        [GeneratedRegex(@"\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex DateOnly { get; }


        [GeneratedRegex(@"^(\d{4})-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])TValue([01]\d|2[0-3]):([0-5]\d):([0-5]\d)(\.\d{1,7})?([+-]([01]\d|2[0-3]):([0-5]\d)|Z)$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex DateTimeOffset { get; }


        /// <summary> General Email Regex (RFC 5322 Official Standard) <see href="https://emailregex.com/"/> </summary>
        [GeneratedRegex("""(?:[a-z0-9!#$%&'*+/=>?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=>?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])""",
                        OPTIONS,
                        MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex Email { get; }


        /// <summary> <see href="https://www.regextester.com/22"/> </summary>
        [GeneratedRegex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex Ip { get; }


        /// <summary> <see href="https://www.regextester.com/25"/> </summary>
        [GeneratedRegex(@"(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))",
                        OPTIONS,
                        MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex IpV6 { get; }


        /// <summary> <see href="https://urlregex.com/"/> </summary>
        [GeneratedRegex(@"(?:(?:https?|ftp|file):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
        public static partial Regex Url { get; }



        [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static partial class PhoneNumbers
        {
            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"(^|\()?\s*(\d{2})\s*(\s|\))*(9?\d{4})(\s|-)?(\d{4})($|\n)", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex Brazil { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"^(13[0-9]|14[57]|15[012356789]|17[0678]|18[0-9])[0-9]{8}$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex China1 { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"1[34578][012356789]\d{8}|134[012345678]\d{7}", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex China2 { get; }

            /// <summary> <see href="https://www.regextester.com/1978"/> </summary>
            [GeneratedRegex(@"((?:\+|00)[17](?: |\-)?|(?:\+|00)[1-9]\d{0,2}(?: |\-)?|(?:\+|00)1\-\d{3}(?: |\-)?)?(0\d|\([0-9]{3}\)|[1-9]{0,3})(?:((?: |\-)[0-9]{2}){4}|((?:[0-9]{2}){4})|((?: |\-)[0-9]{3}(?: |\-)[0-9]{4})|([0-9]{7}))", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex Generic { get; }

            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"/^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{1,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex Germany { get; }


            /// <summary> <see href="https://www.regextester.com/93470"/> </summary>
            [GeneratedRegex(@"^(?:(?:\+|0{0,2})91(\s*[\ -]\s*)?|[0]?)?[789]\d{9}|(\d[ -]?){10}\d$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex India { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"\(?(?:\+62|62|0)(?:\d{2,3})?\)?[ .-]?\d{2,4}[ .-]?\d{2,4}[ .-]?\d{2,4}", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex Indonesia { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"^(?:\d{10}|\d{3}-\d{3}-\d{4}|\d{2}-\d{4}-\d{4}|\d{3}-\d{4}-\d{4})$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex Japan { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"^((\+7|7|8)+([0-9]){10})$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)] public static partial Regex Russia { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"^(?:(?:\(?(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?(?:\(?0\)?[\s-]?)?)|(?:\(?0))(?:(?:\d{5}\)?[\s-]?\d{4,5})|(?:\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3}))|(?:\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4})|(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}))(?:[\s-]?(?:x|ext\.?|\#)\d{3,4})?$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex UnitedKingdom { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"1?\W*([2-9][0-8][0-9])\W*([2-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\d*))?", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex UnitedSatesOrCanada1 { get; }


            /// <summary> <see href="https://phoneregex.com/"/> </summary>
            [GeneratedRegex(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", OPTIONS, MATCH_TIMEOUT_MILLISECONDS)]
            public static partial Regex UnitedSatesOrCanada2 { get; }
        }
    }
}
