using Garmetix.Base.PageModels;
using Microsoft.Maui.Controls;

namespace Garmetix.Base.Views
{
    public partial class AboutUsPage : ContentPage
    {
        public AboutUsPage()
        {
            InitializeComponent();
            BindingContext = new AboutUsViewModel();
        }
    }
}