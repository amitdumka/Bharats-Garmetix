namespace Garmetix.Billing.Helpers
{
    /// <summary>
    /// Store Info need to be handle properly  use if older system is alread created. other this is fine
    /// </summary>
    public static class StoreInfo
    {
        public static string StoreName { get; set; } = "AADWIKA FASHION";
        public static string StoreAddress { get; set; } = "Bhagalpur Road, Near TATA Showroom,\nDumka, Jharkhand";
        public static string ContactInfo { get; set; } = "Contact: +91 9334799099";
        public static string GSTIN { get; set; } = "20AJHPA7396P1ZV"; // Starts with 20 (Jharkhand State Code)
        public static string StateCode { get; set; } = "20";
    }

}
