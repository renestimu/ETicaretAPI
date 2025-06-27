using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<(object,int)> GetRolesAsync(int page,int size);
        Task<(string id,string name)> GetRoleByIdAsync(string id);
        Task<bool> CreateRole(string name);
        Task <bool> DeleteRole(string Id);
        
        Task<bool> UpdateRole(string id, string name); // Uncomment if you need to update roles
    }
}
