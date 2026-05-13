//using Microsoft.Maui.Controls;

//namespace Garmetix.AI.Billing.Views
//{
//    public partial class PaymentRegistryPage : ContentPage
//    {
//        private readonly ViewModels.PaymentHistoryViewModel _viewModel;

//        public PaymentRegistryPage(ViewModels.PaymentHistoryViewModel viewModel)
//        {
//            InitializeComponent();
//            _viewModel = viewModel;
//            BindingContext = _viewModel;
//        }

//        protected override async void OnAppearing()
//        {
//            base.OnAppearing();
//            await _viewModel.LoadDataAsync();
//        }
//    }
//}