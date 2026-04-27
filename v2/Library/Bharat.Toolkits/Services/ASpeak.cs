namespace Bharat.ToolKits.Services
{
    public class ASpeak
    {
        private bool isBusy = false;
        public static async void Speak(string text) => await TextToSpeech.Default.SpeakAsync(text);

        private CancellationTokenSource? cts;

        public async Task SpeakNowDefaultSettingsAsync()
        {
            cts = new CancellationTokenSource();
#pragma warning disable CA1416 // Validate platform compatibility
            await TextToSpeech.Default.SpeakAsync("Hello World", cancelToken: cts.Token);
#pragma warning restore CA1416 // Validate platform compatibility

            // This method will block until utterance finishes.
        }

        // Cancel speech if a cancellation token exists & hasn't been already requested.
        public void CancelSpeech()
        {
            if (cts?.IsCancellationRequested ?? true)
            {
                return;
            }

            cts.Cancel();
        }

       

        public void SpeakMultiple(List<string> texts)
        {
            isBusy = true;
            Task[] tList = new Task[texts.Count];

            int i = 0;
            foreach (var text in texts)
            {
                tList[i++] = TextToSpeech.Default.SpeakAsync(text);
            }

            Task.WhenAll(tList).ContinueWith((t) => { isBusy = false; },
                TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}