using Microsoft.Maui.Controls;

namespace Garmetix.Core.Views
{
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.ContactUsViewModel();
        }
    }
}