using ETicaretAPI.Application.RequestParameters;
using MediatR;

namespace ETicaretAPI.Application.Features.Queries.Role.GetRoles
{
    public class GetRolesQueryRequest :Pagination, IRequest<GetRolesQueryResponse>
    {
    }
}