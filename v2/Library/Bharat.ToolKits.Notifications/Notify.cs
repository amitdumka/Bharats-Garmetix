using Bharat.ToolKits.Helpers;
using Bharat.ToolKits.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Font = Microsoft.Maui.Font;

namespace Bharat.ToolKits.Notifications
{
    public class Notify
    {
        public static readonly bool SpeakerIsOn = StorageOps.GetPref("SpeakerIsOn", false);

        /// <summary>
        /// The Logger manager
        /// </summary>
        private static readonly ILogger<Notify>? Logger =
           Application.Current?.Windows[0].Page?.Handler?.MauiContext?.Services.GetService<ILogger<Notify>>();

        public static void LogError(Exception ex)
        {
            if (Logger != null)
            {
                Logger.LogError(ex, "Error");
            }
        }

        public static void LogError(string message)
        {
            if (Logger != null)
            {
                Logger.LogError(message);
            }
        }

        public static void LogInfoAsync(string message)
        {
            if (Logger != null)
            {
                Logger.LogInformation(message);
            }
        }

        public static void LogWarningAsync(string message)
        {
            if (Logger != null)
            {
                Logger.LogWarning(message);
            }
        }

        public static Task ShowError(string message, bool speak = false)
        {
            if (SpeakerIsOn)
            {
                speak = true;
            }

            Debug.WriteLine("Error: " + message);
            ShowToast(message, true, speak);
            return Task.CompletedTask;
        }

        public static Task ShowError(Exception ex)
        {
            Debug.WriteLine("Error: " + ex.Message, ex.StackTrace);
            ShowToast("An error occurred: " + ex.Message, true, false);
            return Task.CompletedTask;
        }

        public static Task ShowSuccess(string message, bool speak = false)
        {
            if (SpeakerIsOn)
            {
                speak = true;
            }

            Debug.WriteLine("Success: " + message);
            ShowToast(message, true, speak);
            return Task.CompletedTask;
        }

        public static Task ShowWarning(string message, bool speak = false)
        {
            if (SpeakerIsOn)
            {
                speak = true;
            }

            Debug.WriteLine("Success: " + message);
            ShowToast(message, false, speak);
            return Task.CompletedTask;
        }

        public static Task ShowSuccess(string message, bool snabackbar = false, bool isLong = false, bool speak = false)
        {
            Debug.WriteLine("Success: " + message);

            if (snabackbar)
            {
                _ = DisplaySnackbarAsync(message);
            }

            var toast = Toast.Make(message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (SpeakerIsOn)
            {
                speak = true;
            }

            if (speak)
            {
                Task.Run(() => ASpeak.Speak(message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));

            LogInfoAsync($"Success: " + message);
            return Task.CompletedTask;
        }

        public static Task ShowSuccess(string title, string message, bool snabackbar = false, bool isLong = false, bool speak = false)
        {
            Debug.WriteLine($"Success: {title}: " + message);

            if (snabackbar)
            {
                _ = DisplaySnackbarAsync(message);
            }

            var toast = Toast.Make(message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (SpeakerIsOn)
            {
                speak = true;
            }

            if (speak)
            {
                Task.Run(() => ASpeak.Speak(message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));

            LogInfoAsync($"Success: {title}: " + message);

            return Task.CompletedTask;
        }

        public static Task ShowError(string message, bool snabackbar = false, bool isLong = false, bool speak = false)
        {
            Debug.WriteLine($"Error:   " + message);
            if (snabackbar)
            {
                _ = DisplaySnackbarAsync(message);
            }

            var toast = Toast.Make(message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (SpeakerIsOn)
            {
                speak = true;
            }

            if (speak)
            {
                Task.Run(() => ASpeak.Speak(message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));

            LogError(message);
            return Task.CompletedTask;
        }

        public static Task ShowError(string title, string message, bool snabackbar = false, bool isLong = false, bool speak = false)
        {
            Debug.WriteLine($"Error: {title}: " + message);
            if (snabackbar)
            {
                _ = DisplaySnackbarAsync("Error: " + message);
            }

            var toast = Toast.Make("Error: " + message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (SpeakerIsOn)
            {
                speak = true;
            }

            if (speak)
            {
                Task.Run(() => ASpeak.Speak("Error: " + message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));

            LogError(message);
            return Task.CompletedTask;
        }

        public static Task ShowError(Exception ex, bool snabackbar = false, bool isLong = false, bool speak = false)
        {
            Debug.WriteLine($"Error:   " + ex.Message);
            if (snabackbar)
            {
                _ = DisplaySnackbarAsync(ex.Message);
            }

            var toast = Toast.Make(ex.Message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (SpeakerIsOn)
            {
                speak = true;
            }

            if (speak)
            {
                Task.Run(() => ASpeak.Speak(ex.Message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));

            LogError(ex);
            return Task.CompletedTask;
        }

        public static Task ShowError(string title, Exception ex, bool snabackbar = false, bool isLong = false, bool speak = false)
        {
            Debug.WriteLine($"Error: {title}: " + ex.Message);
            if (snabackbar)
            {
                _ = DisplaySnackbarAsync("Error: " + ex.Message);
            }

            var toast = Toast.Make("Error: " + ex.Message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (SpeakerIsOn)
            {
                speak = true;
            }

            if (speak)
            {
                Task.Run(() => ASpeak.Speak("Error: " + ex.Message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));

            LogError(ex);
            return Task.CompletedTask;
        }

        public static Task ShowToast(string message, bool isLong = false, bool speak = false)
        {
            if (SpeakerIsOn)
            {
                speak = true;
            }

            var toast = Toast.Make(message, textSize: 18, duration: isLong ? ToastDuration.Long : ToastDuration.Short);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            if (speak)
            {
                Task.Run(() => ASpeak.Speak(message));
            }

            Task toastTask = Task.Run(async () => await toast.Show(cts.Token));
            return Task.CompletedTask;
        }

        public static void ShowToastError(string msg) => Toast.Make(msg).Show();

        public static void ShowToastError(Exception ex) => Toast.Make(ex.Message).Show();

        public static void ShowSuccess(string msg) => Toast.Make(msg).Show();

        /// <summary>
        /// Displays a notification message with optional speak, snackbar, and long duration
        /// </summary>
        /// <param name="message"></param>
        /// <param name="speak"></param>
        /// <param name="snackbar"></param>
        /// <param name="isLong"></param>
        /// <returns></returns>
        public static async Task DisplayNotificationAsync(string message, bool speak = false, bool snackbar = false, bool isLong = false)
        {
            if (speak)
            {
                ASpeak.Speak(message);
            }
            if (snackbar)
            {
                await DisplaySnackbarAsync(message);
            }
            else
            {
                if (isLong)
                {
                    ToastMsg(message, true);
                }
                await DisplayToastAsync(message);
            }
        }

        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#FF3300"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(0),
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };

            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);

            await snackbar.Show(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Toast Message Async Function
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task DisplayToastAsync(string message)
        {
            // Toast is currently not working in MCT on Windows
            // if (OperatingSystem.IsWindows())
            // {
            //     return;
            // }

            var toast = Toast.Make(message, textSize: 18);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }

        /// <summary>
        /// Toast Message , Default is short duration
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isLong"></param>
        public static void ToastMsg(string msg, bool isLong = false)
        {
            Toast.Make(msg, isLong ? CommunityToolkit.Maui.Core.ToastDuration.Long : CommunityToolkit.Maui.Core.ToastDuration.Short, 18).Show();
        }
    }
}