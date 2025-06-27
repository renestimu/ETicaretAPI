using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.Role.CreateRole;
using ETicaretAPI.Application.Features.Commands.Role.DeleteRole;
using ETicaretAPI.Application.Features.Commands.Role.UpdateRole;
using ETicaretAPI.Application.Features.Queries.Role.GetRoleById;
using ETicaretAPI.Application.Features.Queries.Role.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class RoleController : ControllerBase
    {

        readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AuthorizeDefinition(ActionType =ActionType.Reading,Definition = "GetRoles",Menu ="Roles")]
        public async Task<IActionResult> GetRoles([FromQuery]GetRolesQueryRequest getRolesQuery)
        {
         GetRolesQueryResponse response=await   _mediator.Send(getRolesQuery);


            return Ok(response);
        }
        [HttpGet("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "GetRoleById", Menu = "Roles")]
        public async Task<IActionResult> GetRole([FromRoute] GetRoleByIdQueryRequest request)
        {
           
            GetRoleByIdQueryResponse response = await _mediator.Send(request);

            return Ok(response);
        }
        [HttpPost]
        [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "CreateRole", Menu = "Roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommandRequest request)
        {
            CreateRoleCommandResponse response =await _mediator.Send(request);
           
            return Ok(response);
        }
        [HttpPut("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "UpdateRole", Menu = "Roles")]
        public async Task<IActionResult> UpdateRole([FromBody,FromRoute] UpdateRoleCommandRequest request)
        {
            UpdateRoleCommandResponse response =await _mediator.Send(request);


            return Ok(response);
        }
        [HttpDelete("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "DeleteRole", Menu = "Roles")]
        public async Task<IActionResult> DeleteRole([FromRoute] DeleteRoleCommandRequest request)
        {
           DeleteRoleCommandResponse response=  await _mediator.Send(request);
            return Ok(response);
        }
    }
}
