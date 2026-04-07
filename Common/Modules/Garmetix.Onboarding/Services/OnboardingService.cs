using Garmetix.Onboarding.Models;
namespace Garmetix.Onboarding.Services
{
    public class OnboardingService : IOnboardingSubmissionService
    {
        private readonly IOnboardingSubmissionService _submissionService;
        public OnboardingService(IOnboardingSubmissionService submissionService)
        {
            _submissionService = submissionService;
        }
        public async Task<bool> SubmitOnboardingDataAsync(OnboardingData data)
        {
            return await _submissionService.SubmitOnboardingDataAsync(data);
        }
    }
}
