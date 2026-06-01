using CommunityToolkit.Maui.Views;
using Garmetix.Core.Models.Inventory;

namespace Garmetix.Billing.Pages.Popups;

public partial class RedeemCreditPopup : Popup<decimal>
{
    private decimal _maxAvailable;

    public RedeemCreditPopup(Invoice creditNote, decimal currentBillTotal)
    {
        InitializeComponent();

        NoteRefLabel.Text = creditNote.InvoiceNumber;
        AvailableBalanceLabel.Text = $"₹ {creditNote.BillAmount:N2}";

        _maxAvailable = creditNote.BillAmount;

        // Auto-fill the entry with either the full bill amount OR the max credit available
        decimal suggestedRedemption = Math.Min(_maxAvailable, currentBillTotal);
        RedeemAmountEntry.Text = suggestedRedemption.ToString("0.##");
    }

    private async void Apply_Clicked(object sender, EventArgs e)
    {
        if (decimal.TryParse(RedeemAmountEntry.Text, out decimal requestedAmount))
        {
            if (requestedAmount <= 0)
            {
                // Optionally highlight error
                return;
            }

            if (requestedAmount > _maxAvailable)
            {
                // Cannot redeem more than available
                RedeemAmountEntry.Text = _maxAvailable.ToString("0.##");
                return;
            }

            await CloseAsync(requestedAmount);
        }
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(0m);
    }
}