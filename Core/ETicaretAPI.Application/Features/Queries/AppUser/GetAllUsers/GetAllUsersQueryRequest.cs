using ETicaretAPI.Application.RequestParameters;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AppUser.GetAllUsers
{
    public class GetAllUsersQueryRequest:Pagination, IRequest<GetAllUsersQueryResponse>
    {
    }
}