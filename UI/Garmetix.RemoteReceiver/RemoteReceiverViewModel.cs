using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Garmetix.RemoteReceiver.Services;

namespace Garmetix.RemoteReceiver.ViewModels
{
    public partial class RemoteReceiverViewModel : ObservableObject
    {
        private HubConnection _hubConnection;
        private readonly IPlatformPrinter _platformPrinter;

        [ObservableProperty] private string _branchId = "BRANCH_02"; // Unique ID for this remote site
        [ObservableProperty] private string _statusMessage = "Disconnected";
        [ObservableProperty] private bool _isConnected;

        public ObservableCollection<string> PrintLog { get; } = new();

        public RemoteReceiverViewModel(IPlatformPrinter platformPrinter)
        {
            _platformPrinter = platformPrinter;
            SetupSignalR();
        }

        private void SetupSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://your-cloud-server.com/printhub")
                .WithAutomaticReconnect()
                .Build();

            // Register event listener for incoming cloud print payloads
            _hubConnection.On<string, byte[]>("ReceivePrintJob", async (invoiceNo, printData) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    PrintLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Received Job: {invoiceNo}"));

                try
                {
                    // Fire raw print payload straight to local hardware (Bluetooth/USB)
                    await _platformPrinter.PrintRawPayloadAsync(printData);

                    MainThread.BeginInvokeOnMainThread(() =>
                        PrintLog.Insert(0, $"[{DateTime.Now:HH:mm:ss}] Print Success: {invoiceNo}"));
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                        PrintLog.Insert(0, $"ERROR printing {invoiceNo}: {ex.Message}"));
                }
            });

            _hubConnection.Reconnecting += (error) => { StatusMessage = "Reconnecting..."; return Task.CompletedTask; };
            _hubConnection.Reconnected += async (connectionId) => { await RegisterOnHub(); };
        }

        [RelayCommand]
        public async Task ToggleConnectionAsync()
        {
            if (IsConnected)
            {
                await _hubConnection.StopAsync();
                IsConnected = false;
                StatusMessage = "Disconnected Manually";
            }
            else
            {
                StatusMessage = "Connecting...";
                try
                {
                    await _hubConnection.StartAsync();
                    await RegisterOnHub();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Connection Failed: {ex.Message}";
                }
            }
        }

        private async Task RegisterOnHub()
        {
            await _hubConnection.InvokeAsync("RegisterBranch", BranchId);
            IsConnected = true;
            StatusMessage = $"Monitoring Branch: {BranchId} (Online)";
        }
    }
}
