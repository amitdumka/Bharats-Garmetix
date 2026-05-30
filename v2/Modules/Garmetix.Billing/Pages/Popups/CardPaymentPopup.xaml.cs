using CommunityToolkit.Maui.Views;
using Garmetix.Core.Enums; 

namespace Garmetix.Billing.Pages.Popups;

public class CardPaymentDto
{
    public int CardNumber { get; set; }
    public int AuthCode { get; set; }
    public Card Card { get; set; }
    public CardType CardType { get; set; }
    public string BankName { get; set; }
    public decimal Amount { get; set; } = 0;
}
public partial class CardPaymentPopup : Popup<CardPaymentDto>
{
    public CardPaymentPopup()
    {
        InitializeComponent();

        // Auto-populate pickers directly from your Enums!
        CardModePicker.ItemsSource = Enum.GetValues(typeof(Card));
        CardTypePicker.ItemsSource = Enum.GetValues(typeof(CardType));

        // Set defaults if desired
        CardModePicker.SelectedItem = Card.DebitCard;
        CardTypePicker.SelectedItem = CardType.Rupay;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        // Safely parse the integers to prevent crashes if the user types letters
        int.TryParse(CardNumberEntry.Text, out int parsedCardNumber);
        int.TryParse(AuthCodeEntry.Text, out int parsedAuthCode);

        var dto = new CardPaymentDto
        {
            Card = (Card)CardModePicker.SelectedItem,
            CardType = (CardType)CardTypePicker.SelectedItem,
            BankName = BankNameEntry.Text,
            CardNumber = parsedCardNumber,
            AuthCode = parsedAuthCode, 
            Amount = decimal.TryParse(AmountEntry.Text, out decimal parsedAmount) ? parsedAmount : 0
        };

        await CloseAsync(dto);
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await CloseAsync(null);
    }
}