namespace PardakhtNovin.Parbad.Gateway.Exceptions
{
    using global::Parbad.Options;
    using System;

    public class MerchantLoginFailedException : Exception
    {
        public MerchantLoginFailedException(MessagesOptions _messagesOptions) : base(_messagesOptions.UnexpectedErrorText)
        { }
    }
}
