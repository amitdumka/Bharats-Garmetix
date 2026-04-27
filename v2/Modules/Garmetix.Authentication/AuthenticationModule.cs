using Garmetix.Authentication.Pages;

namespace Garmetix.Authentication
{
    // All the code in this file is included in all platforms.
    public static class AuthenticationModule
    {
        public static MauiAppBuilder UseAuthentication(this MauiAppBuilder builder)
        {
             
            // Register the AuthenticationService as a singleton
            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddTransient<Login>();
           // builder.Services.AddTransient<SignUp>();
            builder.Services.AddTransient<ForgotPassword>();
            builder.Services.AddTransient<ResetPassword>();

            return builder;
        }
        public static void RegisterAuthenticationRoute()
        {
            // Register the AuthenticationService as a singleton
            // Add Route log out and log in, register, forgot password, reset password
        }

    }
}