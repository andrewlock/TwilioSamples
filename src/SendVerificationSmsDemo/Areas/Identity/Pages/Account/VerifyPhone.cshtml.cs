using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Twilio.Rest.Verify.V2.Service;

namespace SendVerificationSmsDemo.Areas.Identity.Pages.Account
{
    public class VerifyPhoneModel : PageModel
    {
        private readonly TwilioVerifySettings _settings;
        private readonly UserManager<IdentityUser> _userManager;

        public VerifyPhoneModel(IOptions<TwilioVerifySettings> settings, UserManager<IdentityUser> userManager)
        {
            _settings = settings.Value;
            _userManager = userManager;
        }

        public string PhoneNumber { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            PhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                }

                var phone = await _userManager.GetPhoneNumberAsync(user);
                var verification = await VerificationResource.CreateAsync(
                    to: phone,
                    channel: "sms",
                    pathServiceSid: _settings.VerificationServiceSid
                );

                if (verification.Status == "pending" || verification.Status == "approved")
                {
                    return RedirectToPage("ConfirmPhone");
                }

                ModelState.AddModelError("", $"There was an error sending the verification code: {verification.Status}");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", 
                    "There was an error sending the verification code, please check the phone number is correct and try again");
            }

            return Page();
        }
    }
}