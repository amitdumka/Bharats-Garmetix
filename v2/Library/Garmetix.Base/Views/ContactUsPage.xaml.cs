using Garmetix.Base.PageModels;

namespace Garmetix.Base.Views
{
    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
            BindingContext = new ContactUsViewModel();
        }
    }
}