namespace Garmetix.Models
{
    namespace Bharat.Enums
    {
        public enum Order
        { Asc, Desc }

        public enum UserAccess
        {
            Admin,
            SuperAdmin,
            SuperUser,
            PowerUser,
            User,
            Guest,
        }

        public enum Unit
        {
            Meters,
            Nos,
            Pcs,
            Packets,
            Grams,
            Kgs,
            Liter,
            NoUnit,
            Than, Boxes
        }

        public enum UOM
        {
            Meters,
            Nos,
            Pcs,
            Packets,
            Grams,
            Kgs,
            Liter,
            NoUnit,
            Than,
            Boxes
        }

        public enum AttUnit
        {
            Present,
            Absent,
            HalfDay,
            Sunday,
            Holiday,
            StoreClosed,
            SundayHoliday,
            SickLeave,
            PaidLeave,
            CasualLeave,
            OnLeave,
            Leave,
            WorkFromHome
        }

        public enum TaxType
        {
            GST,
            SGST,
            CGST,
            IGST,
            VAT,
            CST,
        }

        public enum NotesType
        {
            DebitNote,
            CreditNote,
        }

        public enum InvoiceType
        {
            Sales,
            SalesReturn,
            ManualSale,
            ManualSaleReturn,
        }

        public enum PurchaseInvoiceType
        {
            Purchase,
            PurchaseReturn,
        }

        public enum EntryStatus
        {
            Added,
            Approved,
            Rejected,
            Updated,
            Deleted,
            DeleteApproved,
        }

        public enum LedgerEntryType
        {
            Expenses,
            Payment,
            Receipt,
            Salary,
            AdvancePayment,
            AdvanceReceipt,
            ArvindLimited,
            Others,
        }

        public enum NoteType
        {
            DebitNote,
            CreditNote,
        }

        public enum PayMode
        {
            Cash,
            Card,
            RTGS,
            NEFT,
            IMPS,
            Wallets,
            Cheque,
            DemandDraft,
            Others,
            Coupons,
            MixPayments,
            UPI,
            SaleReturn,
        }

        public enum Size2
        {
            S,
            M,
            L,
            XL,
            XXL,
            XXXL,
            T28,
            T3,
            T32,
            T34,
            T36,
            T38,
            T4,
            T41,
            T42,
            T44,
            T46,
            T48,
            FreeSize,
            NS,
            NOTVALID,
            B36,
            B38,
            B4,
            B42,
            B44,
            B46,
            B96,
        }

        public enum Size
        {
            S,
            M,
            L,
            XL,
            XXL,
            XXXL,
            C28,
            C3,
            C32,
            C34,
            C36,
            C38,
            C4,
            C41,
            C42,
            C44,
            C46,
            C48,
            C96,
            FreeSize,
            NS,
            NOTVALID,
            C39,
            C92,
        }

        public enum ProductCategory
        {
            Fabric,
            Apparel,
            Accessories,
            Tailoring,
            Trims,
            PromoItems,
            Coupons,
            GiftVouchers,
            Others,
            SuitCovers,
            InnerWear,
        }

        public enum CARD
        {
            DebitCard,
            CreditCard,
            AmexCard,
            GiftCard,
            Other
        }

        public enum CARDType
        {
            Visa,
            MasterCard,
            Maestro,
            AmexCard,
            Dinners,
            Rupay,
            RupayCredit,
            Others,
        }

        public enum VendorType
        {
            EBO,
            MBO,
            Tailoring,
            NonSalable,
            OtherSaleable,
            Others,
            TempVendor,
            InHouse,
            Distributor,
            Brands,
            BrandAuth,
        }

        public enum DebitCredit
        {
            In,
            Out,
        }

        public enum RolePermission
        {
            Owner,
            GeneralManager,
            GroupManager,
            Accountant,
            CA,
            StoreManager,
            Salesmen,
            Guest,
            Other,
        }
    }
}