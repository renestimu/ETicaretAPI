using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Order.GetAllOrders
{
    public class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQueryRequest,GetAllOrderQueryResponse>
    {
        readonly IOrderService _orderService;

        public GetAllOrderQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task <GetAllOrderQueryResponse> Handle(GetAllOrderQueryRequest request, CancellationToken cancellationToken)
        {
          var data=  await _orderService.GetAllOrdersAsync(request.Page,request.Size);

            return new()
            {
                TotalCount = data.TotalCount,
                Orders = data.Orders
            };

            //return data.Select(o => new GetAllOrderQueryResponse
            //{
            //    OrderCode = o.OrderCode,
            //    CreateDate = o.CreateDate,
            //    UserName = o.UserName,
            //    TotalPrice = o.TotalPrice
            //}).ToList();
        }
    }
}
