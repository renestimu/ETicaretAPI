using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthorizationEndPointService : IAuthorizationEndPointService
    {
        readonly IApplicationService _applicationService;
        readonly IEndPointReadRepository _endPointReadRepository;
        readonly IEndPointWriteRepository _endPointWriteRepository;
        readonly IMenuReadRepository _menuReadRepository;
        readonly IMenuWriteRepository _menuWriteRepository;
        readonly RoleManager<AppRole> _roleManager;

        public AuthorizationEndPointService(IApplicationService applicationService, IEndPointReadRepository endPointReadRepository, IEndPointWriteRepository endPointWriteRepository, IMenuReadRepository menuReadRepository, IMenuWriteRepository menuWriteRepository, RoleManager<AppRole> roleManager)
        {
            _applicationService = applicationService;
            _endPointReadRepository = endPointReadRepository;
            _endPointWriteRepository = endPointWriteRepository;
            _menuReadRepository = menuReadRepository;
            _menuWriteRepository = menuWriteRepository;
            _roleManager = roleManager;
        }
        public async Task<bool> AssignRoleEndPointAsync(string[] roles,string menu, string endPointCode,Type type)
        {
            Menu? menuEntity = await _menuReadRepository.GetSingleAsync(x => x.Name == menu);
            if (menuEntity == null)
            {
                menuEntity = new Menu
                {
                    Id = Guid.NewGuid(),
                    Name = menu
                };
               await _menuWriteRepository.AddAsync(menuEntity);
               await _menuWriteRepository.SaveAsync();
            }


            EndPoint? endPoint=  await _endPointReadRepository.Table.Include(e=>e.Menu).Include(e=>e.AppRoles).FirstOrDefaultAsync(x => x.Code == endPointCode && x.Menu.Name==menu);
            if (endPoint == null)
            {
               var action= _applicationService.GetAuthorizeDefinitionEndPoints(type)
                    .FirstOrDefault(x => x.Name == menu)?
                    .Actions.FirstOrDefault(x => x.Code == endPointCode);

                endPoint = new EndPoint
                {
                    Id = Guid.NewGuid(),
                    Menu = menuEntity,
                    ActionType = action?.ActionType,
                    Definition = action?.Definition,
                    Code = action?.Code,
                    HttpType = action?.HttpType
                };


                await _endPointWriteRepository.AddAsync(endPoint);
                await _endPointWriteRepository.SaveAsync();
            }
            else
            {
                foreach (var role in endPoint.AppRoles)
                {
                    endPoint.AppRoles.Remove(role);

                }
            }
                var appRoles = await _roleManager.Roles
                    .Where(x => roles.Contains(x.Name))
                    .ToListAsync();
            foreach (var role in appRoles)
            {
                endPoint.AppRoles.Add(role);

            }
            await _endPointWriteRepository.SaveAsync();
            return true;
        }

        public async Task<List<string>> GetRolesToEndPointAsync(string code, string menu)
        {
          EndPoint? endPoint=  await _endPointReadRepository.Table.Include(e => e.AppRoles) .Include(e => e.Menu)
                .FirstOrDefaultAsync(x => x.Code ==code && x.Menu.Name==menu);
            if (endPoint != null)
                return endPoint.AppRoles.Select(x => x.Name).ToList();
            else return new();
        }
    }
}
