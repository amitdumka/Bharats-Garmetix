using Syncfusion.Pdf.Graphics;

// Final Version
namespace Garmetix.PdfServices.Base
{
    /// <summary>
    /// Provides consistent PDF styling, including fonts and brushes,
    /// with robust font loading and proper resource management.
    /// This class should be initialized asynchronously using the <see cref="CreateAsync"/> factory method.
    /// </summary>
    internal class PdfStyles : IDisposable
    {
        // --- Static (Constant) Styles ---
        // These are standard fonts and brushes that do not depend on dynamic loading or multipliers.
        // They are initialized once and can be reused across multiple PdfStyles instances.
        public static readonly PdfFont TitleFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 20, PdfFontStyle.Bold);

        public static readonly PdfFont HeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 12, PdfFontStyle.Bold);
        public static readonly PdfFont RegularFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        public static readonly PdfFont BalanceFont = new PdfStandardFont(PdfFontFamily.Courier, 14, PdfFontStyle.Bold);
        public static readonly PdfBrush BlackBrush = new PdfSolidBrush(new PdfColor(0, 0, 0));
        public static readonly PdfBrush GrayBrush = new PdfSolidBrush(new PdfColor(100, 100, 100));
        public static readonly PdfBrush TableHeaderBrush = new PdfSolidBrush(new PdfColor(230, 230, 230));
        public static readonly PdfBrush TableEvenRowBrush = new PdfSolidBrush(new PdfColor(245, 245, 245));

        // --- Instance (Dynamic) Styles ---
        // These depend on the FontSizeMultiplier and dynamically loaded fonts.
        // They are specific to each PdfStyles instance.
        public float FontSizeMultiplier { get; private set; } = 1;

        // Private fields to hold the font streams. These must be disposed.
        private Stream? _fontStreamRegular;

        private Stream? _fontStreamSemibold;

        // Public properties for the dynamically loaded fonts.
        public PdfFont? HeaderFont { get; private set; }

        public PdfFont? SubHeaderFont { get; private set; }
        public PdfFont? NormalFont { get; private set; }
        public PdfFont? BoldFont { get; private set; }
        public static PdfStyles? Instance { get; private set; }
        // Private constructor to enforce asynchronous initialization via the factory method.
        private PdfStyles()
        {
            // Constructor is intentionally empty. Initialization happens in CreateAsync.
        }

        /// <summary>
        /// Asynchronously creates and initializes a new instance of the PdfStyles class.
        /// This is the recommended way to obtain a fully configured PdfStyles object.
        /// </summary>
        /// <param name="fontSizeMultiplier">Optional multiplier for dynamically sized fonts. Defaults to 1.</param>
        /// <returns>A fully initialized PdfStyles instance, or null if font loading fails.</returns>
        public static async Task<PdfStyles?> CreateAsync(float fontSizeMultiplier = 1f)
        {
            var instance = new PdfStyles
            {
                FontSizeMultiplier = fontSizeMultiplier
            };

            // Attempt to load fonts. If successful, return the instance.
            // If not, dispose any partially loaded resources and return null.
            if (await instance.SetFontsAsync())
            {
                Instance = instance;
                return Instance;
            }
            else
            {
                instance.Dispose(); // Clean up if initialization fails
                return null;
            }
        }

        /// <summary>
        /// Helper method to load font files embedded as MauiAssets.
        /// This method reads the asset stream into a <see cref="MemoryStream"/> to ensure
        /// robustness and seekability, especially across different platforms like Android.
        /// The returned <see cref="MemoryStream"/> must be disposed by the managing class.
        /// </summary>
        /// <param name="fontFileName">The name of the font file (e.g., "Roboto-Regular.ttf").</param>
        /// <returns>A <see cref="MemoryStream"/> containing the font data.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the font file is not found in app package assets.</exception>
        /// <exception cref="IOException">Thrown if there's an error reading the asset stream.</exception>
        private async Task<Stream> GetFontStreamAsync(string fontFileName)
        {
            try
            {
                // OpenAppPackageFileAsync returns a Stream that needs to be disposed.
                // The 'using' statement ensures the original assetStream is closed.
                using (Stream assetStream = await FileSystem.Current.OpenAppPackageFileAsync(fontFileName))
                {
                    if (assetStream == null)
                    {
                        // FileSystem.Current.OpenAppPackageFileAsync might return null or throw.
                        // We explicitly throw FileNotFoundException for clear error reporting.
                        throw new FileNotFoundException($"Font file '{fontFileName}' not found in app package assets.");
                    }

                    // Copy the asset stream content into a MemoryStream.
                    // This makes the stream independent of the original asset file and seekable,
                    // which is often required by PDF libraries.
                    MemoryStream memoryStream = new();
                    await assetStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Reset stream position to the beginning for PdfTrueTypeFont
                    return memoryStream;
                }
            }
            catch (FileNotFoundException)
            {
                // Re-throw specific exception to be handled by the caller (SetFontsAsync).
                throw;
            }
            catch (Exception ex)
            {
                // Catch any other IO or unexpected exceptions during stream handling.
                throw new IOException($"An error occurred while reading font asset '{fontFileName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads fonts for PDF generation from embedded resources.
        /// This method stores the font streams as private fields and creates <see cref="PdfTrueTypeFont"/> instances.
        /// It's crucial to get a new stream each time if <see cref="PdfTrueTypeFont"/> consumes the stream upon creation.
        /// </summary>
        /// <returns>True if all fonts are loaded successfully, false otherwise.</returns>
        private async Task<bool> SetFontsAsync()
        {
            try
            {
                // Dispose any previously loaded font streams to prevent resource leaks
                // if SetFontsAsync were to be called multiple times on the same instance.
                DisposeFontStreams();

                _fontStreamRegular = await GetFontStreamAsync("Roboto-Regular.ttf");
                _fontStreamSemibold = await GetFontStreamAsync("OpenSans-Semibold.ttf");

                // Although GetFontStreamAsync throws on failure, this check provides
                // an extra layer of defense before attempting to create PdfTrueTypeFont objects.
                if (_fontStreamRegular == null || _fontStreamSemibold == null)
                {
                    Console.Error.WriteLine("Error: One or both font streams are null after loading attempt.");
                    return false;
                }

                // Create PdfTrueTypeFont instances using the loaded streams and the multiplier.
                HeaderFont = new PdfTrueTypeFont(_fontStreamSemibold, 14 * FontSizeMultiplier);
                SubHeaderFont = new PdfTrueTypeFont(_fontStreamSemibold, 10 * FontSizeMultiplier);
                NormalFont = new PdfTrueTypeFont(_fontStreamRegular, 8 * FontSizeMultiplier);
                BoldFont = new PdfTrueTypeFont(_fontStreamSemibold, 8 * FontSizeMultiplier);

                return true;
            }
            catch (FileNotFoundException ex)
            {
                string errorMessage = $"A required font file is missing: {ex.Message}";
                Console.Error.WriteLine(errorMessage); // Log to console for debugging
                await Shell.Current.DisplayAlertAsync("Font Loading Error", errorMessage, "OK");
                // SentrySdk.CaptureException(ex); // Uncomment if Sentry is configured
                // NotificationService.AddNotification("Error", errorMessage); // Uncomment if NotificationService is configured
                return false;
            }
            catch (IOException ex)
            {
                string errorMessage = $"An I/O error occurred while loading fonts: {ex.Message}";
                Console.Error.WriteLine(errorMessage); // Log to console
                await Shell.Current.DisplayAlertAsync("Font Loading Error", errorMessage, "OK");
                // SentrySdk.CaptureException(ex); // Uncomment if Sentry is configured
                // NotificationService.AddNotification("Error", errorMessage); // Uncomment if NotificationService is configured
                return false;
            }
            catch (Exception ex)
            {
                // Catch any other unexpected errors during font loading or PdfTrueTypeFont creation.
                string errorMessage = $"An unexpected error occurred while loading PDF fonts: {ex.Message}";
                Console.Error.WriteLine(errorMessage); // Log to console
                await Shell.Current.DisplayAlertAsync("Error", errorMessage, "OK");
                // SentrySdk.CaptureMessage($"Failed to load fonts: {ex.Message}"); // Uncomment if Sentry is configured
                // SentrySdk.CaptureException(ex); // Uncomment if Sentry is configured
                // _ = NotificationService.AddNotification("Error", errorMessage); // Uncomment if NotificationService is configured
                return false;
            }
        }

        /// <summary>
        /// Disposes of the managed resources used by the <see cref="PdfStyles"/> instance,
        /// specifically the font streams.
        /// </summary>
        public void Dispose()
        {
            // Call the protected Dispose method to clean up resources.
            Dispose(true);
            // Suppress finalization to prevent the garbage collector from running
            // the finalizer, as resources have already been explicitly disposed.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected virtual method for disposing resources.
        /// This pattern is standard for implementing IDisposable.
        /// </summary>
        /// <param name="disposing">True if called from Dispose() (user code), false if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects).
                // This includes the MemoryStream objects that hold font data.
                DisposeFontStreams();

                // Set font properties to null to release references.
                // Syncfusion's PdfFont objects generally don't require explicit Dispose() themselves,
                // as they manage their internal resources or are lightweight.
                HeaderFont = null;
                SubHeaderFont = null;
                NormalFont = null;
                BoldFont = null;
            }
            // No unmanaged resources are directly held by this class, so no unmanaged cleanup here.
        }

        /// <summary>
        /// Helper method to safely dispose of the font streams and set their references to null.
        /// </summary>
        private void DisposeFontStreams()
        {
            _fontStreamRegular?.Dispose(); // Safely dispose if not null
            _fontStreamRegular = null;     // Clear the reference

            _fontStreamSemibold?.Dispose(); // Safely dispose if not null
            _fontStreamSemibold = null;     // Clear the reference
        }

        // Optional: Finalizer (destructor)
        // A finalizer is typically only needed if the class directly holds unmanaged resources
        // and you want a fallback cleanup in case Dispose() is not called.
        // For this class, since it primarily manages managed MemoryStream objects,
        // and we've implemented IDisposable correctly, a finalizer is generally not necessary
        // and can even be detrimental to performance.
        // ~PdfStyles()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }
    }
}