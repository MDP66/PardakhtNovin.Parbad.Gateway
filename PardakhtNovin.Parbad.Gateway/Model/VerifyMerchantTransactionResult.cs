namespace PardakhtNovin.Parbad.Gateway.Model
{
    using System;

    public class VerifyMerchantTransactionResult
    {
        public bool TransactionVerifiedSuccessfully()
        {
            return string.Equals(Result, "ersucceed", StringComparison.CurrentCultureIgnoreCase);
        }

        public string RefNum { get; set; }
        public string Result { get; set; }
        public decimal Amount { get; set; }
    }
}
