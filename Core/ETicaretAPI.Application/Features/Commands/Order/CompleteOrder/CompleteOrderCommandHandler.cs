using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.CompleteOrder
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommandRequest, CompleteOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IMailService _mailService;

        public CompleteOrderCommandHandler(IOrderService orderService, IMailService mailService)
        {
            _orderService = orderService;
            _mailService = mailService;
        }

        public async Task<CompleteOrderCommandResponse> Handle(CompleteOrderCommandRequest request, CancellationToken cancellationToken)
        {
          (bool succeded,CompletedOrderDTO dTO) result=    await _orderService.CompleteOrderAsync(request.Id);
            if(!result.succeded)
            {
                await _mailService.SendCompletedOrderEmailAsync(result.dTO.Email,result.dTO.OrderCode,result.dTO.OrderDate,result.dTO.NameSurname);
            }
            return new ();
        }
    }
}
