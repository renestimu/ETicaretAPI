using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AuthorizationEndPoint.AssignRoleEndPoint
{
    public class AssignRoleEndPointCommandRequest : IRequest<AssignRoleEndPointCommandResponse>
    {
        public string[] Roles { get; set; }
        public string Code { get; set; }
        public string Menu { get; set; }
        public Type? Type { get; set; }

    }
}