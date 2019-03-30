using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace SendVerificationSmsDemo
{
    public class TwilioVerifyClient
    {
        private readonly HttpClient _client;
        private const string SendCodeUrl = "protected/json/phones/verification/start";
        private const string CheckCodeUrl = "protected/json/phones/verification/check";

        public TwilioVerifyClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<TwilioSendVerificationCodeResponse> StartVerification(int countryCode, string phoneNumber)
        {
            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string, string>("via", "sms"),
                new KeyValuePair<string, string>("country_code", countryCode.ToString()),
                new KeyValuePair<string, string>("phone_number", phoneNumber),
            });

            var response = await _client.PostAsync(SendCodeUrl, requestContent);

            var content = await response.Content.ReadAsStringAsync();

            // this will throw if the response is not valid
            return JsonConvert.DeserializeObject<TwilioSendVerificationCodeResponse>(content);
        }

        public async Task<TwilioCheckCodeResponse> CheckVerificationCode(int countryCode, string phoneNumber, string verificationCode)
        {
            var queryParams = new Dictionary<string, string>()
            {
                {"country_code", countryCode.ToString()},
                {"phone_number", phoneNumber},
                {"verification_code", verificationCode },
            };

            var url = QueryHelpers.AddQueryString(CheckCodeUrl, queryParams);

            var response = await _client.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            // this will throw if the response is not valid
            return JsonConvert.DeserializeObject<TwilioCheckCodeResponse>(content);
        }
    }
}
