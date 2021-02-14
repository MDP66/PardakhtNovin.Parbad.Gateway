namespace PardakhtNovin.Parbad.Gateway
{
    using global::Parbad.Abstraction;

    public class PardakhtNovinGatewayAccount : GatewayAccount
    {
        public long TerminalId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
