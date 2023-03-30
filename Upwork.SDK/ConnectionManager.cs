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

        internal ConnectionManager(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
            clientId = configuration.GetValue<string>("ClientId");
            clientSecret = configuration.GetValue<string>("ClientSecret");
            returnUrl = configuration.GetValue<string>("ReturnUlr");
        }

        internal Task<string> GetAuthorizationTokenAsync()
        {
            var code = GetAuthorizationCodeAsync();

            var resp = await client.PostAsJsonAsync($" /api/v3/oauth2/token", new AuthorizationRequest
            {
                Code = code,
                ClientId = clientId,
                ClientSecret = clientSecret
            });

            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                    throw new Exception("Upwork API : 'Unauthorized' response returned while fetching access token!");
                }
                throw new Exception("Upwork API : Unable to get access token!");
            }

            var tokens = await resp.Content.ReadFromJsonAsync<AuthorizationResponse>();
            if (tokens is default)
            {
                throw new Exception("Upwork API : Unable to parse tokens!");
            }

            return tokens.AccessToken;
        }

        private string GetAuthorizationCodeAsync()
        {
            var resp = await client.GetAsync($"/ab/account-security/oauth2/authorize?response_type=code&clientId={clientId}&return_url={returnUrl}");
            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                    throw new Exception("Upwork API : 'Unauthorized' response returned while fetching authorization code!");
                }
                throw new Exception("Upwork API : Unable to get authorization code!");
            }
            var code = await resp.Content.ReadAsStringAsync(); // TODO : Check here
            return code;
        }
    }
}
