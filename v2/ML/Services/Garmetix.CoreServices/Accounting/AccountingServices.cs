using Bharat.ToolKits.Extensions;
using Bharat.ToolKits.Helpers;
using DocumentFormat.OpenXml.InkML;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Inventory;
using Garmetix.Core.Sessions;
using Garmetix.Databases.Services;
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

        //TODO: implements 

        /// <summary>
        /// Verify and Update the payment History 
        /// </summary>
        /// <param name="invId"></param>
        /// <returns></returns>
        public static async Task<bool> VerifyAndUpdateInvoicePayment(Guid invId)
        {
            var invoice = await Db.Invoices.FirstOrDefaultAsync(x => x.Id == invId);
            if (invoice == null)
                return false;
            //For Check for Payment History
            var totalPaid = await Db.InvoicePayments.Where(x => x.InvoiceId == invoice.Id).SumAsync(x => x.Amount);
            var dueAmount = invoice.BillAmount - totalPaid;
            if (dueAmount <= 0)
            {
                // Update Invoice as Paid
                invoice.PaidAmount = invoice.BillAmount;
                Db.Invoices.Update(invoice);
                // Clear Customer Due
                var dueRecord = await Db.CustomerDues.FirstOrDefaultAsync(x => x.InvoiceNumber == invoice.InvoiceNumber);
                if (dueRecord != null)
                {
                    dueRecord.ClearingDate = DateTime.UtcNow;
                    dueRecord.Paid = true;
                    dueRecord.UpdatedAt = DateTime.UtcNow;
                    Db.CustomerDues.Update(dueRecord);
                }
                await Db.SaveChangesAsync();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Get due amount after verifying the invoice with invoice number and also check for payment history and return the due amount, if invoice is not found then return -999 as due amount, this method is used in due recovery to get the due amount of the invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public static async Task<decimal> GetInvoiceDueAmount(string invoiceNumber)
        {
            var invoice = await Db.Invoices.FirstOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber);
            if (invoice == null)
                return -999;
            //For Check for Payment History
            var totalPaid = await Db.InvoicePayments.Where(x => x.InvoiceId == invoice.Id).SumAsync(x => x.Amount);
            var dueAmount = invoice.BillAmount - totalPaid;
            return dueAmount;

        }
        /// <summary>
        /// Get Invoice Due Amount after verifying the invoice with invoice id and also check for payment history and return the due amount, if invoice is not found then return -999 as due amount, this method is used in due recovery to get the due amount of the invoice
        /// </summary>
        /// <param name="invId"></param>
        /// <returns></returns>
        public static async Task<decimal> GetInvoiceDueAmount(Guid  invId)
        {
            var invoice = await Db.Invoices.FirstOrDefaultAsync(x => x.Id == invId);
            if (invoice == null)
                return -999;
            //For Check for Payment History
            var totalPaid = await Db.InvoicePayments.Where(x => x.InvoiceId == invoice.Id).SumAsync(x => x.Amount);
            var dueAmount = invoice.BillAmount - totalPaid;
            return dueAmount;

        }


        /// <summary>
        /// Update Due Invoice Based on Due Recovery and also update the Customer Due if invoice is fully paid, also add entry in Invoice Payment and Card Payment if payment mode is card payment
        /// </summary>
        /// <param name="recovery">DueRecoveryEntry type </param>
        /// <param name="cardPayment">CardPayment </param>
        /// <returns>returns true or false </returns>

        public static async Task<bool> UpdateDueInvoiceAsync(DueRecovery recovery, CardPayment? cardPayment = null)
        {
            // Fail fast on invalid inputs
            if (recovery == null || string.IsNullOrWhiteSpace(recovery.InvoiceNumber))
                return false;

            // Wrap the entire multi-table operation in an atomic transaction
            using var transaction = await Db.Database.BeginTransactionAsync();

            try
            {
                // 1. Fetch the Invoice
                var invoice = await Db.Invoices
                    .FirstOrDefaultAsync(x => x.InvoiceNumber == recovery.InvoiceNumber);

                if (invoice == null)
                    return false;

                // Check if this payment entry already exists in the database
                var existingPayment = await Db.InvoicePayments
                    .FirstOrDefaultAsync(p => p.Id == recovery.Id);

                decimal amountDifference = 0;

                if (existingPayment == null)
                {
                    // ==========================================
                    // CONDITION 1: ADD NEW PAYMENT
                    // ==========================================
                    amountDifference = recovery.Amount; // Add the full amount

                    var newPayment = new InvoicePayment
                    {
                        Id = recovery.Id == Guid.Empty ? Guid.NewGuid() : recovery.Id,
                        CompanyId = invoice.CompanyId,
                        Amount = recovery.Amount,
                        OnDate = recovery.OnDate,
                        PaymentMode = recovery.PaymentMode,
                        CreatedBy = DatabaseService.Instance.CurrentUser.UserName,
                        Deleted = false,
                        Synced = false,
                        InvoiceId = invoice.Id,
                        ReferenceNumber = recovery.PaymentDetails ?? string.Empty,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                    };

                    await Db.InvoicePayments.AddAsync(newPayment);

                    // Add Card Details if applicable
                    if (cardPayment != null && recovery.PaymentMode == PaymentMode.Card) // Assuming PaymentMode enum
                    {
                        cardPayment.InvoiceId = invoice.Id;
                        await Db.CardPayments.AddAsync(cardPayment);
                    }
                }
                else
                {
                    // ==========================================
                    // CONDITION 2 & 3: UPDATE EXISTING PAYMENT
                    // ==========================================

                    // Calculate the difference to adjust the invoice total accurately
                    amountDifference = recovery.Amount - existingPayment.Amount;

                    existingPayment.Amount = recovery.Amount;
                    existingPayment.OnDate = recovery.OnDate;
                    existingPayment.ReferenceNumber = recovery.PaymentDetails ?? string.Empty;
                    existingPayment.UpdatedAt = DateTime.UtcNow;

                    // Handle Condition 3: Payment Mode Changed
                    if (existingPayment.PaymentMode != recovery.PaymentMode)
                    {
                        existingPayment.PaymentMode = recovery.PaymentMode;

                        // Find any existing card payment linked to this invoice/payment
                        var existingCard = await Db.CardPayments
                            .FirstOrDefaultAsync(c => c.InvoiceId == invoice.Id);

                        if (recovery.PaymentMode == PaymentMode.Card && cardPayment != null)
                        {
                            // Switched TO Card
                            if (existingCard == null)
                            {
                                cardPayment.InvoiceId = invoice.Id;
                                await Db.CardPayments.AddAsync(cardPayment);
                            }
                            else
                            {
                                // Update existing card record values
                                Db.Entry(existingCard).CurrentValues.SetValues(cardPayment);
                            }
                        }
                        else if (recovery.PaymentMode != PaymentMode.Card && existingCard != null)
                        {
                            // Switched FROM Card to something else (Cash, UPI, etc.) -> Delete the card record
                            Db.CardPayments.Remove(existingCard);
                        }
                    }

                    Db.InvoicePayments.Update(existingPayment);
                }

                // ==========================================
                // INVOICE & DUE LOGIC
                // ==========================================

                // Adjust the Paid Amount dynamically based on the calculated difference
                invoice.PaidAmount += amountDifference;

                // Assuming BalanceAmount is a property you manage manually (if computed, ignore this line)
                // invoice.BalanceAmount = invoice.TotalAmount - invoice.PaidAmount; 

                Db.Invoices.Update(invoice);

                // Unify the ClearCustomerDue logic
                var dueRecord = await Db.CustomerDues
                    .FirstOrDefaultAsync(x => x.InvoiceNumber == recovery.InvoiceNumber);

                if (dueRecord != null)
                {

                    //TODO:  Handle  for case invoice is due but due entry is paid, 
                    //then just update that
                    // If fully paid, clear the due
                    //TODO: check with accountservice for Invoice Number payment and due 
                    if(dueRecord.Amount==recovery.Amount || recovery.Paid)
                    {
                        dueRecord.ClearingDate = recovery.OnDate;
                        dueRecord.Paid = true;
                        dueRecord.UpdatedAt = DateTime.UtcNow;
                    } 
                    else if (invoice.BalanceAmount <= 0)
                    {
                        dueRecord.ClearingDate = recovery.OnDate;
                        dueRecord.Paid = true;
                        dueRecord.UpdatedAt = DateTime.UtcNow;
                    }
                    // Edge case: If an update *reduced* the payment amount, reopening the due balance
                    else if (dueRecord.Paid && invoice.BalanceAmount > 0)
                    {
                        dueRecord.ClearingDate = null;
                        dueRecord.Paid = false;
                        dueRecord.UpdatedAt = DateTime.UtcNow;
                    }
                    Db.CustomerDues.Update(dueRecord);
                }

                // ==========================================
                // COMMIT & SAVE
                // ==========================================

                var changes = await Db.SaveChangesAsync();

                if (changes > 0)
                {
                    await transaction.CommitAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // If anything fails (Db error, null ref, etc.), completely roll back the database
                await transaction.RollbackAsync();

                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"[DB ERROR] UpdateDueInvoiceAsync failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);

                return false;
            }
        }


        //public static async Task<bool> UpdateDueInvoice( DueRecovery recovery, CardPayment? cardPayment=null)
        //{
        //    //TODO: Condition to remove or add need to handle. 

        //    //Condition 1: Add , all the relevent Amounts, 
        //    //Condition 2: Update, update all relvent amount and models 
        //    //Condition 3: Payment mode is changed  then updare, add or remove relevent models, 

        //    //TODO: rewirite the code to handle all the condition and move to service and make it async as well return Task.Run(async delegate=>{});

        //    // Return false if recovery is null or invoice is null
        //    if (recovery == null) return false;


        //    var invoice = Db.Invoices.Where(x => x.InvoiceNumber == recovery.InvoiceNumber).FirstOrDefault();

        //    if (invoice == null) return false;

        //    invoice.PaidAmount += recovery.Amount;

        //    var invpayyment = new InvoicePayment
        //    {
        //        Id = Guid.NewGuid(),
        //        CompanyId = invoice.CompanyId,
        //        Amount = recovery.Amount,
        //        OnDate = recovery.OnDate,
        //        PaymentMode = recovery.PaymentMode,
        //        CreatedBy = DatabaseService.Instance.CurrentUser.UserName,
        //        Deleted = false,
        //        Synced = false,
        //        InvoiceId = invoice.Id,
        //        ReferenceNumber = recovery.PaymentDetails ?? "",
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow, 
        //    };
        //    Db.InvoicePayments.Add(invpayyment);

        //    if (cardPayment != null) { 

        //        cardPayment.InvoiceId= invpayyment.InvoiceId;
        //        Db.CardPayments.Add(cardPayment);
        //    }


        //    var result = (await Db.SaveChangesAsync()) > 0;

        //    if (invoice.BalanceAmount <= 0)
        //    {
        //        result = await ClearCustomerDue(recovery.InvoiceNumber, recovery.OnDate);

        //    }

        //    return result;
        //}
        //public static async Task<bool> ClearCustomerDue(string InvoiceNumber, DateTime payingDate)
        //{

        //    //TODO : Clear Customer Due when invoice is fully paid and update the due table with clearing date and paid status
        //    var due = Db.CustomerDues.Where(x => x.InvoiceNumber == InvoiceNumber).FirstOrDefault();
        //    if (due != null)
        //    {
        //        due.ClearingDate = payingDate;
        //        due.Paid = true;
        //        due.UpdatedAt = DateTime.UtcNow;
        //        Db.CustomerDues.Update(due);
        //    }

        //    return await Db.SaveChangesAsync() > 0;
        //}

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

        public static async Task<Guid> GetLedgerGroupForPartyAsync(PartyType partyType)
        {
            //TODO : Move to Service and make it async as well return Task.Run(async delegate=>{});

            //Guid? id;
            // Corrected the issue with nullable type and async-await usage
            Guid id = Guid.Empty;

            id = partyType switch
            {
                PartyType.Customer => ((await Db.LedgerGroups.FirstOrDefaultAsync(static x => x.Name == "Customers"))?.Id) ?? Guid.Empty,
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