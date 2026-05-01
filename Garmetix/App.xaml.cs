using Garmetix.Databases.Services;
using Garmetix.SRP;

namespace Garmetix
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

        }
    }
}