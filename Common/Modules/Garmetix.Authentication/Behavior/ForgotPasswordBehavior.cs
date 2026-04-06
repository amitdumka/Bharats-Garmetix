using Garmetix.Authentication.Pages;

using Syncfusion.Maui.DataForm;
using Syncfusion.Maui.Toolkit.Buttons;

namespace Garmetix.Authentication.Behavior
{
    public class ForgotPasswordBehavior : Behavior<ForgotPassword>
    {
        /// <summary>
        /// The data form.
        /// </summary>
        private SfDataForm? dataForm;

        /// <summary>
        /// The send button.
        /// </summary>
        private SfButton? sendButton;

        protected override void OnAttachedTo(BindableObject bindable)
        {
            base.OnAttachedTo(bindable);
            ForgotPassword? forgotPasswordPage = bindable as ForgotPassword;
            if (forgotPasswordPage == null)
            {
                return;
            }

            dataForm = (SfDataForm)forgotPasswordPage.Content.FindByName("dataForm");
            //if (this.dataForm != null)
            //{
            //    dataForm.ItemsSourceProvider = new ItemsSourceProvider();
            //}

            sendButton = (SfButton)forgotPasswordPage.Content.FindByName("sendButton");
            if (sendButton != null)
            {
                sendButton.Clicked += OnSendButtonClicked;
            }
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            base.OnDetachingFrom(bindable);
            ForgotPassword? forgotPasswordPage = bindable as ForgotPassword;

            if (forgotPasswordPage == null)
            {
                return;
            }

            dataForm = (SfDataForm)forgotPasswordPage.Content.FindByName("dataForm");
            //if (this.dataForm != null)
            //{
            //    dataForm.ItemsSourceProvider = new ItemsSourceProvider();
            //}

            sendButton = (SfButton)forgotPasswordPage.Content.FindByName("sendButton");
            if (sendButton != null)
            {
                sendButton.Clicked -= OnSendButtonClicked;
            }
        }

        private void OnSendButtonClicked(object? sender, EventArgs e)
        {
            dataForm?.Validate();
        }
    }
}
