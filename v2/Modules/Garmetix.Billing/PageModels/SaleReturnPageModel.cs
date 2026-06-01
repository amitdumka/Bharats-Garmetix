using Garmetix.Base.PageModels;
using Garmetix.Core.Models.Inventory;
using Syncfusion.Maui.DataGrid;

namespace Garmetix.Billing.PageModels
{
    /// <summary>
    /// PageModel for managing and displaying sale return invoices, providing functionalities to view, sort, and interact with sale return data in a structured grid format.
    /// </summary>
    public partial class SaleReturnPageModel:PageModel<Invoice>
    {
        //TODO: use the Sale Invoice page to list , as this Doesnt Have  DTO or One Type of Invoice 

        public SaleReturnPageModel() : base()
        {
            DefaultSortedColName = nameof(Invoice.OnDate);
            DefaultSortedOrder = Descending;
        }


        protected override ColumnCollection SetGridColumns()
        {
            GridColumns = [
                AddDateGridColumns(nameof(Invoice.OnDate)),
                AddGridColumns(nameof(Invoice.InvoiceNumber)),
                AddGridColumns(nameof(Invoice.BillAmount)),
                AddGridColumns(nameof(Invoice.CustomerMobileNumber)),
                AddGridColumns(nameof(Invoice.Quantity)),
                AddGridColumns(nameof(Invoice.PaymentMode))
                
                
               ];
            return GridColumns;
        }
    }
}
