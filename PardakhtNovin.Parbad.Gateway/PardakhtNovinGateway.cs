namespace PardakhtNovin.Parbad.Gateway
{
    using System.Threading;
    using System.Threading.Tasks;
    using global::Parbad;
    using global::Parbad.Abstraction;
    using global::Parbad.GatewayBuilders;

    public class PardakhtNovinGateway : GatewayBase<PardakhtNovinGatewayAccount>
    {
        public const string Name = "PardakhtNovin";
        public PardakhtNovinGateway(IGatewayAccountProvider<PardakhtNovinGatewayAccount> accountProvider) : base(accountProvider)
        { }

        public override Task<IPaymentRequestResult> RequestAsync(Invoice invoice, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public override Task<IPaymentVerifyResult> VerifyAsync(InvoiceContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }

        public override Task<IPaymentRefundResult> RefundAsync(InvoiceContext context, Money amount, CancellationToken cancellationToken = new CancellationToken())
        {
            throw new System.NotImplementedException();
        }
    }
}
