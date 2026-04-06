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