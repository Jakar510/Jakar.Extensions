namespace Jakar.Extensions;


public static class MimeTypes
{
    public static readonly FrozenSet<MimeType> All = Enum.GetValues<MimeType>()
                                                         .ToFrozenSet();
    public static readonly FrozenDictionary<string, MimeType> ReverseNames          = All.ToFrozenDictionary(ToStringFast, SelectSelf);
    public static readonly FrozenDictionary<MimeType, string> Names                 = All.ToFrozenDictionary(SelectSelf,   ToStringFast);
    public static readonly FrozenDictionary<MimeType, string> ContentNames          = All.ToFrozenDictionary(SelectSelf,   ToContentType);
    public static readonly FrozenDictionary<MimeType, string> Extensions            = All.ToFrozenDictionary(SelectSelf,   ToExtension);
    public static readonly FrozenDictionary<MimeType, string> ExtensionsWithPeriods = All.ToFrozenDictionary(SelectSelf,   static x => $".{x.ToExtension()}");
    private static         int?                               __maxLength;


    public static int MaxLength => __maxLength ??= Names.Values.Max(static x => x.Length);

    private static TValue SelectSelf<TValue>( TValue v ) => v;


    /// <summary> Gets the <see cref="MimeType"/> of the provided extension <see cref="string"/> . </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="MimeType"/>
    /// </returns>
    public static MimeType FromExtension( this string mime ) => mime.AsSpan()
                                                                    .FromExtension();


    /// <summary> Gets the <see cref="MimeType"/> of the provided extension <see cref="string"/> . </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="MimeType"/>
    /// </returns>
    [SuppressMessage("ReSharper", "ReplaceSequenceEqualWithConstantPattern")] public static MimeType FromExtension( this ReadOnlySpan<char> mime )
    {
        Span<char> span = stackalloc char[mime.Length];
        mime.ToLowerInvariant(span);

        return span switch
               {
                   "text" or ".text"                                    => MimeType.PlainText,
                   "txt" or ".txt"                                      => MimeType.Text,
                   "html" or ".html" or "htm" or ".htm"                 => MimeType.Html,
                   "xml" or ".xml"                                      => MimeType.Xml,
                   "xaml" or ".xaml"                                    => MimeType.Xaml,
                   "rt" or ".rt"                                        => MimeType.RichText,
                   "css" or ".css"                                      => MimeType.Css,
                   "csv" or ".csv"                                      => MimeType.Csv,
                   "ics" or ".ics"                                      => MimeType.Calendar,
                   "ini" or ".ini"                                      => MimeType.Ini,
                   "cfg" or ".cfg"                                      => MimeType.Cfg,
                   "conf" or ".conf"                                    => MimeType.Config,
                   "b64" or ".b64"                                      => MimeType.Base64,
                   "soap" or ".soap"                                    => MimeType.Soap,
                   "stream" or ".stream"                                => MimeType.Stream,
                   "bin" or ".bin"                                      => MimeType.Binary,
                   "pdf" or ".pdf"                                      => MimeType.Pdf,
                   "rtf" or ".rtf"                                      => MimeType.Rtf,
                   "json" or ".json"                                    => MimeType.Json,
                   "xul" or ".xul"                                      => MimeType.Xul,
                   "js" or ".js"                                        => MimeType.JavaScript,
                   "vbs" or ".vbs"                                      => MimeType.Vbs,
                   "sds" or ".sds"                                      => MimeType.Sds,
                   "tds" or ".tds"                                      => MimeType.Tds,
                   "coa" or ".coa"                                      => MimeType.Coa,
                   "xmla" or ".xmla"                                    => MimeType.XmlApp,
                   "url" or ".url"                                      => MimeType.UrlEncodedContent,
                   "license" or ".license" or "licenses" or ".licenses" => MimeType.Licenses,
                   "dll" or ".dll"                                      => MimeType.Dll,
                   "zip" or ".zip"                                      => MimeType.Zip,
                   "7z" or ".7z"                                        => MimeType.SevenZip,
                   "bz" or ".bz"                                        => MimeType.Bzip,
                   "bz2" or ".bz2"                                      => MimeType.Bzip2,
                   "gz" or ".gz"                                        => MimeType.Gzip,
                   "tar.gz" or ".tar.gz"                                => MimeType.Tar,
                   "doc" or ".doc"                                      => MimeType.Doc,
                   "docx" or ".docx"                                    => MimeType.Docx,
                   "xls" or ".xls"                                      => MimeType.Xls,
                   "xlsx" or ".xlsx"                                    => MimeType.Xlsx,
                   "ppt" or ".ppt"                                      => MimeType.Ppt,
                   "pptx" or ".pptx"                                    => MimeType.Pptx,
                   "3gppa" or ".3gppa"                                  => MimeType.ThreeGppAudio,
                   "3gpp2a" or ".3gpp2a"                                => MimeType.ThreeGpp2Audio,
                   "aac" or ".aac"                                      => MimeType.Aac,
                   "mpega" or ".mpega"                                  => MimeType.MpegAudio,
                   "mp3" or ".mp3"                                      => MimeType.Mp3,
                   "weba" or ".weba"                                    => MimeType.Weba,
                   "wav" or ".wav"                                      => MimeType.Wav,
                   "3gpp" or ".3gpp"                                    => MimeType.ThreeGppVideo,
                   "3gpp2" or ".3gpp2"                                  => MimeType.ThreeGpp2Video,
                   "mp4" or ".mp4"                                      => MimeType.Mp4,
                   "mpeg" or ".mpeg"                                    => MimeType.MpegVideo,
                   "mpeg4" or ".mpeg4"                                  => MimeType.Mpeg4,
                   "webm" or ".webm"                                    => MimeType.Webm,
                   "h264" or ".h264"                                    => MimeType.H264,
                   "avi" or ".avi"                                      => MimeType.Avi,
                   "mov" or ".mov"                                      => MimeType.Mov,
                   "mpg" or ".mpg"                                      => MimeType.Mpg,
                   "ogg" or ".ogg"                                      => MimeType.Ogg,
                   "mkv" or ".mkv"                                      => MimeType.Mkv,
                   "gif" or ".gif"                                      => MimeType.Gif,
                   "tif" or ".tif"                                      => MimeType.Tiff,
                   "png" or ".png"                                      => MimeType.Png,
                   "jpeg" or ".jpeg"                                    => MimeType.Jpeg,
                   "jpg" or ".jpg"                                      => MimeType.Jpg,
                   "bmp" or ".bmp"                                      => MimeType.Bmp,
                   "webp" or ".webp"                                    => MimeType.Webp,
                   "ico" or ".ico"                                      => MimeType.Icon,
                   "svg" or ".svg"                                      => MimeType.Svg,
                   "ttf" or ".ttf"                                      => MimeType.TrueType,
                   "otf" or ".otf"                                      => MimeType.OpenType,
                   "fd" or ".fd"                                        => MimeType.FormData,
                   "dat" or ".dat"                                      => MimeType.Unknown,
                   "file"                                               => MimeType.NotSet,
                   _                                                    => MimeType.NotSet
               };
    }


    public static string ToStringFast( this MimeType mime ) => mime switch
                                                               {
                                                                   MimeType.NotSet            => nameof(MimeType.NotSet),
                                                                   MimeType.Unknown           => nameof(MimeType.Unknown),
                                                                   MimeType.Text              => nameof(MimeType.Text),
                                                                   MimeType.PlainText         => nameof(MimeType.PlainText),
                                                                   MimeType.Html              => nameof(MimeType.Html),
                                                                   MimeType.Xml               => nameof(MimeType.Xml),
                                                                   MimeType.Xaml              => nameof(MimeType.Xaml),
                                                                   MimeType.RichText          => nameof(MimeType.RichText),
                                                                   MimeType.Css               => nameof(MimeType.Css),
                                                                   MimeType.Csv               => nameof(MimeType.Csv),
                                                                   MimeType.Calendar          => nameof(MimeType.Calendar),
                                                                   MimeType.Ini               => nameof(MimeType.Ini),
                                                                   MimeType.Config            => nameof(MimeType.Config),
                                                                   MimeType.Cfg               => nameof(MimeType.Cfg),
                                                                   MimeType.UrlEncodedContent => nameof(MimeType.UrlEncodedContent),
                                                                   MimeType.Soap              => nameof(MimeType.Soap),
                                                                   MimeType.Binary            => nameof(MimeType.Binary),
                                                                   MimeType.Stream            => nameof(MimeType.Stream),
                                                                   MimeType.Rtf               => nameof(MimeType.Rtf),
                                                                   MimeType.Pdf               => nameof(MimeType.Pdf),
                                                                   MimeType.Json              => nameof(MimeType.Json),
                                                                   MimeType.XmlApp            => nameof(MimeType.XmlApp),
                                                                   MimeType.Xul               => nameof(MimeType.Xul),
                                                                   MimeType.JavaScript        => nameof(MimeType.JavaScript),
                                                                   MimeType.Vbs               => nameof(MimeType.Vbs),
                                                                   MimeType.Base64            => nameof(MimeType.Base64),
                                                                   MimeType.Sds               => nameof(MimeType.Sds),
                                                                   MimeType.Tds               => nameof(MimeType.Tds),
                                                                   MimeType.Coa               => nameof(MimeType.Coa),
                                                                   MimeType.Zip               => nameof(MimeType.Zip),
                                                                   MimeType.SevenZip          => nameof(MimeType.SevenZip),
                                                                   MimeType.Bzip              => nameof(MimeType.Bzip),
                                                                   MimeType.Bzip2             => nameof(MimeType.Bzip2),
                                                                   MimeType.Gzip              => nameof(MimeType.Gzip),
                                                                   MimeType.Tar               => nameof(MimeType.Tar),
                                                                   MimeType.Doc               => nameof(MimeType.Doc),
                                                                   MimeType.Docx              => nameof(MimeType.Docx),
                                                                   MimeType.Xls               => nameof(MimeType.Xls),
                                                                   MimeType.Xlsx              => nameof(MimeType.Xlsx),
                                                                   MimeType.Ppt               => nameof(MimeType.Ppt),
                                                                   MimeType.Pptx              => nameof(MimeType.Pptx),
                                                                   MimeType.ThreeGppAudio     => nameof(MimeType.ThreeGppAudio),
                                                                   MimeType.ThreeGpp2Audio    => nameof(MimeType.ThreeGpp2Audio),
                                                                   MimeType.Aac               => nameof(MimeType.Aac),
                                                                   MimeType.MpegAudio         => nameof(MimeType.MpegAudio),
                                                                   MimeType.Mp3               => nameof(MimeType.Mp3),
                                                                   MimeType.Weba              => nameof(MimeType.Weba),
                                                                   MimeType.Wav               => nameof(MimeType.Wav),
                                                                   MimeType.ThreeGppVideo     => nameof(MimeType.ThreeGppVideo),
                                                                   MimeType.ThreeGpp2Video    => nameof(MimeType.ThreeGpp2Video),
                                                                   MimeType.Mp4               => nameof(MimeType.Mp4),
                                                                   MimeType.MpegVideo         => nameof(MimeType.MpegVideo),
                                                                   MimeType.Mpeg4             => nameof(MimeType.Mpeg4),
                                                                   MimeType.Webm              => nameof(MimeType.Webm),
                                                                   MimeType.H264              => nameof(MimeType.H264),
                                                                   MimeType.Avi               => nameof(MimeType.Avi),
                                                                   MimeType.Mov               => nameof(MimeType.Mov),
                                                                   MimeType.Mpg               => nameof(MimeType.Mpg),
                                                                   MimeType.Ogg               => nameof(MimeType.Ogg),
                                                                   MimeType.Mkv               => nameof(MimeType.Mkv),
                                                                   MimeType.Gif               => nameof(MimeType.Gif),
                                                                   MimeType.Tiff              => nameof(MimeType.Tiff),
                                                                   MimeType.Png               => nameof(MimeType.Png),
                                                                   MimeType.Jpeg              => nameof(MimeType.Jpeg),
                                                                   MimeType.Jpg               => nameof(MimeType.Jpg),
                                                                   MimeType.Bmp               => nameof(MimeType.Bmp),
                                                                   MimeType.Webp              => nameof(MimeType.Webp),
                                                                   MimeType.Icon              => nameof(MimeType.Icon),
                                                                   MimeType.Svg               => nameof(MimeType.Svg),
                                                                   MimeType.TrueType          => nameof(MimeType.TrueType),
                                                                   MimeType.OpenType          => nameof(MimeType.OpenType),
                                                                   MimeType.FormData          => nameof(MimeType.FormData),
                                                                   MimeType.Licenses          => nameof(MimeType.Licenses),
                                                                   MimeType.Dll               => nameof(MimeType.Dll),
                                                                   _                          => throw new OutOfRangeException(mime)
                                                               };


    /// <summary> Converts the provided ContentType <see cref="string"/> to it's associated <see cref="MimeType"/> via <seealso cref="MimeTypeNames"/> </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="MimeType"/>
    /// </returns>
    public static MimeType ToMimeType( this string mime ) =>
        mime switch
        {
            null                                               => MimeType.NotSet,
            EMPTY                                              => MimeType.NotSet,
            MimeTypeNames.Text.PLAIN                           => MimeType.PlainText,
            MimeTypeNames.Text.HTML                            => MimeType.Html,
            MimeTypeNames.Text.XML                             => MimeType.Xml,
            MimeTypeNames.Text.XAML                            => MimeType.Xaml,
            MimeTypeNames.Text.RICH_TEXT                       => MimeType.RichText,
            MimeTypeNames.Text.CALENDAR                        => MimeType.Calendar,
            MimeTypeNames.Text.CASCADING_STYLE_SHEETS          => MimeType.Css,
            MimeTypeNames.Text.COMMA_SEPARATED_VALUES          => MimeType.Csv,
            MimeTypeNames.Text.INI                             => MimeType.Ini,
            MimeTypeNames.Text.CONFIG                          => MimeType.Cfg,
            MimeTypeNames.Text.CONFIGURATION                   => MimeType.Config,
            MimeTypeNames.Application.URL_ENCODED_CONTENT      => MimeType.UrlEncodedContent,
            MimeTypeNames.Application.SOAP                     => MimeType.Soap,
            MimeTypeNames.Application.BINARY                   => MimeType.Stream,
            MimeTypeNames.Application.BASE64                   => MimeType.Base64,
            MimeTypeNames.Application.RTF                      => MimeType.Rtf,
            MimeTypeNames.Application.PDF                      => MimeType.Pdf,
            MimeTypeNames.Application.JSON                     => MimeType.Json,
            MimeTypeNames.Application.XML                      => MimeType.XmlApp,
            MimeTypeNames.Application.XUL                      => MimeType.Xul,
            MimeTypeNames.Application.JAVA_SCRIPT              => MimeType.JavaScript,
            MimeTypeNames.Application.VBS                      => MimeType.Vbs,
            MimeTypeNames.Application.LICENSES                 => MimeType.Licenses,
            MimeTypeNames.Application.DLL                      => MimeType.Dll,
            MimeTypeNames.Application.SAFETY_DATA_SHEET        => MimeType.Sds,
            MimeTypeNames.Application.TECHNICAL_DATA_SHEET     => MimeType.Tds,
            MimeTypeNames.Application.CERTIFICATE_AUTHENTICITY => MimeType.Coa,
            MimeTypeNames.Application.Archive.ZIP              => MimeType.Zip,
            MimeTypeNames.Application.Archive.SEVEN_ZIP        => MimeType.SevenZip,
            MimeTypeNames.Application.Archive.B_ZIP            => MimeType.Bzip,
            MimeTypeNames.Application.Archive.B_ZIP_2          => MimeType.Bzip2,
            MimeTypeNames.Application.Archive.GZIP             => MimeType.Gzip,
            MimeTypeNames.Application.Archive.TAR              => MimeType.Tar,
            MimeTypeNames.Application.MsOffice.DOC             => MimeType.Doc,
            MimeTypeNames.Application.MsOffice.DOCX            => MimeType.Docx,
            MimeTypeNames.Application.MsOffice.XLS             => MimeType.Xls,
            MimeTypeNames.Application.MsOffice.XLSX            => MimeType.Xlsx,
            MimeTypeNames.Application.MsOffice.PPT             => MimeType.Ppt,
            MimeTypeNames.Application.MsOffice.PPTX            => MimeType.Pptx,
            MimeTypeNames.Audio.THREE_GPP_AUDIO                => MimeType.ThreeGppAudio,
            MimeTypeNames.Audio.THREE_GPP2_AUDIO               => MimeType.ThreeGpp2Audio,
            MimeTypeNames.Audio.AAC                            => MimeType.Aac,
            MimeTypeNames.Audio.MPEG                           => MimeType.MpegAudio,
            MimeTypeNames.Audio.MP3                            => MimeType.Mp3,
            MimeTypeNames.Audio.WEBA                           => MimeType.Weba,
            MimeTypeNames.Audio.WAVE                           => MimeType.Wav,
            MimeTypeNames.Video.THREE_GPP_VIDEO                => MimeType.ThreeGppVideo,
            MimeTypeNames.Video.THREE_GPP2_VIDEO               => MimeType.ThreeGpp2Video,
            MimeTypeNames.Video.MP4                            => MimeType.Mp4,
            MimeTypeNames.Video.MPEG                           => MimeType.MpegVideo,
            MimeTypeNames.Video.MPEG4                          => MimeType.Mpeg4,
            MimeTypeNames.Video.WEBM                           => MimeType.Webm,
            MimeTypeNames.Video.H264                           => MimeType.H264,
            MimeTypeNames.Video.AVI                            => MimeType.Avi,
            MimeTypeNames.Video.MOV                            => MimeType.Mov,
            MimeTypeNames.Video.MPG                            => MimeType.Mpg,
            MimeTypeNames.Video.OGG                            => MimeType.Ogg,
            MimeTypeNames.Video.MKV                            => MimeType.Mkv,
            MimeTypeNames.Image.GIF                            => MimeType.Gif,
            MimeTypeNames.Image.TIFF                           => MimeType.Tiff,
            MimeTypeNames.Image.PNG                            => MimeType.Png,
            MimeTypeNames.Image.JPEG                           => MimeType.Jpeg,
            MimeTypeNames.Image.JPG                            => MimeType.Jpg,
            MimeTypeNames.Image.BMP                            => MimeType.Bmp,
            MimeTypeNames.Image.WEBP                           => MimeType.Webp,
            MimeTypeNames.Image.ICON                           => MimeType.Icon,
            MimeTypeNames.Image.SVG                            => MimeType.Svg,
            MimeTypeNames.Font.TRUE_TYPE                       => MimeType.TrueType,
            MimeTypeNames.Font.OPEN_TYPE                       => MimeType.OpenType,
            MimeTypeNames.MultiPart.FORM_DATA                  => MimeType.FormData,
            _                                                  => MimeType.Unknown
        };


    /// <summary> Converts the provided <see cref="MimeType"/> to it's associated ContentType via <seealso cref="MimeTypeNames"/> </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string ToContentType( this MimeType mime ) =>
        mime switch
        {
            MimeType.Text              => MimeTypeNames.Text.PLAIN,
            MimeType.PlainText         => MimeTypeNames.Text.PLAIN,
            MimeType.Html              => MimeTypeNames.Text.HTML,
            MimeType.Xml               => MimeTypeNames.Text.XML,
            MimeType.Xaml              => MimeTypeNames.Text.XAML,
            MimeType.RichText          => MimeTypeNames.Text.RICH_TEXT,
            MimeType.Css               => MimeTypeNames.Text.CASCADING_STYLE_SHEETS,
            MimeType.Csv               => MimeTypeNames.Text.COMMA_SEPARATED_VALUES,
            MimeType.Calendar          => MimeTypeNames.Text.CALENDAR,
            MimeType.Ini               => MimeTypeNames.Text.INI,
            MimeType.Config            => MimeTypeNames.Text.CONFIGURATION,
            MimeType.Cfg               => MimeTypeNames.Text.CONFIG,
            MimeType.UrlEncodedContent => MimeTypeNames.Application.URL_ENCODED_CONTENT,
            MimeType.Soap              => MimeTypeNames.Application.SOAP,
            MimeType.Stream            => MimeTypeNames.Application.BINARY,
            MimeType.Unknown           => MimeTypeNames.Application.BINARY,
            MimeType.Binary            => MimeTypeNames.Application.BINARY,
            MimeType.Base64            => MimeTypeNames.Application.BASE64,
            MimeType.Rtf               => MimeTypeNames.Application.RTF,
            MimeType.Pdf               => MimeTypeNames.Application.PDF,
            MimeType.Json              => MimeTypeNames.Application.JSON,
            MimeType.XmlApp            => MimeTypeNames.Application.XML,
            MimeType.Xul               => MimeTypeNames.Application.XUL,
            MimeType.JavaScript        => MimeTypeNames.Application.JAVA_SCRIPT,
            MimeType.Dll               => MimeTypeNames.Application.DLL,
            MimeType.Vbs               => MimeTypeNames.Application.VBS,
            MimeType.Licenses          => MimeTypeNames.Application.LICENSES,
            MimeType.Sds               => MimeTypeNames.Application.SAFETY_DATA_SHEET,
            MimeType.Tds               => MimeTypeNames.Application.TECHNICAL_DATA_SHEET,
            MimeType.Coa               => MimeTypeNames.Application.CERTIFICATE_AUTHENTICITY,
            MimeType.Zip               => MimeTypeNames.Application.Archive.ZIP,
            MimeType.SevenZip          => MimeTypeNames.Application.Archive.SEVEN_ZIP,
            MimeType.Bzip              => MimeTypeNames.Application.Archive.B_ZIP,
            MimeType.Bzip2             => MimeTypeNames.Application.Archive.B_ZIP_2,
            MimeType.Gzip              => MimeTypeNames.Application.Archive.GZIP,
            MimeType.Tar               => MimeTypeNames.Application.Archive.TAR,
            MimeType.Doc               => MimeTypeNames.Application.MsOffice.DOC,
            MimeType.Docx              => MimeTypeNames.Application.MsOffice.DOCX,
            MimeType.Xls               => MimeTypeNames.Application.MsOffice.XLS,
            MimeType.Xlsx              => MimeTypeNames.Application.MsOffice.XLSX,
            MimeType.Ppt               => MimeTypeNames.Application.MsOffice.PPT,
            MimeType.Pptx              => MimeTypeNames.Application.MsOffice.PPTX,
            MimeType.ThreeGppAudio     => MimeTypeNames.Audio.THREE_GPP_AUDIO,
            MimeType.ThreeGpp2Audio    => MimeTypeNames.Audio.THREE_GPP2_AUDIO,
            MimeType.Aac               => MimeTypeNames.Audio.AAC,
            MimeType.MpegAudio         => MimeTypeNames.Audio.MPEG,
            MimeType.Mp3               => MimeTypeNames.Audio.MP3,
            MimeType.Weba              => MimeTypeNames.Audio.WEBA,
            MimeType.Wav               => MimeTypeNames.Audio.WAVE,
            MimeType.ThreeGppVideo     => MimeTypeNames.Video.THREE_GPP_VIDEO,
            MimeType.ThreeGpp2Video    => MimeTypeNames.Video.THREE_GPP2_VIDEO,
            MimeType.Mp4               => MimeTypeNames.Video.MP4,
            MimeType.MpegVideo         => MimeTypeNames.Video.MPEG,
            MimeType.Mpeg4             => MimeTypeNames.Video.MPEG4,
            MimeType.Webm              => MimeTypeNames.Video.WEBM,
            MimeType.H264              => MimeTypeNames.Video.H264,
            MimeType.Avi               => MimeTypeNames.Video.AVI,
            MimeType.Mov               => MimeTypeNames.Video.MOV,
            MimeType.Mpg               => MimeTypeNames.Video.MPG,
            MimeType.Ogg               => MimeTypeNames.Video.OGG,
            MimeType.Mkv               => MimeTypeNames.Video.MKV,
            MimeType.Gif               => MimeTypeNames.Image.GIF,
            MimeType.Tiff              => MimeTypeNames.Image.TIFF,
            MimeType.Png               => MimeTypeNames.Image.PNG,
            MimeType.Jpeg              => MimeTypeNames.Image.JPEG,
            MimeType.Jpg               => MimeTypeNames.Image.JPG,
            MimeType.Bmp               => MimeTypeNames.Image.BMP,
            MimeType.Webp              => MimeTypeNames.Image.WEBP,
            MimeType.Icon              => MimeTypeNames.Image.ICON,
            MimeType.Svg               => MimeTypeNames.Image.SVG,
            MimeType.TrueType          => MimeTypeNames.Font.TRUE_TYPE,
            MimeType.OpenType          => MimeTypeNames.Font.OPEN_TYPE,
            MimeType.FormData          => MimeTypeNames.MultiPart.FORM_DATA,
            MimeType.NotSet            => EMPTY,
            _                          => throw new OutOfRangeException(mime)
        };


    /// <summary> Gets the extension of the provided <see cref="MimeType"/> . </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string ToExtensionWithPeriod( this MimeType mime ) => ExtensionsWithPeriods[mime];


    /// <summary> Gets the extension of the provided <see cref="MimeType"/> . </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string ToExtension( this MimeType mime ) =>
        mime switch
        {
            MimeType.Text              => "txt",
            MimeType.PlainText         => "text",
            MimeType.Html              => "html",
            MimeType.Xml               => "xml",
            MimeType.Xaml              => "xaml",
            MimeType.RichText          => "rt",
            MimeType.Css               => "css",
            MimeType.Csv               => "csv",
            MimeType.Calendar          => "ics",
            MimeType.Ini               => "ini",
            MimeType.Config            => "conf",
            MimeType.Cfg               => "cfg",
            MimeType.Base64            => "b64",
            MimeType.UrlEncodedContent => "url",
            MimeType.Soap              => "soap",
            MimeType.Stream            => "stream",
            MimeType.Binary            => "bin",
            MimeType.Rtf               => "rtf",
            MimeType.Pdf               => "pdf",
            MimeType.Json              => "json",
            MimeType.XmlApp            => "xmla",
            MimeType.Xul               => "xul",
            MimeType.JavaScript        => "js",
            MimeType.Vbs               => "vbs",
            MimeType.Sds               => "sds",
            MimeType.Tds               => "tds",
            MimeType.Coa               => "coa",
            MimeType.Licenses          => "licenses",
            MimeType.Dll               => "dll",
            MimeType.Zip               => "zip",
            MimeType.SevenZip          => "7z",
            MimeType.Bzip              => "bz",
            MimeType.Bzip2             => "bz2",
            MimeType.Gzip              => "gz",
            MimeType.Tar               => "tar.gz",
            MimeType.Doc               => "doc",
            MimeType.Docx              => "docx",
            MimeType.Xls               => "xls",
            MimeType.Xlsx              => "xlsx",
            MimeType.Ppt               => "ppt",
            MimeType.Pptx              => "pptx",
            MimeType.ThreeGppAudio     => "3gppa",
            MimeType.ThreeGpp2Audio    => "3gpp2a",
            MimeType.Aac               => "aac",
            MimeType.MpegAudio         => "mpega",
            MimeType.Mp3               => "mp3",
            MimeType.Weba              => "weba",
            MimeType.Wav               => "wav",
            MimeType.ThreeGppVideo     => "3gpp",
            MimeType.ThreeGpp2Video    => "3gpp2",
            MimeType.Mp4               => "mp4",
            MimeType.MpegVideo         => "mpeg",
            MimeType.Mpeg4             => "mpeg4",
            MimeType.Webm              => "webm",
            MimeType.H264              => "h264",
            MimeType.Avi               => "avi",
            MimeType.Mov               => "mov",
            MimeType.Mpg               => "mpg",
            MimeType.Ogg               => "ogg",
            MimeType.Mkv               => "mkv",
            MimeType.Gif               => "gif",
            MimeType.Tiff              => "tif",
            MimeType.Png               => "png",
            MimeType.Jpeg              => "jpeg",
            MimeType.Jpg               => "jpg",
            MimeType.Bmp               => "bmp",
            MimeType.Webp              => "webp",
            MimeType.Icon              => "ico",
            MimeType.Svg               => "svg",
            MimeType.TrueType          => "ttf",
            MimeType.OpenType          => "otf",
            MimeType.FormData          => "fd",
            MimeType.Unknown           => "dat",
            MimeType.NotSet            => "file",
            _                          => throw OutOfRangeException.Create(mime)
        };


    /// <summary> Uses the provided <paramref name="fileName"/> and adds the extension based on provided <see cref="MimeType"/> . </summary>
    /// <param name="mime"> </param>
    /// <param name="fileName"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string ToFileName( this MimeType mime, string fileName )
    {
        if ( string.IsNullOrWhiteSpace(fileName) ) { throw new NullReferenceException(nameof(mime)); }

        return $"{fileName}.{mime.ToExtension()}";
    }


    /// <summary>
    ///     <seealso href="https://docs.microsoft.com/en-us/office/client-developer/office-uri-schemes"/>
    /// </summary>
    /// <param name="mime"> </param>
    /// <returns>
    ///     <see cref="string"/>
    /// </returns>
    public static string ToUriScheme( this MimeType mime ) =>

        // TODO: get more uri schemes
        mime switch
        {
            MimeType.NotSet  => throw new OutOfRangeException(mime, "Must be a valid MimeType, not MimeType.NotSet"),
            MimeType.Unknown => throw new OutOfRangeException(mime, "Cannot discern UriScheme for MimeType.Unknown"),
            MimeType.Doc     => "ms-word",
            MimeType.Docx    => "ms-word",
            MimeType.Xls     => "ms-excel",
            MimeType.Xlsx    => "ms-excel",
            MimeType.Ppt     => "ms-powerpoint",
            MimeType.Pptx    => "ms-powerpoint",
            _                => "file"

            //_ => throw new OutOfRangeException( mime, $"Cannot discern UriScheme for {typeof(mime).FullName}.{mime}" )
        };


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsAnyBinary( this   MimeType mime ) => mime.IsBinary() || mime.IsPDF()    || mime.IsAudio() || mime.IsVideo() || mime.IsFont() || mime.IsCompressedFile() || mime.IsOfficeDocument();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsApplication( this MimeType mime ) => mime.IsPDF()    || mime.IsBinary() || mime is MimeType.UrlEncodedContent or MimeType.Soap or MimeType.Rtf or MimeType.XmlApp or MimeType.Xul or MimeType.JavaScript or MimeType.Vbs or MimeType.Json;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsText( this MimeType mime ) =>
        mime is MimeType.Text or MimeType.PlainText or MimeType.Html or MimeType.Xml or MimeType.Xaml or MimeType.RichText or MimeType.Css or MimeType.Csv or MimeType.Ini or MimeType.Config or MimeType.Cfg or MimeType.Json or MimeType.UrlEncodedContent or MimeType.JavaScript or MimeType.Base64 or MimeType.Vbs or MimeType.Rtf or MimeType.Calendar;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsBinary( this         MimeType mime ) => mime is MimeType.Binary or MimeType.Stream;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsPDF( this            MimeType mime ) => mime is MimeType.Pdf or MimeType.Sds or MimeType.Sds or MimeType.Tds or MimeType.Coa or MimeType.Licenses;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsAudio( this          MimeType mime ) => mime is MimeType.ThreeGppAudio or MimeType.ThreeGpp2Audio or MimeType.Aac or MimeType.MpegAudio or MimeType.Mp3 or MimeType.Weba or MimeType.Wav;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsVideo( this          MimeType mime ) => mime is MimeType.ThreeGppVideo or MimeType.ThreeGpp2Video or MimeType.Mp4 or MimeType.MpegVideo or MimeType.Mpeg4 or MimeType.Webm or MimeType.H264 or MimeType.Avi or MimeType.Mov or MimeType.Mpg or MimeType.Ogg or MimeType.Mkv;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsImage( this          MimeType mime ) => mime is MimeType.Gif or MimeType.Tiff or MimeType.Png or MimeType.Jpeg or MimeType.Bmp or MimeType.Webp or MimeType.Icon or MimeType.Svg;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsFont( this           MimeType mime ) => mime is MimeType.TrueType or MimeType.OpenType;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsCompressedFile( this MimeType mime ) => mime is MimeType.Zip or MimeType.SevenZip or MimeType.Bzip or MimeType.Bzip2 or MimeType.Gzip or MimeType.Tar;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsOfficeDocument( this MimeType mime ) => mime is MimeType.Doc or MimeType.Docx or MimeType.Xls or MimeType.Xlsx or MimeType.Ppt or MimeType.Pptx;
}
