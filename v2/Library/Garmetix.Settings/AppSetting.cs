using Garmetix.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garmetix.Settings
{
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
        public string StoreAddress { get; set; } = "Bhagalpur Road Dumka";
        public string StoreCity { get; set; } = "Dumka";
        public string StoreState { get; set; } = "Jharkhand";
        public string StorePincode { get; set; } = "814101";
        public string StoreEmail { get; set; } = "aadwikafashion@gmail.com";
        public string StoreContact { get; set; } = "1234567890";


        public WorkingMode WorkingMode { get; set; } = WorkingMode.Company;
        public bool EnableMultiStore { get; set; } = false;
        public bool EnableAutoLogin { get; set; } = false;
        public bool EnablePrinting { get; set; } = true;

    }
}
