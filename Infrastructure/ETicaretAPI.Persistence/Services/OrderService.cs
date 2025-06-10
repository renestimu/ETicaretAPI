using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using Microsoft.EntityFrameworkCore;
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
        readonly IOrderReadRepository _orderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
        }

        public async Task CreateOrderAsync(CreateOrder createOrder)
        {
            string orderCode = (new Random().NextDouble() * 10000).ToString();
            orderCode = orderCode.Substring(orderCode.IndexOf('.') + 1, orderCode.Length - (orderCode.IndexOf('.') + 1));
            await _orderWriteRepository.AddAsync(new Domain.Entities.Order
            {
                Id = Guid.Parse(createOrder.BasketId),
                Description = createOrder.Description,
                Address = createOrder.Address,
                OrderCode = orderCode
            });
            await _orderWriteRepository.SaveAsync();
        }

        public async Task<ListOrder> GetAllOrdersAsync(int page, int size)
        {
            //return await _orderReadRepository.Table.Include(o => o.Basket)
            //           .ThenInclude(b => b.User)
            //           .Include(o => o.Basket)
            //           .ThenInclude(b => b.BasketItems)
            //           .ThenInclude(bi => bi.Product)
            //           .Select(o => new ListOrder
            //           {

            //               OrderCode = o.OrderCode,
            //               CreateDate = o.CreateDate,
            //               UserName = o.Basket.User.UserName,
            //               TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity)


            //           })
            //           .Skip(page * size).Take(size)
            //          // .Take((page*size)..size)
            //           .ToListAsync();


            var query = _orderReadRepository.Table.Include(o => o.Basket)
                    .ThenInclude(b => b.User)
                    .Include(o => o.Basket)
                    .ThenInclude(b => b.BasketItems)
                    .ThenInclude(bi => bi.Product);


            var data = query.Skip(page * size).Take(size);
            return new()
            {
                TotalCount = await query.CountAsync(),
                Orders = await data.Select(o => new
                {
                    Id=o.Id,
                    OrderCode = o.OrderCode,
                    CreateDate = o.CreateDate,
                    UserName = o.Basket.User.UserName,
                    TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity)

                }).ToListAsync()
            };

        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = await _orderReadRepository.Table.Include(o => o.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            return new()
            {
                Id = data.Id.ToString(),
                OrderCode = data.OrderCode,
                CreateDate = data.CreateDate,
                Description = data.Description,
                Address = data.Address,               
                BasketItems = data.Basket.BasketItems.Select(bi => new
                {
                    
                    Name = bi.Product.Name,
                    Quantity = bi.Quantity,
                    Price = bi.Product.Price
                }).ToList()

            };
        }
    }
}
