using Twilio.Rest.Lookups.V1;

namespace ValidatePhoneNumberDemo
{
    public static class PhoneNumberResourceExtensions
    {
        public static PhoneNumberResource.TypeEnum GetPhoneNumberType(this PhoneNumberResource phoneNumber)
        {
            if (phoneNumber?.Carrier != null 
                && phoneNumber.Carrier.TryGetValue("type", out string rawType))
            {
                return rawType;
            }

            return null;
        }
    }
}