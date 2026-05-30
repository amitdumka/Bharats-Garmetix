using CommunityToolkit.Maui.Views;
using Garmetix.Core.Models.Inventory;

namespace Garmetix.Billing.Pages.Popups;

public partial class CardPaymentPopup : Popup<CardPayment>
{
    public CardPaymentPopup()
    {
        InitializeComponent();
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        // Basic Validation
        if (CardTypePicker.SelectedIndex == -1 || string.IsNullOrWhiteSpace(LastFourEntry.Text))
        {
            // Note: Popups cannot call DisplayAlert directly. We just highlight the entry or return.
            return;
        }

        var details = new CardPaymentDetails
        {
            CardType = CardTypePicker.SelectedItem.ToString(),
            LastFourDigits = LastFourEntry.Text,
            AuthCode = AuthCodeEntry.Text
        };

        await CloseAsync(details);
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }
}