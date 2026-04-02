using Garmetix.Models;
using Garmetix.PageModels;

namespace Garmetix.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}