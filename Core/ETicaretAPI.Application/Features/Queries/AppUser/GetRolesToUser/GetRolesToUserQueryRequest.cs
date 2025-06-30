using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AppUser.GetRolesToUser
{
    public class GetRolesToUserQueryRequest : IRequest<GetRolesToUserQueryResponse>
    {
        public string UserId { get; set; } // The ID of the user for whom roles are being requested
    }
}