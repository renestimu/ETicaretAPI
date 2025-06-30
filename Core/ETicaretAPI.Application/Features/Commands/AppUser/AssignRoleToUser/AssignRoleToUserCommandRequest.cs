using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.AssignRoleToUser
{
    public class AssignRoleToUserCommandRequest:IRequest<AssignRoleToUserCommandResponse>
    {
        // Add properties relevant to the request here
        public string UserId { get; set; }
        public string[] Roles { get; set; }
    }
}