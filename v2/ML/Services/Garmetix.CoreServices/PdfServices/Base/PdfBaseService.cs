// Assuming these namespaces exist in your project.
// If not, you might need to adjust or create dummy classes.
// For SentrySdk, if it's truly used globally.
// using Sentry;
//Final Version 
using Syncfusion.Pdf.Graphics;

namespace Garmetix.Services.PdfServices.Base
{
    internal class PdfBaseService
    {
        public required PdfService pdfService;

        // Define fonts and brushes for consistent styling
        protected PdfFont titleFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 20, PdfFontStyle.Bold);

        protected PdfFont headingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 12, PdfFontStyle.Bold);
        protected PdfFont regularFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        protected PdfFont balanceFont = new PdfStandardFont(PdfFontFamily.Courier, 14, PdfFontStyle.Bold); // Renamed for clarity
  
        protected PdfBrush blackBrush = new PdfSolidBrush(new PdfColor(0, 0, 0));
        protected PdfBrush grayBrush = new PdfSolidBrush(new PdfColor(100, 100, 100));
        
        protected PdfBrush tableHeaderBrush = new PdfSolidBrush(new PdfColor(230, 230, 230));
        protected PdfBrush tableEvenRowBrush = new PdfSolidBrush(new PdfColor(245, 245, 245));
        public async Task Init()
        {
            if (pdfService == null)
                pdfService = (await PdfService.CreateAsync(1))!;

            if (pdfService.PdfStyles == null)
                pdfService.PdfStyles = await PdfStyles.CreateAsync(1);
        }
        public PdfBaseService()
        {
            _=Init();
        }
    }
}