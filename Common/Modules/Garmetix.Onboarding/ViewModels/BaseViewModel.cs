using CommunityToolkit.Mvvm.ComponentModel;


namespace Garmetix.Onboarding
{
    [ObservableRecipient]
    public partial class BaseViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        [ObservableProperty]
        string _title;

        public bool IsNotBusy => !IsBusy;
    }
}
