using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class RoleService : IRoleService
    {
        readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRole(string name)
        {
           IdentityResult identityResult= await _roleManager.CreateAsync(new AppRole {Id=Guid.NewGuid().ToString(), Name = name });
            if (identityResult.Succeeded)
                return true;
            foreach (IdentityError error in identityResult.Errors)
            {
                // Log the error or handle it as needed
                Console.WriteLine(error.Description);
            }
            return false;
        }

        public async Task<bool> DeleteRole(string Id)
        {
           IdentityResult result= await _roleManager.DeleteAsync(await _roleManager.FindByIdAsync(Id));
            return result.Succeeded;

        }

        public async Task<(string id, string name)> GetRoleByIdAsync(string id)
        {
          string role=  await _roleManager.GetRoleIdAsync(new() { Id= id });
            return (id, role);
        }

        public async Task<(object,int)> GetRolesAsync(int page, int size)
        {
            var res = _roleManager.Roles.Skip(page * size).Take(size).Select(r => new { r.Id, r.Name }).ToList();
            return (res, _roleManager.Roles.Count());
        }

        public async Task<bool> UpdateRole(string id, string name)
        {
            IdentityResult identityResult =await _roleManager.UpdateAsync(new AppRole { Id = id, Name = name });
            return identityResult.Succeeded;
        }
    }
}
