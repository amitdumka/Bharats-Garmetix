using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Garmetix.Core.ViewModels
{
    public partial class ContactUsViewModel : ObservableObject
    {
        [ObservableProperty] private string storeName = "Aadwika Fashion";
        [ObservableProperty] private string address = "Bhagalpur Road, Near TATA Showroom\nDumka, Jharkhand 814101";
        [ObservableProperty] private string email = "aadwikafashion@gmail.com";
        [ObservableProperty] private string phone = "+919334799099";

        [RelayCommand]
        public void CallPhone()
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(Phone);
        }

        [RelayCommand]
        public async Task SendEmailAsync()
        {
            // 1. Explicitly call the MAUI Communication library so it doesn't clash with your string property
            if (Microsoft.Maui.ApplicationModel.Communication.Email.Default.IsComposeSupported)
            {
                var message = new EmailMessage
                {
                    Subject = "Inquiry regarding Aadwika Fashion",
                    // 2. Use 'this.Email' to clarify we want the string variable containing the address
                    To = new List<string> { this.Email }
                };

                // 3. Explicitly call the MAUI Communication library again to send it
                await Microsoft.Maui.ApplicationModel.Communication.Email.Default.ComposeAsync(message);
            }
        }

        [RelayCommand]
        public async Task OpenMapAsync()
        {
            // Opens Google Maps directly to your Dumka store location
            var placemark = new Placemark
            {
                Thoroughfare = "Bhagalpur Road, Near TATA Showroom",
                Locality = "Dumka",
                AdminArea = "Jharkhand",
                PostalCode = "814101",
                CountryName = "India"
            };
            var options = new MapLaunchOptions { Name = StoreName };
            await Map.Default.OpenAsync(placemark, options);
        }
    }
}