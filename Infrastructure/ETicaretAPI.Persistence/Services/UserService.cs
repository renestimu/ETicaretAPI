using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;

        public UserService(UserManager<Domain.Entities.Identity.AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<CreateUserResponse> CreateAsync(CreateUser model)
        {
            IdentityResult result = await _userManager.CreateAsync(new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Email = model.Email,
                NameSurname = model.NameSurname
            }, model.Password);

            CreateUserResponse response = new() { Succeeded = result.Succeeded };

            if (result.Succeeded)
                response.Message = "Başarılı";

            else
            {
                foreach (var error in result.Errors)
                {
                    response.Message += $"{error.Code} - {error.Description}  \n";
                }
            }
            return response;

        }



        public async Task UpdateRefreshTokenAsync(string refreshToken, Domain.Entities.Identity.AppUser user, DateTime accessTokenDate, int refreshTokenLifeTime)
        {

            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.ReRefreshTokenEndDate = accessTokenDate.AddSeconds(refreshTokenLifeTime);
                await _userManager.UpdateAsync(user);
            }
            else
                throw new NotFoundUserException();
        }

        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                resetToken = resetToken.UrlDecode();
                IdentityResult identityResult = await _userManager.ResetPasswordAsync(appUser, resetToken, newPassword);
                if (identityResult.Succeeded) {
                  await  _userManager.UpdateSecurityStampAsync(appUser);
                }else
                    throw new PasswordChangeFailedException();
            }
        }
    }
}
