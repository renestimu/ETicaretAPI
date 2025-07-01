using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        readonly IEndPointReadRepository _endPointReadRepository;

        public UserService(UserManager<Domain.Entities.Identity.AppUser> userManager, IEndPointReadRepository endPointReadRepository)
        {
            _userManager = userManager;
            _endPointReadRepository = endPointReadRepository;
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

        public async Task<(List<ListUser>, int)> GetAllUsersAsync(int page, int size)
        {
            var usersQuery = _userManager.Users;

            var userList = await usersQuery
                .Skip(page * size)
                .Take(size)
                .Select(u => new ListUser
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    NameSurname = u.NameSurname,
                    TwoFactorEnabled = u.TwoFactorEnabled,
                })
                .ToListAsync();

            int totalUsersCount = await usersQuery.CountAsync();

            return (userList, totalUsersCount);
        }

        public async Task AssignRoleToUserAsync(string userId, string[] roles)
        {
           AppUser appUser=await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(appUser);
                await _userManager.RemoveFromRolesAsync(appUser, currentRoles);
                await _userManager.AddToRolesAsync(appUser, roles);
            }

        }

        public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
        {
            AppUser appUser = await _userManager.FindByIdAsync(userIdOrName);
            if(appUser==null)             {
                appUser = await _userManager.FindByNameAsync(userIdOrName);
            }
            if (appUser != null)
            {
                var userRoles = await _userManager.GetRolesAsync(appUser);
                return userRoles.ToArray();
            }
            else
            {
                return Array.Empty<string>();
            }
        }

        public async Task<bool> HasRolePermissionToEndPointAsync(string name, string code)
        {
           var userRoles=await GetRolesToUserAsync(name);
           if(!userRoles.Any())
                { return false; }
            EndPoint? endPoint= await _endPointReadRepository.Table.Include(e => e.AppRoles).FirstOrDefaultAsync(x=>x.Code==code);
            if (endPoint == null)
            {
                return false;
            }
            foreach (var role in userRoles)
            {
                if (endPoint.AppRoles.Any(r => r.Name == role))
                {
                    return true;
                }
            }
            return false;

        }
    }
}
