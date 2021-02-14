namespace PardakhtNovin.Parbad.Gateway.Exceptions
{
    using System;
    using global::Parbad.Options;

    public class GenerateTransactionDataFailedException : Exception
    {
        public GenerateTransactionDataFailedException(MessagesOptions messagesOptions) : base(messagesOptions.InvalidDataReceivedFromGateway)
        { }
    }
}
