using MediatR;

namespace ETicaretAPI.Application.Features.Queries.AuthorizationEndPoint.GetRolesToEndPoint
{
    public class GetRolesToEndPointQueryRequest : IRequest<GetRolesToEndPointQueryResponse>
    {
        public string Code { get; set; }
        public string Menu { get; set; }

    }
}