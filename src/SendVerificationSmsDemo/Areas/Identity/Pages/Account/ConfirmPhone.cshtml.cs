using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SendVerificationSmsDemo.Areas.Identity.Pages.Account
{
    public class ConfirmPhoneModel : PageModel
    {
        private readonly TwilioVerifyClient _client;
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmPhoneModel(TwilioVerifyClient client, UserManager<IdentityUser> userManager)
        {
            _client = client;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Country dialing code")]
            public int DialingCode { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Code")]
            public string VerificationCode { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _client.CheckVerificationCode(Input.DialingCode, Input.PhoneNumber, Input.VerificationCode);
                if (result.Success)
                {
                    var identityUser = await _userManager.GetUserAsync(User);
                    identityUser.PhoneNumberConfirmed = true;
                    var updateResult =  await _userManager.UpdateAsync(identityUser);

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
                    ModelState.AddModelError("", $"There was an error confirming the verification code: {result.Message}");
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