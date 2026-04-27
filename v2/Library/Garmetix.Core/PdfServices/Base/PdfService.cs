// Assuming these namespaces exist in your project.
// If not, you might need to adjust or create dummy classes.
using Bharat.ToolKits.Helpers;
using Garmetix.Databases;
using Garmetix.Databases.Services;
using Garmetix.PdfServices.Interfaces;
using Sentry;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using PointF = Syncfusion.Drawing.PointF;
using SizeF = Syncfusion.Drawing.SizeF;

// For SentrySdk, if it's truly used globally.
// using Sentry;
//Final Version
namespace Garmetix.PdfServices.Base
{
    /// <summary>
    /// Provides services for generating PDF documents, including voucher and report generation.
    /// This class must be initialized asynchronously using the <see cref="CreateAsync"/> factory method.
    /// </summary>
    internal class PdfService : IPdfService, IDisposable
    {
        // --- Company Details Information (Read-only static for consistency) ---
        // These values are loaded once from Preferences.
        public static readonly string CompanyName = Preferences.Get("CompanyName", "Aadwika Fashions");

        public static readonly string CompanyAddress = Preferences.Get("Address", "Bhagalpur Road, Dumka, Jharkhand");
        public static readonly string CompanyPhone = Preferences.Get("Phone", "06434224461");
        public static readonly string CompanyEmail = Preferences.Get("Email", "aadwikafashion@gmail.com");
        public static readonly string CompanyGstin = Preferences.Get("GSTIN", "20AJHPA7396P1ZV");

        // Static access to the database context, assuming it's correctly managed.
        public static DatabaseContext Db => DatabaseService.Instance.LocalDB;

        private static string _lastGeneratedPdfPath = string.Empty;
        public static string LastGeneratedPdfPath => _lastGeneratedPdfPath;

        // Instance specific multiplier for font sizes, allows dynamic scaling.
        public float FontSizeMultiplier { get; private set; } = 1f;

        // Instance of PdfStyles, which manages fonts and brushes. Must be initialized asynchronously.
        public PdfStyles? PdfStyles=PdfStyles.Instance; // Make nullable to indicate it's not initialized in constructor.

        private bool _disposed = false; // To track disposal status.

        /// <summary>
        /// Private constructor to enforce asynchronous initialization via <see cref="CreateAsync"/>.
        /// </summary>
        private PdfService()
        { }

        /// <summary>
        /// Asynchronously creates and initializes a new instance of the <see cref="PdfService"/> class.
        /// This is the recommended way to get a fully initialized <see cref="PdfService"/> object.
        /// </summary>
        /// <param name="fontSizeMultiplier">Optional multiplier for dynamic font sizes. Defaults to 1.</param>
        /// <returns>A fully initialized <see cref="PdfService"/> instance, or null if initialization fails.</returns>
        public static async Task<PdfService?> CreateAsync(float fontSizeMultiplier = 1f)
        {
            var instance = new PdfService
            {
                FontSizeMultiplier = fontSizeMultiplier
            };

            if (instance.PdfStyles != null)
            {
                // If PdfStyles is already initialized, return the existing instance.
                return instance;
            }
            // Initialize PdfStyles asynchronously. This is crucial.
            instance.PdfStyles = await PdfStyles.CreateAsync(fontSizeMultiplier);

            if (instance.PdfStyles == null)
            {
                // If PdfStyles failed to initialize, dispose the PdfService instance and return null.
                instance.Dispose();
                return null;
            }

            return instance;
        }

        // ---

        //  ### Static Helper Functions

        //  These functions provide common PDF drawing operations that don't depend on an instance of <see cref="PdfService"/>.

        /// <summary>
        /// Draws the company and report title section, typically used for ledgers.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="currentY">The current Y-coordinate for drawing, updated by the method.</param>
        /// <param name="titleName">The title of the report (e.g., "LEDGER").</param>
        /// <param name="pageWidth">The width of the PDF page.</param>
        /// <returns>The updated Y-coordinate after drawing this section.</returns>
        public static float CompanyReportTitle(ref PdfGraphics graphics, float currentY, string titleName, float pageWidth)
        {
            // Center the report title
            graphics.DrawString(titleName, PdfStyles.TitleFont, PdfStyles.BlackBrush,
                                new PointF(pageWidth / 2, currentY),
                                new PdfStringFormat { Alignment = PdfTextAlignment.Center, LineAlignment = PdfVerticalAlignment.Middle });
            currentY += PdfStyles.TitleFont.Height - 2;

            // Draw a line separating the title from company info.
            graphics.DrawLine(PdfPens.Red, 0, currentY, pageWidth, currentY);
            currentY += 5; // Add some padding

            // Draw company name
            graphics.DrawString(CompanyName, PdfStyles.TitleFont, PdfStyles.BlackBrush, new PointF(180, currentY)); // Consider centering or dynamic positioning
            currentY += PdfStyles.TitleFont.Height;

            // Draw company address
            graphics.DrawString(CompanyAddress, PdfStyles.HeadingFont, PdfStyles.BlackBrush, new PointF(130, currentY)); // Consider centering or dynamic positioning
            currentY += PdfStyles.HeadingFont.Height + 5;

            // Draw another separator line
            graphics.DrawLine(PdfPens.Red, 0, currentY, pageWidth, currentY);
            currentY += 5; // Add some padding

            return currentY;
        }

        /// <summary>
        /// Draws party information, suitable for ledgers or other detailed documents.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="currentY">The current Y-coordinate for drawing, updated by the method.</param>
        /// <param name="title">The title for the party information section (e.g., "Party Info").</param>
        /// <param name="partyName">The name of the party.</param>
        /// <param name="address">The address of the party/department.</param>
        /// <param name="phone">The phone number of the party.</param>
        /// <param name="email">The email of the party.</param>
        /// <param name="gstin">The GSTIN of the party (if applicable).</param>
        /// <param name="ledgerType">The type of ledger (e.g., "Customer", "Supplier").</param>
        /// <param name="isParty">A flag indicating if the information is for a 'Party' (true) or 'Department' (false).</param>
        /// <returns>The updated Y-coordinate after drawing this section.</returns>
        public static float PartyInformation(ref PdfGraphics graphics, float currentY, string title, string partyName, string address, string phone, string email, string gstin, string ledgerType, bool isParty = false)
        {
            float labelWidth = 80;
            float valueX = labelWidth + 10;
            float lineSpacing = PdfStyles.RegularFont.Height + 5; // Consistent line spacing

            // Section Title
            graphics.DrawString($"{title}:", PdfStyles.HeadingFont, PdfStyles.BlackBrush, new PointF(0, currentY));
            currentY += PdfStyles.HeadingFont.Height + 5;

            // Name/Party Name
            graphics.DrawString(isParty ? "Party Name:" : "Name:", PdfStyles.RegularFont, PdfStyles.GrayBrush, new PointF(0, currentY));
            graphics.DrawString(partyName, PdfStyles.RegularFont, PdfStyles.BlackBrush, new PointF(valueX, currentY));
            currentY += lineSpacing;

            // Address/Department
            graphics.DrawString(isParty ? "Address:" : "Department:", PdfStyles.RegularFont, PdfStyles.GrayBrush, new PointF(0, currentY));
            graphics.DrawString(address, PdfStyles.RegularFont, PdfStyles.BlackBrush, new PointF(valueX, currentY));
            currentY += lineSpacing;

            // Phone
            graphics.DrawString("Phone:", PdfStyles.RegularFont, PdfStyles.GrayBrush, new PointF(0, currentY));
            graphics.DrawString(phone, PdfStyles.RegularFont, PdfStyles.BlackBrush, new PointF(valueX, currentY));
            currentY += lineSpacing;

            // Email
            graphics.DrawString("Email:", PdfStyles.RegularFont, PdfStyles.GrayBrush, new PointF(0, currentY));
            graphics.DrawString(email, PdfStyles.RegularFont, PdfStyles.BlackBrush, new PointF(valueX, currentY));

            if (isParty)
            {
                currentY += lineSpacing;
                // GSTIN
                graphics.DrawString("GSTIN:", PdfStyles.RegularFont, PdfStyles.GrayBrush, new PointF(0, currentY));
                graphics.DrawString(gstin, PdfStyles.RegularFont, PdfStyles.BlackBrush, new PointF(valueX, currentY));
                currentY += lineSpacing;
                // Ledger Type
                graphics.DrawString("Ledger Type:", PdfStyles.RegularFont, PdfStyles.GrayBrush, new PointF(0, currentY));
                graphics.DrawString(ledgerType, PdfStyles.RegularFont, PdfStyles.BlackBrush, new PointF(valueX, currentY));
            }
            currentY += lineSpacing + 10; // Extra padding at the end of the section.

            return currentY;
        }

        /// <summary>
        /// Creates a new <see cref="PdfDocument"/> with specified page size and default settings.
        /// </summary>
        /// <param name="pdfSize">The size of the PDF page.</param>
        /// <returns>A new <see cref="PdfDocument"/> instance.</returns>
        public static PdfDocument CreateNewPdfDocument(SizeF pdfSize)
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Orientation = PdfPageOrientation.Portrait;
            document.PageSettings.Margins.All = 20; // Default margins
            document.PageSettings.Size = pdfSize;
            return document;
        }

        // ---

        // ### Basic PDF Generation Helpers

        //These methods assist in setting up PDF documents and handling basic operations like saving.

        /// <summary>
        /// Configures PDF document settings based on whether duplicates are being printed.
        /// This method also adjusts the global <see cref="FontSizeMultiplier"/>.
        /// </summary>
        /// <param name="printDuplicate">If true, settings are optimized for two vouchers on A5. If false, for a single voucher on Landscape.</param>
        /// <returns>A new <see cref="PdfDocument"/> instance with the configured settings.</returns>
        public PdfDocument SetupDocumentSettings(bool printDuplicate = true)
        {
            // Ensure PdfStyles is initialized before accessing its properties.
            if (PdfStyles == null)
            {
                // This scenario should ideally not happen if PdfService is created via CreateAsync.
                throw new InvalidOperationException("PdfStyles is not initialized. Ensure PdfService.CreateAsync was awaited.");
            }

            PdfDocument document;
            if (!printDuplicate)
            {
                FontSizeMultiplier = 1.5f; // Larger font for single voucher
                document = CreateNewPdfDocument(PdfPageSize.A5); // Use A5 for single voucher
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 25;
            }
            else
            {
                FontSizeMultiplier = 1f; // Standard font for duplicate vouchers
                document = CreateNewPdfDocument(PdfPageSize.A5); // Use A5 for duplicate vouchers
                document.PageSettings.Orientation = PdfPageOrientation.Portrait; // Default for A5 vouchers
                document.PageSettings.Margins.All = 10; // Reduced margins for two vouchers on A5
            }

            // TODO:Urgent Update PdfStyles with the new multiplier
            // Re-creating PdfStyles might be an option if font sizes are strictly tied to the multiplier,
            // but for existing PdfTrueTypeFont instances, this multiplier is only used on creation.
            // A more robust solution might involve PdfStyles having a method to update font sizes.
            // For now, assuming FontSizeMultiplier will affect newly created fonts or is used directly in drawing.
            // If the fonts in PdfStyles need to be re-instantiated with the new multiplier,
            // PdfStyles.SetFontsAsync(FontSizeMultiplier); would be called here if PdfStyles supported it.
            // As per PdfStyles, CreateAsync takes the multiplier, so we rely on its initial creation.
            // If font sizes need to dynamically change after PdfStyles creation, a mechanism for that
            // would be needed in PdfStyles itself. For now, this is left as a future consideration.

            return document;
        }
        public PdfDocument SetupDocumentSettings(SizeF pageSize,bool printDuplicate = true)
        {
            // Ensure PdfStyles is initialized before accessing its properties.
            if (PdfStyles == null)
            {
                // This scenario should ideally not happen if PdfService is created via CreateAsync.
                throw new InvalidOperationException("PdfStyles is not initialized. Ensure PdfService.CreateAsync was awaited.");
            }

            PdfDocument document;
            if (!printDuplicate)
            {
                FontSizeMultiplier = 1.5f; // Larger font for single voucher
                document = CreateNewPdfDocument(pageSize); // Use A5 for single voucher
                document.PageSettings.Orientation = PdfPageOrientation.Landscape;
                document.PageSettings.Margins.All = 25;
            }
            else
            {
                FontSizeMultiplier = 1f; // Standard font for duplicate vouchers
                document = CreateNewPdfDocument(pageSize); // Use A5 for duplicate vouchers
                document.PageSettings.Orientation = PdfPageOrientation.Portrait; // Default for A5 vouchers
                document.PageSettings.Margins.All = 10; // Reduced margins for two vouchers on A5
            }

            // TODO:Urgent Update PdfStyles with the new multiplier
            // Re-creating PdfStyles might be an option if font sizes are strictly tied to the multiplier,
            // but for existing PdfTrueTypeFont instances, this multiplier is only used on creation.
            // A more robust solution might involve PdfStyles having a method to update font sizes.
            // For now, assuming FontSizeMultiplier will affect newly created fonts or is used directly in drawing.
            // If the fonts in PdfStyles need to be re-instantiated with the new multiplier,
            // PdfStyles.SetFontsAsync(FontSizeMultiplier); would be called here if PdfStyles supported it.
            // As per PdfStyles, CreateAsync takes the multiplier, so we rely on its initial creation.
            // If font sizes need to dynamically change after PdfStyles creation, a mechanism for that
            // would be needed in PdfStyles itself. For now, this is left as a future consideration.

            return document;
        }

        /// <summary>
        /// Saves the generated PDF document to a file and optionally opens it.
        /// </summary>
        /// <param name="document">The <see cref="PdfDocument"/> to be saved.</param>
        /// <param name="fileName">The desired file name for the PDF (e.g., "Voucher_123.pdf").</param>
        /// <param name="directory">The sub-directory within the app's cache (e.g., "Vouchers", "Reports").</param>
        /// <param name="externalSave">If true, attempts to save and view the file using an external service (e.g., for sharing).</param>
        /// <returns>The full path to the saved PDF file.</returns>
        /// <exception cref="IOException">Thrown if there is an error saving the file.</exception>
        /// <exception cref="Exception">Catches other unexpected errors during file operations or launching.</exception>
        public async Task<string> SaveAndLaunchPdf(PdfDocument document, string fileName, string directory = "Vouchers", bool externalSave = false)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // Save the document to a memory stream.
                    document.Save(stream);
                    stream.Position = 0; // Reset stream position for reading.

                    // Define the file path in the app's cache directory.
                    string appSpecificDirectory = Path.Combine(FileSystem.CacheDirectory, "BharatGarmetix", directory);
                    Directory.CreateDirectory(appSpecificDirectory); // Ensure the directory exists.
                    string filePath = Path.Combine(appSpecificDirectory, fileName);

                    // Write the PDF content from the stream to the file.
                    await File.WriteAllBytesAsync(filePath, stream.ToArray()); // Use async for file writing.
                    _lastGeneratedPdfPath = filePath;

                    // Use the default launcher to open the file.
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });

                    if (externalSave)
                    {
                        // Safely attempt to save and view using an external service.
                        // Assuming Library.Helpers.ServiceHelper and Library.Services.SaveService exist.
                        // Add null checks for robustness.
                        //TODO : TO BE added Save Service
                       
                         //ServiceHelper.Current.GetService< SaveService>()?
                         //      .SaveAndView(fileName, "application/pdf", stream);
                    }

                    return filePath;
                }
            }
            catch (IOException ex)
            {
                // Specific error for file operations.
                Console.Error.WriteLine($"IOException in SaveAndLaunchPdf: {ex.Message}");
               //TODO: Enable _ = NotificationService.AddNotification("Error", $"Failed to save PDF file: {ex.Message}");
                 SentrySdk.CaptureException(ex);
                throw new IOException($"Failed to save PDF: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors.
                Console.Error.WriteLine($"Unexpected error in SaveAndLaunchPdf: {ex.Message}");
               //TODO: Enable _ = NotificationService.AddNotification("Error", $"An unexpected error occurred while saving or launching PDF: {ex.Message}");
                 SentrySdk.CaptureException(ex);
                throw; // Re-throw to allow higher-level handling.
            }
        }

        /// <summary>
        /// Helper method to add a new row to a <see cref="PdfGrid"/> with specified label and value.
        /// </summary>
        /// <param name="grid">The <see cref="PdfGrid"/> to which the row will be added.</param>
        /// <param name="labelFont">The font to use for the label cell.</param>
        /// <param name="label">The text for the label cell.</param>
        /// <param name="value">The text for the value cell.</param>
        public void AddGridRow(PdfGrid grid, PdfFont labelFont, string label, string value)
        {
            var row = grid.Rows.Add();
            row.Cells[0].Value = label;
            row.Cells[0].Style.Font = labelFont;
            row.Cells[1].Value = value;
            // Default styling for borders is usually applied by the grid itself or its default cell style.
        }

        /// <summary>
        /// Helper method to add a new row to a <see cref="PdfGrid"/> with specified label and value,
        /// and explicitly remove borders for a cleaner look.
        /// </summary>
        /// <param name="grid">The <see cref="PdfGrid"/> to which the row will be added.</param>
        /// <param name="labelFont">The font to use for the label cell.</param>
        /// <param name="label">The text for the label cell.</param>
        /// <param name="value">The text for the value cell.</param>
        public void AddGridRowNoBorder(PdfGrid grid, PdfFont labelFont, string label, string value)
        {
            var row = grid.Rows.Add();
            row.Cells[0].Value = label;
            row.Cells[0].Style.Font = labelFont;
            row.Cells[1].Value = value;

            // Explicitly set borders to empty for a no-border effect.
            row.Cells[0].Style.Borders.Right = new PdfPen(PdfColor.Empty);
            row.Cells[0].Style.Borders.Top = new PdfPen(PdfColor.Empty);
            row.Cells[0].Style.Borders.Bottom = new PdfPen(PdfColor.Empty);

            row.Cells[1].Style.Borders.Top = new PdfPen(PdfColor.Empty);
            row.Cells[1].Style.Borders.Bottom = new PdfPen(PdfColor.Empty);
            row.Cells[1].Style.Borders.Left = new PdfPen(PdfColor.Empty);
        }
        public void AddGridRowLastNoBorder(PdfGrid grid, PdfFont labelFont, string label, string value)
        {
            var row = grid.Rows.Add();
            row.Cells[0].Value = label;
            row.Cells[0].Style.Font = labelFont;
            row.Cells[1].Value = value;

            // Explicitly set borders to empty for a no-border effect.
            row.Cells[0].Style.Borders.Right = new PdfPen(PdfColor.Empty);
            row.Cells[0].Style.Borders.Top = new PdfPen(PdfColor.Empty);
            //row.Cells[0].Style.Borders.Bottom = new PdfPen(PdfColor.Empty);

            row.Cells[1].Style.Borders.Top = new PdfPen(PdfColor.Empty);
            //row.Cells[1].Style.Borders.Bottom = new PdfPen(PdfColor.Empty);
            row.Cells[1].Style.Borders.Left = new PdfPen(PdfColor.Empty);
        }

        /// <summary>
        /// Draws a separator line with "Cut Here" text, typically between two voucher copies.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="height">The Y-coordinate where the separator line should start.</param>
        /// <param name="pageWidth">The width of the PDF page.</param>
        public void DrawSeparatorLine(PdfGraphics graphics, float height, float pageWidth)
        {
            try
            {
                float separatorY = height + 5;
                graphics.DrawLine(PdfPens.Gray, 0, separatorY, pageWidth, separatorY);

                // Draw "Cut Here" text centered on the line.
                graphics.DrawString("----------------------- Cut Here -----------------------",
                                     new PdfStandardFont(PdfFontFamily.Helvetica, 6),
                                     PdfBrushes.Gray,
                                     new PointF(pageWidth / 2, separatorY + 4),
                                     new PdfStringFormat() { Alignment = PdfTextAlignment.Center });
            }
            catch (Exception ex)
            {
                // Log and re-throw the exception, letting the caller handle UI/Sentry.
                Console.Error.WriteLine($"Error drawing separator line: {ex.Message}");
                // SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to draw separator line: {ex.Message}", ex);
            }
        }

        // ---

        //  ### Voucher Specific Helpers

        // These methods are tailored for drawing specific sections of a voucher.

        /// <summary>
        /// Adds a header title to the PDF, typically for a voucher section.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="title">The title text for the header.</param>
        /// <param name="currentRelativeY">The current relative Y-coordinate, updated by the method.</param>
        /// <param name="bounds">The rectangular bounds for the drawing area.</param>
        /// <exception cref="InvalidOperationException">Thrown if PdfStyles or its HeaderFont is not initialized.</exception>
        public void AddHeaderToPdf(PdfGraphics graphics, string title, ref float currentRelativeY, RectangleF bounds)
        {
            try
            {
                // Ensure PdfStyles and its HeaderFont are ready.
                if (PdfStyles?.HeaderFont == null)
                    throw new InvalidOperationException("Header font is not initialized in PdfStyles.");

                PdfStringFormat formatCenter = new() { Alignment = PdfTextAlignment.Center };
                graphics.DrawString(title, PdfStyles.HeaderFont, PdfBrushes.Black,
                                    new PointF(bounds.X + (bounds.Width / 2), bounds.Y + currentRelativeY),
                                    formatCenter);
                currentRelativeY += PdfStyles.HeaderFont.MeasureString(title).Height + 10; // Add padding after header
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding header to PDF: {ex.Message}");
                SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to add header to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds company information and voucher details (ID and Date/Time) to the PDF.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="currentRelativeY">The current relative Y-coordinate, updated by the method.</param>
        /// <param name="bounds">The rectangular bounds for the drawing area.</param>
        /// <param name="date">The date and time for the voucher.</param>
        /// <param name="idNumber">The ID number of the voucher.</param>
        /// <exception cref="InvalidOperationException">Thrown if PdfStyles or its fonts are not initialized.</exception>
        public void AddCompanyInfo(PdfGraphics graphics, ref float currentRelativeY, RectangleF bounds, DateTime date, string idNumber)
        {
            try
            {
                // Ensure PdfStyles and its fonts are ready.
                if (PdfStyles?.SubHeaderFont == null || PdfStyles?.BoldFont == null || PdfStyles?.NormalFont == null)
                    throw new InvalidOperationException("Required fonts for company info are not initialized in PdfStyles.");

                PdfGrid infoGrid = new PdfGrid();
                infoGrid.Style.CellPadding = new PdfPaddings(2, 2, 2, 2);
                infoGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
                infoGrid.Style.Font = PdfStyles.NormalFont;

                infoGrid.Columns.Add(2);
                infoGrid.Columns[0].Width = bounds.Width * 0.60f; // Company details on left
                infoGrid.Columns[1].Width = bounds.Width * 0.40f; // Voucher details on right

                // Row for Company Name and Voucher ID
                var row1 = infoGrid.Rows.Add();
                row1.Cells[0].Value = CompanyName;
                row1.Cells[0].Style.Font = PdfStyles.SubHeaderFont;
                row1.Cells[0].Style.TextPen = new PdfPen(PdfBrushes.DarkSlateBlue, 0.5f);
                row1.Cells[0].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Left }; // Align left
                row1.Cells[1].Value = $"{idNumber}";
                row1.Cells[1].Style.Font = PdfStyles.BoldFont;
                row1.Cells[1].Style.TextBrush = PdfBrushes.DarkGreen;
                row1.Cells[1].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Top, Alignment = PdfTextAlignment.Center }; // Align right

                // Row for Company Address/Contact and Date/Time
                var row2 = infoGrid.Rows.Add();
                row2.Cells[0].Value = $"{CompanyAddress} \nGSTIN: {CompanyGstin}\t\t\t\t\t Phone: {CompanyPhone}";
                row2.Cells[0].Style.Font = PdfStyles.NormalFont;
                row2.Cells[0].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Top, Alignment = PdfTextAlignment.Center }; // Align left
                row2.Cells[1].Value = $"Date: {date:dd-MMM-yyyy}\nTime: {date:HH:mm:ss}";
                row2.Cells[1].Style.StringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Center }; // Align right

                // Remove all borders for this grid to give a cleaner, card-like look.
                //foreach (PdfGridRow row in infoGrid.Rows)
                //{
                //    foreach (PdfGridCell cell in row.Cells)
                //    {
                //        cell.Style.Borders.All = new PdfPen(PdfColor.Empty);
                //    }
                //}

                infoGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + currentRelativeY, bounds.Width, bounds.Height - currentRelativeY));
                currentRelativeY += infoGrid.Rows[0].Height + infoGrid.Rows[1].Height; // Estimate height based on rows + padding
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding company info to PDF: {ex.Message}");
                SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to add company information to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds a total amount grid to the PDF voucher.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="currentRelativeY">The current relative Y-coordinate, updated by the method.</param>
        /// <param name="bounds">The rectangular bounds for the drawing area.</param>
        /// <param name="amount">The total amount to display.</param>
        /// <exception cref="InvalidOperationException">Thrown if PdfStyles or its fonts are not initialized.</exception>
        public void AddAmountGrid(PdfGraphics graphics, ref float currentRelativeY, RectangleF bounds, decimal amount)
        {
            try
            {
                // Ensure PdfStyles and its fonts are ready.
                if (PdfStyles?.BoldFont == null || PdfStyles?.SubHeaderFont == null)
                    throw new InvalidOperationException("Required fonts for amount grid are not initialized in PdfStyles.");

                PdfGrid amountGrid = new PdfGrid();
                amountGrid.Columns.Add(2);
                amountGrid.Columns[0].Width = bounds.Width * 0.7f; // Label column
                amountGrid.Columns[1].Width = bounds.Width * 0.3f; // Value column
                amountGrid.Style.Font = PdfStyles.BoldFont;

                var amountRow = amountGrid.Rows.Add();
                amountRow.Cells[0].Value = "Total Amount (Rs.) ";
                amountRow.Cells[0].Style.StringFormat = new PdfStringFormat() { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle, WordSpacing = 1, ParagraphIndent = 1 };
                amountRow.Cells[1].Value = $" {amount:N2} "; // Format amount to 2 decimal places with currency style
                amountRow.Cells[1].Style.StringFormat = new PdfStringFormat() { Alignment = PdfTextAlignment.Left, LineAlignment = PdfVerticalAlignment.Middle, WordSpacing = 1 };
                amountRow.Height = 18; // Set a fixed height for the amount row
                amountRow.Style.BackgroundBrush = PdfBrushes.LightGray; // Highlight the amount row
                amountRow.Style.TextBrush = PdfBrushes.DarkBlue;
                amountRow.Style.Font = PdfStyles.SubHeaderFont;

                // Draw amount grid
                amountGrid.Draw(graphics, new RectangleF(bounds.X, bounds.Y + currentRelativeY, bounds.Width, amountRow.Height));
                currentRelativeY += amountRow.Height + 2; // Use actual row height + padding
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding amount grid to PDF: {ex.Message}");
                SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to add amount grid to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds a footnote to the PDF voucher.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="currentRelativeY">The current relative Y-coordinate, updated by the method.</param>
        /// <param name="bounds">The rectangular bounds for the drawing area.</param>
        /// <param name="footNote">The footnote text.</param>
        /// <exception cref="InvalidOperationException">Thrown if PdfStyles or its NormalFont is not initialized.</exception>
        public void AddFootNote(PdfGraphics graphics, ref float currentRelativeY, RectangleF bounds, string footNote)
        {
            try
            {
                if (PdfStyles?.NormalFont == null)
                    throw new InvalidOperationException("Normal font is not initialized in PdfStyles.");

                graphics.DrawString(footNote, PdfStyles.NormalFont, PdfBrushes.DarkBlue, new PointF(bounds.X, bounds.Y + currentRelativeY));
                currentRelativeY += PdfStyles.NormalFont.Height + 10; // Add some space after the note
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding footnote to PDF: {ex.Message}");
                SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to add footnote to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds signature lines for Authorized Signatory and Receiver to the PDF.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="authorizedSignatoryName">The name of the authorized signatory.</param>
        /// <param name="currentRelativeY">The current relative Y-coordinate, updated by the method.</param>
        /// <param name="bounds">The rectangular bounds for the drawing area.</param>
        /// <exception cref="InvalidOperationException">Thrown if PdfStyles or its fonts are not initialized.</exception>
        public void AddSignatureToPdf(PdfGraphics graphics, string authorizedSignatoryName, ref float currentRelativeY, RectangleF bounds)
        {
            try
            {
                if (PdfStyles?.BoldFont == null || PdfStyles?.NormalFont == null)
                    throw new InvalidOperationException("Required fonts for signature are not initialized in PdfStyles.");

                // Authorized Signatory (right-aligned)
                float signatoryX = bounds.X + bounds.Width - 150; // Position from right edge
                graphics.DrawString("____________________", PdfStyles.BoldFont, PdfBrushes.Black, new PointF(signatoryX, bounds.Y + currentRelativeY));
                currentRelativeY += PdfStyles.BoldFont.Height + 5;
                graphics.DrawString($"( {authorizedSignatoryName} )", PdfStyles.NormalFont, PdfBrushes.Black, new PointF(signatoryX, bounds.Y + currentRelativeY));
                currentRelativeY += PdfStyles.NormalFont.Height + 5;
                graphics.DrawString("Authorized Signatory", PdfStyles.NormalFont, PdfBrushes.Black, new PointF(signatoryX, bounds.Y + currentRelativeY));

                // Receiver's Signature (left-aligned)
                float receiverX = bounds.X + 10; // Position from left edge
                // Adjust Y to place it somewhat aligned horizontally with authorized signatory,
                // but below the company info if the voucher is short.
                // This 'currentRelativeY - (signatory section height)' attempts to align roughly.
                float receiverSignatureY = bounds.Y + currentRelativeY - (PdfStyles.BoldFont.Height + PdfStyles.NormalFont.Height * 2 + 10);
                if (receiverSignatureY < bounds.Y + 100) // Prevent drawing too high
                {
                    receiverSignatureY = bounds.Y + 100; // Set a minimum Y for receiver's signature
                }

                graphics.DrawString("____________________", PdfStyles.BoldFont, PdfBrushes.Black, new PointF(receiverX, receiverSignatureY));
                graphics.DrawString("( Receiver's Signature )", PdfStyles.NormalFont, PdfBrushes.Black, new PointF(receiverX, receiverSignatureY + PdfStyles.BoldFont.Height + 5));
                currentRelativeY = Math.Max(currentRelativeY, receiverSignatureY + PdfStyles.BoldFont.Height + PdfStyles.NormalFont.Height + 5); // Update currentY to the lowest point
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding signature to PDF: {ex.Message}");
                SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to add signature section to PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds a QR code to the PDF voucher.
        /// </summary>
        /// <param name="graphics">The <see cref="PdfGraphics"/> object to draw on.</param>
        /// <param name="qrCodeText">The text content to encode in the QR code.</param>
        /// <param name="currentRelativeY">The current relative Y-coordinate, updated by the method.</param>
        /// <param name="bounds">The rectangular bounds for the drawing area.</param>
        /// <param name="isSingleVoucher">If true, adjusts QR code position for a single voucher layout.</param>
        /// <exception cref="InvalidOperationException">Thrown if PdfStyles or its NormalFont is not initialized.</exception>
        public void AddQrCodeToPdf(PdfGraphics graphics, string qrCodeText, ref float currentRelativeY, RectangleF bounds, bool isSingleVoucher = false)
        {
            try
            {
                if (PdfStyles?.NormalFont == null)
                    throw new InvalidOperationException("Normal font is not initialized in PdfStyles.");

                // Adjust X position based on single/duplicate voucher layout.
                float extraX = isSingleVoucher ? 50 : 0;
                // Define QR code square size (width and height).
                float qrSize = isSingleVoucher ? 70 : 60; 
                // Move Y up slightly to make space for the QR code.
                currentRelativeY = Math.Max(currentRelativeY, bounds.Y + bounds.Height-10); // Ensure QR code is near the bottom
                float qrCodeY = currentRelativeY-qrSize-5;

                PdfQRBarcode qrCode = new PdfQRBarcode
                {
                    Text = qrCodeText,
                    ErrorCorrectionLevel = PdfErrorCorrectionLevel.High,
                    XDimension = 3 // Controls the module size, influencing overall QR code size.
                };

               

                // Calculate QR code position to be roughly centered relative to bounds, with adjustments.
                // The 160 offset seems specific; consider making it dynamic based on bounds.Width if possible.
                float qrCodeX = bounds.X + (bounds.Width / 2) - (qrSize / 2) + extraX; // Centered X, then adjusted by extraX

                // Draw a black rectangle border around the QR code.
                graphics.DrawRectangle(PdfPens.Black, new RectangleF(qrCodeX, qrCodeY, qrSize, qrSize));
                // Draw the QR code slightly inset within the border.
                qrCode.Draw(graphics, new RectangleF(qrCodeX + 2, qrCodeY + 2, qrSize - 4, qrSize - 4));

                // Adjust text position slightly for single voucher.
                float textXOffset = isSingleVoucher ? 0 : -8; // Minor horizontal adjustment for text

                // Add descriptive text below the QR code.
                graphics.DrawString("Scan for details", PdfStyles.NormalFont, PdfBrushes.DarkRed,
                                    new PointF(qrCodeX + qrSize / 2 , qrCodeY + qrSize + 5),
                                    new PdfStringFormat { Alignment = PdfTextAlignment.Center });
                currentRelativeY += qrSize + PdfStyles.NormalFont.Height + 10; // Update Y for next elements
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding QR code to PDF: {ex.Message}");
                SentrySdk.CaptureException(ex);
                throw new InvalidOperationException($"Failed to add QR code to PDF: {ex.Message}", ex);
            }
        }

        // ---

        //### Resource Disposal

        /// <summary>
        /// Disposes of the managed and unmanaged resources used by the <see cref="PdfService"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevent the finalizer from running
        }

        /// <summary>
        /// Protected virtual method for disposing resources.
        /// </summary>
        /// <param name="disposing">True if called from Dispose() (user code), false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return; // Already disposed, prevent multiple calls

            if (disposing)
            {
                // Dispose managed state (managed objects)
                PdfStyles?.Dispose(); // Dispose the PdfStyles instance
                PdfStyles = null;     // Release reference
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // if there were any. (None directly managed here)

            _disposed = true; // Mark as disposed
        }

        // Finalizer (destructor) - only if directly holding unmanaged resources.
        // For this class, relying on IDisposable for managed resources is sufficient.
        // ~PdfService()
        // {
        //     Dispose(false);
        // }
    }
}