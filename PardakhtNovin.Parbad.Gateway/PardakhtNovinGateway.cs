namespace PardakhtNovin.Parbad.Gateway
{
    using global::Parbad;
    using global::Parbad.Abstraction;
    using global::Parbad.GatewayBuilders;
    using global::Parbad.Internal;
    using global::Parbad.Options;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using Exceptions;
    using global::Parbad.Http;
    using global::Parbad.Net;
    using Model;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Logging;

    [Gateway(Name)]
    public class PardakhtNovinGateway : GatewayBase<PardakhtNovinGatewayAccount>
    {
        public const string Name = "PardakhtNovin";
        private readonly ILogger<PardakhtNovinGateway> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private PardakhtNovinGatewayOptions _gatewayOptions;
        private MessagesOptions _messagesOptions;
        public PardakhtNovinGateway(
            ILogger<PardakhtNovinGateway> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IOptions<PardakhtNovinGatewayOptions> gatewayOptions,
            IOptions<MessagesOptions> messagesOptions,
            IGatewayAccountProvider<PardakhtNovinGatewayAccount> accountProvider) : base(accountProvider)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _gatewayOptions = gatewayOptions.Value;
            _messagesOptions = messagesOptions.Value;
        }

        public override async Task<IPaymentRequestResult> RequestAsync(Invoice invoice, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var account = await GetAccountAsync();
                await DoMerchantLoginAsync(account, cancellationToken);
                var dataToSign = await GenerateDataToSignAsync(account, invoice, cancellationToken);
                var token = await GetTokenFromSignedDataAsync(account, dataToSign, cancellationToken);

                return PaymentRequestResult.SucceedWithPost(
                    account.Name,
                    _httpContextAccessor.HttpContext,
                    _gatewayOptions.PaymentPageUrl,
                    new Dictionary<string, string>
                    {
                        {"token", token.Token},
                        {"language","fa"},
                        {"RedirectURL", invoice.CallbackUrl}
                    });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in RequestAsync");
                return PaymentRequestResult.Failed(_messagesOptions.PaymentFailed);
            }
        }

        public override async Task<IPaymentVerifyResult> VerifyAsync(InvoiceContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var account = await GetAccountAsync();
                var paymentData = await ExtractPaymentDataFromBankResponseAsync(context, cancellationToken);
                if (!paymentData.IsPaymentSuccessFul()) return PaymentVerifyResult.Failed(_messagesOptions.PaymentFailed);

                var verifyResult = await VerifyTransactionAsync(account, paymentData, cancellationToken);
                return PaymentVerifyResult.Succeed(verifyResult.RefNum, _messagesOptions.PaymentSucceed);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in VerifyAsync");
                return PaymentVerifyResult.Failed(e.Message);
            }
        }

        public override async Task<IPaymentRefundResult> RefundAsync(InvoiceContext context, Money amount, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                Guard.Against.Null(context, nameof(context));
                var account = await GetAccountAsync(context.Payment);

                await RefundTransactionAsync(context, cancellationToken, account);
                return PaymentRefundResult.Succeed(_messagesOptions.PaymentFailed);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in RefundAsync");
                return PaymentRefundResult.Failed(e.Message);
            }
        }

        #region Privates Request
        private async Task<PardakhtNovinGatewayAccount> GetAccountAsync()
        {
            var accountProvider = await AccountProvider
                .LoadAccountsAsync();
            var account = accountProvider
                .FirstOrDefault();
            Guard.Against.Null(account, nameof(account));
            return account;
        }
        private async Task DoMerchantLoginAsync(PardakhtNovinGatewayAccount account, CancellationToken cancellationToken)
        {
            using(var httpClient = _httpClientFactory.CreateClient(Name))
            {
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                var result = await httpClient.PostJsonAsync(
                        _gatewayOptions.ApiLoginUrl,
                        new { account.UserName, account.Password },
                        cancellationToken);
                if (!result.IsSuccessStatusCode) throw new MerchantLoginFailedException(_messagesOptions);
            }
        }
        private async Task<TransactionDataResult> GenerateDataToSignAsync(PardakhtNovinGatewayAccount account, Invoice invoice, CancellationToken cancellationToken)
        {
            var data = new TransactionDataRequest(
                GenerateWSContext(account),
                invoice.TrackingNumber,
                account.TerminalId,
                invoice.Amount,
                invoice.CallbackUrl
            );
            using(var httpClient = _httpClientFactory.CreateClient(Name))
            {
                var signData = await httpClient.PostJsonAsync(
                        _gatewayOptions.ApiTransactionDataToSignUrl,
                        data,
                        cancellationToken);
                if (!signData.IsSuccessStatusCode) throw new GenerateTransactionDataFailedException(_messagesOptions);

                return await DeserializeResponseMessageTo<TransactionDataResult>(signData);
            }
        }
        private async Task<SignedDataTokenResult> GetTokenFromSignedDataAsync(PardakhtNovinGatewayAccount account, TransactionDataResult dataToSign, CancellationToken cancellationToken)
        {
            var data = new SignedDataTokenRequest(
                GenerateWSContext(account),
                "72726e87-e5e2-4d2a-8f16-c4de22d380e7-6b06d7fa-6dab-475a-9991-a23ee7f24d45",
                dataToSign.UniqueId
            );
            using(var httpClient = _httpClientFactory.CreateClient(Name))
            {
                var tokenData = await httpClient.PostJsonAsync(
                _gatewayOptions.ApiGenerateTokenUrl,
                    data,
                    cancellationToken);
                if (!tokenData.IsSuccessStatusCode) throw new GetTokenDataFailedException(_messagesOptions);
                    return await DeserializeResponseMessageTo<SignedDataTokenResult>(tokenData);
            }
        }
        #endregion

        #region Privates Verify
        private async Task<NovinPardakhtPaymentData> ExtractPaymentDataFromBankResponseAsync(InvoiceContext context, CancellationToken cancellationToken)
        {
            var paymentData = new NovinPardakhtPaymentData();
            var token = await
                _httpContextAccessor.HttpContext.Request.TryGetParamAsync(nameof(NovinPardakhtPaymentData.Token),
                    cancellationToken);
            if (token.Exists)
                paymentData.Token = token.Value;

            var state = await
                _httpContextAccessor.HttpContext.Request.TryGetParamAsync(nameof(NovinPardakhtPaymentData.State),
                    cancellationToken);
            if (state.Exists)
                paymentData.State = state.Value;

            var resNum = await
                _httpContextAccessor.HttpContext.Request.TryGetParamAsync(nameof(NovinPardakhtPaymentData.ResNum),
                    cancellationToken);
            if (resNum.Exists)
                paymentData.ResNum = resNum.Value;

            var refNum = await
                _httpContextAccessor.HttpContext.Request.TryGetParamAsync(nameof(NovinPardakhtPaymentData.RefNum),
                    cancellationToken);
            if (refNum.Exists)
                paymentData.RefNum = refNum.Value;

            var customerRefNum = await
                _httpContextAccessor.HttpContext.Request.TryGetParamAsync(nameof(NovinPardakhtPaymentData.CustomerRefNum),
                    cancellationToken);
            if (customerRefNum.Exists)
                paymentData.CustomerRefNum = customerRefNum.Value;
            return paymentData;
        }
        private async Task<VerifyMerchantTransactionResult> VerifyTransactionAsync(PardakhtNovinGatewayAccount account, NovinPardakhtPaymentData paymentData, CancellationToken cancellationToken)
        {
            var data = new VerifyMerchantTransactionRequest(
                GenerateWSContext(account),
                paymentData.Token,
                paymentData.RefNum
            );
            using(var httpClient = _httpClientFactory.CreateClient(Name))
            {
                var verifyTransaction = await httpClient.PostJsonAsync(_gatewayOptions.ApiVerificationUrl, data, cancellationToken);

                var responseData =
                        await verifyTransaction
                            .Content
                            .ReadAsStringAsync();

                Guard.Against.NullOrEmpty(responseData, nameof(responseData));

                var verifyTransactionResult = JsonConvert.DeserializeObject<VerifyMerchantTransactionResult>(responseData);
                if (!verifyTransactionResult.TransactionVerifiedSuccessfully()) throw new TransactionCanceledException(_messagesOptions);

                return verifyTransactionResult;
            }
        }
        #endregion

        #region Privates Refund

        private async Task RefundTransactionAsync(InvoiceContext context, CancellationToken cancellationToken,
            PardakhtNovinGatewayAccount account)
        {
            var data = new TransactionRefundRequest(
                GenerateWSContext(account),
                context.Payment.Token,
                context.Payment.TransactionCode
            );
            using(var httpClient = _httpClientFactory.CreateClient(Name))
            {
            
                var refundTransaction = await httpClient.PostJsonAsync(
                        _gatewayOptions.ApiRefundUrl,
                        data,
                        cancellationToken);
                var responseData = await refundTransaction
                        .Content
                        .ReadAsStringAsync();
                    Guard.Against.NullOrEmpty(responseData, nameof(responseData));

                var refundTransactionStatus = JsonConvert.DeserializeObject<TransactionRefundResult>(responseData);
                if (!refundTransactionStatus.TransactionRefundedSuccessfully())
                    throw new RefundTransactionFailedException(_messagesOptions);
            }
        }

        #endregion

        #region Privates
        private WSContext GenerateWSContext(PardakhtNovinGatewayAccount account)
        {
            return new WSContext(account.UserName, account.Password);
        }

        private async Task<T> DeserializeResponseMessageTo<T>(HttpResponseMessage responseMessage) where T : new()
        {
            var responseData = await responseMessage
                .Content
                .ReadAsStringAsync();

            Guard.Against.NullOrEmpty(responseData, nameof(responseData));

            return JsonConvert.DeserializeObject<T>(responseData);
        }
        #endregion
    }

}
