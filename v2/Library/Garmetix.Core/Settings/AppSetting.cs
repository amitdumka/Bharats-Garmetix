using System.ComponentModel.DataAnnotations;

namespace Garmetix.Core.Settings
{
    public enum WorkingMode
    {
        Company,   // 0
        Store,     // 1
        Group      // 2
    }

    public enum AppMode
    {
        Standalone,  // 0
        Remote,      // 1
        Dual         // 2
    }

    /// <summary>
    /// App Setting
    /// AFSBGP-202401-0001
    /// </summary>
    public class AppSettings
    {
        [Key]
        public Guid AppId { get; set; } = Guid.NewGuid();
        public AppMode AppMode { get; set; } = AppMode.Standalone;
        public bool EnableRemoteSync { get; set; } = false;
        public string RemoteUrl { get; set; } = "https://localhost:7288/";
        public bool EnableLocalCache { get; set; } = false;
        public bool EnableRemoteCache { get; set; } = false;

        //Company details
        public string CompanyName { get; set; } = "Aadwika Fashion";
        public string GSTIN { get; set; } = "20CLEPK0467L1Z8";
        public string CompanyStoreCode { get; set; } = "AFSMBO";

        //Group details
        public string GroupName { get; set; } = "Aadwika Fashion Group";

        //Store Details
        public string StoreName { get; set; } = "Aadwika Fashion Store";
        public string StoreAddress { get; set; } = "123 Fashion Street";
        public string StoreCity { get; set; } = "Fashion City";
        public string StoreState { get; set; } = "Fashion State";
        public string StorePincode { get; set; } = "123456";
        public string StoreEmail { get; set; } = "0K9Hs@example.com";
        public string StoreContact { get; set; } = "1234567890";


        public WorkingMode WorkingMode { get; set; } = WorkingMode.Company;
        public bool EnableMultiStore { get; set; } = false;
        public bool EnableAutoLogin { get; set; } = false;
        public bool EnablePrinting { get; set; } = true;

    }
}
