using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using IpayDemo.Net.Models;

namespace IpayDemo.Net
{
    public interface IIpayService
    {
        Task<CompletedOrder> MakeOrder(Intent intent, List<Item> items, string redirectUrl, string locale, string orderId);
    }

    public class IpayConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class IpayService : IIpayService
    {
        private readonly IpayConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public IpayService(IpayConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<CompletedOrder> MakeOrder(Intent intent, List<Item> items, string redirectUrl, string locale, string orderId)
        {
            var tokenResponse = await GetToken();

            return await PlaceOrder(tokenResponse.AccessToken, intent, redirectUrl, locale, orderId, true, items);
        }

        private async Task<TokenResponse> GetToken()
        {
            var httpClient = httpClientFactory.CreateClient();

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = $"{configuration.BaseUrl}oauth2/token",
                ClientId = configuration.ClientId,
                ClientSecret = configuration.ClientSecret,
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader
            };

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest);

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.ErrorDescription);
            }

            return tokenResponse;
        }

        private async Task<CompletedOrder> PlaceOrder(string token, Intent intent, string redirectUrl, string locale, string shopOrderId, bool showShopOrderIdInStatement, List<Item> items)
        {
            var order = new Order(items)
            {
                Intent = intent,
                Locale = locale,
                ShopOrderId = shopOrderId,
                RedirectUrl = redirectUrl,
                ShowShopOrderIdOnExtract = showShopOrderIdInStatement
            };

            var httpClient = httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var responseMessage = await httpClient.PostAsJsonAsync($"{configuration.BaseUrl}checkout/orders", order);
            var completedOrder = await responseMessage.Content.ReadAsJsonAsync<CompletedOrder>();

            return completedOrder;
        }

    }
}
