using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.Facebook;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly HttpClient _httpClient; // kullanabilmek için ServiceRegistration'a ekle
        readonly IConfiguration _configuration;

        public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            // facebook login olduktan sonra gelen token'ı alıyoruz
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["Facebook:ClientId"]}&client_secret={_configuration["Facebook:SecretId"]}&grant_type=client_credentials");

            // gelen response'daki değerleri DTO'muza bu şekilde dönüştürdük
            FacebookAccessTokenResponse facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>( accessTokenResponse );

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookAccessTokenResponse.AccessToken}");

            // gelen bilgileri DTO ile istediğimiz formata çevirdik
            FacebookUserAccessTokenValidation validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>( userAccessTokenValidation );

            if (validation.Data.IsValid)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");
                FacebookUserInfoResponse userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");

                // eğer kullanıcı database'de varsa bu şekilde login ediyoruz
                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                bool result = user != null;

                //kullanıcı database'de yoksa
                if (user == null)
                {
                    // garanti olsun diye email database'de var mı diye kontrol ettik
                    user = await _userManager.FindByEmailAsync(userInfo.Email);
                    result = true;

                    if (user == null)
                    {
                        user = new()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = userInfo.Email,
                            UserName = userInfo.Email,
                            NameSurname = userInfo.Name,
                        };
                        // bu şekilde kullanıcıyı database'in aspnetusers kısmına kaydetmiş oluyoruz
                        var identityUser = await _userManager.CreateAsync(user);
                        result = identityUser.Succeeded;
                    }
                }
                if (result)
                {
                    // eğer kullanıcı başarılı bir şekilde kaydedildiyse ve dış kaynaktan geldiğini biliyorsak bu kullanıcıyı aspnetuserlogins tablosuna da kaydetmiş oluyoruz
                    await _userManager.AddLoginAsync(user, info);

                    // authentication başarılıysa token dönüyoruz
                    Token token = _tokenHandler.CreateAccessToken(5);

                    return new()
                    {
                        Token = token
                    };
                }
            }
            throw new Exception("Invalid external authentication!");



            
        }
    }
}
