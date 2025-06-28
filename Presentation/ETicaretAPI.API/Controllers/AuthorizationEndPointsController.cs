using ETicaretAPI.Application.Features.Commands.AuthorizationEndPoint.AssignRoleEndPoint;
using ETicaretAPI.Application.Features.Queries.AuthorizationEndPoint.GetRolesToEndPoint;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationEndPointsController : ControllerBase
    {
        readonly IMediator _mediator;
        public AuthorizationEndPointsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("get-roles-to-endpoint")]
        public async Task<IActionResult> GetRolesToEndPoint( GetRolesToEndPointQueryRequest request)
        {
            GetRolesToEndPointQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }



        [HttpPost]
        public async Task<IActionResult> AssignRoleEndPoint([FromBody] AssignRoleEndPointCommandRequest request)
        {
            request.Type = typeof(Program);
            AssignRoleEndPointCommandResponse response = await _mediator.Send(request);

            return Ok(response);
        }

    }
}
