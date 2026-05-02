using Garmetix;
using Garmetix.Databases.Services;
using Garmetix.SRP;
using Microsoft.Extensions.DependencyInjection;

namespace Dukan
{
    public partial class App : Application
    {
        public App(IDatabaseService ds)
        {
            _ = GarmetixSRP.InitApp();
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainWindow = GarmetixSRP.CreateMainWindow(activationState, new GarmetixShell()).GetAwaiter().GetResult();

            return mainWindow;
            //return new Window(new AppShell());
        }
    }
}