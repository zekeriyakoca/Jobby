using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Upwork.SDK.Dtos.Internal;

namespace Upwork.SDK
{
    internal class ConnectionManager
    {
        private readonly HttpClient client;
        private readonly IConfiguration configuration;

        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string returnUrl;

        private string authToken;
        private string refreshToken;

        internal ConnectionManager(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
            clientId = configuration.GetValue<string>("ClientId") ?? throw new ArgumentNullException("ClientId cannot be null");
            clientSecret = configuration.GetValue<string>("ClientSecret") ?? throw new ArgumentNullException("ClientSecret cannot be null");
            returnUrl = configuration.GetValue<string>("ReturnUrl") ?? throw new ArgumentNullException("ReturnUlr cannot be null");
        }

        internal async ValueTask<string> GetAuthorizationTokenAsync()
        {
            if (authToken != null)
            {
                return authToken;
            }

            var code = await GetAuthorizationCodeAsync();

            var resp = await client.PostAsJsonAsync($" /api/v3/oauth2/token", new AuthorizationRequest
            {
                Code = code,
                ClientId = clientId,
                ClientSecret = clientSecret
            });

            if (!resp.IsSuccessStatusCode)
            {
                HandleUnauthorizedResponse(resp);
                throw new Exception("Upwork API : Unable to get access token!");
            }

            var tokens = await resp.Content.ReadFromJsonAsync<AuthorizationResponse>();
            if (tokens == default)
            {
                throw new Exception("Upwork API : Unable to parse tokens!");
            }
            CacheTokens(tokens);

            return tokens.AccessToken;
        }

        internal async ValueTask<string> RefreshAuthorizationTokenAsync()
        {
            if (String.IsNullOrWhiteSpace(refreshToken))
                return await GetAuthorizationCodeAsync();

            var resp = await client.GetAsync($"/ab/account-security/oauth2/authorize?response_type=refresh_token&client_secret={clientSecret}&refresh_token={refreshToken}");
            if (!resp.IsSuccessStatusCode)
            {
                HandleUnauthorizedResponse(resp);
                throw new Exception("Upwork API : Unable to refresh token!");
            }
            var resposne = await resp.Content.ReadAsStringAsync(); // TODO : Complete from here
            // Cache tokens
            return resposne.Token;
        }

        private async Task<string> GetAuthorizationCodeAsync()
        {
            var resp = await client.GetAsync($"/ab/account-security/oauth2/authorize?response_type=authorization_code&clientId={clientId}&return_url={returnUrl}");
            if (!resp.IsSuccessStatusCode)
            {
                HandleUnauthorizedResponse(resp);
                throw new Exception("Upwork API : Unable to get authorization code!");
            }
            var code = await resp.Content.ReadAsStringAsync(); // TODO : Check here
            return code;
        }

        private void CacheTokens(AuthorizationResponse tokens)
        {
            authToken = tokens.AccessToken;
            refreshToken = tokens.RefreshToken;
        }

        private static void HandleUnauthorizedResponse(HttpResponseMessage resp)
        {
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Upwork API : 'Unauthorized' response returned!");
            }
        }
    }
}
