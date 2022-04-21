namespace Jakar.Extensions.Http;


public static class MimeTypeExtensions
{
    /// <summary>
    /// <seealso href="https://docs.microsoft.com/en-us/office/client-developer/office-uri-schemes"/>
    /// </summary>
    /// <param name="mime"></param>
    /// <returns><see cref="string"/></returns>
    public static string ToUriScheme( this MimeType mime ) =>

        // TODO: get more uri schemes
        mime switch
        {
            MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime), mime, "Must be a valid MimeType, not MimeType.NotSet"),
            MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime), mime, "Cannot discern UriScheme for MimeType.Unknown"),

            MimeType.Doc  => "ms-word",
            MimeType.Docx => "ms-word",

            MimeType.Xls  => "ms-excel",
            MimeType.Xlsx => "ms-excel",

            MimeType.Ppt  => "ms-powerpoint",
            MimeType.Pptx => "ms-powerpoint",

            _ => "file",

            //_ => throw new ArgumentOutOfRangeException(nameof(mime), mime, $"Cannot discern UriScheme for {typeof(mime).FullName}.{mime}")
        };


    /// <summary>
    /// Uses the provided <paramref name="fileName"/> and adds the extension based on provided <see cref="MimeType"/>.
    /// </summary>
    /// <param name="mime"></param>
    /// <param name="fileName"></param>
    /// <returns><see cref="string"/></returns>
    public static string ToFileName( this MimeType mime, string fileName )
    {
        if ( string.IsNullOrWhiteSpace(fileName) ) { throw new NullReferenceException(nameof(mime)); }

        return $"{fileName}.{mime.ToExtension().ToLower()}";
    }


    /// <summary>
    /// Gets the extension of the provided <see cref="MimeType"/>.
    /// </summary>
    /// <param name="mime"></param>
    /// <param name="includePeriod"></param>
    /// <returns><see cref="string"/></returns>
    public static string ToExtension( this MimeType mime, bool includePeriod = false )
    {
        string result = mime switch
                        {
                            MimeType.Text      => "text",
                            MimeType.PlainText => "txt",
                            MimeType.Html      => "html",
                            MimeType.Xml       => "xml",
                            MimeType.RichText  => "rt",
                            MimeType.Css       => "css",
                            MimeType.Csv       => "csv",
                            MimeType.Calendar  => "ics",
                            MimeType.Ini       => "ini",
                            MimeType.Config    => "conf",
                            MimeType.Cfg       => "cfg",
                            MimeType.Base64    => "b64",

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

                            MimeType.Zip      => "zip",
                            MimeType.SevenZip => "7z",
                            MimeType.Bzip     => "bz",
                            MimeType.Bzip2    => "bz2",
                            MimeType.Gzip     => "gz",
                            MimeType.Tar      => "tar.gz",

                            MimeType.Doc  => "doc",
                            MimeType.Docx => "docx",
                            MimeType.Xls  => "xls",
                            MimeType.Xlsx => "xlsx",
                            MimeType.Ppt  => "ppt",
                            MimeType.Pptx => "pptx",

                            MimeType.ThreeGppAudio  => "3gppa",
                            MimeType.ThreeGpp2Audio => "3gpp2a",
                            MimeType.Aac            => "aac",
                            MimeType.MpegAudio      => "mpega",
                            MimeType.Mp3            => "mp3",
                            MimeType.Weba           => "weba",
                            MimeType.Wav            => "wav",

                            MimeType.ThreeGppVideo  => "3gpp",
                            MimeType.ThreeGpp2Video => "3gpp2",
                            MimeType.Mp4            => "mp4",
                            MimeType.MpegVideo      => "mpeg",
                            MimeType.Mpeg4          => "mpeg4",
                            MimeType.Webm           => "webm",
                            MimeType.H264           => "h264",
                            MimeType.Avi            => "avi",
                            MimeType.Mov            => "mov",
                            MimeType.Mpg            => "mpg",
                            MimeType.Ogg            => "ogg",
                            MimeType.Mkv            => "mkv",

                            MimeType.Gif  => "gif",
                            MimeType.Tiff => "tif",
                            MimeType.Png  => "png",
                            MimeType.Jpeg => "jpeg",
                            MimeType.Jpg  => "jpg",
                            MimeType.Bmp  => "bmp",
                            MimeType.Webp => "webp",
                            MimeType.Icon => "ico",
                            MimeType.Svg  => "svg",

                            MimeType.TrueType => "ttf",
                            MimeType.OpenType => "otf",
                            MimeType.FormData => "fd",


                            MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime)),
                            MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime)),
                            _                => throw new ArgumentOutOfRangeException(nameof(mime)),
                        };

        if ( includePeriod ) { result = $".{includePeriod}"; }

        return result;
    }

    /// <summary>
    /// Gets the <see cref="MimeType"/> of the provided extension <see cref="string"/>.
    /// </summary>
    /// <param name="mime"></param>
    /// <returns><see cref="MimeType"/></returns>
    public static MimeType FromExtension( this string mime ) =>
        mime.ToLower() switch
        {
            "text"  => MimeType.Text,
            ".text" => MimeType.Text,
            "txt"   => MimeType.PlainText,
            ".txt"  => MimeType.PlainText,
            "html"  => MimeType.Html,
            ".html" => MimeType.Html,
            "htm"   => MimeType.Html,
            ".htm"  => MimeType.Html,
            "xml"   => MimeType.Xml,
            ".xml"  => MimeType.Xml,
            "rt"    => MimeType.RichText,
            ".rt"   => MimeType.RichText,
            "css"   => MimeType.Css,
            ".css"  => MimeType.Css,
            "csv"   => MimeType.Csv,
            ".csv"  => MimeType.Csv,
            "ics"   => MimeType.Calendar,
            ".ics"  => MimeType.Calendar,
            "ini"   => MimeType.Ini,
            ".ini"  => MimeType.Ini,
            "cfg"   => MimeType.Cfg,
            ".cfg"  => MimeType.Cfg,
            "conf"  => MimeType.Config,
            ".conf" => MimeType.Config,
            "b64"   => MimeType.Base64,
            ".b64"  => MimeType.Base64,

            "soap"      => MimeType.Soap,
            ".soap"     => MimeType.Soap,
            "stream"    => MimeType.Stream,
            ".stream"   => MimeType.Stream,
            "bin"       => MimeType.Binary,
            ".bin"      => MimeType.Binary,
            "pdf"       => MimeType.Pdf,
            ".pdf"      => MimeType.Pdf,
            "rtf"       => MimeType.Rtf,
            ".rtf"      => MimeType.Rtf,
            "json"      => MimeType.Json,
            ".json"     => MimeType.Json,
            "xul"       => MimeType.Xul,
            ".xul"      => MimeType.Xul,
            "js"        => MimeType.JavaScript,
            ".js"       => MimeType.JavaScript,
            "vbs"       => MimeType.Vbs,
            ".vbs"      => MimeType.Vbs,
            "sds"       => MimeType.Sds,
            ".sds"      => MimeType.Sds,
            "tds"       => MimeType.Tds,
            ".tds"      => MimeType.Tds,
            "coa"       => MimeType.Coa,
            ".coa"      => MimeType.Coa,
            "xmla"      => MimeType.XmlApp,
            ".xmla"     => MimeType.XmlApp,
            "url"       => MimeType.UrlEncodedContent,
            ".url"      => MimeType.UrlEncodedContent,
            "license"   => MimeType.Licenses,
            ".license"  => MimeType.Licenses,
            "licenses"  => MimeType.Licenses,
            ".licenses" => MimeType.Licenses,
            "dll"       => MimeType.Dll,
            ".dll"      => MimeType.Dll,

            "zip"     => MimeType.Zip,
            ".zip"    => MimeType.Zip,
            "7z"      => MimeType.SevenZip,
            ".7z"     => MimeType.SevenZip,
            "bz"      => MimeType.Bzip,
            ".bz"     => MimeType.Bzip,
            "bz2"     => MimeType.Bzip2,
            ".bz2"    => MimeType.Bzip2,
            "gz"      => MimeType.Gzip,
            ".gz"     => MimeType.Gzip,
            "tar.gz"  => MimeType.Tar,
            ".tar.gz" => MimeType.Tar,

            "doc"   => MimeType.Doc,
            ".doc"  => MimeType.Doc,
            "docx"  => MimeType.Docx,
            ".docx" => MimeType.Docx,
            "xls"   => MimeType.Xls,
            ".xls"  => MimeType.Xls,
            "xlsx"  => MimeType.Xlsx,
            ".xlsx" => MimeType.Xlsx,
            "ppt"   => MimeType.Ppt,
            ".ppt"  => MimeType.Ppt,
            "pptx"  => MimeType.Pptx,
            ".pptx" => MimeType.Pptx,

            "3gppa"   => MimeType.ThreeGppAudio,
            ".3gppa"  => MimeType.ThreeGppAudio,
            "3gpp2a"  => MimeType.ThreeGpp2Audio,
            ".3gpp2a" => MimeType.ThreeGpp2Audio,
            "aac"     => MimeType.Aac,
            ".aac"    => MimeType.Aac,

            "mpega"  => MimeType.MpegAudio,
            ".mpega" => MimeType.MpegAudio,
            "mp3"    => MimeType.Mp3,
            ".mp3"   => MimeType.Mp3,
            "weba"   => MimeType.Weba,
            ".weba"  => MimeType.Weba,
            "wav"    => MimeType.Wav,
            ".wav"   => MimeType.Wav,

            "3gpp"   => MimeType.ThreeGppVideo,
            ".3gpp"  => MimeType.ThreeGppVideo,
            "3gpp2"  => MimeType.ThreeGpp2Video,
            ".3gpp2" => MimeType.ThreeGpp2Video,
            "mp4"    => MimeType.Mp4,
            ".mp4"   => MimeType.Mp4,
            "mpeg"   => MimeType.MpegVideo,
            ".mpeg"  => MimeType.MpegVideo,
            "mpeg4"  => MimeType.Mpeg4,
            ".mpeg4" => MimeType.Mpeg4,
            "webm"   => MimeType.Webm,
            ".webm"  => MimeType.Webm,
            "h264"   => MimeType.H264,
            ".h264"  => MimeType.H264,
            "avi"    => MimeType.Avi,
            ".avi"   => MimeType.Avi,
            "mov"    => MimeType.Mov,
            ".mov"   => MimeType.Mov,
            "mpg"    => MimeType.Mpg,
            ".mpg"   => MimeType.Mpg,
            "ogg"    => MimeType.Ogg,
            ".ogg"   => MimeType.Ogg,
            "mkv"    => MimeType.Mkv,
            ".mkv"   => MimeType.Mkv,

            "gif"   => MimeType.Gif,
            ".gif"  => MimeType.Gif,
            "tif"   => MimeType.Tiff,
            ".tif"  => MimeType.Tiff,
            "png"   => MimeType.Png,
            ".png"  => MimeType.Png,
            "jpeg"  => MimeType.Jpeg,
            ".jpeg" => MimeType.Jpeg,
            "jpg"   => MimeType.Jpg,
            ".jpg"  => MimeType.Jpg,
            "bmp"   => MimeType.Bmp,
            ".bmp"  => MimeType.Bmp,
            "webp"  => MimeType.Webp,
            ".webp" => MimeType.Webp,
            "ico"   => MimeType.Icon,
            ".ico"  => MimeType.Icon,
            "svg"   => MimeType.Svg,
            ".svg"  => MimeType.Svg,

            "ttf"  => MimeType.TrueType,
            ".ttf" => MimeType.TrueType,
            "otf"  => MimeType.OpenType,
            ".otf" => MimeType.OpenType,
            "fd"   => MimeType.FormData,
            ".fd"  => MimeType.FormData,

            _ => MimeType.Unknown,
        };


    /// <summary>
    /// Converts the provided ContentType <see cref="string"/> to it's associated <see cref="MimeType"/> via <seealso cref="MimeTypeNames"/>
    /// </summary>
    /// <param name="mime"></param>
    /// <returns><see cref="MimeType"/></returns>
    public static MimeType ToMimeType( this string mime ) =>
        mime switch
        {
            null => throw new NullReferenceException(nameof(mime)),

            MimeTypeNames.Text.PLAIN                  => MimeType.PlainText,
            MimeTypeNames.Text.HTML                   => MimeType.Html,
            MimeTypeNames.Text.XML                    => MimeType.Xml,
            MimeTypeNames.Text.RICH_TEXT              => MimeType.RichText,
            MimeTypeNames.Text.CALENDAR               => MimeType.Calendar,
            MimeTypeNames.Text.CASCADING_STYLE_SHEETS => MimeType.Css,
            MimeTypeNames.Text.COMMA_SEPARATED_VALUES => MimeType.Csv,
            MimeTypeNames.Text.INI                    => MimeType.Ini,
            MimeTypeNames.Text.CONFIG                 => MimeType.Cfg,
            MimeTypeNames.Text.CONFIGURATION          => MimeType.Config,

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

            MimeTypeNames.Application.Archive.ZIP       => MimeType.Zip,
            MimeTypeNames.Application.Archive.SEVEN_ZIP => MimeType.SevenZip,
            MimeTypeNames.Application.Archive.B_ZIP     => MimeType.Bzip,
            MimeTypeNames.Application.Archive.B_ZIP_2   => MimeType.Bzip2,
            MimeTypeNames.Application.Archive.GZIP      => MimeType.Gzip,
            MimeTypeNames.Application.Archive.TAR       => MimeType.Tar,

            MimeTypeNames.Application.MsOffice.DOC  => MimeType.Doc,
            MimeTypeNames.Application.MsOffice.DOCX => MimeType.Docx,
            MimeTypeNames.Application.MsOffice.XLS  => MimeType.Xls,
            MimeTypeNames.Application.MsOffice.XLSX => MimeType.Xlsx,
            MimeTypeNames.Application.MsOffice.PPT  => MimeType.Ppt,
            MimeTypeNames.Application.MsOffice.PPTX => MimeType.Pptx,

            MimeTypeNames.Audio.THREE_GPP_AUDIO  => MimeType.ThreeGppAudio,
            MimeTypeNames.Audio.THREE_GPP2_AUDIO => MimeType.ThreeGpp2Audio,
            MimeTypeNames.Audio.AAC              => MimeType.Aac,
            MimeTypeNames.Audio.MPEG             => MimeType.MpegAudio,
            MimeTypeNames.Audio.MP3              => MimeType.Mp3,
            MimeTypeNames.Audio.WEBA             => MimeType.Weba,
            MimeTypeNames.Audio.WAVE             => MimeType.Wav,

            MimeTypeNames.Video.THREE_GPP_VIDEO  => MimeType.ThreeGppVideo,
            MimeTypeNames.Video.THREE_GPP2_VIDEO => MimeType.ThreeGpp2Video,
            MimeTypeNames.Video.MP4              => MimeType.Mp4,
            MimeTypeNames.Video.MPEG             => MimeType.MpegVideo,
            MimeTypeNames.Video.MPEG4            => MimeType.Mpeg4,
            MimeTypeNames.Video.WEBM             => MimeType.Webm,
            MimeTypeNames.Video.H264             => MimeType.H264,
            MimeTypeNames.Video.AVI              => MimeType.Avi,
            MimeTypeNames.Video.MOV              => MimeType.Mov,
            MimeTypeNames.Video.MPG              => MimeType.Mpg,
            MimeTypeNames.Video.OGG              => MimeType.Ogg,
            MimeTypeNames.Video.MKV              => MimeType.Mkv,

            MimeTypeNames.Image.GIF  => MimeType.Gif,
            MimeTypeNames.Image.TIFF => MimeType.Tiff,
            MimeTypeNames.Image.PNG  => MimeType.Png,
            MimeTypeNames.Image.JPEG => MimeType.Jpeg,
            MimeTypeNames.Image.JPG  => MimeType.Jpg,
            MimeTypeNames.Image.BMP  => MimeType.Bmp,
            MimeTypeNames.Image.WEBP => MimeType.Webp,
            MimeTypeNames.Image.ICON => MimeType.Icon,
            MimeTypeNames.Image.SVG  => MimeType.Svg,

            MimeTypeNames.Font.TRUE_TYPE => MimeType.TrueType,
            MimeTypeNames.Font.OPEN_TYPE => MimeType.OpenType,

            MimeTypeNames.MultiPart.FORM_DATA => MimeType.FormData,

            _ => MimeType.Unknown
        };

    /// <summary>
    /// Converts the provided <see cref="MimeType"/> to it's associated ContentType via <seealso cref="MimeTypeNames"/>
    /// </summary>
    /// <param name="mime"></param>
    /// <returns><see cref="string"/></returns>
    public static string ToContentType( this MimeType mime ) =>
        mime switch
        {
            MimeType.Text      => MimeTypeNames.Text.PLAIN,
            MimeType.PlainText => MimeTypeNames.Text.PLAIN,
            MimeType.Html      => MimeTypeNames.Text.HTML,
            MimeType.Xml       => MimeTypeNames.Text.XML,
            MimeType.RichText  => MimeTypeNames.Text.RICH_TEXT,
            MimeType.Css       => MimeTypeNames.Text.CASCADING_STYLE_SHEETS,
            MimeType.Csv       => MimeTypeNames.Text.COMMA_SEPARATED_VALUES,
            MimeType.Calendar  => MimeTypeNames.Text.CALENDAR,
            MimeType.Ini       => MimeTypeNames.Text.INI,
            MimeType.Config    => MimeTypeNames.Text.CONFIGURATION,
            MimeType.Cfg       => MimeTypeNames.Text.CONFIG,

            MimeType.UrlEncodedContent => MimeTypeNames.Application.URL_ENCODED_CONTENT,
            MimeType.Soap              => MimeTypeNames.Application.SOAP,
            MimeType.Stream            => MimeTypeNames.Application.BINARY,
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

            MimeType.Zip      => MimeTypeNames.Application.Archive.ZIP,
            MimeType.SevenZip => MimeTypeNames.Application.Archive.SEVEN_ZIP,
            MimeType.Bzip     => MimeTypeNames.Application.Archive.B_ZIP,
            MimeType.Bzip2    => MimeTypeNames.Application.Archive.B_ZIP_2,
            MimeType.Gzip     => MimeTypeNames.Application.Archive.GZIP,
            MimeType.Tar      => MimeTypeNames.Application.Archive.TAR,

            MimeType.Doc  => MimeTypeNames.Application.MsOffice.DOC,
            MimeType.Docx => MimeTypeNames.Application.MsOffice.DOCX,
            MimeType.Xls  => MimeTypeNames.Application.MsOffice.XLS,
            MimeType.Xlsx => MimeTypeNames.Application.MsOffice.XLSX,
            MimeType.Ppt  => MimeTypeNames.Application.MsOffice.PPT,
            MimeType.Pptx => MimeTypeNames.Application.MsOffice.PPTX,

            MimeType.ThreeGppAudio  => MimeTypeNames.Audio.THREE_GPP_AUDIO,
            MimeType.ThreeGpp2Audio => MimeTypeNames.Audio.THREE_GPP2_AUDIO,
            MimeType.Aac            => MimeTypeNames.Audio.AAC,
            MimeType.MpegAudio      => MimeTypeNames.Audio.MPEG,
            MimeType.Mp3            => MimeTypeNames.Audio.MP3,
            MimeType.Weba           => MimeTypeNames.Audio.WEBA,
            MimeType.Wav            => MimeTypeNames.Audio.WAVE,

            MimeType.ThreeGppVideo  => MimeTypeNames.Video.THREE_GPP_VIDEO,
            MimeType.ThreeGpp2Video => MimeTypeNames.Video.THREE_GPP2_VIDEO,
            MimeType.Mp4            => MimeTypeNames.Video.MP4,
            MimeType.MpegVideo      => MimeTypeNames.Video.MPEG,
            MimeType.Mpeg4          => MimeTypeNames.Video.MPEG4,
            MimeType.Webm           => MimeTypeNames.Video.WEBM,
            MimeType.H264           => MimeTypeNames.Video.H264,
            MimeType.Avi            => MimeTypeNames.Video.AVI,
            MimeType.Mov            => MimeTypeNames.Video.MOV,
            MimeType.Mpg            => MimeTypeNames.Video.MPG,
            MimeType.Ogg            => MimeTypeNames.Video.OGG,
            MimeType.Mkv            => MimeTypeNames.Video.MKV,

            MimeType.Gif  => MimeTypeNames.Image.GIF,
            MimeType.Tiff => MimeTypeNames.Image.TIFF,
            MimeType.Png  => MimeTypeNames.Image.PNG,
            MimeType.Jpeg => MimeTypeNames.Image.JPEG,
            MimeType.Jpg  => MimeTypeNames.Image.JPG,
            MimeType.Bmp  => MimeTypeNames.Image.BMP,
            MimeType.Webp => MimeTypeNames.Image.WEBP,
            MimeType.Icon => MimeTypeNames.Image.ICON,
            MimeType.Svg  => MimeTypeNames.Image.SVG,

            MimeType.TrueType => MimeTypeNames.Font.TRUE_TYPE,
            MimeType.OpenType => MimeTypeNames.Font.OPEN_TYPE,
            MimeType.FormData => MimeTypeNames.MultiPart.FORM_DATA,

            MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime), mime, null),
            MimeType.Unknown => throw new ArgumentOutOfRangeException(nameof(mime), mime, null),
            _                => throw new ArgumentOutOfRangeException(nameof(mime), mime, null)
        };
}
