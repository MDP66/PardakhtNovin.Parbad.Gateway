namespace PardakhtNovin.Parbad.Gateway
{
    public class PardakhtNovinGatewayOptions
    {
        public string ApiLoginUrl { get; set; } = "https://pna.shaparak.ir/ref-payment2/RestServices/mts/merchantLogin/";
        public string PaymentPageUrl { get; set; } = "https://pna.shaparak.ir/_ipgw_/payment/";

        public string ApiTransactionDataToSignUrl { get; set; } = "https://pna.shaparak.ir/ref-payment2/RestServices/mts/generateTransactionDataToSign/";
        public string ApiGenerateTokenUrl { get; set; } = "https://pna.shaparak.ir/ref-payment2/RestServices/mts/generateSignedDataToken/";

        public string ApiVerificationUrl { get; set; } = "https://pna.shaparak.ir/ref-payment2/RestServices/mts/verifyMerchantTrans/";

        public string ApiRefundUrl { get; set; } = "https://pna.shaparak.ir/ref-payment2/RestServices/mts/reverseMerchantTrans/";
    }
}
