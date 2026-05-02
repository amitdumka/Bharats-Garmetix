using CommunityToolkit.Mvvm.ComponentModel;

namespace Garmetix.Models.Reports
{
    public partial class SalaryPaymentDetails : BaseDetail
    {
        [ObservableProperty]
        private string _vouherNumber = "0001";

        [ObservableProperty]
        private string _authorizedSignatory = "Manager";

        [ObservableProperty]
        private string _staffName = "Aadwika Staff";

        [ObservableProperty]
        private DateTime _date = DateTime.Today;

        [ObservableProperty]
        private decimal _amount = 0;

        [ObservableProperty]
        private string _period = string.Empty;

        [ObservableProperty]
        private string _amountInWords= string.Empty;

        [ObservableProperty]
        private string _narration = string.Empty;

        [ObservableProperty]
        private string _paymentMode = string.Empty;

        [ObservableProperty]
        private string _onAccount = string.Empty;
    }
}
