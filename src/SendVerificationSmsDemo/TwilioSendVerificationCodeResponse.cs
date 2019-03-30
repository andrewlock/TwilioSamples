using System;

namespace SendVerificationSmsDemo
{
    public class TwilioSendVerificationCodeResponse
    {
        public string Carrier { get; set; }
        public bool IsCellphone { get; set; }
        public string Message { get; set; }
        public string SecondsToExpire { get; set; }
        public Guid Uuid { get; set; }
        public bool Success { get; set; }
    }
}