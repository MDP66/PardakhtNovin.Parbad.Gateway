namespace PardakhtNovin.Parbad.Gateway.Model
{
    using Ardalis.GuardClauses;
    using global::Parbad;
    using System;

    [Serializable]
    public class TransactionDataRequest
    {
        public TransactionDataRequest(WSContext wsContext, long reserveNum, long terminalId, Money amount, string redirectUrl)
        {
            Guard.Against.Null(wsContext, nameof(wsContext));
            Guard.Against.NegativeOrZero(reserveNum, nameof(reserveNum));
            Guard.Against.NegativeOrZero(terminalId, nameof(terminalId));
            Guard.Against.NegativeOrZero(amount, nameof(amount));
            Guard.Against.NullOrEmpty(redirectUrl, nameof(redirectUrl));

            WSContext = wsContext;
            ReserveNum = reserveNum;
            TerminalId = terminalId;
            Amount = amount.Value;
            RedirectUrl = redirectUrl;
        }
        public string TransType => "EN_GOODS";
        public WSContext WSContext { get; private set; }
        public long ReserveNum { get; private set; }
        public long TerminalId { get; private set; }
        public decimal Amount { get; private set; }
        public string RedirectUrl { get; private set; }
    }
}
