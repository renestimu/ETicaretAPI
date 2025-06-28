using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.AuthorizationEndPoint.GetRolesToEndPoint
{
    public class GetRolesToEndPointQueryHandler : IRequestHandler<GetRolesToEndPointQueryRequest, GetRolesToEndPointQueryResponse>
    {
        readonly IAuthorizationEndPointService _authorizationEndPointService;

        public GetRolesToEndPointQueryHandler(IAuthorizationEndPointService authorizationEndPointService)
        {
            _authorizationEndPointService = authorizationEndPointService;
        }

        public async Task<GetRolesToEndPointQueryResponse> Handle(GetRolesToEndPointQueryRequest request, CancellationToken cancellationToken)
        {
          var datas=await _authorizationEndPointService.GetRolesToEndPointAsync(request.Code,request.Menu);

            return new()
            {
                Roles = datas
            };
        }
    }
}
