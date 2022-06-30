#nullable enable
namespace Jakar.Extensions;


public static class MimeTypeExtensions
{
    /// <summary>
    ///     <seealso href = "https://docs.microsoft.com/en-us/office/client-developer/office-uri-schemes" />
    /// </summary>
    /// <param name = "mime" > </param>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
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

            _ => "file"

            //_ => throw new ArgumentOutOfRangeException(nameof(mime), mime, $"Cannot discern UriScheme for {typeof(mime).FullName}.{mime}")
        };


    /// <summary>
    ///     Uses the provided
    ///     <paramref name = "fileName" />
    ///     and adds the extension based on provided
    ///     <see cref = "MimeType" />
    ///     .
    /// </summary>
    /// <param name = "mime" > </param>
    /// <param name = "fileName" > </param>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public static string ToFileName( this MimeType mime, string fileName )
    {
        if ( string.IsNullOrWhiteSpace(fileName) ) { throw new NullReferenceException(nameof(mime)); }

        return $"{fileName}.{mime.ToExtension().ToLower()}";
    }


    /// <summary>
    ///     Gets the extension of the provided
    ///     <see cref = "MimeType" />
    ///     .
    /// </summary>
    /// <param name = "mime" > </param>
    /// <param name = "includePeriod" > </param>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public static string ToExtension( this MimeType mime, bool includePeriod = false )
    {
        ReadOnlySpan<char> result = mime switch
                                    {
                                        MimeType.Text      => "text",
                                        MimeType.PlainText => "txt",
                                        MimeType.Html      => "html",
                                        MimeType.Xml       => "xml",
                                        MimeType.Xaml      => "xaml",
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


                                        MimeType.Unknown => "dat",
                                        MimeType.NotSet  => throw new ArgumentOutOfRangeException(nameof(mime)),
                                        _                => throw new ArgumentOutOfRangeException(nameof(mime))
                                    };


        if ( !includePeriod ) { return result.ToString(); }

        Span<char> span = stackalloc char[result.Length + 1];
        span[0] = '.';
        result.CopyTo(span[1..]);
        return span.ToString();
    }


    /// <summary>
    ///     Gets the <see cref = "MimeType" /> of the provided extension <see cref = "string" />.
    /// </summary>
    /// <param name = "mime" > </param>
    /// <returns>
    ///     <see cref = "MimeType" />
    /// </returns>
    public static MimeType FromExtension( this string mime ) => mime.AsSpan().FromExtension();
    /// <summary>
    ///     Gets the <see cref = "MimeType" /> of the provided extension <see cref = "string" />.
    /// </summary>
    /// <param name = "mime" > </param>
    /// <returns>
    ///     <see cref = "MimeType" />
    /// </returns>
    public static MimeType FromExtension( this ReadOnlySpan<char> mime )
    {
        Span<char> span = stackalloc char[mime.Length];
        mime.ToLowerInvariant(span);

        if ( span.SequenceEqual("text") || span.SequenceEqual(".text") ) { return MimeType.Text; }

        if ( span.SequenceEqual("txt") || span.SequenceEqual(".txt") ) { return MimeType.PlainText; }

        if ( span.SequenceEqual("html") || span.SequenceEqual(".html") || span.SequenceEqual("htm") || span.SequenceEqual(".htm") ) { return MimeType.Html; }

        if ( span.SequenceEqual("xml") || span.SequenceEqual(".xml") ) { return MimeType.Xml; }

        if ( span.SequenceEqual("xaml") || span.SequenceEqual(".xaml") ) { return MimeType.Xaml; }

        if ( span.SequenceEqual("rt") || span.SequenceEqual(".rt") ) { return MimeType.RichText; }

        if ( span.SequenceEqual("css") || span.SequenceEqual(".css") ) { return MimeType.Css; }

        if ( span.SequenceEqual("csv") || span.SequenceEqual(".csv") ) { return MimeType.Csv; }

        if ( span.SequenceEqual("ics") || span.SequenceEqual(".ics") ) { return MimeType.Calendar; }

        if ( span.SequenceEqual("ini") || span.SequenceEqual(".ini") ) { return MimeType.Ini; }

        if ( span.SequenceEqual("cfg") || span.SequenceEqual(".cfg") ) { return MimeType.Cfg; }

        if ( span.SequenceEqual("conf") || span.SequenceEqual(".conf") ) { return MimeType.Config; }

        if ( span.SequenceEqual("b64") || span.SequenceEqual(".b64") ) { return MimeType.Base64; }

        if ( span.SequenceEqual("soap") || span.SequenceEqual(".soap") ) { return MimeType.Soap; }

        if ( span.SequenceEqual("stream") || span.SequenceEqual(".stream") ) { return MimeType.Stream; }

        if ( span.SequenceEqual("bin") || span.SequenceEqual(".bin") ) { return MimeType.Binary; }

        if ( span.SequenceEqual("pdf") || span.SequenceEqual(".pdf") ) { return MimeType.Pdf; }

        if ( span.SequenceEqual("rtf") || span.SequenceEqual(".rtf") ) { return MimeType.Rtf; }

        if ( span.SequenceEqual("json") || span.SequenceEqual(".json") ) { return MimeType.Json; }

        if ( span.SequenceEqual("xul") || span.SequenceEqual(".xul") ) { return MimeType.Xul; }

        if ( span.SequenceEqual("js") || span.SequenceEqual(".js") ) { return MimeType.JavaScript; }

        if ( span.SequenceEqual("vbs") || span.SequenceEqual(".vbs") ) { return MimeType.Vbs; }

        if ( span.SequenceEqual("sds") || span.SequenceEqual(".sds") ) { return MimeType.Sds; }

        if ( span.SequenceEqual("tds") || span.SequenceEqual(".tds") ) { return MimeType.Tds; }

        if ( span.SequenceEqual("coa") || span.SequenceEqual(".coa") ) { return MimeType.Coa; }

        if ( span.SequenceEqual("xmla") || span.SequenceEqual(".xmla") ) { return MimeType.XmlApp; }

        if ( span.SequenceEqual("url") || span.SequenceEqual(".url") ) { return MimeType.UrlEncodedContent; }

        if ( span.SequenceEqual("license") || span.SequenceEqual(".license") || span.SequenceEqual("licenses") || span.SequenceEqual(".licenses") ) { return MimeType.Licenses; }

        if ( span.SequenceEqual("dll") || span.SequenceEqual(".dll") ) { return MimeType.Dll; }

        if ( span.SequenceEqual("zip") || span.SequenceEqual(".zip") ) { return MimeType.Zip; }

        if ( span.SequenceEqual("7z") || span.SequenceEqual(".7z") ) { return MimeType.SevenZip; }

        if ( span.SequenceEqual("bz") || span.SequenceEqual(".bz") ) { return MimeType.Bzip; }

        if ( span.SequenceEqual("bz2") || span.SequenceEqual(".bz2") ) { return MimeType.Bzip2; }

        if ( span.SequenceEqual("gz") || span.SequenceEqual(".gz") ) { return MimeType.Gzip; }

        if ( span.SequenceEqual("tar.gz") || span.SequenceEqual(".tar.gz") ) { return MimeType.Tar; }

        if ( span.SequenceEqual("doc") || span.SequenceEqual(".doc") ) { return MimeType.Doc; }

        if ( span.SequenceEqual("docx") || span.SequenceEqual(".docx") ) { return MimeType.Docx; }

        if ( span.SequenceEqual("xls") || span.SequenceEqual(".xls") ) { return MimeType.Xls; }

        if ( span.SequenceEqual("xlsx") || span.SequenceEqual(".xlsx") ) { return MimeType.Xlsx; }

        if ( span.SequenceEqual("ppt") || span.SequenceEqual(".ppt") ) { return MimeType.Ppt; }

        if ( span.SequenceEqual("pptx") || span.SequenceEqual(".pptx") ) { return MimeType.Pptx; }

        if ( span.SequenceEqual("3gppa") || span.SequenceEqual(".3gppa") ) { return MimeType.ThreeGppAudio; }

        if ( span.SequenceEqual("3gpp2a") || span.SequenceEqual(".3gpp2a") ) { return MimeType.ThreeGpp2Audio; }

        if ( span.SequenceEqual("aac") || span.SequenceEqual(".aac") ) { return MimeType.Aac; }

        if ( span.SequenceEqual("mpega") || span.SequenceEqual(".mpega") ) { return MimeType.MpegAudio; }

        if ( span.SequenceEqual("mp3") || span.SequenceEqual(".mp3") ) { return MimeType.Mp3; }

        if ( span.SequenceEqual("weba") || span.SequenceEqual(".weba") ) { return MimeType.Weba; }

        if ( span.SequenceEqual("wav") || span.SequenceEqual(".wav") ) { return MimeType.Wav; }

        if ( span.SequenceEqual("3gpp") || span.SequenceEqual(".3gpp") ) { return MimeType.ThreeGppVideo; }

        if ( span.SequenceEqual("3gpp2") || span.SequenceEqual(".3gpp2") ) { return MimeType.ThreeGpp2Video; }

        if ( span.SequenceEqual("mp4") || span.SequenceEqual(".mp4") ) { return MimeType.Mp4; }

        if ( span.SequenceEqual("mpeg") || span.SequenceEqual(".mpeg") ) { return MimeType.MpegVideo; }

        if ( span.SequenceEqual("mpeg4") || span.SequenceEqual(".mpeg4") ) { return MimeType.Mpeg4; }

        if ( span.SequenceEqual("webm") || span.SequenceEqual(".webm") ) { return MimeType.Webm; }

        if ( span.SequenceEqual("h264") || span.SequenceEqual(".h264") ) { return MimeType.H264; }

        if ( span.SequenceEqual("avi") || span.SequenceEqual(".avi") ) { return MimeType.Avi; }

        if ( span.SequenceEqual("mov") || span.SequenceEqual(".mov") ) { return MimeType.Mov; }

        if ( span.SequenceEqual("mpg") || span.SequenceEqual(".mpg") ) { return MimeType.Mpg; }

        if ( span.SequenceEqual("ogg") || span.SequenceEqual(".ogg") ) { return MimeType.Ogg; }

        if ( span.SequenceEqual("mkv") || span.SequenceEqual(".mkv") ) { return MimeType.Mkv; }

        if ( span.SequenceEqual("gif") || span.SequenceEqual(".gif") ) { return MimeType.Gif; }

        if ( span.SequenceEqual("tif") || span.SequenceEqual(".tif") ) { return MimeType.Tiff; }

        if ( span.SequenceEqual("png") || span.SequenceEqual(".png") ) { return MimeType.Png; }

        if ( span.SequenceEqual("jpeg") || span.SequenceEqual(".jpeg") ) { return MimeType.Jpeg; }

        if ( span.SequenceEqual("jpg") || span.SequenceEqual(".jpg") ) { return MimeType.Jpg; }

        if ( span.SequenceEqual("bmp") || span.SequenceEqual(".bmp") ) { return MimeType.Bmp; }

        if ( span.SequenceEqual("webp") || span.SequenceEqual(".webp") ) { return MimeType.Webp; }

        if ( span.SequenceEqual("ico") || span.SequenceEqual(".ico") ) { return MimeType.Icon; }

        if ( span.SequenceEqual("svg") || span.SequenceEqual(".svg") ) { return MimeType.Svg; }

        if ( span.SequenceEqual("ttf") || span.SequenceEqual(".ttf") ) { return MimeType.TrueType; }

        if ( span.SequenceEqual("otf") || span.SequenceEqual(".otf") ) { return MimeType.OpenType; }

        if ( span.SequenceEqual("fd") || span.SequenceEqual(".fd") ) { return MimeType.FormData; }

        if ( span.SequenceEqual("dat") || span.SequenceEqual(".dat") ) { return MimeType.Unknown; }

        return MimeType.NotSet;
    }


    /// <summary>
    ///     Converts the provided ContentType
    ///     <see cref = "string" />
    ///     to it's associated
    ///     <see cref = "MimeType" />
    ///     via
    ///     <seealso cref = "MimeTypeNames" />
    /// </summary>
    /// <param name = "mime" > </param>
    /// <returns>
    ///     <see cref = "MimeType" />
    /// </returns>
    public static MimeType ToMimeType( this string mime ) =>
        mime switch
        {
            null => throw new NullReferenceException(nameof(mime)),

            MimeTypeNames.Text.PLAIN                  => MimeType.PlainText,
            MimeTypeNames.Text.HTML                   => MimeType.Html,
            MimeTypeNames.Text.XML                    => MimeType.Xml,
            MimeTypeNames.Text.XAML                   => MimeType.Xaml,
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
    ///     Converts the provided
    ///     <see cref = "MimeType" />
    ///     to it's associated ContentType via
    ///     <seealso cref = "MimeTypeNames" />
    /// </summary>
    /// <param name = "mime" > </param>
    /// <returns>
    ///     <see cref = "string" />
    /// </returns>
    public static string ToContentType( this MimeType mime ) =>
        mime switch
        {
            MimeType.Text      => MimeTypeNames.Text.PLAIN,
            MimeType.PlainText => MimeTypeNames.Text.PLAIN,
            MimeType.Html      => MimeTypeNames.Text.HTML,
            MimeType.Xml       => MimeTypeNames.Text.XML,
            MimeType.Xaml      => MimeTypeNames.Text.XAML,
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

            MimeType.NotSet => throw new ArgumentOutOfRangeException(nameof(mime), mime, null),
            _               => throw new ArgumentOutOfRangeException(nameof(mime), mime, null)
        };
}
