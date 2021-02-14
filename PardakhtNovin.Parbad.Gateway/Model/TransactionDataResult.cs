namespace PardakhtNovin.Parbad.Gateway.Model
{
    using System;

    [Serializable]
    public class TransactionDataResult
    {
        public string Result { get; set; }
        public string DataToSign { get; set; }
        public string UniqueId { get; set; }
    }
}
