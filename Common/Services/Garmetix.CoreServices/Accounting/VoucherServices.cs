using Bharat.ToolKits.Notifications;
using Garmetix.CoreServices; 
using Garmetix.Models.Accounting;
using Garmetix.Models.Reports;
using Microsoft.EntityFrameworkCore;

namespace Garmetix.ModuleService
{
    public class VoucherServices : BaseServices
    {
        public static VoucherServices Instance { get; } = new VoucherServices();
        public VoucherServices() : base() { 
        
           if(Instance == null)
            {
               // Instance = this;
            }
        }

        /// <summary>
        /// Check if Ledger is Party
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool?> IsParyLedgerAsync(Guid id)
        {
            if(id == Guid.Empty) return false;
            return (await Db.Ledgers.FindAsync(id))!.IsParty;
        }

        /// <summary>
        /// Sort Ledgers by Date and calculate balance
        /// </summary>
        /// <param name="ledgers"></param>
        /// <returns></returns>
        public static List<LedgerInfo> SortLedgers(List<LedgerInfo> ledgers)
        {
            if (ledgers == null || ledgers.Count == 0)
            {
                return [];
            }

            ledgers = ledgers.OrderBy(c => c.Date).ToList();
            decimal balance = 0;
            foreach (var item in ledgers)
            {
              balance=  item.Balance = (balance + item.In - item.Out);
            }
            return ledgers;
        }
        public GeneralLedger? GetLedgersById(Guid id)
        {

             try
            {
                var ledger = Db.Ledgers.Find(id);
                if (ledger == null)
                {
                    return null;
                }

                 
                var vouchers = Db.Vouchers.Where(c => c.LedgerId == ledger.Id).Select(c => new { c.VoucherType, c.Particulars, c.VoucherNumber, c.Amount, c.PaymentMode, c.PaymentDetails, c.OnDate, c.Remarks })
                    .ToList();
                var cashVouchers = Db.CashVouchers.Include(c=> c.Transaction).Where(c => c.LedgerId == ledger.Id).Select(c => new { c.VoucherType, c.Particulars, c.Amount, c.Transaction!.Name, c.VoucherNumber, c.OnDate, c.Remarks })
                    .ToList();
                var receipts = vouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = c.Amount,
                        Out = 0,
                        Balance = 0
                    })
                    .ToList();
                var payments = vouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = 0,
                        Out = c.Amount,
                        Balance = 0
                    })
                    .ToList();
                var cashreceipts = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    In = c.Amount,
                    Out = 0,
                    Balance = 0
                }).ToList();
                var cashpayments = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    Out = c.Amount,
                    In = 0,
                    Balance = 0
                }).ToList();

                var genLedegersInfo = new GeneralLedger
                {
                    LegerType = ledger.LedgerType.ToString(),
                    LedgerName = ledger.Name,
                    Ledgers = [],
                };

                genLedegersInfo.Ledgers.AddRange(receipts);
                genLedegersInfo.Ledgers.AddRange(payments);
                genLedegersInfo.Ledgers.AddRange(cashpayments);
                genLedegersInfo.Ledgers.AddRange(cashreceipts);

                genLedegersInfo.Ledgers = VoucherServices.SortLedgers(genLedegersInfo.Ledgers);

                return genLedegersInfo;
            }
            catch (Exception ex)
            {
                _ = Notify.DisplayNotificationAsync(ex.Message, isLong: true );
                return null;
            }

        }
        public GeneralLedger? GetLedgersById(Guid id, int startMonth, int startYear, int? endMonth = null, int? endYear = null)
        {
            try
            {
                var ledger = Db.Ledgers.Find(id);
                if (ledger == null)
                {
                    return null;
                }

                // Determine the date range
                DateTime startDate = new DateTime(startYear, startMonth, 1);
                DateTime? endDate = null;

                if (endMonth.HasValue && endYear.HasValue)
                {
                    // If both end month and year are provided, set the end date to the last day of that month
                    endDate = new DateTime(endYear.Value, endMonth.Value, DateTime.DaysInMonth(endYear.Value, endMonth.Value));
                }
                else
                {
                    // If only start month and year are provided, the range is just that month
                    endDate = new DateTime(startYear, startMonth, DateTime.DaysInMonth(startYear, startMonth));
                }

                var vouchers = Db.Vouchers
                    .Where(c => c.LedgerId == ledger.Id && c.OnDate >= startDate && c.OnDate <= endDate)
                    .Select(c => new { c.VoucherType, c.Particulars, c.VoucherNumber, c.Amount, c.PaymentMode, c.PaymentDetails, c.OnDate, c.Remarks })
                    .ToList();

                var cashVouchers = Db.CashVouchers.Include(c => c.Transaction)
                    .Where(c => c.LedgerId == ledger.Id && c.OnDate >= startDate && c.OnDate <= endDate)
                    .Select(c => new { c.VoucherType, c.Particulars, c.Amount, c.Transaction!.Name, c.VoucherNumber, c.OnDate, c.Remarks })
                    .ToList();

                var receipts = vouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = c.Amount,
                        Out = 0,
                        Balance = 0
                    })
                    .ToList();

                var payments = vouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = 0,
                        Out = c.Amount,
                        Balance = 0
                    })
                    .ToList();

                var cashreceipts = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    In = c.Amount,
                    Out = 0,
                    Balance = 0
                }).ToList();

                var cashpayments = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    Out = c.Amount,
                    In = 0,
                    Balance = 0
                }).ToList();

                var genLedegersInfo = new GeneralLedger
                {
                    LegerType = ledger.LedgerType.ToString(),
                    LedgerName = ledger.Name,
                    Ledgers = [],
                };

                genLedegersInfo.Ledgers.AddRange(receipts);
                genLedegersInfo.Ledgers.AddRange(payments);
                genLedegersInfo.Ledgers.AddRange(cashpayments);
                genLedegersInfo.Ledgers.AddRange(cashreceipts);

                genLedegersInfo.Ledgers = VoucherServices.SortLedgers(genLedegersInfo.Ledgers);

                return genLedegersInfo;
            }
            catch (Exception ex)
            {
                _ = Notify.DisplayNotificationAsync(ex.Message, isLong: true);
                return null;
            }
        }

        /// <summary>
        /// Get Party Ledger by Id 
        /// </summary>
        /// <param name="id"></param>
        public PartyLedger? GetPartyLedgerById(Guid id)
        {
            try
            {
                var party = Db.Parties.Include(c => c.Ledger).Where(c => c.Id == id || c.LedgerId == id).FirstOrDefault();

                if (party == null)
                {
                    return null;
                }

                
                PartyLedger partyLedger = new()
                {
                    Address =party?.Address??"",
                    Email = party?.EmailId?? "",
                    Gstin = party?.GSTIN ?? "",
                    Phone = party?.Phone ?? "",
                    PartyName = party?.Name??"",
                    LegerType = party?.Category.ToString() + " / " + party!.Ledger?.LedgerType.ToString(),
                };

                var vouchers = Db.Vouchers.Where(c => c.PartyId == id || c.LedgerId == party.LedgerId)
                    .Select(c => new { c.VoucherType, c.Particulars, c.VoucherNumber, c.Amount, c.PaymentMode, c.PaymentDetails, c.OnDate, c.Remarks })
                    .ToList();

                var cashVouchers = Db.CashVouchers.Include(c => c.Transaction).Where(c => c.LedgerId == party.LedgerId)
                    .Select(c => new { c.VoucherType, c.Particulars, c.Amount, c.Transaction!.Name, c.VoucherNumber, c.OnDate, c.Remarks })
                    .ToList();
                // Receipts
                var receipts = vouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = c.Amount,
                        Out = 0,
                        Balance = 0
                    })
                    .ToList();
                var payments = vouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = 0,
                        Out = c.Amount,
                        Balance = 0
                    })
                    .ToList();
                var cashreceipts = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    In = c.Amount,
                    Out = 0,
                    Balance = 0
                }).ToList();
                var cashpayments = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    Out = c.Amount,
                    In = 0,
                    Balance = 0
                }).ToList();

                partyLedger.Ledgers.AddRange(receipts);
                partyLedger.Ledgers.AddRange(payments);
                partyLedger.Ledgers.AddRange(cashpayments);
                partyLedger.Ledgers.AddRange(cashreceipts);

                partyLedger.Ledgers = VoucherServices.SortLedgers(partyLedger.Ledgers);

                return partyLedger;
            }
            catch (Exception ex)
            {
                _ = Notify.DisplayNotificationAsync(ex.Message, isLong: true);
                return null;
            }
        }
        public PartyLedger? GetPartyLedgerById(Guid id, int startMonth, int startYear, int? endMonth = null, int? endYear = null)
        {
            try
            {
                var party = Db.Parties.Include(c => c.Ledger).Where(c => c.Id == id || c.LedgerId == id).FirstOrDefault();

                if (party == null)
                {
                    return null;
                }

                PartyLedger partyLedger = new()
                {
                    Address = party?.Address ?? "",
                    Email = party?.EmailId ?? "",
                    Gstin = party?.GSTIN ?? "",
                    Phone = party?.Phone ?? "",
                    PartyName = party?.Name ?? "",
                    LegerType = party?.Category.ToString() + " / " + party!.Ledger?.LedgerType.ToString(),
                };

                // Determine the date range
                DateTime startDate = new DateTime(startYear, startMonth, 1);
                DateTime? endDate = null;

                if (endMonth.HasValue && endYear.HasValue)
                {
                    // If both end month and year are provided, set the end date to the last day of that month
                    endDate = new DateTime(endYear.Value, endMonth.Value, DateTime.DaysInMonth(endYear.Value, endMonth.Value));
                }
                else
                {
                    // If only start month and year are provided, the range is just that month
                    endDate = new DateTime(startYear, startMonth, DateTime.DaysInMonth(startYear, startMonth));
                }


                var vouchers = Db.Vouchers
                    .Where(c => (c.PartyId == id || c.LedgerId == party.LedgerId) && c.OnDate >= startDate && c.OnDate <= endDate)
                    .Select(c => new { c.VoucherType, c.Particulars, c.VoucherNumber, c.Amount, c.PaymentMode, c.PaymentDetails, c.OnDate, c.Remarks })
                    .ToList();

                var cashVouchers = Db.CashVouchers.Include(c => c.Transaction)
                    .Where(c => c.LedgerId == party.LedgerId && c.OnDate >= startDate && c.OnDate <= endDate)
                    .Select(c => new { c.VoucherType, c.Particulars, c.Amount, c.Transaction!.Name, c.VoucherNumber, c.OnDate, c.Remarks })
                    .ToList();

                // Receipts
                var receipts = vouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = c.Amount,
                        Out = 0,
                        Balance = 0
                    })
                    .ToList();

                var payments = vouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt)
                    .Select(c => new LedgerInfo
                    {
                        PaymentMode = c.PaymentMode.ToString(),
                        Particulars = $"VN:{c.VoucherNumber}, {c.Particulars}",
                        Date = c.OnDate,
                        In = 0,
                        Out = c.Amount,
                        Balance = 0
                    })
                    .ToList();

                var cashreceipts = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType == VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    In = c.Amount,
                    Out = 0,
                    Balance = 0
                }).ToList();

                var cashpayments = cashVouchers.Where(c => c.Amount > 0 && c.VoucherType != VoucherType.Receipt).Select(c => new LedgerInfo
                {
                    PaymentMode = c.Name,
                    Particulars = c.Particulars,
                    Date = c.OnDate,
                    Out = c.Amount,
                    In = 0,
                    Balance = 0
                }).ToList();

                partyLedger.Ledgers.AddRange(receipts);
                partyLedger.Ledgers.AddRange(payments);
                partyLedger.Ledgers.AddRange(cashpayments);
                partyLedger.Ledgers.AddRange(cashreceipts);

                partyLedger.Ledgers = VoucherServices.SortLedgers(partyLedger.Ledgers);

                return partyLedger;
            }
            catch (Exception ex)
            {
                _ = Notify.DisplayNotificationAsync(ex.Message, isLong: true);
                return null;
            }
        }
    }
}