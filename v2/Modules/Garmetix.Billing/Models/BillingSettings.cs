namespace Garmetix.Billing.Models
{
    /// <summary>
    /// Billing Setting Class is used to set the biller information.
    /// so it can be used to supply Biller details to Invoice pdf or print
    /// </summary>
    internal class BillingSettings
    {
        public Guid Guid { get; set; } = Guid.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string? CompanyPhone { get; set; } = string.Empty;
        public string? CompanyEmail { get; set; } = string.Empty;
        public string? CompanyGSTIN { get; set; } = string.Empty;
        public string? CompanyState { get; set; } = string.Empty;
    }
}
