using ECommerceAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        // UserManager'ı Dependency Injection ile alıyoruz. UserManager , Identity framework'ün kullanıcı yönetimi için kullanılan bir sınıftır. Bu yüzden Repository pattern kullanmıyoruz.
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public CreateUserCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                // Id'yi burda vermemiz gerekiyor çünkü AppUser'a string olarak verdiğimiz için kendisi veremez ve hata verir
                Id = Guid.NewGuid().ToString(),
                NameSurname = request.NameSurname,
                UserName = request.Username,
                Email = request.Email
            }, request.Password);

            CreateUserCommandResponse response = new()
            {
                Succeeded = result.Succeeded
            };

            if (result.Succeeded)
                response.Message = "Kullanıcı başarıyla oluşturuldu.";
            else
                foreach(var error in result.Errors)
                    response.Message += $"{error.Code} - {error.Description}\n";
            return response;    

        }
    }
}
