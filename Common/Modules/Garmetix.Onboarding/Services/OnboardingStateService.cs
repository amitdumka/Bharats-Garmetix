using Garmetix.Onboarding.Models;
using System.Diagnostics;
namespace Garmetix.Onboarding.Services
{
    public class OnboardingStateService
    {
        private static OnboardingStateService _instance;
        public static OnboardingStateService Instance { get { return _instance ?? (_instance = new OnboardingStateService()); } }
        public OnboardingData CurrentOnboardingData { get; private set; }

        public OnboardingStateService()
        {
            Debug.WriteLine("Initializing OnboardingStateService...");
            CurrentOnboardingData = new OnboardingData();
            _instance = this;
        }

        public void ResetOnboardingData()
        {
            CurrentOnboardingData.Reset();
        }
    }
}
