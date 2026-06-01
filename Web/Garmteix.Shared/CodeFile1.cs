using Garmetix.PrintHub.Hubs;
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



// Step 2 
Step 3: Wire up SignalR in Program.cs
Now you must tell your web server to enable SignalR and map the URL so your POS apps know where to connect.

Open the Program.cs file inside Garmetix.PrintHub and replace the contents with this:


    Code:

using Garmetix.PrintHub.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. Add SignalR Services to the container
builder.Services.AddSignalR();

// Optional: Add CORS if you want to allow a web-dashboard to connect later
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(origin => true) // Allow any client (MAUI)
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();

// 2. Map the Hub to a specific URL endpoint
app.MapHub<PrintRoutingHub>("/printhub");

app.MapGet("/", () => "Garmetix Cloud Print Hub is running!");

app.Run();


Step 4: How to Test it Locally
To test the whole system on your local machine without buying a cloud server yet:

Right - click the Garmetix.PrintHub project and choose Set as Startup Project.

Run the project. A browser window will open showing "Garmetix Cloud Print Hub is running!".

Look at the URL in your browser (e.g., https://localhost:7123).

In your MAUI Sender App and your MAUI Receiver App, change the _hubUrl variable to match that local URL:
private readonly string _hubUrl = "https://localhost:7123/printhub";

Pro - Tip: To test both at once, right-click your Solution -> Configure Startup Projects... -> select Multiple startup projects and set both the PrintHub and your MAUI apps to "Start".

Step 5: Moving to Production
When you are ready to go live across Dumka and your remote branches, you will publish this Garmetix.PrintHub project to a real web server.

Once it is hosted on the internet (e.g., [https://api.aadwikafashion.com/printhub](https://api.aadwikafashion.com/printhub)), you update the _hubUrl in your MAUI apps one final time, compile the APK/EXEs, and your multi-branch printing will work globally!