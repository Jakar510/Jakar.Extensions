using Jakar.Extensions.Http;
using NUnit.Framework;


namespace Jakar.Extensions.Tests.Http;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class MimeType_ContentType_Tests : Assert
{
#region Test_ToMimeType

    [Test]
    [TestCase(MimeTypeNames.Text.CONFIG, MimeType.Cfg)]
    [TestCase(MimeTypeNames.Text.CONFIGURATION, MimeType.Config)]
    [TestCase(MimeTypeNames.Text.INI, MimeType.Ini)]
    [TestCase(MimeTypeNames.Text.CALENDAR, MimeType.Calendar)]
    [TestCase(MimeTypeNames.Text.COMMA_SEPARATED_VALUES, MimeType.Csv)]
    [TestCase(MimeTypeNames.Text.CASCADING_STYLE_SHEETS, MimeType.Css)]
    [TestCase(MimeTypeNames.Text.RICH_TEXT, MimeType.RichText)]
    [TestCase(MimeTypeNames.Text.XML, MimeType.Xml)]
    [TestCase(MimeTypeNames.Text.HTML, MimeType.Html)]
    [TestCase(MimeTypeNames.Text.PLAIN, MimeType.PlainText)]
    public void Test_ToMimeType_Text( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Application.URL_ENCODED_CONTENT, MimeType.UrlEncodedContent)]
    [TestCase(MimeTypeNames.Application.SOAP, MimeType.Soap)]
    [TestCase(MimeTypeNames.Application.BINARY, MimeType.Stream)]
    [TestCase(MimeTypeNames.Application.BASE64, MimeType.Base64)]
    [TestCase(MimeTypeNames.Application.RTF, MimeType.Rtf)]
    [TestCase(MimeTypeNames.Application.PDF, MimeType.Pdf)]
    [TestCase(MimeTypeNames.Application.JSON, MimeType.Json)]
    [TestCase(MimeTypeNames.Application.XML, MimeType.XmlApp)]
    [TestCase(MimeTypeNames.Application.XUL, MimeType.Xul)]
    [TestCase(MimeTypeNames.Application.JAVA_SCRIPT, MimeType.JavaScript)]
    [TestCase(MimeTypeNames.Application.VBS, MimeType.Vbs)]
    [TestCase(MimeTypeNames.Application.LICENSES, MimeType.Licenses)]
    public void Test_ToMimeType_Applications( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Application.Archive.ZIP, MimeType.Zip)]
    [TestCase(MimeTypeNames.Application.Archive.SEVEN_ZIP, MimeType.SevenZip)]
    [TestCase(MimeTypeNames.Application.Archive.B_ZIP, MimeType.Bzip)]
    [TestCase(MimeTypeNames.Application.Archive.B_ZIP_2, MimeType.Bzip2)]
    [TestCase(MimeTypeNames.Application.Archive.GZIP, MimeType.Gzip)]
    [TestCase(MimeTypeNames.Application.Archive.TAR, MimeType.Tar)]
    public void Test_ToMimeType_Archives( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Application.MsOffice.DOC, MimeType.Doc)]
    [TestCase(MimeTypeNames.Application.MsOffice.DOCX, MimeType.Docx)]
    [TestCase(MimeTypeNames.Application.MsOffice.XLS, MimeType.Xls)]
    [TestCase(MimeTypeNames.Application.MsOffice.XLSX, MimeType.Xlsx)]
    [TestCase(MimeTypeNames.Application.MsOffice.PPT, MimeType.Ppt)]
    [TestCase(MimeTypeNames.Application.MsOffice.PPTX, MimeType.Pptx)]
    public void Test_ToMimeType_Office( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Audio.THREE_GPP2_AUDIO, MimeType.ThreeGpp2Audio)]
    [TestCase(MimeTypeNames.Audio.THREE_GPP_AUDIO, MimeType.ThreeGppAudio)]
    [TestCase(MimeTypeNames.Audio.AAC, MimeType.Aac)]
    [TestCase(MimeTypeNames.Audio.MPEG, MimeType.MpegAudio)]
    [TestCase(MimeTypeNames.Audio.MP3, MimeType.Mp3)]
    [TestCase(MimeTypeNames.Audio.WEBA, MimeType.Weba)]
    [TestCase(MimeTypeNames.Audio.WAVE, MimeType.Wav)]
    public void Test_ToMimeType_Audio( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Video.THREE_GPP2_VIDEO, MimeType.ThreeGpp2Video)]
    [TestCase(MimeTypeNames.Video.THREE_GPP_VIDEO, MimeType.ThreeGppVideo)]
    [TestCase(MimeTypeNames.Video.MP4, MimeType.Mp4)]
    [TestCase(MimeTypeNames.Video.MPEG, MimeType.MpegVideo)]
    [TestCase(MimeTypeNames.Video.MPEG4, MimeType.Mpeg4)]
    [TestCase(MimeTypeNames.Video.WEBM, MimeType.Webm)]
    [TestCase(MimeTypeNames.Video.H264, MimeType.H264)]
    [TestCase(MimeTypeNames.Video.AVI, MimeType.Avi)]
    [TestCase(MimeTypeNames.Video.MOV, MimeType.Mov)]
    [TestCase(MimeTypeNames.Video.MPG, MimeType.Mpg)]
    [TestCase(MimeTypeNames.Video.OGG, MimeType.Ogg)]
    [TestCase(MimeTypeNames.Video.MKV, MimeType.Mkv)]
    public void Test_ToMimeType_Videos( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Image.GIF, MimeType.Gif)]
    [TestCase(MimeTypeNames.Image.TIFF, MimeType.Tiff)]
    [TestCase(MimeTypeNames.Image.PNG, MimeType.Png)]
    [TestCase(MimeTypeNames.Image.JPEG, MimeType.Jpeg)]
    [TestCase(MimeTypeNames.Image.JPG, MimeType.Jpg)]
    [TestCase(MimeTypeNames.Image.BMP, MimeType.Bmp)]
    [TestCase(MimeTypeNames.Image.WEBP, MimeType.Webp)]
    [TestCase(MimeTypeNames.Image.ICON, MimeType.Icon)]
    [TestCase(MimeTypeNames.Image.SVG, MimeType.Svg)]
    public void Test_ToMimeType_Images( string s, MimeType mime ) => Test_ToMimeType(s, mime);


    [TestCase(MimeTypeNames.Font.TRUE_TYPE, MimeType.TrueType)]
    [TestCase(MimeTypeNames.Font.OPEN_TYPE, MimeType.OpenType)]
    [TestCase(MimeTypeNames.MultiPart.FORM_DATA, MimeType.FormData)]
    public void Test_ToMimeType_Fonts( string s, MimeType mime ) => Test_ToMimeType(s, mime);

    private static void Test_ToMimeType( string s, MimeType mime ) => AreEqual(s.ToMimeType(), mime);

    #endregion


#region Test_ToContentType

    [Test]
    [TestCase(MimeTypeNames.Text.CONFIG, MimeType.Cfg)]
    [TestCase(MimeTypeNames.Text.CONFIGURATION, MimeType.Config)]
    [TestCase(MimeTypeNames.Text.INI, MimeType.Ini)]
    [TestCase(MimeTypeNames.Text.CALENDAR, MimeType.Calendar)]
    [TestCase(MimeTypeNames.Text.COMMA_SEPARATED_VALUES, MimeType.Csv)]
    [TestCase(MimeTypeNames.Text.CASCADING_STYLE_SHEETS, MimeType.Css)]
    [TestCase(MimeTypeNames.Text.RICH_TEXT, MimeType.RichText)]
    [TestCase(MimeTypeNames.Text.XML, MimeType.Xml)]
    [TestCase(MimeTypeNames.Text.HTML, MimeType.Html)]
    [TestCase(MimeTypeNames.Text.PLAIN, MimeType.PlainText)]
    public void Test_ToContentType_Text( string s, MimeType mime ) => Test_ToContentType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Application.URL_ENCODED_CONTENT, MimeType.UrlEncodedContent)]
    [TestCase(MimeTypeNames.Application.SOAP, MimeType.Soap)]
    [TestCase(MimeTypeNames.Application.BINARY, MimeType.Binary)]
    [TestCase(MimeTypeNames.Application.BASE64, MimeType.Base64)]
    [TestCase(MimeTypeNames.Application.RTF, MimeType.Rtf)]
    [TestCase(MimeTypeNames.Application.PDF, MimeType.Pdf)]
    [TestCase(MimeTypeNames.Application.JSON, MimeType.Json)]
    [TestCase(MimeTypeNames.Application.XML, MimeType.XmlApp)]
    [TestCase(MimeTypeNames.Application.XUL, MimeType.Xul)]
    [TestCase(MimeTypeNames.Application.JAVA_SCRIPT, MimeType.JavaScript)]
    [TestCase(MimeTypeNames.Application.VBS, MimeType.Vbs)]
    [TestCase(MimeTypeNames.Application.LICENSES, MimeType.Licenses)]
    public void Test_ToContentType_Applications( string s, MimeType mime ) => Test_ToContentType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Application.Archive.ZIP, MimeType.Zip)]
    [TestCase(MimeTypeNames.Application.Archive.SEVEN_ZIP, MimeType.SevenZip)]
    [TestCase(MimeTypeNames.Application.Archive.B_ZIP, MimeType.Bzip)]
    [TestCase(MimeTypeNames.Application.Archive.B_ZIP_2, MimeType.Bzip2)]
    [TestCase(MimeTypeNames.Application.Archive.GZIP, MimeType.Gzip)]
    [TestCase(MimeTypeNames.Application.Archive.TAR, MimeType.Tar)]
    public void Test_ToContentType_Archives( string s, MimeType mime ) => Test_ToContentType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Application.MsOffice.DOC, MimeType.Doc)]
    [TestCase(MimeTypeNames.Application.MsOffice.DOCX, MimeType.Docx)]
    [TestCase(MimeTypeNames.Application.MsOffice.XLS, MimeType.Xls)]
    [TestCase(MimeTypeNames.Application.MsOffice.XLSX, MimeType.Xlsx)]
    [TestCase(MimeTypeNames.Application.MsOffice.PPT, MimeType.Ppt)]
    [TestCase(MimeTypeNames.Application.MsOffice.PPTX, MimeType.Pptx)]
    public void Test_ToContentType_Office( string s, MimeType mime ) => Test_ToContentType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Audio.THREE_GPP2_AUDIO, MimeType.ThreeGpp2Audio)]
    [TestCase(MimeTypeNames.Audio.THREE_GPP_AUDIO, MimeType.ThreeGppAudio)]
    [TestCase(MimeTypeNames.Audio.AAC, MimeType.Aac)]
    [TestCase(MimeTypeNames.Audio.MPEG, MimeType.MpegAudio)]
    [TestCase(MimeTypeNames.Audio.MP3, MimeType.Mp3)]
    [TestCase(MimeTypeNames.Audio.WEBA, MimeType.Weba)]
    [TestCase(MimeTypeNames.Audio.WAVE, MimeType.Wav)]
    public void Test_ToContentType_Audio( string s, MimeType mime ) => Test_ToContentType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Video.THREE_GPP2_VIDEO, MimeType.ThreeGpp2Video)]
    [TestCase(MimeTypeNames.Video.THREE_GPP_VIDEO, MimeType.ThreeGppVideo)]
    [TestCase(MimeTypeNames.Video.MP4, MimeType.Mp4)]
    [TestCase(MimeTypeNames.Video.MPEG, MimeType.MpegVideo)]
    [TestCase(MimeTypeNames.Video.MPEG4, MimeType.Mpeg4)]
    [TestCase(MimeTypeNames.Video.WEBM, MimeType.Webm)]
    [TestCase(MimeTypeNames.Video.H264, MimeType.H264)]
    [TestCase(MimeTypeNames.Video.AVI, MimeType.Avi)]
    [TestCase(MimeTypeNames.Video.MOV, MimeType.Mov)]
    [TestCase(MimeTypeNames.Video.MPG, MimeType.Mpg)]
    [TestCase(MimeTypeNames.Video.OGG, MimeType.Ogg)]
    [TestCase(MimeTypeNames.Video.MKV, MimeType.Mkv)]
    public void Test_ToContentType_Video( string s, MimeType mime ) => Test_ToContentType(s, mime);

    [Test]
    [TestCase(MimeTypeNames.Image.GIF, MimeType.Gif)]
    [TestCase(MimeTypeNames.Image.TIFF, MimeType.Tiff)]
    [TestCase(MimeTypeNames.Image.PNG, MimeType.Png)]
    [TestCase(MimeTypeNames.Image.JPEG, MimeType.Jpeg)]
    [TestCase(MimeTypeNames.Image.JPG, MimeType.Jpg)]
    [TestCase(MimeTypeNames.Image.BMP, MimeType.Bmp)]
    [TestCase(MimeTypeNames.Image.WEBP, MimeType.Webp)]
    [TestCase(MimeTypeNames.Image.ICON, MimeType.Icon)]
    [TestCase(MimeTypeNames.Image.SVG, MimeType.Svg)]
    public void Test_ToContentType_Images( string s, MimeType mime ) => Test_ToContentType(s, mime);


    [Test]
    [TestCase(MimeTypeNames.Font.TRUE_TYPE, MimeType.TrueType)]
    [TestCase(MimeTypeNames.Font.OPEN_TYPE, MimeType.OpenType)]
    [TestCase(MimeTypeNames.MultiPart.FORM_DATA, MimeType.FormData)]
    public void Test_ToContentType_Fonts( string s, MimeType mime ) => Test_ToContentType(s, mime);

    private static void Test_ToContentType( string s, MimeType mime ) => AreEqual(s, mime.ToContentType());

    #endregion


#region Test_ContentType

    [Test]

    // TEXT
    [TestCase(MimeTypeNames.Text.CONFIG)]
    [TestCase(MimeTypeNames.Text.CONFIGURATION)]
    [TestCase(MimeTypeNames.Text.INI)]
    [TestCase(MimeTypeNames.Text.CALENDAR)]
    [TestCase(MimeTypeNames.Text.COMMA_SEPARATED_VALUES)]
    [TestCase(MimeTypeNames.Text.CASCADING_STYLE_SHEETS)]
    [TestCase(MimeTypeNames.Text.RICH_TEXT)]
    [TestCase(MimeTypeNames.Text.XML)]
    [TestCase(MimeTypeNames.Text.HTML)]
    [TestCase(MimeTypeNames.Text.PLAIN)]

    // GENERIC APPLICATIONS
    [TestCase(MimeTypeNames.Application.URL_ENCODED_CONTENT)]
    [TestCase(MimeTypeNames.Application.SOAP)]
    [TestCase(MimeTypeNames.Application.BINARY)]
    [TestCase(MimeTypeNames.Application.BASE64)]
    [TestCase(MimeTypeNames.Application.RTF)]
    [TestCase(MimeTypeNames.Application.PDF)]
    [TestCase(MimeTypeNames.Application.JSON)]
    [TestCase(MimeTypeNames.Application.XML)]
    [TestCase(MimeTypeNames.Application.XUL)]
    [TestCase(MimeTypeNames.Application.JAVA_SCRIPT)]
    [TestCase(MimeTypeNames.Application.VBS)]
    [TestCase(MimeTypeNames.Application.LICENSES)]

    // ARCHIVES
    [TestCase(MimeTypeNames.Application.Archive.ZIP)]
    [TestCase(MimeTypeNames.Application.Archive.SEVEN_ZIP)]
    [TestCase(MimeTypeNames.Application.Archive.B_ZIP)]
    [TestCase(MimeTypeNames.Application.Archive.B_ZIP_2)]
    [TestCase(MimeTypeNames.Application.Archive.GZIP)]
    [TestCase(MimeTypeNames.Application.Archive.TAR)]

    // OFFICE
    [TestCase(MimeTypeNames.Application.MsOffice.DOC)]
    [TestCase(MimeTypeNames.Application.MsOffice.DOCX)]
    [TestCase(MimeTypeNames.Application.MsOffice.XLS)]
    [TestCase(MimeTypeNames.Application.MsOffice.XLSX)]
    [TestCase(MimeTypeNames.Application.MsOffice.PPT)]
    [TestCase(MimeTypeNames.Application.MsOffice.PPTX)]

    // AUDIO
    [TestCase(MimeTypeNames.Audio.THREE_GPP2_AUDIO)]
    [TestCase(MimeTypeNames.Audio.THREE_GPP_AUDIO)]
    [TestCase(MimeTypeNames.Audio.AAC)]
    [TestCase(MimeTypeNames.Audio.MPEG)]
    [TestCase(MimeTypeNames.Audio.MP3)]
    [TestCase(MimeTypeNames.Audio.WEBA)]
    [TestCase(MimeTypeNames.Audio.WAVE)]

    // VIDEOS
    [TestCase(MimeTypeNames.Video.THREE_GPP2_VIDEO)]
    [TestCase(MimeTypeNames.Video.THREE_GPP_VIDEO)]
    [TestCase(MimeTypeNames.Video.MP4)]
    [TestCase(MimeTypeNames.Video.MPEG)]
    [TestCase(MimeTypeNames.Video.MPEG4)]
    [TestCase(MimeTypeNames.Video.WEBM)]
    [TestCase(MimeTypeNames.Video.H264)]
    [TestCase(MimeTypeNames.Video.AVI)]
    [TestCase(MimeTypeNames.Video.MOV)]
    [TestCase(MimeTypeNames.Video.MPG)]
    [TestCase(MimeTypeNames.Video.OGG)]
    [TestCase(MimeTypeNames.Video.MKV)]

    // IMAGES
    [TestCase(MimeTypeNames.Image.GIF)]
    [TestCase(MimeTypeNames.Image.TIFF)]
    [TestCase(MimeTypeNames.Image.PNG)]
    [TestCase(MimeTypeNames.Image.JPEG)]
    [TestCase(MimeTypeNames.Image.JPG)]
    [TestCase(MimeTypeNames.Image.BMP)]
    [TestCase(MimeTypeNames.Image.WEBP)]
    [TestCase(MimeTypeNames.Image.ICON)]
    [TestCase(MimeTypeNames.Image.SVG)]
    [TestCase(MimeTypeNames.Font.TRUE_TYPE)]
    [TestCase(MimeTypeNames.Font.OPEN_TYPE)]
    [TestCase(MimeTypeNames.MultiPart.FORM_DATA)]
    public void Test_ContentType( string s ) => AreEqual(s.ToMimeType().ToContentType(), s);

    #endregion
}
