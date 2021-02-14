namespace PardakhtNovin.Parbad.Gateway.Exceptions
{
    using System;
    using global::Parbad.Options;

    public class RefundTransactionFailedException : Exception
    {
        public RefundTransactionFailedException(MessagesOptions messagesOptions) : base(messagesOptions.UnexpectedErrorText)
        { }
    }
}
