using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Garmetix.Billing.Models;
using Garmetix.Billing.PageModels.Invoices;
using Garmetix.Billing.Services;
using Garmetix.Core.Enums;
using Garmetix.Core.Models.Inventory;
using Garmetix.Databases.Services;

namespace Garmetix.Billing.PageModels
{
    [QueryProperty(nameof(InvoiceId), "InvoiceId")]
    public partial class InvoiceEditPageModel : BaseInvoiceFormModel
    {
        [ObservableProperty] protected string invoiceId = string.Empty;
        [ObservableProperty] protected Invoice orginalInvoice;
        public InvoiceEditPageModel(InvoiceService invoiceService) : base(invoiceService)
        {
        }

        partial void OnInvoiceIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value)) _ = LoadInvoiceDataAsync(Guid.Parse(value));
        }

        protected async Task LoadInvoiceDataAsync(Guid id)
        {
            IsBusy = true;
            try
            {
                OrginalInvoice = await _invoiceService.GetInvoiceByIdAsync(id);
                CurrentInvoice = OrginalInvoice.ToInvoiceDto();

                if (CurrentInvoice.GlobalDiscountAmount > 0)
                {
                    GlobalDiscountInput = CurrentInvoice.GlobalDiscountAmount;
                    GlobalDiscountTypeInput = "Amount";
                }

                InvoiceItems.Clear();

                foreach (var item in OrginalInvoice.InvoiceItems)
                {
                    var dtoItem = item.ToEntryItem() ?? new EntryItem();
                    dtoItem.PropertyChanged += (s, e) => CalculateTotals();
                    //Aading Product Name 
                    dtoItem.ProductName= GetContext().Products.Where(p => p.Id == item.ProductId).Select(p => p.Name).FirstOrDefault() ?? "";
                    InvoiceItems.Add(dtoItem);
                }

                // 4. Load Split Payments
                Payments.Clear();

                foreach (var item in orginalInvoice.Payments)
                {
                    if (item.PaymentMode == PaymentMode.Card)
                    {
                        var card = orginalInvoice.CardPayments.Where(x => x.Id == item.Id).FirstOrDefault();
                        Payments.Add(item.ToPaymentDetail(card) ?? new PaymentDetail());
                    }
                    else
                    {
                        Payments.Add(item.ToPaymentDetail() ?? new PaymentDetail());
                    }
                }

                CalculateTotals();
            }
            catch (Exception ex) { await InvoiceService.ShowErrorAsync("Load Error", ex); }
            finally { IsBusy = false; }
        }
        // Replace 'partial void OnSelectedProductChanged' with this:
        //protected override void HandleProductSelected(Product? value)
        //{
        //    if (value != null)
        //    {
        //        var newItem = new EntryItem
        //        {
        //            InvoiceId = CurrentInvoice.Id,
        //            ProductName = value.Name,
        //            Category = value.ProductType,
        //            BasePrice = value.BasicPrice,
        //            BilledQuantity = 1m, Barcode=value.Barcode,
        //        };
        //        newItem.PropertyChanged += (s, e) => CalculateTotals();
        //        InvoiceItems.Add(newItem);

        //        SearchText = string.Empty;
        //        CalculateTotals();
        //    }
        //}

        [RelayCommand]
        public void RemoveItem(EntryItem item)
        {//TODO: handle for edititems  vs invoice items, we need to track deleted items to remove from db on save
            if (item != null && InvoiceItems.Contains(item))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    InvoiceItems.Remove(item);
                    CalculateTotals();
                });
            }
        }



        [RelayCommand]
        public async Task SaveChangesAsync()
        {
            if (InvoiceItems.Count == 0) return;
            IsBusy = true;
            try
            {
                // Call your update logic here mapping InvoiceItems and Payments to Entities
                var result = await UpdateInvoicesAsync(CurrentInvoice, InvoiceItems, Payments);
                if (result)
                {
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Success", "Invoice updated successfully.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            finally { IsBusy = false; }
        }

        public override async Task ClearFormAsync()
        {
            bool confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Cancel", "Discard your edits?", "Yes", "No");
            if (confirm) await Shell.Current.GoToAsync("..");
        }


        [RelayCommand]
        public async Task CancelEditAsync()
        {
            await ClearFormAsync();
            //TODO: make it more robust by checking if there are unsaved changes before prompting
        }

        //TODO: move to service or make it robust
        public async Task<bool> UpdateInvoicesAsync(InvoiceDTO invoicedto, IEnumerable<EntryItem> InvoiceItems, IEnumerable<PaymentDetail> paymentDetails)
        {
            if (InvoiceItems.Count() == 0)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Validation", "Cannot update empty invoice.", "OK");
                return false;
            }

            if (isSaving)
                return false;

            try
            {
                isSaving = true;

                // Payment details is updated
                var invoicePaymentList = new List<InvoicePayment>();
                var cardpaymentList = new List<CardPayment>();

                // Invoice details are updated
                var editedInvoice = new Invoice
                {
                    Synced = false,
                    Deleted = false,
                    CompanyId = orginalInvoice.CompanyId,
                    StoreId = orginalInvoice.StoreId,

                    InvoiceStatus = orginalInvoice.InvoiceStatus, //TODO: need to handle invoice status changes during edit
                    InvoiceType = orginalInvoice.InvoiceType,
                    SaleInvoiceType = orginalInvoice.SaleInvoiceType,
                    PaymentMode = orginalInvoice.PaymentMode, //TODO: need to handle payment mode changes during edit

                    SalemanId = orginalInvoice.SalemanId, //TODO: need to handle salesman changes
                    CustomerId = orginalInvoice.CustomerId, //TODO: need to handle customer changes

                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = DatabaseService.Instance.CurrentUser.Name,
                    CreatedAt = orginalInvoice.CreatedAt,

                    B2BSale = invoicedto.IsB2BSale,
                    InterState = invoicedto.IsInterStateSale,
                    ReturnInvoice = false,

                    Id = orginalInvoice.Id,
                    InvoiceNumber = OrginalInvoice.InvoiceNumber,

                    OnDate = invoicedto.OnDate,

                    CustomerGSTIN = invoicedto.CustomerGSTIN,
                    CustomerMobileNumber = invoicedto.CustomerMobileNumber,
                    CustomerName = invoicedto.CustomerName,

                    ItemCount = InvoiceItems.Count(), //Count of the Item in invoice
                    Quantity = invoicedto.BilledQuantity, //Total Qty of the invoice 

                    CreditSale = invoicedto.BalanceAmount > 0 ? true : false,

                    BasePrice = invoicedto.SubTotal,//TODO: need to handle base price changes during edit

                    DiscountAmount = invoicedto.TotalDiscount,

                    NetAmount = invoicedto.SubTotal, //TODO: need to handle net amount changes during edit

                    TaxAmount = invoicedto.TotalTax,
                    CGSTAmount = invoicedto.TotalTax / 2m,
                    SGSTAmount = invoicedto.TotalTax / 2m,
                    IGSTAmount = invoicedto.TotalTax,

                    BillDiscountAmount = invoicedto.GlobalDiscountAmount,

                    RoundOff = invoicedto.RoundOffAmount,
                    BillAmount = invoicedto.GrandTotal,

                    PaidAmount = invoicedto.PaidAmount,

                };

                if (editedInvoice.PaidAmount < editedInvoice.BillAmount)
                {
                    bool proceed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Part Payment", $"Balance of ₹ {invoicedto.BalanceAmount} is unpaid. Proceed?", "Yes", "No");
                    if (!proceed) return false;
                }

                // Map EntryItems to InvoiceItems and handle additions, updates, and deletions
                var invoiceItemList = new List<InvoiceItem>();
                try
                {
                    foreach (var item in InvoiceItems)
                    {
                        invoiceItemList.Add(new InvoiceItem
                        {
                            CompanyId = editedInvoice.CompanyId,
                            Deleted = false,
                            Synced = false,
                            CreatedAt = editedInvoice.CreatedAt,
                            UpdatedAt = editedInvoice.UpdatedAt,
                            CreatedBy = editedInvoice.CreatedBy,
                            InvoiceId = editedInvoice.Id,
                            Id = Guid.NewGuid(),// item.Id,
                            Barcode = item.Barcode,
                            BilledQuantity = item.BilledQuantity,
                            BasePrice = item.BasePrice,
                            DiscountAmount = item.DiscountAmount,
                            Amount = item.TotalAmount,
                            Category = item.Category,
                            TaxAmount = item.TaxAmount,
                            TaxPercentage = item.GstPercentage,
                            TaxType = editedInvoice.InterState ? TaxType.IGST : TaxType.GST,
                            ProductId = item.ProductId,
                            MRP = item.MRP,
                            TaxId = BillingService.GetTaxIdByType(editedInvoice.InterState ? TaxType.IGST : TaxType.GST, item.GstPercentage, true)

                        });
                    }

                    editedInvoice.MRP = invoiceItemList.Sum(i => i.MRP);

                    // 1. Identify distinct payment modes in the current transaction
                    var distinctModes = paymentDetails.Select(p => p.PaymentMode).Distinct().ToList();

                    // 2. Set overall paymentMode based on the count of distinct modes
                    editedInvoice.PaymentMode = distinctModes.Count > 1
                         ? PaymentMode.MixPayments
                         : distinctModes.FirstOrDefault();

                    foreach (var item in paymentDetails)
                    {
                        if (item.PaymentMode == PaymentMode.Card)
                        {
                            cardpaymentList.Add(new CardPayment
                            {
                                InvoiceId = editedInvoice.Id,

                                Amount = item.Amount,
                                UpdatedAt = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                Deleted = false,
                                CreatedBy = editedInvoice.CreatedBy,
                                Id = Guid.NewGuid(),// item.Guid,
                                OnDate = item.PaymentDate,
                                Synced = false,
                                CardNumber = item.CardPaymentNumber.Value,
                                CardType = item.CardType.Value,
                                Card = item.Card.Value,
                                BankName = item.CardPaymentBank,
                                AuthCode = item.AuthCode.Value,
                                CompanyId = editedInvoice.CompanyId,
                                StoreId = editedInvoice.StoreId,

                            });
                        }

                        invoicePaymentList.Add(new InvoicePayment
                        {
                            StoreId = editedInvoice.StoreId,
                            Amount = item.Amount,
                            CompanyId = editedInvoice.CompanyId,
                            CreatedAt = editedInvoice.CreatedAt,
                            UpdatedAt = DateTime.UtcNow,
                            CreatedBy = editedInvoice.CreatedBy,
                            Id = Guid.NewGuid(),// item.Guid,
                            Deleted = false,
                            OnDate = item.PaymentDate,
                            InvoiceId = editedInvoice.Id,
                            PaymentMode = item.PaymentMode,
                            Synced = false,
                            ReferenceNumber = item.PaymentNote
                        });
                    }

                    return await _invoiceService.UpdateInvoicesAsync(editedInvoice, invoiceItemList, invoicePaymentList, cardpaymentList);
                }
                catch (Exception ex)
                {
                    await InvoiceService.ShowErrorAsync("Update Invoice Error", ex);
                    throw;
                }
                finally
                {
                    // Clear the lists to free memory
                    invoiceItemList.Clear();
                    invoicePaymentList.Clear();
                    cardpaymentList.Clear();
                }
                // Notify the dashboard that the database has changed!
                // DashboardDataService.Instance.InvalidateCache();
                //DatabaseService.Instance.InvalidateCache();
                //return true;
            }
            catch (Exception ex)
            {
                await InvoiceService.ShowErrorAsync("Save Invoice Error", ex);
                return false;
            }
            finally
            {
                isSaving = false;
            }
        }

    }
}