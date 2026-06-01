using Garmetix.RemoteReceiver.ViewModels;

namespace Garmetix.RemoteReceiver.Views
{
    public partial class RemoteReceiverPage : ContentPage
    {
        public RemoteReceiverPage(RemoteReceiverViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}