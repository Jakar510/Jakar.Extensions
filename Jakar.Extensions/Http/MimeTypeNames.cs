// unset


namespace Jakar.Extensions.Http;


/// <summary>
///     <para>
///         <seealso href = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types" />
///     </para>
///     <para>
///         <seealso href = "https://www.freeformatter.com/mime-types-list.html" />
///     </para>
/// </summary>
public static class MimeTypeNames
{
    public static class Text
    {
        public const string PLAIN                  = "text/plain";
        public const string HTML                   = "text/html";
        public const string XML                    = "text/xml";
        public const string RICH_TEXT              = "text/richtext"; // rtf
        public const string CASCADING_STYLE_SHEETS = "text/css";
        public const string COMMA_SEPARATED_VALUES = "text/csv";
        public const string CALENDAR               = "text/calendar"; // ics
        public const string INI                    = "text/ini";
        public const string CONFIG                 = "text/Config";
        public const string CONFIGURATION          = "text/Configuration";
    }



    public static class Application
    {
        public const string URL_ENCODED_CONTENT = "application/x-www-form-urlencoded";

        public const string BINARY = "application/octet-stream";

        public const string SOAP                     = "application/soap+xml";
        public const string RTF                      = "application/rtf";
        public const string BASE64                   = "application/base64";
        public const string PDF                      = "application/pdf";
        public const string JSON                     = "application/json";
        public const string XML                      = "application/xml";
        public const string JAVA_SCRIPT              = "application/js";
        public const string VBS                      = "application/vbs";
        public const string DLL                      = "application/dll";
        public const string XUL                      = "application/vnd.mozilla.xul+xml";
        public const string LICENSES                 = "application/Licenses";
        public const string SAFETY_DATA_SHEET        = "application/pdf+safety-data-sheet";
        public const string TECHNICAL_DATA_SHEET     = "application/pdf+technical-data-sheet";
        public const string CERTIFICATE_AUTHENTICITY = "application/authenticity-certificate";



        public static class Archive
        {
            public const string TAR       = "application/tar+gzip"; // tar.gz
            public const string GZIP      = "application/gzip";     // gz
            public const string ZIP       = "application/zip";
            public const string B_ZIP     = "application/x-bzip";  // bz
            public const string B_ZIP_2   = "application/x-bzip2"; // bz2
            public const string SEVEN_ZIP = "application/x-7z-compressed";
        }



        public static class MsOffice
        {
            public const string DOC  = "application/msword";
            public const string DOCX = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            public const string XLS  = "application/vnd.ms-excel";
            public const string XLSX = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            public const string PPT  = "application/vnd.ms-powerpoint";
            public const string PPTX = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
        }
    }



    public static class Audio
    {
        public const string THREE_GPP_AUDIO  = "audio/3gpp";
        public const string THREE_GPP2_AUDIO = "audio/3gpp2";
        public const string AAC              = "audio/aac";
        public const string MPEG             = "audio/mpeg";
        public const string MP3              = "audio/mp3";
        public const string WEBA             = "audio/webm";
        public const string WAVE             = "audio/wav";
    }



    public static class Video
    {
        public const string THREE_GPP_VIDEO  = "video/3gpp";
        public const string THREE_GPP2_VIDEO = "video/3gpp2";
        public const string MP4              = "video/mp4";
        public const string MPEG             = "video/mpeg";
        public const string MPG              = "video/mpg";
        public const string MPEG4            = "video/mpeg4";
        public const string WEBM             = "video/webm";
        public const string H264             = "video/h264";
        public const string MOV              = "video/mov";
        public const string OGG              = "video/ogg";
        public const string MKV              = "video/mkv";
        public const string AVI              = "video/x-msvideo";
    }



    public static class Image
    {
        public const string GIF  = "image/gif";
        public const string TIFF = "image/tiff";
        public const string JPEG = "image/jpeg";
        public const string JPG  = "image/jpg";
        public const string PNG  = "image/png";
        public const string BMP  = "image/bmp";
        public const string WEBP = "image/webp";
        public const string ICON = "image/x-icon"; // ico
        public const string SVG  = "image/svg+xml";
    }



    public static class Font
    {
        public const string TRUE_TYPE = "font/ttf";
        public const string OPEN_TYPE = "font/otf";
    }



    public static class MultiPart
    {
        public const string FORM_DATA = "multipart/form-data";
    }
}
