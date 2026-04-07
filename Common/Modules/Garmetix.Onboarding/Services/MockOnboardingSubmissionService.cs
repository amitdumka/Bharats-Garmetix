using Garmetix.Onboarding.Models;
using System.Diagnostics;
namespace Garmetix.Onboarding.Services
{
    [Obsolete("This is a mock service and should not be used in production.")]
    public class MockOnboardingSubmissionService : IOnboardingSubmissionService
    {
        public async Task<bool> SubmitOnboardingDataAsync(OnboardingData data)
        {
            // Simulate an API call
            Debug.WriteLine("--- Submitting Onboarding Data ---");
            Debug.WriteLine($"Client: {data.ClientDetails.FirstName} {data.ClientDetails.LastName}, Email: {data.ClientDetails.Email}");
            Debug.WriteLine($"DOB: {data.ClientDetails.DateOfBirth}, Phone: {data.ClientDetails.PhoneNumber}, Gender: {data.ClientDetails.Gender}");
            Debug.WriteLine($"Address: {data.AddressDetails.StreetAddress}, {data.AddressDetails.City}, {data.AddressDetails.StateOrProvince}");
            Debug.WriteLine($"Postal: {data.AddressDetails.PostalCode}, Country: {data.AddressDetails.Country}");

            await Task.Delay(1500); // Simulate network latency

            // In a real app, you would make an HTTP request to your backend.
            // For this mock, we'll assume success.
            Debug.WriteLine("--- Submission Successful (Mock) ---");
            return true;
        }
    }
}
