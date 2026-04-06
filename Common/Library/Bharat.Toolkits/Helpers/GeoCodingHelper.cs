
namespace Bharat.ToolKits.Helpers
{
    public static class GeoCodingHelper
    {
        public static double CalculateDistance(Location from, Location to)
        {
            double kms = Location.CalculateDistance(from, to, DistanceUnits.Kilometers);
            return kms;
        }

        public static double DistanceTest()
        {
            Location boston = new(42.358056, -71.063611);
            Location sanFrancisco = new(37.783333, -122.416667);

            double miles = Location.CalculateDistance(boston, sanFrancisco, DistanceUnits.Miles);
            return miles;
        }

        public static string GetCountryCodeFromCoordinates(double latitude, double longitude)
        {
            // This is a placeholder for the actual implementation.
            // In a real application, you would use a geocoding service to get the country code.
            // For example, you could use an API like Google Maps Geocoding API or OpenStreetMap Nominatim.
            // Here we return a dummy country code for demonstration purposes.
            return "US"; // United States
        }

        public static async Task<Location?> GetLocation(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                address = "Microsoft Building 25 Redmond WA USA";
            }

            IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

            Location? location = locations?.FirstOrDefault();

            if (location != null)
            {
                Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
            }

            return location;
        }

        public static async Task<string> GetCachedLocation()
        {
            try
            {
                Location? location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location != null)
                {
                    return $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";
                }
            }
            catch (FeatureNotSupportedException)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException)
            {
                // Handle permission exception
            }
            catch (Exception)
            {
                // Unable to get location
            }

            return "None";
        }

        public static async Task<string> GetGeocodeReverseData(double latitude = 47.673988, double longitude = -122.121513)
        {
            IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

            Placemark placemark = placemarks?.FirstOrDefault()!;

            if (placemark != null)
            {
                return
                    $"AdminArea:       {placemark.AdminArea}\n" +
                    $"CountryCode:     {placemark.CountryCode}\n" +
                    $"CountryName:     {placemark.CountryName}\n" +
                    $"FeatureName:     {placemark.FeatureName}\n" +
                    $"Locality:        {placemark.Locality}\n" +
                    $"PostalCode:      {placemark.PostalCode}\n" +
                    $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    $"SubLocality:     {placemark.SubLocality}\n" +
                    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    $"Thoroughfare:    {placemark.Thoroughfare}\n";
            }

            return "";
        }
    }
}
