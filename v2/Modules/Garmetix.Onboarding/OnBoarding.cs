// ----------------------------------------------------------------------
// Project Structure Overview
// ----------------------------------------------------------------------
// YourProjectName/
// ├── App.xaml
// ├── App.xaml.cs
// ├── AppShell.xaml
// ├── AppShell.xaml.cs
// ├── MauiProgram.cs
// ├── Platforms/
// ├── Resources/
// │   └── Styles/
// │       └── DefaultStyles.xaml (you can add custom styles here)
// ├── Models/
// │   ├── ClientInfo.cs
// │   ├── AddressInfo.cs
// │   └── OnboardingData.cs
// ├── ViewModels/
// │   ├── BaseViewModel.cs
// │   ├── Step1BasicInfoViewModel.cs
// │   ├── Step2PersonalDetailsViewModel.cs
// │   ├── Step3AddressViewModel.cs
// │   ├── ReviewViewModel.cs
// │   └── CompletionViewModel.cs
// ├── Views/
// │   ├── Step1BasicInfoPage.xaml
// │   ├── Step1BasicInfoPage.xaml.cs
// │   ├── Step2PersonalDetailsPage.xaml
// │   ├── Step2PersonalDetailsPage.xaml.cs
// │   ├── Step3AddressPage.xaml
// │   ├── Step3AddressPage.xaml.cs
// │   ├── ReviewPage.xaml
// │   ├── ReviewPage.xaml.cs
// │   └── CompletionPage.xaml
// │   └── CompletionPage.xaml.cs
// └── Services/
//     ├── OnboardingStateService.cs
//     ├── IOnboardingSubmissionService.cs
//     └── MockOnboardingSubmissionService.cs
//
// Note: You'll need to install the CommunityToolkit.Mvvm NuGet package.
// `dotnet add package CommunityToolkit.Mvvm`
// ----------------------------------------------------------------------

