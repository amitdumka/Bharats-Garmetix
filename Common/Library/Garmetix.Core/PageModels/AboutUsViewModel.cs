using CommunityToolkit.Mvvm.ComponentModel;


namespace Garmetix.Core.ViewModels
{
    public partial class AboutUsViewModel : ObservableObject
    {
        [ObservableProperty] private string appName = "Garmetix";
        [ObservableProperty] private string version = "Version 5.0";
        [ObservableProperty] private string developer = "Amit Kumar";
        [ObservableProperty] private string clientName = "Aadwika Fashion";
    }
}
