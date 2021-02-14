namespace PardakhtNovin.Parbad.Gateway.Model
{
    using System;

    public class TransactionRefundResult
    {
        public bool TransactionRefundedSuccessfully()
        {
            return string.Equals(Result, "er_succeed", StringComparison.CurrentCultureIgnoreCase);
        }

        public string RefNum { get; set; }
        public string Result { get; set; }
        public decimal Amount { get; set; }
    }
}
