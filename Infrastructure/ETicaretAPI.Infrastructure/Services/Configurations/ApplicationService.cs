using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Menu;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Configurations
{
    public class ApplicationService : IApplicationService
    {
        public List<Menu> GetAuthorizeDefinitionEndPoints(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));
            List<Menu> menus = new List<Menu>();
            if (controllers != null)
            {
                foreach (var controller in controllers)
                {
                    var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute), true));
                    if (actions != null)
                        foreach (var action in actions)
                        {
                            var attributes = action.GetCustomAttributes(true);
                            if (attributes != null)
                            {
                                Menu menu = null;
                                var authorizeDefinition = attributes.FirstOrDefault(x => x.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                                if (!menus.Any(m => m.Name == authorizeDefinition.Menu))
                                {
                                    menu = new() { Name = authorizeDefinition.Menu };
                                    menus.Add(menu);
                                }
                                else
                                {
                                    menu = menus.FirstOrDefault(m => m.Name == authorizeDefinition.Menu);
                                }
                                Application.DTOs.Menu.Action _action = new()
                                {                                  
                                    ActionType = authorizeDefinition.ActionType.ToString(),
                                    Definition = authorizeDefinition.Definition
                                };
                               var httpAttribute= attributes.FirstOrDefault(x => x.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute;
                                if (httpAttribute != null)
                                {
                                    _action.HttpType = httpAttribute.HttpMethods.First();
                                }else
                                    _action.HttpType = HttpMethods.Get; // Default to GET if no HttpMethodAttribute is found
                                _action.Code = $"{_action.HttpType}.{_action.ActionType}.{_action.Definition}";
                                menu.Actions.Add(_action);
                            }
                        }
                }
            }

            return menus;
        }
    }
}
