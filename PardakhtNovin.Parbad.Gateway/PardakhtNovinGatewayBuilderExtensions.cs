namespace PardakhtNovin.Parbad.Gateway
{
    using global::Parbad;
    using global::Parbad.GatewayBuilders;
    using global::Parbad.InvoiceBuilder;
    using System;

    public static class PardakhtNovinGatewayBuilderExtensions
    {
        public static IGatewayConfigurationBuilder<PardakhtNovinGateway> AddPardakhtNovin(this IGatewayBuilder builder)
        {
            return builder.AddGateway<PardakhtNovinGateway>();
        }

        public static IGatewayConfigurationBuilder<PardakhtNovinGateway> WithAccounts(this IGatewayConfigurationBuilder<PardakhtNovinGateway> builder, Action<IGatewayAccountBuilder<PardakhtNovinGatewayAccount>> configureAccounts)
        {
            return builder.WithAccounts(configureAccounts);
        }

        public static IInvoiceBuilder UsePardakhtNovin(this IInvoiceBuilder builder)
        {
            return builder.SetGateway(PardakhtNovinGateway.Name);
        }
    }
}
