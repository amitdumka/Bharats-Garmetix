using CommunityToolkit.Mvvm.ComponentModel;
using Garmetix.Databases;
using Garmetix.Databases.Services;

namespace Garmetix.Reports.PageModels
{
    [ObservableRecipient]
    public partial class BasePageModel : ObservableObject
    {
        [ObservableProperty]
        protected bool _isBusy = false; // Indicates if data is currently being loaded

        public static DatabaseContext Db => DatabaseService.Instance.LocalDB;

        [ObservableProperty]
        protected List<int> _years = [];

        [ObservableProperty]
        protected List<int> _months = [];

        [ObservableProperty]
        private int _year;

        [ObservableProperty]
        private int _month;

        [ObservableProperty]
        private int _yearEnd;

        [ObservableProperty]
        private int _monthEnd;
         

        protected void LoadPeriod()
        {
            if (Years == null || Years.Count <= 0)
                Years = new List<int>();
            for (int i = 2016; i <= DateTime.Now.Year + 1; i++)
                {
                    Years.Add(i);
                }
            if (Months == null || Months.Count <= 0)
                Months = new List<int>();
                for (int i = 1; i <= 12; i++)
                {
                    Months.Add(i);
                }
        }
    }
}