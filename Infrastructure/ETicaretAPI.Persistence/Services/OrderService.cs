using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {
   
        readonly IOrderWriteRepository _orderWriteRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository)
        {
            _orderWriteRepository = orderWriteRepository;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
         await _orderWriteRepository.AddAsync(new Domain.Entities.Order
            {
                Id = Guid.Parse( createOrder.BasketId),
                Description = createOrder.Description,
                Address = createOrder.Address
            });
            await _orderWriteRepository.SaveAsync();
        }
    }
}
