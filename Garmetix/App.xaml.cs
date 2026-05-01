using Garmetix.Databases.Services;
using Microsoft.Extensions.DependencyInjection;
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
            // CreateMainWindow returns a Task<Window>; get the result synchronously because the override is not async.
            var mainWindow = GarmetixSRP.CreateMainWindow(activationState, new GarmetixShell()).GetAwaiter().GetResult();

            // Application does not have a MainWindow property (hence CS1061).
            // Use the Application APIs to register/open/activate the window instead.
          //  if (Application.Current != null)
          //  {
               // Application.Current.OpenWindow(mainWindow);
               // Application.Current.ActivateWindow(mainWindow);
         //   }
            return mainWindow;

            //TODO: Consider refactoring CreateMainWindow to be synchronous if it does not perform any truly asynchronous work, or if the asynchronous work can be handled within the method without needing to return a Task<Window>.
            //return  GarmetixHelpers.CreateMainWindow(activationState, new AppShell()).ContinueWith(task =>
            //{
            //    if (task.IsCompletedSuccessfully)
            //    {
            //        var mainWindow = task.Result;
            //        Application.Current.Dispatcher.Dispatch(() =>
            //        {
            //            Application.Current.ActivateWindow(mainWindow);

            //        });
            //    }
            //    else
            //    {
            //        // Handle exceptions if needed
            //        Console.WriteLine($"Error creating main window: {task.Exception}");
            //    }
            //});
            //return new Window(new AppShell());
        }
    }
}