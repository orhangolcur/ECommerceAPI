using ECommerceAPI.Application.Abstractions.Token;
using ECommerceAPI.Application.DTOs;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly IConfiguration _configuration;

        public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler, IConfiguration configuration)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            // settings'e Google Cloud'dan aldığımız Client Id'yi veriyoruz
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["Google:ClientId"] }
            };

            // ValidateAsync ile IdToken'la settings'i doğrularız ve payloadları elde ederiz
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            // Eğer kullanıcı Database'de kayutlı değilse kaydedebilmek için bu bilgiyi kullanmamız gerekiyor
            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider);
           
            // eğer kullanıcı database'de varsa bu şekilde login ediyoruz
            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null;

            //kullanıcı database'de yoksa
            if (user == null) 
            {
                // garanti olsun diye email database'de var mı diye kontrol ettik
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null) 
                {
                    user = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        NameSurname = payload.Name,
                    };
                    // bu şekilde kullanıcıyı database'in aspnetusers kısmına kaydetmiş oluyoruz
                    var identityUser = await _userManager .CreateAsync(user);
                    result = identityUser.Succeeded;
                }
            }
            if (result)
            {
                // eğer kullanıcı başarılı bir şekilde kaydedildiyse ve dış kaynaktan geldiğini biliyorsak bu kullanıcıyı aspnetuserlogins tablosuna da kaydetmiş oluyoruz
                await _userManager.AddLoginAsync(user, info);
            }
            else
            {
                throw new Exception("Invalid external authentication!");
            }

            // authentication başarılıysa token dönüyoruz
            Token token = _tokenHandler.CreateAccessToken(5);
            return new()
            {
                Token = token
            };
        }
    }
}
