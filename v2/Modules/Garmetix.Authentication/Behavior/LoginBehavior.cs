using Bharat.ToolKits.Notifications;
using Garmetix.Authentication.Models;
using Garmetix.Authentication.Pages;
using Garmetix.Databases.Services;
using Syncfusion.Maui.DataForm;
using Syncfusion.Maui.Toolkit.Buttons;

namespace Garmetix.Authentication.Behavior;

public class LoginBehavior : Behavior<Login>
{
    /// <summary>
    /// Holds the data form object.
    /// </summary>
    private SfDataForm? logInForm;
    private Shell? AppShell;
    private static  AuthenticationService authService=> AuthenticationService.Instance;

    /// <summary>
    /// Holds the save button instance.
    /// </summary>
    private SfButton? saveButton;
    private void MaximizeOrFullScreen()
    {
        //var win = Application.Current.Windows[0];
#if WINDOWS
        // --- Windows: Maximize the Window ---
        var window = Application.Current?.Windows[0];
        if (window?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }
        }
#elif ANDROID
        // --- Android: Go into Immersive Full Screen ---
        var activity = Platform.CurrentActivity;
        if (activity?.Window != null)
        {
            // Tells Android we are handling the system windows ourselves
            AndroidX.Core.View.WindowCompat.SetDecorFitsSystemWindows(activity.Window, false);

            var windowInsetsController = AndroidX.Core.View.WindowCompat.GetInsetsController(activity.Window, activity.Window.DecorView);
            if (windowInsetsController != null)
            {
                // Hide both the status bar (top) and navigation bar (bottom)
                windowInsetsController.Hide(AndroidX.Core.View.WindowInsetsCompat.Type.SystemBars());

                // Allow users to swipe from the edges to temporarily reveal the bars
                windowInsetsController.SystemBarsBehavior = AndroidX.Core.View.WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            }
        }
#endif
    }
    protected override void OnAttachedTo(BindableObject bindable)
    {
        base.OnAttachedTo(bindable);
        Login? loginPage = bindable as Login;

        if (loginPage == null)
        {
            return;
        }
        //Adding link to app shell
        AppShell = loginPage.AppShell;

        logInForm = (SfDataForm)loginPage.Content.FindByName("logInForm");

        saveButton = (SfButton)loginPage.Content.FindByName("saveButton");
        if (saveButton != null)
        {
            saveButton.Clicked += OnSaveButtonClicked;
        }
    }

    protected override void OnDetachingFrom(BindableObject bindable)
    {
        base.OnDetachingFrom(bindable);
        if (bindable is not Login loginPage)
        {
            return;
        }

        logInForm = (SfDataForm?)loginPage.Content.FindByName("logInForm");
        this.AppShell = null;
        saveButton = (SfButton?)loginPage.Content.FindByName("saveButton");
        if (saveButton != null)
        {
            saveButton.Clicked -= OnSaveButtonClicked;
        }
    }

    /// <summary>
    /// Invokes on save button click.
    /// </summary>
    /// <param name="sender">The button.</param>
    /// <param name="e">The event arguments.</param>
    private void OnSaveButtonClicked(object? sender, EventArgs e)
    {
        //TODO: Move to Auth Service
        logInForm?.Validate();
        logInForm?.Commit();

        LoginInfo? info = logInForm?.DataObject as LoginInfo;
        if (info != null && !string.IsNullOrEmpty(info.Email) && !string.IsNullOrEmpty(info.Password))
        {
           
            var user = authService.DoLogin(info);
            if (user != null)
            {
                _ = Task.Run(delegate
                {
                    DatabaseService.Instance.CurrentUser = user;
                    _ = authService.PostLogin(user, info.RememberMe);
                });

                MaximizeOrFullScreen();
                // Fix for CS0618 and CS8602
                var currentWindow = Application.Current?.Windows.FirstOrDefault();
                if (currentWindow != null)
                {
                    currentWindow.Page = this.AppShell;// new AppShell();
                }
            }
            else
            {
                _ = Notify.DisplayNotificationAsync("Invalid username or password", speak: true);
            }
        }
        else
        {
            _ = Notify.DisplayNotificationAsync("Enter username and password", speak: true);
        }
    }

    
    
}