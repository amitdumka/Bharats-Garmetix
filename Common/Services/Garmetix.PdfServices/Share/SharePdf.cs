
using Bharat.ToolKits.Helpers;
using Garmetix.Models.Reports;

namespace Garmetix.PdfServices.Share
{//TODO: Port and adjust
    public class SharePdf
    {
        //TODO: Handle this urgently SharePdf LastGeneratedPdfPath
        public static string LastGeneratedPdfPath { get; set; }=String.Empty;
        public static async Task<bool> ShareVoucher(VoucherDetails VoucherDetails)
        {
            if (string.IsNullOrEmpty(LastGeneratedPdfPath))
            {
                return false;
            }

            try
            {
                // Use the IShare interface to request sharing the file.
                // This will open the native OS share sheet.
                var _share = ServiceHelper.Current.GetService<IShare>();
                await _share!.RequestAsync(new ShareFileRequest
                {
                    Title = $"{VoucherDetails.CompanyName} {VoucherDetails.VoucherType} Voucher No. {VoucherDetails.VoucherNumber}",
                    File = new ShareFile(LastGeneratedPdfPath)
                });
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not share file: {ex.Message}", "OK");
                return false;
            }
        }
        public static async Task<bool> ShareVoucherOverEmail(VoucherDetails VoucherDetails, string emailid)
        {
            if (string.IsNullOrEmpty(LastGeneratedPdfPath))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailid))
            {
                await Shell.Current.DisplayAlert("Input Needed", "Please enter an email address to share.", "OK");
                return false;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = $"From {VoucherDetails.CompanyName},  {VoucherDetails.VoucherType} Voucher - No. {VoucherDetails.VoucherNumber} for {VoucherDetails.PayeeOrPayerName}, Dated: {VoucherDetails.Date}",
                    Body = $"Please find the attached {VoucherDetails.VoucherType} voucher.\n \nBest regards,\n{VoucherDetails.CompanyName}\n{VoucherDetails.CompanyAddress}\n{VoucherDetails.CompanyPhone}",
                    To = [emailid]
                };

                message?.Attachments?.Add(new EmailAttachment( LastGeneratedPdfPath));
                ServiceHelper.Current.GetService<IEmail>()?.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException)
            {
                await Shell.Current.DisplayAlert("Not Supported", "Email is not supported on this device.", "OK");
                return false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to send email: {ex.Message}", "OK");
                return false;
            }
        }

    }
}
