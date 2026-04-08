using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Models.ViewModels;
using Garmetix.ModuleService;
using Garmetix.PdfServices.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Garmetix.Reports.PageModels
{
    public partial class LedgerReportPageModel : BasePageModel
    {
        [ObservableProperty]
        private ObservableCollection<ComboBoxItemVM> _ledgersList = [];

        [ObservableProperty]
        private Guid _selectedLedger = Guid.Empty;

        [ObservableProperty]
        private int _selectedMonth = DateTime.Now.Month;

        [ObservableProperty]
        private int _selectedYear = DateTime.Now.Year;

        [ObservableProperty]
        private bool _full = false;

        [ObservableProperty]
        private bool _period = false;

        private List<ComboBoxItemVM> _legers;
        private readonly IPdfAccountingService _pdfService;

        [RelayCommand]
        private void Appearing()
        {
            LoadLedgers();
            //LoadPeriod();
            SelectedYear = DateTime.Now.Year;
            SelectedMonth = DateTime.Now.Month;
        }

        public LedgerReportPageModel()
        {
            //LoadLedgers();
            LoadPeriod();
            SelectedYear = DateTime.Now.Year;
            SelectedMonth = DateTime.Now.Month;
            _pdfService= ServiceHelper.GetService<IPdfAccountingService>();
        }

        [RelayCommand]
        private void PrintLedger()
        {
            if (this.SelectedLedger == Guid.Empty)
            {
                _ = Notify.DisplayNotificationAsync("Please select a Ledger", isLong: true);
                return;
            }
            GenerateLedgerPdf(this.SelectedLedger);
        }

        private async void GenerateLedgerPdf(Guid LedgerId)
        {
            //1. Decide party or general
            //2. Genrate
            //3. Print

            try
            {
                if (LedgerId == Guid.Empty) return;
                var isParty = await VoucherServices.IsParyLedgerAsync(LedgerId);
                MemoryStream pdfStream = null;
                string fileName = $"Ledger.pdf";
                //TODO: Call PDF Generation Service to generate the PDF
                if (isParty.Value)
                {
                    if (Full)
                    {
                        var partyLedgerData = VoucherServices.Instance.GetPartyLedgerById(LedgerId);
                        fileName = $"PartyLedger_{partyLedgerData?.PartyName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        pdfStream = _pdfService.GeneratePartyLedgerPdf(partyLedgerData!);
                    }
                    else if (Period)
                    {
                        var partyLedgerData = VoucherServices.Instance.GetPartyLedgerById(LedgerId, SelectedMonth, SelectedYear);
                        fileName = $"PartyLedger_{partyLedgerData!.PartyName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        pdfStream = _pdfService.GeneratePartyLedgerPdf(partyLedgerData);
                    }
                    else
                    {
                        var partyLedgerData = VoucherServices.Instance.GetPartyLedgerById(LedgerId, SelectedMonth, SelectedYear);
                        fileName = $"PartyLedger_{partyLedgerData!.PartyName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        pdfStream = _pdfService.GeneratePartyLedgerPdf(partyLedgerData);
                    }
                }
                else
                {
                    if (Full)
                    {
                        var generalLedgerData = VoucherServices.Instance.GetLedgersById(LedgerId);
                        fileName = $"Ledger_{generalLedgerData!.LedgerName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        pdfStream = _pdfService.GenerateGeneralLedgerPdf(generalLedgerData);
                    }
                    else if (Period)
                    {
                        var generalLedgerData = VoucherServices.Instance.GetLedgersById(LedgerId, SelectedMonth, SelectedYear);
                        fileName = $"Ledger_{generalLedgerData!.LedgerName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        pdfStream = _pdfService.GenerateGeneralLedgerPdf(generalLedgerData);
                    }
                    else
                    {
                        var generalLedgerData = VoucherServices.Instance.GetLedgersById(LedgerId, SelectedMonth, SelectedYear);
                        fileName = $"Ledger_{generalLedgerData!.LedgerName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                        pdfStream = _pdfService.GenerateGeneralLedgerPdf(generalLedgerData);
                    }
                }

                // 3. Save and Open the PDF (Platform-specific implementation needed)
                //string fileName = $"Ledger_{partyLedgerData.LedgerName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                string filePath = Path.Combine(FileSystem.CacheDirectory, "BharatGarmetix", "Ledgers", fileName); // Use CacheDirectory for temporary storage
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                // For Android, iOS, Mac Catalyst, Windows
                // You might need permission requests for saving to external storage on Android
                await File.WriteAllBytesAsync(filePath, pdfStream.ToArray());

                // Open the PDF using default system viewer
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });

                await Notify.DisplayNotificationAsync("PDF Generated \n" + $"PDF saved to: {filePath}");
            }
            catch (Exception ex)
            {
                await Notify.DisplayNotificationAsync("PDF Generated \n" + $"Failed to generate PDF: {ex.Message}");
                Console.WriteLine($"PDF Generation Error: {ex}");
            }
        }

        private void LoadLedgers()
        {
            try
            {
                if (IsBusy) return;
                IsBusy = true;
                if (_legers == null || _legers.Count <= 0)
                    _legers = Db.Ledgers.Select(c => new ComboBoxItemVM { Id = c.Id, Name = c.Name }).ToList();

                if (LedgersList != null || LedgersList.Count > 0)
                    LedgersList.Clear();

                if (_legers != null && _legers.Count > 0)
                    foreach (var item in _legers)
                    {
                        LedgersList.Add(item);
                    }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                SentrySdk.CaptureMessage(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}