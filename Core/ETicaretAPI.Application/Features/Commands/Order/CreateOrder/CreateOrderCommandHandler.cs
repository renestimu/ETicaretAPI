﻿using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandHandler :IRequestHandler<CreateOrderCommandRequest, CreateOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IBasketService _basketService;
        readonly IOrderHubService _orderHubService;
        public CreateOrderCommandHandler(IOrderService orderService, IBasketService basketService, IOrderHubService orderHubService )
        {
            _orderService = orderService;
            _basketService = basketService;
            _orderHubService = orderHubService;
        }

        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommandRequest request, CancellationToken cancellationToken)
        {
           await _orderService.CreateOrderAsync(new Application.DTOs.Order.CreateOrder
            {
                BasketId = _basketService.GetUserActiveBasket?.Id.ToString(),
                Description = request.Description,
                Address = request.Address
            });
           await _orderHubService.OrderAddedMessageAsync("Yeni bir sipariş geldi");
            return new();
        }
    }
    
}
