using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IAuthorizationEndPointService
    {
        public Task<bool> AssignRoleEndPointAsync(string[] roles,string menu, string endPointCode,Type type);
        public Task<List<string>> GetRolesToEndPointAsync(string code,string menu);
    }
}
