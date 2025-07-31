using ECommerceAPI.Application.Abstractions.Services;
using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.DTOs.Facebook;
using ECommerceAPI.Application.Exceptions;
using ECommerceAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;
using System.Text.Json;

namespace ECommerceAPI.Persistance.Services
{
    public class AuthService : IAuthService
    {
        readonly HttpClient _httpClient;
        readonly IConfiguration _configuration;
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly SignInManager<ECommerceAPI.Domain.Entities.Identity.AppUser> _signInManager;
        readonly IUserService _userService;
        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration, UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, SignInManager<AppUser> signInManager, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _signInManager = signInManager;
            _userService = userService;
        }

        //Tekrar eden kodları buradaki fonksiyonda yazdık ve uyguladık
        async Task<Token> CreateUserExternalAsync(AppUser user, string email, string name, UserLoginInfo info, int accessTokenLifeTime)
        {
            bool result = user != null;

            //kullanıcı database'de yoksa
            if (user == null)
            {
                // garanti olsun diye email database'de var mı diye kontrol ettik
                user = await _userManager.FindByEmailAsync(email);
                result = true;

                if (user == null)
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        UserName = email,
                        NameSurname = name
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
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);

                // normal tokeni oluşturduktan sonra burada refresh tokeni de oluşturuyoruz
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 5);
                return token;
            }
            throw new Exception("Invalid external authentication!");

        }

        public async Task<Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime)
        {
            // facebook login olduktan sonra gelen token'ı alıyoruz
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_configuration["Facebook:ClientId"]}&client_secret={_configuration["Facebook:SecretId"]}&grant_type=client_credentials");

            // gelen response'daki değerleri DTO'muza bu şekilde dönüştürdük
            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse?.AccessToken}");

            // gelen bilgileri DTO ile istediğimiz formata çevirdik
            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation?.Data.IsValid != null)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");
                FacebookUserInfoResponse? userInfo = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK");

                // eğer kullanıcı database'de varsa bu şekilde login ediyoruz
                Domain.Entities.Identity.AppUser? user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                return await CreateUserExternalAsync(user, userInfo.Email, userInfo.Name, info, accessTokenLifeTime);
            }
            throw new Exception("Invalid external authentication!");
        }

        public async Task<Token> GoogleLoginAsync(string idToken, int accessTokenLifeTime)
        {
            // settings'e Google Cloud'dan aldığımız Client Id'yi veriyoruz
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["Google:ClientId"] }
            };

            // ValidateAsync ile IdToken'la settings'i doğrularız ve payloadları elde ederiz
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            // Eğer kullanıcı Database'de kayutlı değilse kaydedebilmek için bu bilgiyi kullanmamız gerekiyor
            var info = new UserLoginInfo("GOOGLE", payload.Subject, "GOOGLE");

            // eğer kullanıcı database'de varsa bu şekilde login ediyoruz
            Domain.Entities.Identity.AppUser? user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            return await CreateUserExternalAsync(user, payload.Email, payload.Name, info, accessTokenLifeTime);
        }

        public async Task<Token> LoginAsync(string usernameOrEmail, string password, int accessTokenLifeTime)
        {
            ECommerceAPI.Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(usernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(usernameOrEmail);

            if (user == null)
                throw new NotFoundUserException();

            // Eğer kullanıcı bulunursa, şifresini kontrol et
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            // Authentication başarılı
            if (result.Succeeded)
            {
                Token token = _tokenHandler.CreateAccessToken(accessTokenLifeTime);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 5);
                return token;
            }
            throw new AutheticationErrorException(); // Authentication başarısız olduğunda bu şekilde hata da fırlatılabilir
        }

        public async Task<Token> RefreshTokenLoginAsync(string refreshToken)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user != null && user?.RefreshTokenEndDate > DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(15);
                await _userService.UpdateRefreshToken(token.RefreshToken, user, token.Expiration, 15);
                return token;
            }
            else
            {
                throw new NotFoundUserException();
            }
        }
    }
}
