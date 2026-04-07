using Garmetix.Onboarding.Models;
namespace Garmetix.Onboarding.Services
{
    public interface IOnboardingSubmissionService
    {
        Task<bool> SubmitOnboardingDataAsync(OnboardingData data);
    }
}
