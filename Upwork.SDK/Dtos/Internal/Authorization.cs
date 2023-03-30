using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Upwork.SDK.Dtos.Internal
{
    internal record AuthorizationRequest
    {
        [JsonPropertyName("grant_type")]
        internal string GrandType { get; set; }
        [JsonPropertyName("client_id")]
        internal string ClientId { get; set; }
        [JsonPropertyName("client_secret")]
        internal string ClientSecret { get; set; }
        [JsonPropertyName("code")]
        internal string Code { get; set; }
        [JsonPropertyName("return_uri")]
        internal string RedirectUri { get; set; }
        [JsonPropertyName("refresh_token")]
        internal string RefreshToken { get; set; }
    }

    internal record AuthorizationResponse([JsonPropertyName("access_token")]  string AccessToken, [JsonPropertyName("refresh_token")] string RefreshToken, )
        
}
