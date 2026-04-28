/*
 * Garmetix
 * Author: Amit Kumar
 * https://garmetix.com/
 * Copyright (c) 2026. All rights reserved.
 * Version: 6.0.0
 * License: https://garmetix.com/license
 * Website: https://garmetix.com/
*/
/*
 * Invoicing.cs
 * 
 * This file contains the definitions of classes related to invoicing in the Garmetix application.
 * It includes classes for Salesman, BaseInvoice, Customer, and Vendor.
 * These classes are designed to represent the various entities and their relationships within the invoicing system.
 * 
 * The classes are structured to support features such as tracking invoice details, calculating tax amounts, and managing customer and vendor information.
 * They also include properties for handling payment modes, GST system, and VAT system.
 * 
 * The use of attributes like [JsonIgnore] helps to control the serialization of certain properties when converting objects to JSON format.
 * This is particularly useful for properties that are calculated or derived from other properties, ensuring that only relevant data is included in API responses or data storage.
 *
 * 
 * Garmetix is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Garmetix.  If not, see <http://www.gnu.org/licenses/>.
 */

using Garmetix.Core.Enums;
using Garmetix.Core.Models.Accounting;
using Garmetix.Core.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Garmetix.Core.Models.Inventory
{
    public class Salesman : StoreBase
    {
        [Display(Name = "Name")] public string Name { get; set; } = "Manager";
        [Display(Name = "Employee", AutoGenerateField = false)] public Guid? EmployeeId { get; set; }
        [Display(Name = "Active")] public bool Active { get; set; } = true;
    }

    public abstract class BaseInvoice : CompanyBase
    {
        [Display(Name = "Invoice Number")] public required string InvoiceNumber { get; set; }
        [Display(Name = "Date")] public DateTime OnDate { get; set; }
        [Display(Name = "Return Invoice")] public bool ReturnInvoice { get; set; } = false;
        [Display(Name = "MRP")] public decimal MRP { get; set; }

        [Display(Name = "Base Price")] public decimal BasePrice { get; set; }
        [Display(Name = "Discount Amount")] public decimal DiscountAmount { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Net Amount")] public decimal NetAmount { get; set; }
        [Display(Name = "Round Off")] public decimal RoundOff { get; set; } = 0;
        [Display(Name = "Bill Amount")] public decimal BillAmount { get; set; }

        [Display(Name = "Quantity")] public decimal Quantity { get; set; }
        [Display(Name = "Actual Quantity")] public decimal ActualQuantity { get; set; } = 0;
        [Display(Name = "Billed Quantity")] public decimal BilledQuantity { get; set; } = 0;
        [Display(Name = "Item Count")] public int ItemCount { get; set; }
        [Display(Name = "Payment Mode")] public PaymentMode? PaymentMode { get; set; }
        //Handling GST System and Vat System as well

        [Display(Name = "CGST Amount")] public decimal? CGSTAmount { get; set; }
        [Display(Name = "SGST Amount")] public decimal? SGSTAmount { get; set; }
        [Display(Name = "IGST Amount")] public decimal? IGSTAmount { get; set; }
        [Display(Name = "Inter State")] public bool InterState { get; set; } = false;

    }


    public class Customer : CompanyBase
    {
        [Display(Name = "Name")] public required string Name { get; set; }
        [Display(Name = "Address")] public string Address { get; set; } = "Dumka";
        [Display(Name = "City")] public string City { get; set; } = "Dumka";
        [Display(Name = "Zip Code")] public string ZipCode { get; set; } = "814101";
        [Display(Name = "State")] public string? State { get; set; } = "Jharkhand";
        [Display(Name = "Country")] public string Country { get; set; } = "India";
        [Display(Name = "Mobile Number")] public required string MobileNumber { get; set; }
        [Display(Name = "Email")] public string? Email { get; set; }

        [Display(Name = "Birth Date")] public DateTime? BirthDate { get; set; }
        [Display(Name = "Anniversary")] public DateTime? Aniversary { get; set; }
        [Display(Name = "Party ID")] public Guid? PartyId { get; set; }
        [Display(Name = "Registered")] public bool Registred { get; set; } = false;
        [Display(Name = "GSTIN")] public string? GSTIN { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; } = 0;
        [Display(Name = "Bill Count")] public int BillCount { get; set; } = 0;
    }

    public class Vendor : CompanyBase
    {
        [Display(Name = "Name")] public required string Name { get; set; }
        [Display(Name = "Address")] public required string Address { get; set; }
        [Display(Name = "City")] public required string City { get; set; }
        [Display(Name = "Zip Code")] public string? ZipCode { get; set; }
        [Display(Name = "Mobile Number")] public required string MobileNumber { get; set; }
        [Display(Name = "Email")] public string? Email { get; set; }
        [Display(Name = "Start Date")] public DateTime StartDate { get; set; } = DateTime.Now;
        [Display(Name = "End Date")] public DateTime? EndDate { get; set; } = null;

        [Display(Name = "GSTIN")] public string? GSTIN { get; set; }
        [Display(Name = "PAN")] public string? Pan { get; set; }
        [Display(Name = "TAN")] public string? Tan { get; set; }
        [Display(Name = "Active")] public bool Active { get; set; }
        [Display(Name = "Party", AutoGenerateField = false)] public Guid? PartyId { get; set; }

        [Display(Name = "Party", AutoGenerateField = false)] public virtual Party? Party { get; set; }
        [Display(Name = "Bill Count")] public int BillCount { get; set; } = 0;
        [Display(Name = "Bill Amount")] public decimal BillAmount { get; set; } = 0;
        [Display(Name = "Paid Amount")] public decimal Paid { get; set; } = 0;
        [Display(Name = "Balance Amount")]
        [JsonIgnore]
        public decimal Balance
        { get { return Math.Round(BillAmount - Paid, 0); } }
    }


    public class PurchaseInvoice : BaseInvoice
    {
        [Display(Name = "Vendor", AutoGenerateField = false)] public Guid VendorId { get; set; }
        [Display(Name = "Vendor Name")] public string? VendorName { get; set; }
        [Display(Name = "Vendor GSTIN")] public string? VendorGSTIN { get; set; }
        [Display(Name = "Inward Number")] public required string InwardNumber { get; set; }
        [Display(Name = "Inward Date")] public DateTime InwardDate { get; set; } = DateTime.Now.AddDays(-1);
        [Display(Name = "Fright Amount")] public decimal FrightAmount { get; set; } = 0;
        [Display(Name = "Vendor", AutoGenerateField = false)] public virtual Vendor? Vendor { get; set; }
        [Display(Name = "Due Date")] public DateTime DueDate { get; set; } = DateTime.Today.AddDays(45);
    }

    public class Invoice : BaseInvoice
    {
        [Display(Name = "Customer", AutoGenerateField = false)] public Guid CustomerId { get; set; }
        [Display(Name = "Salesman", AutoGenerateField = false)] public Guid SalemanId { get; set; }

        [Display(Name = "Customer Name")] public string? CustomerName { get; set; }
        [Display(Name = "Customer Mobile Number")] public required string CustomerMobileNumber { get; set; }
        [Display(Name = "Customer GSTIN")] public string? CustomerGSTIN { get; set; }

        [Display(Name = "Credit Sale")] public bool CreditSale { get; set; }
        [Display(Name = "B2B Sale")] public bool B2BSale { get; set; } = false;

        [Display(Name = "Salesman", AutoGenerateField = false)] public virtual Salesman? Saleman { get; set; }
        [Display(Name = "Customer", AutoGenerateField = false)] public virtual Customer? Customer { get; set; }
        [Display(Name = "Invoice Items", AutoGenerateField = false)] public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    }

    public class InvoicePayment : CompanyBase
    {
        [Display(Name = "Invoice", AutoGenerateField = false)] public Guid InvoiceId { get; set; }
        [Display(Name = "Date")] public DateTime OnDate { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; }
        [Display(Name = "Reference Number")] public string? ReferenceNumber { get; set; }

        [Display(Name = "Payment Mode")] public PaymentMode PaymentMode { get; set; }
    }

    public class CardPayment : CompanyBase
    {
        [Display(Name = "Invoice", AutoGenerateField = false)] public Guid InvoiceId { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; }
        [Display(Name = "Date")] public DateTime OnDate { get; set; }
        [Display(Name = "Auth Code")] public int AuthCode { get; set; }
        [Display(Name = "Card Number")] public int CardNumber { get; set; }
        [Display(Name = "Card Type")] public CARDType CardType { get; set; }
        [Display(Name = "Card")] public CARD Card { get; set; }
        [Display(Name = "Bank Name")] public string? BankName { get; set; }
    }

    public class VendorPayment : CompanyBase
    {
        [Display(Name = "Vendor", AutoGenerateField = false)] public Guid VendorId { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; }
        [Display(Name = "UTR Number")] public string? UTRNumber { get; set; }
        [Display(Name = "Cheque Number")] public string? ChequeNumber { get; set; }
        [Display(Name = "Date")] public DateTime OnDate { get; set; }
        [Display(Name = "Invoice", AutoGenerateField = false)] public Guid InvoiceId { get; set; }
        [Display(Name = "Invoice", AutoGenerateField = false)] public virtual Invoice? Invoice { get; set; }
        [Display(Name = "Vendor", AutoGenerateField = false)] public virtual Vendor? Vendor { get; set; }
    }

    //TODO: need to make it robust so no need to store in db which can be calculated
    //TODO: add JsonIgnore attribute for those properties
    public class InvoiceItem : CompanyBase
    {
        [Display(Name = "Invoice", AutoGenerateField = false)] public Guid InvoiceId { get; set; }
        [Display(Name = "Product", AutoGenerateField = false)] public Guid ProductId { get; set; }
        [Display(Name = "Barcode")] public required string Barcode { get; set; }

        [Display(Name = "MRP")] public decimal MRP { get; set; }
        [Display(Name = "Discount Amount")] public decimal DiscountAmount { get; set; }
        [Display(Name = "Base Price")] public decimal BasePrice { get; set; }
        [Display(Name = "Tax Percentage")] public decimal TaxPercentage { get; set; }
        [Display(Name = "Tax Amount")] public decimal TaxAmount { get; set; }
        [Display(Name = "Amount")] public decimal Amount { get; set; }

        [Display(Name = "Tax Type")] public TaxType TaxType { get; set; }
        [Display(Name = "Tax", AutoGenerateField = false)] public Guid TaxId { get; set; }
        [Display(Name = "Tax", AutoGenerateField = false)] public virtual Tax? Tax { get; set; }
        [Display(Name = "Billed Quantity")] public decimal BilledQuantity { get; set; }
        [Display(Name = "Actual Quantity")] public decimal ActualQuantity { get; set; }

        [JsonIgnore]
        [Display(Name = "Product", AutoGenerateField = false)] public virtual Product? Product { get; set; }
        [JsonIgnore]
        [Display(Name = "Invoice", AutoGenerateField = false)] public virtual Invoice? Invoice { get; set; }

        [JsonIgnore]
        [Display(Name = "Taxable Amount")] public decimal TaxableAmount { get { return Math.Round((MRP - DiscountAmount) / (1 + (TaxPercentage / 100)), 2); } }
        [JsonIgnore]
        [Display(Name = "Total Tax Amount")] public decimal TotalTaxAmount { get { return Math.Round(TaxableAmount * (TaxPercentage / 100), 2); } }

        [JsonIgnore]
        [Display(Name = "Line Total")] public decimal LineTotal { get { return Math.Round(TaxableAmount + TotalTaxAmount, 2); } }

    }

    public class PurchaseInvoiceItem : InvoiceItem
    {

        [Display(Name = "Invoice", AutoGenerateField = false)] public virtual new PurchaseInvoice? Invoice { get; set; }
    }
}