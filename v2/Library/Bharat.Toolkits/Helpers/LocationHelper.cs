
using System.Diagnostics;

namespace Bharat.ToolKits.Helpers
{
    public class LocationHelper
    {
        private CancellationTokenSource? _cancelTokenSource;
        private bool _isCheckingLocation;

        public async Task<Location?> GetCurrentLocation()
        {
            try
            {
                _isCheckingLocation = true;

                GeolocationRequest request = new(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                _cancelTokenSource = new CancellationTokenSource();

                Location? location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
                return location;
            }
            // Catch one of the following exceptions:
            //   FeatureNotSupportedException
            //   FeatureNotEnabledException
            //   PermissionException
            catch (Exception)
            {
                // Unable to get location
                return null;
            }
            finally
            {
                _isCheckingLocation = false;
            }
        }

        public void CancelRequest()
        {
            if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
            {
                _cancelTokenSource.Cancel();
            }
        }

        public void OnStopListening()
        {
            try
            {
                Geolocation.LocationChanged -= Geolocation_LocationChanged!;
                Geolocation.StopListeningForeground();
                string status = "Stopped listening for foreground location updates";
                Debug.WriteLine(status);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Unable to stop listening for location changes
            }
        }

        public async void OnStartListening()
        {
            try
            {
                Geolocation.LocationChanged += Geolocation_LocationChanged!;
                var request = new GeolocationListeningRequest(GeolocationAccuracy.Best);
                var success = await Geolocation.StartListeningForegroundAsync(request);

                string status = success
                    ? "Started listening for foreground location updates"
                    : "Couldn't start listening";
            }
            catch (Exception)
            {
                // Unable to start listening for location changes
            }
        }

        public void Geolocation_LocationChanged(object sender, GeolocationLocationChangedEventArgs e)
        {
            // Process e.Location to get the new location
        }
    }
}
