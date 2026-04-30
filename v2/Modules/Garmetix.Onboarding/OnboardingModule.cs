using Garmetix.Onboarding.Pages;
using Garmetix.Onboarding.Services;
using Garmetix.Onboarding.ViewModels;

namespace Garmetix.Onboarding
{
    // All the code in this file is included in all platforms.
    public static class OnboardingModule
    {

        public static void RegisterRouteOnboarding() {
            ////Onboarding Pages
            Routing.RegisterRoute(nameof(Step1BasicInformationPage), typeof(Step1BasicInformationPage));
            Routing.RegisterRoute(nameof(Step2CompanyDetailsPage), typeof(Step2CompanyDetailsPage));
            Routing.RegisterRoute(nameof(Step3AddressPage), typeof(Step3AddressPage));

            Routing.RegisterRoute(nameof(Step4CompanyConfigPage), typeof(Step4CompanyConfigPage));
            Routing.RegisterRoute(nameof(Step5KeyPersonalDetailsPage), typeof(Step5KeyPersonalDetailsPage));

            Routing.RegisterRoute(nameof(ReviewPage), typeof(ReviewPage));
            Routing.RegisterRoute(nameof(CompletionPage), typeof(CompletionPage));

        }
        public static MauiAppBuilder UseOnboarding(this MauiAppBuilder builder) {

            // Register Services
            builder.Services.AddTransient<OnboardingStateService>();
           // builder.Services.AddTransient<IOnboardingSubmissionService, MockOnboardingSubmissionService>();

            // Register ViewModels
            builder.Services.AddTransient<Step1BasicInfoViewModel>();
            builder.Services.AddTransient<Step2CompanyDetailsViewModel>();
            builder.Services.AddTransient<Step3AddressViewModel>();
            builder.Services.AddTransient<Step4CompanyConfigInfoViewModel>();
            builder.Services.AddTransient<Step5KeyPersonalInfoViewModel>();
            builder.Services.AddTransient<ReviewViewModel>();
            builder.Services.AddTransient<CompletionViewModel>();

            //// Register Views (Pages)
            builder.Services.AddTransient<Step1BasicInformationPage>();
            builder.Services.AddTransient<Step2CompanyDetailsPage>();
            builder.Services.AddTransient<Step3AddressPage>();
            builder.Services.AddTransient<Step4CompanyConfigPage>();
            builder.Services.AddTransient<Step5KeyPersonalDetailsPage>();
            builder.Services.AddTransient<ReviewPage>();
            builder.Services.AddTransient<CompletionPage>();

            

            return builder;
        }
    }
}
