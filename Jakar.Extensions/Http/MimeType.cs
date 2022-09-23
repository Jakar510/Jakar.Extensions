// unset


#nullable enable
namespace Jakar.Extensions;


public enum MimeType
{
    NotSet,
    Unknown,

    // Text
    Text,
    PlainText,
    Html,
    Xml,
    Xaml,
    RichText,
    Css,
    Csv,
    Calendar,
    Ini,
    Config,
    Cfg,

    // application
    UrlEncodedContent,
    Soap,
    Binary,
    Stream,
    Rtf,
    Pdf,
    Json,
    XmlApp,
    Xul,
    JavaScript,
    Vbs,
    Base64,
    Sds,
    Tds,
    Coa,

    // application.archive
    Zip,
    SevenZip,
    Bzip,
    Bzip2,
    Gzip,
    Tar,

    // application.office
    Doc,
    Docx,
    Xls,
    Xlsx,
    Ppt,
    Pptx,

    // audio
    ThreeGppAudio,
    ThreeGpp2Audio,
    Aac,
    MpegAudio,
    Mp3,
    Weba,
    Wav,

    // video
    ThreeGppVideo,
    ThreeGpp2Video,
    Mp4,
    MpegVideo,
    Mpeg4,
    Webm,
    H264,
    Avi,
    Mov,
    Mpg,
    Ogg,
    Mkv,

    // images
    Gif,
    Tiff,
    Png,
    Jpeg,
    Jpg,
    Bmp,
    Webp,
    Icon,
    Svg,

    // font
    TrueType,
    OpenType,

    // multipart
    FormData,

    // misc
    Licenses,
    Dll
}
