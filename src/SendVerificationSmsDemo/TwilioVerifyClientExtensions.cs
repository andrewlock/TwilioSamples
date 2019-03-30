using System;
using Microsoft.Extensions.DependencyInjection;

namespace SendVerificationSmsDemo
{
    public static class TwilioVerifyClientExtensions
    {
        public static IHttpClientBuilder AddTwilioVerifyClient(this IServiceCollection services, string apiKey)
        {
            return services
                .AddHttpClient<TwilioVerifyClient>(client =>
                {
                    client.BaseAddress = new Uri("https://api.authy.com/");
                    client.DefaultRequestHeaders.Add("X-Authy-API-Key", apiKey);
                });
        }
    }
}