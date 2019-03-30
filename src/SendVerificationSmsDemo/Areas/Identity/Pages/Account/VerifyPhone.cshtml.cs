using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SendVerificationSmsDemo.Areas.Identity.Pages.Account
{
    public class VerifyPhoneModel : PageModel
    {
        private readonly TwilioVerifyClient _client;

        public VerifyPhoneModel(TwilioVerifyClient client)
        {
            _client = client;
        }

        [BindProperty]
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
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _client.StartVerification(Input.DialingCode, Input.PhoneNumber);
                if (result.Success)
                {
                    return RedirectToPage("ConfirmPhone", new {Input.DialingCode, Input.PhoneNumber});
                }

                ModelState.AddModelError("", $"There was an error sending the verification code: {result.Message}");
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