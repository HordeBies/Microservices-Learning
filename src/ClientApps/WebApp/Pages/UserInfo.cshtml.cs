using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WebApp.Models;

namespace WebApp.Pages
{
    [Authorize]
    public class UserInfoModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;

        public UserInfoModel(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public UserInfoViewModel UserInfo { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var idpClient = httpClientFactory.CreateClient("IDPClient");
            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = false
                }
            });
            if (metaDataResponse.IsError)
            {
                throw new Exception(metaDataResponse.Error);
            }

            var accessToken = await HttpContext?.GetUserAccessTokenAsync() ?? throw new Exception("Access token not found");

            var userInfoResponse = await idpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken,
            });
            if (userInfoResponse.IsError)
            {
                throw new Exception(metaDataResponse.Error);
            }

            //var userInfoDictionary = new Dictionary<string, string>();
            //foreach (var claim in userInfoResponse.Claims)
            //{
            //    userInfoDictionary.Add(claim.Type, claim.Value);
            //}
            var dict = userInfoResponse.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
            dict["accessToken"] = accessToken;
            dict["refreshToken"] = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            dict["id_token"] = await HttpContext?.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            UserInfo = new UserInfoViewModel(dict);
            return Page();
        }
    }
}
