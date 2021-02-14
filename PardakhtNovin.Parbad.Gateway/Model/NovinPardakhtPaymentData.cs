namespace PardakhtNovin.Parbad.Gateway.Model
{
    using System;

    public class NovinPardakhtPaymentData
    {
        public string CustomerRefNum;
        public string Token { get; set; }
        public string State { get; set; }
        public string ResNum { get; set; }
        public string RefNum { get; set; }

        public bool IsPaymentSuccessFul()
        {
            return string.Equals(State, "ok", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
