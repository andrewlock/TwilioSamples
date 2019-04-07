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
    public class ConfirmPhoneModel : PageModel
    {
        private readonly TwilioVerifySettings _settings;
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmPhoneModel(UserManager<IdentityUser> userManager, IOptions<TwilioVerifySettings> settings)
        {
            _userManager = userManager;
            _settings = settings.Value;
        }

        public string PhoneNumber { get; set; }

        [BindProperty()]
        [Required]
        [Display(Name = "Code")]
        public string VerificationCode { get; set; }


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
                var verification = await VerificationCheckResource.CreateAsync(
                    to: phone,
                    code: VerificationCode,
                    pathServiceSid: _settings.VerificationServiceSid
                );
                if (verification.Valid ?? true)
                {
                    var identityUser = await _userManager.GetUserAsync(User);
                    identityUser.PhoneNumberConfirmed = true;
                    var updateResult = await _userManager.UpdateAsync(identityUser);

                    if (updateResult.Succeeded)
                    {
                        return RedirectToPage("ConfirmPhoneSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was an error confirming the verification code, please try again");
                    }
                }
                else
                {
                    ModelState.AddModelError("", $"There was an error confirming the verification code: {verification.Status}");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "There was an error confirming the code, please check the verification code is correct and try again");
            }

            return Page();
        }
    }
}