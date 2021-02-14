namespace PardakhtNovin.Parbad.Gateway.Exceptions
{
    using System;
    using global::Parbad.Options;

    public class GetTokenDataFailedException : Exception
    {
        public GetTokenDataFailedException(MessagesOptions _messagesOptions) : base(_messagesOptions.InvalidDataReceivedFromGateway)
        { }
    }
}
