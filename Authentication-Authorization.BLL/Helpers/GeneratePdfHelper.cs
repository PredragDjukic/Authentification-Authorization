using IronPdf;

namespace Authentication_Authorization.BLL.Helpers
{
    public static class GeneratePdfHelper
    {
        public static byte[] GeneratePdf(string html)
        {
            HtmlToPdf renderer = new HtmlToPdf();
            byte[] pdf = renderer.RenderHtmlAsPdf(html).BinaryData;

            return pdf;
        }
    }
}
