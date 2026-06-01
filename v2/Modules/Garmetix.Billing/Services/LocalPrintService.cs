using Microsoft.AspNetCore.SignalR.Client;

namespace Garmetix.Billing.Services

{
    public class LocalPrintService
    {
        private HubConnection _hubConnection;
        private readonly string _hubUrl = "https://your-cloud-server.com/printhub"; // Replace with your server URL

        public LocalPrintService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task InitializeConnectionAsync()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"SignalR Connection failed: {ex.Message}");
                }
            }
        }

        public async Task ProcessDualPrintAsync(string targetBranchId, string invoiceNo, byte[] pdfBytes)
        {
            // 1. Force Print locally on the main counter's printer first
            await DirectHardwarePrintAsync(pdfBytes);

            // 2. Safely transmit the byte payload to the cloud routing hub
            try
            {
                await InitializeConnectionAsync();

                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("SendPrintJobToRemote", targetBranchId, invoiceNo, pdfBytes);
                }
                else
                {
                    throw new Exception("Cloud print network is currently unreachable.");
                }
            }
            catch (Exception ex)
            {
                // Log failure without crashing checkout workflow
                System.Diagnostics.Debug.WriteLine($"Remote print routing failed: {ex.Message}");
                throw;
            }
        }

        private Task DirectHardwarePrintAsync(byte[] bytes)
        {
            // Your existing local thermal / A4 hardware print implementation
            return Task.CompletedTask;
        }
    }
}



// CLoud Hub COde 


/*
 
 using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Garmetix.PrintHub.Hubs
{
    public class PrintRoutingHub : Hub
    {
        // Thread-safe dictionary tracking which Branch ID maps to which active connection
        private static readonly ConcurrentDictionary<string, string> BranchConnections = new();

        /// <summary>
        /// Called by the Remote Receiver app to register itself as the handler for a specific branch.
        /// </summary>
        public async Task RegisterBranch(string branchId)
        {
            BranchConnections[branchId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, branchId);
            System.Diagnostics.Debug.WriteLine($"Branch registered: {branchId} -> {Context.ConnectionId}");
        }

        /// <summary>
        /// Called by the Main Store POS app to send a print job to a remote branch.
        /// </summary>
        public async Task SendPrintJobToRemote(string targetBranchId, string invoiceNo, byte[] printData)
        {
            if (BranchConnections.TryGetValue(targetBranchId, out var connectionId))
            {
                // Send the raw print byte payload to the specific remote client
                await Clients.Client(connectionId).SendAsync("ReceivePrintJob", invoiceNo, printData);
            }
            else
            {
                throw new HubException($"Remote branch '{targetBranchId}' is offline or not registered.");
            }
        }

        public override Task OnDisconnectedAsync(System.Exception? exception)
        {
            // Clean up disconnected nodes safely
            var item = BranchConnections.FirstOrDefault(kvp => kvp.Value == Context.ConnectionId);
            if (!string.IsNullOrEmpty(item.Key))
            {
                BranchConnections.TryRemove(item.Key, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
 
 */