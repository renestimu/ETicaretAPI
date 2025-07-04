﻿using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateAsync(CreateUser model);
        Task UpdateRefreshTokenAsync(string refreshToken, Domain.Entities.Identity.AppUser user,DateTime accessTokenDate,int refreshTokenLifeTime);

        Task UpdatePasswordAsync(string userId, string resetToken, string newPassword);
        Task<(List<ListUser>,int)> GetAllUsersAsync(int page,int size);
        Task AssignRoleToUserAsync(string userId, string[] roles);
        Task<string[]> GetRolesToUserAsync(string userIdOrName);
        Task<bool> HasRolePermissionToEndPointAsync(string name,string code);

    }
}
