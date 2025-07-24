using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        readonly UserManager<ECommerceAPI.Domain.Entities.Identity.AppUser> _userManager;
        readonly SignInManager<ECommerceAPI.Domain.Entities.Identity.AppUser> _signInManager;
        public LoginUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, SignInManager<Domain.Entities.Identity.AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            ECommerceAPI.Domain.Entities.Identity.AppUser user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            if (user == null)
               user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);

            if (user == null)
                throw new NotFoundUserException("Kullanıcı adı veya şifre hatalı...");

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            // Authentication başarılı
            if (result.Succeeded)
            {
                // yetkileri tanımla
            }
            return null;    
        }
    }
}
