using Bharat.ToolKits.Helpers;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Sessions;
using Garmetix.Models.Reports;
using Garmetix.Services;
using Garmetix.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.CoreServices.Accounting
{
    public class AccountingServices : BaseServices
    {
        public static VoucherDetails? LastVoucherDetails { get; set; } = null;
        public static string? LastGeneratedVoucherPath { get; set; } = null;

        #region PrintVouchers

        public static async Task<bool> WantToSaveVoucherAsync()
        {
            // Use TaskCompletionSource to await the result from the main thread
            var tcs = new TaskCompletionSource<bool>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var result = await Shell.Current.DisplayAlertAsync("Save Voucher", "Do you want to save the voucher?", "Yes", "No");
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return await tcs.Task;
        }

        public static async Task<bool> ShareVoucher()
        {
            if (string.IsNullOrEmpty(LastGeneratedVoucherPath) || LastVoucherDetails == null)
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
                    Title = $"{LastVoucherDetails.CompanyName} {LastVoucherDetails.VoucherType} Voucher No. {LastVoucherDetails.VoucherNumber}",
                    File = new ShareFile(LastGeneratedVoucherPath)
                });
                return true;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Could not share file: {ex.Message}", "OK");
                return false;
            }
        }

        public static async Task<bool> ShareOverEmail(string emailid)
        {
            if (string.IsNullOrEmpty(LastGeneratedVoucherPath) || LastVoucherDetails == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(emailid))
            {
                await Shell.Current.DisplayAlertAsync("Email", "Sharing over to default company email.", "OK");
                emailid = LastVoucherDetails.CompanyEmail;
                // return false;
            }

            try
            {
                var message = new EmailMessage
                {
                    Subject = $"From {LastVoucherDetails.CompanyName},  {LastVoucherDetails.VoucherType} Voucher - No. {LastVoucherDetails.VoucherNumber} for {LastVoucherDetails.PayeeOrPayerName}, Dated: {LastVoucherDetails.Date}",
                    Body = $"Please find the attached {LastVoucherDetails.VoucherType} voucher.\n \nBest regards,\n{LastVoucherDetails.CompanyName}\n{LastVoucherDetails.CompanyAddress}\n{LastVoucherDetails.CompanyPhone}",
                    To = [emailid]
                };

                message?.Attachments?.Add(new EmailAttachment(LastGeneratedVoucherPath));
                ServiceHelper.Current.GetService<IEmail>()?.ComposeAsync(message);
                return true;
            }
            catch (FeatureNotSupportedException)
            {
                await Shell.Current.DisplayAlertAsync("Not Supported", "Email is not supported on this device.", "OK");
                return false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to send email: {ex.Message}", "OK");
                return false;
            }
        }

        public static async Task<bool> WantToShareVoucherAsync()
        {
            // Use TaskCompletionSource to await the result from the main thread
            var tcs = new TaskCompletionSource<bool>();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var result = await Shell.Current.DisplayAlertAsync("Share Voucher", "Do you want to Share the voucher?", "Yes", "No");
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return await tcs.Task;
        }

        //TODO: Print Setting, Create Print Setting in Setting Page so that user can set Print Setting
        //Printing Vouchers
        //TODO: Print the Voucher as per the VoucherType and move to Accouting Service
        public static Task PrintVoucher(CashVoucher voucher, bool isnew = false)
        {
            LastVoucherDetails = new VoucherDetails
            {
                IsCashVoucher = true,
                VoucherType = Enum.GetName(voucher.VoucherType)!,
                VoucherNumber = voucher.VoucherNumber,
                Date = voucher.OnDate,
                PayeeOrPayerName = voucher.PartyName,
                Narration = voucher.Particulars,
                Amount = voucher.Amount,
                PaymentMethod = "Cash",
                PaymentDetails = Db.Transactions?.Find(voucher?.TransactionId)?.Name ?? $"Cash " + voucher!.VoucherType.ToString(),
                AuthorizedSignatory = GetEmployeeName(voucher?.EmployeeId ?? Guid.Empty) ?? "Manager",
                Voucher = voucher!.VoucherType,
                TransactionType = Db.Transactions?.Find(voucher.TransactionId)?.Name ?? $"Cash " + voucher.VoucherType.ToString()
            };
            return ServiceHelper.Current.GetService<IPdfVoucherService>()?.CreatePdfCashVoucherAsync(LastVoucherDetails, isnew)!;
        }

        public static Task PrintVoucher(Voucher voucher, bool isnew = false)
        {
            voucher.Ledger ??= GetLedger(voucher.LedgerId!.Value);
            if (voucher.IsParty)
            {
                voucher.Party = GetPartyDetails(voucher.PartyId!.Value);
            }

            LastVoucherDetails = new VoucherDetails
            {
                VoucherType = Enum.GetName(voucher.VoucherType)!,
                VoucherNumber = voucher.VoucherNumber,
                Date = voucher.OnDate,
                PayeeOrPayerName = voucher.PartyName,
                Narration = voucher.Particulars,
                Amount = voucher.Amount,
                PaymentMethod = Enum.GetName(voucher.PaymentMode)!,
                PaymentDetails = voucher.PaymentDetails ?? "",
                AuthorizedSignatory = GetEmployeeName(voucher.EmployeeId ?? Guid.Empty) ?? "Manager",
                Voucher = voucher.VoucherType,
            };
            if (voucher.IsParty)
            {
                LastVoucherDetails.PartyDetails = voucher.Party?.Address ?? "" + "\n" + voucher.Party?.Phone + "\n" + voucher.Party?.GSTIN ?? "";
            }
            return ServiceHelper.Current.GetService<IPdfVoucherService>()?.CreatePdfVoucherAsync(LastVoucherDetails, isnew)!;
        }

        public static Party? GetPartyDetails(Guid id)
        {
            return Db.Parties.Where(x => x.Id == id).FirstOrDefault();
        }

        public static Ledger? GetLedger(Guid id)
        {
            return Db.Ledgers.Where(x => x.Id == id).FirstOrDefault();
        }

        public static string GetEmployeeName(Guid id)
        {
            return Db.Employees.Where(x => x.Id == id).FirstOrDefault()?.StaffName ?? "";
        }

        public static PrintType ToPrintVoucherType(VoucherType voucherType)
        {
            return voucherType switch
            {
                VoucherType.Payment => PrintType.PaymentVoucher,
                VoucherType.Expense => PrintType.Expenses,
                VoucherType.Receipt => PrintType.ReceiptVocuher,
                _ => PrintType.PaymentVoucher,
            };
        }

        #endregion PrintVouchers

        public static async Task<bool> ClearCustomerDue(string InvoiceNumber, DateTime payingDate)
        {
            //var invoice = Db.Invoices.Where(x => x.InvoiceNumber == InvoiceNumber).FirstOrDefault();
            //if (invoice != null)
            //{
            //    invoice. = payingDate;
            //    invoice.DueAmount = 0;
            //    Db.SaveChanges();
            //}

            var due = Db.CustomerDues.Where(x => x.InvoiceNumber == InvoiceNumber).FirstOrDefault();
            if (due != null)
            {
                due.ClearingDate = payingDate;
                due.Paid = true;
                due.UpdatedAt = DateTime.UtcNow;
                Db.CustomerDues.Update(due);
            }

            return await Db.SaveChangesAsync() > 0;
        }

        public static async Task<Guid?> GetPartyId(Guid? ledgerId)
        {
            if (ledgerId == null || ledgerId == Guid.Empty)
            {
                return null;
            }
            var partyId = await Db.Parties.Where(x => x.LedgerId == ledgerId).Select(c => c.Id).FirstOrDefaultAsync();
            return partyId;
        }

        public static async Task<Guid> GetBankLedgerGroupIdAsync()
        {
            var x = (await Db.LedgerGroups.Where(x => x.Name == "Bank" || x.Name == "Cash").FirstOrDefaultAsync())?.Id;

            return x ?? Guid.Empty;
        }

        /// <summary>
        /// Return Voucher Code based on Voucher Type
        /// Like EXP for expense Voucher, RCP for Receipt, PYM for Payment
        /// CR for Credit
        /// </summary>
        /// <param name="voucherType"></param>
        /// <returns></returns>
        public static string VoucherCode(VoucherType voucherType)
        {
            return voucherType switch
            {
                VoucherType.Expense => "EXP",
                VoucherType.Receipt => "RCP",
                VoucherType.Payment => "PYM",
                _ => "CR",
            };
        }

        /// <summary>
        ///  Add Zeros as Suffix to the number based on length
        ///  move to Utility or Helper class
        /// </summary>
        /// <param name="number"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string AddZerosAsSuffix(int number, int length)
        {
            //TODO: covnert number to string  and number of Zero as suffix based on lenght
            //TODO: example number 3 and length 4 then return 0003
            return number.ToString().PadLeft(length, '0');
        }

        public static string CreateVoucherNumber(DateTime date, VoucherType voucherType)
        {
            string voucherNumber = "";
            string suffix = AddZerosAsSuffix(Db.Vouchers.Where(x => x.VoucherType == voucherType && x.OnDate.Year == date.Year && x.OnDate.Month == date.Month).Count() + 1, 4);

            voucherNumber = SessionService.CompanyStoreCode() + "-" + VoucherCode(voucherType) + "-" + date.ToString("yyyyMM") + "-" + suffix;

            return voucherNumber;
        }

        public static string CreateCashVoucherNumber(DateTime date, VoucherType voucherType)
        {
            string voucherNumber = "";
            string suffix = AddZerosAsSuffix(Db.CashVouchers.Where(x => x.VoucherType == voucherType && x.OnDate.Year == date.Year && x.OnDate.Month == date.Month).Count() + 1, 4);

            voucherNumber = SessionService.CompanyStoreCode() + "-CH" + VoucherCode(voucherType) + "-" + date.ToString("yyyyMM") + "-" + suffix;

            return voucherNumber;
        }

        /// <summary>
        /// Get Ledger Group For Party
        /// </summary>
        /// <param name="partyType"></param>
        /// <returns></returns>

        public static async  Task<Guid> GetLedgerGroupForPartyAsync(PartyType partyType)
        {
            //TODO : Move to Service and make it async as well return Task.Run(async delegate=>{});

            //Guid? id;
            // Corrected the issue with nullable type and async-await usage
            Guid id = Guid.Empty;

            id = partyType switch
            {
                PartyType.Customer =>((await Db.LedgerGroups.FirstOrDefaultAsync(static x => x.Name == "Customers"))?.Id) ?? Guid.Empty,
                PartyType.Supplier => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "Vendors"))?.Id ?? Guid.Empty,
                PartyType.Employee => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "Employees"))?.Id ?? Guid.Empty,
                PartyType.Vendor => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "Vendors"))?.Id ?? Guid.Empty,
                PartyType.Debitor => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "Debitors"))?.Id ?? Guid.Empty,
                PartyType.Creditor => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "Creditors"))?.Id ?? Guid.Empty,
                PartyType.Others => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "No Group"))?.Id ?? Guid.Empty,
                _ => (await Db.LedgerGroups.FirstOrDefaultAsync(x => x.Name == "No Group"))?.Id ?? Guid.Empty,
            };
            return id;
        }
    }
}