using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Enums;
using Imgur.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epicture.Model
{
    public class Auth
    {
        public string _clientId = "56fc55b3d27ad8f";
        public string _clientSecret = "3b32733f688c836d682c07fc08cc609e9b074649";

        public async Task AuthorizationRequest()
        {
            var client = new ImgurClient(_clientId);
            var endpoint = new OAuth2Endpoint(client);
            var authorizationUrl = endpoint.GetAuthorizationUrl(OAuth2ResponseType.Pin);
            await Windows.System.Launcher.LaunchUriAsync(new Uri(authorizationUrl));
        }

        public async Task<IOAuth2Token> ResponseCode(string pin)
        {
            try
            {
                var client = new ImgurClient(_clientId, _clientSecret);
                var endpoint = new OAuth2Endpoint(client);
                var token = await endpoint.GetTokenByPinAsync(pin);
                Debug.WriteLine("test");
                return token;
            }
            catch (ImgurException imgurEx)
            {
                Debug.WriteLine("An error occurred will authentification: " + imgurEx.Message);
                return null;
            }
        }
    }
}
