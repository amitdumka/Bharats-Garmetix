namespace Garmetix.Models.Onboarding
{
    public class OnboardingData
    {
        public ClientInfo ClientDetails { get; set; } //Step 1: Client Details
        public AddressInfo AddressDetails { get; set; } //Step 3: Address Details
        public CompanyInfo CompanyDetails { get; set; } //Step 2: Company Details
        public CompanyConfigInfo CompanyConfig { get; set; } //Step 4: Company Config
        public KeyPersonalInfo KeyPersonalDetails { get; set; } //Step 5: Key Personal Details

        public string? CompanyLogo { get; set; }
        public string? CompanyBanner { get; set; }
        public string? CompanyFavicon { get; set; }

        public string? CompanyWebsite { get; set; }

        public string? CompanyDescription { get; set; }
        public string? CompanyTagline { get; set; }

        public bool IsTermsAccepted { get; set; } = false;
        public bool IsPrivacyPolicyAccepted { get; set; } = false;


        public bool IsCompanyDetailsAdded { get; set; } = false;
        public bool IsAddressDetailsAdded { get; set; } = false;
        public bool IsClientDetailsAdded { get; set; } = false;
        public bool IsKeyPersonalDetailsAdded { get; set; } = false;
        public bool IsCompanyConfigAdded { get; set; } = false;
        public bool IsOnboardingCompleted { get; set; } = false;

        public bool IsOnboardingSkipped { get; set; } = false;

        // Method to check if onboarding is required based on the current state of the onboarding data
        public bool CheckForOnboarding()
        {
            if (!IsOnboardingCompleted && !IsOnboardingSkipped)
            {
                return !IsClientDetailsAdded || !IsAddressDetailsAdded || !IsCompanyDetailsAdded || !IsKeyPersonalDetailsAdded || !IsCompanyConfigAdded;
            }
            return IsOnboardingCompleted;
        }


        public OnboardingData()
        {
            ClientDetails = new ClientInfo();
            AddressDetails = new AddressInfo();
            CompanyDetails = new CompanyInfo();
            CompanyConfig = new CompanyConfigInfo();
            KeyPersonalDetails = new KeyPersonalInfo();

        }

        public void Reset()
        {
            ClientDetails = new ClientInfo();
            AddressDetails = new AddressInfo();
        }
    }
}
