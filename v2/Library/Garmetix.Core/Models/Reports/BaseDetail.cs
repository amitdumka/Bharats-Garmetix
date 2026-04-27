using CommunityToolkit.Mvvm.ComponentModel;

namespace Garmetix.Models.Reports
{
    public partial class BaseDetail : ObservableObject
    {
        [ObservableProperty]
        private string _companyName = "Aadwika Fashion";

        [ObservableProperty]
        private string _companyAddress = "Bhagalpur Road, Dumka , Jharkhand";

        [ObservableProperty]
        private string _companyPhone = "0434-224461";

        [ObservableProperty]
        private string _gstin = "20AJHPA7396P1ZV";

        [ObservableProperty]
        private string _companyEmail = "aadwikafashion@gmail.com";
    }
}
