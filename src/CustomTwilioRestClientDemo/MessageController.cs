using Microsoft.AspNetCore.Mvc;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace CustomTwilioRestClientDemo
{
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ITwilioRestClient _client;

        public MessageController(ITwilioRestClient client)
        {
            _client = client;
        }

        [HttpPost("api/send-sms")]
        public IActionResult SendSms(MessageModel model)
        {
            var message = MessageResource.Create(
                to: new PhoneNumber(model.To),
                from: new PhoneNumber(model.From),
                body: model.Message,
                client: _client);

            return Ok(message.Sid);
        }

        public class MessageModel
        {
            public string To { get; set; }
            public string From { get; set; }
            public string Message { get; set; }
        }
    }
}