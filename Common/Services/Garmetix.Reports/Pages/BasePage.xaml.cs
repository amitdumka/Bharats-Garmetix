using System.Runtime.CompilerServices;

namespace Garmetix.Reports.Pages
{
    public partial class BasePage : ContentPage
    {
       

        public BasePage()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

         
    }
}