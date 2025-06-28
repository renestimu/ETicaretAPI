using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AuthorizationEndPoint.AssignRoleEndPoint
{
    public class AssignRoleEndPointCommandHandler : IRequestHandler<AssignRoleEndPointCommandRequest, AssignRoleEndPointCommandResponse>
    {
        readonly IAuthorizationEndPointService _authorizationEndPointService;

        public AssignRoleEndPointCommandHandler(IAuthorizationEndPointService authorizationEndPointService)
        {
            _authorizationEndPointService = authorizationEndPointService;
        }

        public async Task<AssignRoleEndPointCommandResponse> Handle(AssignRoleEndPointCommandRequest request, CancellationToken cancellationToken)
        {
            await _authorizationEndPointService.AssignRoleEndPointAsync(request.Roles, request.Menu, request.Code, request.Type);
            return new() { };
        }
    }
}
