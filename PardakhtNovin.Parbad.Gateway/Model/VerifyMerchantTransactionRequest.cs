namespace PardakhtNovin.Parbad.Gateway.Model
{
    using Ardalis.GuardClauses;

    public class VerifyMerchantTransactionRequest
    {
        public VerifyMerchantTransactionRequest(WSContext wsContext, string token, string refNum)
        {
            Guard.Against.Null(wsContext, nameof(wsContext));
            Guard.Against.NullOrEmpty(token, nameof(token));
            Guard.Against.NullOrEmpty(refNum, nameof(refNum));

            WSContext = wsContext;
            Token = token;
            RefNum = refNum;
        }

        public WSContext WSContext { get; set; }
        public string Token { get; set; }
        public string RefNum { get; set; }
    }
}
