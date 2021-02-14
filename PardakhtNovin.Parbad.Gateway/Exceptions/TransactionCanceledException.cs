namespace PardakhtNovin.Parbad.Gateway.Exceptions
{
    using global::Parbad.Options;
    using System;

    public class TransactionCanceledException : Exception
    {
        public TransactionCanceledException(MessagesOptions messagesOptions) : base(messagesOptions.PaymentFailed)
        { }
    }
}
