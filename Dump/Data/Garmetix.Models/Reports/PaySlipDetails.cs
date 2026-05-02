using CommunityToolkit.Mvvm.ComponentModel;

namespace Garmetix.Models.Reports
{
    public partial class PaySlipDetails : BaseDetail
    {
        [ObservableProperty]
        private string _staffName = "Aadwika Staff";

        [ObservableProperty]
        private string _period = string.Empty;

        [ObservableProperty]
        private decimal _basicSalary = 0.0m;

        [ObservableProperty]
        private decimal _hra = 0.0m;

        [ObservableProperty]
        private decimal _incentives = 0.0m;

        [ObservableProperty]
        private decimal _totalEarnings = 0.0m;

        [ObservableProperty]
        private decimal _totalDeductions = 0.0m;

        [ObservableProperty]
        private decimal _netSalary = 0.0m;
    }
}
