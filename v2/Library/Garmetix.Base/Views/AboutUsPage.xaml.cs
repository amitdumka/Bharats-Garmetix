using Microsoft.Maui.Controls;

namespace Garmetix.Core.Views
{
    public partial class AboutUsPage : ContentPage
    {
        public AboutUsPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.AboutUsViewModel();
        }
    }
}