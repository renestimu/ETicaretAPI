using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class OrderService : IOrderService
    {

        readonly IOrderWriteRepository _orderWriteRepository;
        readonly IOrderReadRepository _orderReadRepository;
        readonly ICompletedOrderWriteRepository _completedOrderWriteRepository;
        readonly ICompletedOrderReadRepository _completedOrderReadRepository;

        public OrderService(IOrderWriteRepository orderWriteRepository, IOrderReadRepository orderReadRepository, ICompletedOrderWriteRepository completedOrderWriteRepository, ICompletedOrderReadRepository completedOrderReadRepository)
        {
            _orderWriteRepository = orderWriteRepository;
            _orderReadRepository = orderReadRepository;
            _completedOrderWriteRepository = completedOrderWriteRepository;
            _completedOrderReadRepository = completedOrderReadRepository;
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
            var data2 = (from order in data
                         join CompletedOrder in _completedOrderReadRepository.Table on order.Id equals CompletedOrder.OrderId into co
                         from _co in co.DefaultIfEmpty()
                         select new
                         {
                             order.Id,
                             order.CreateDate,
                             order.OrderCode,
                             order.Basket,
                             Completed = _co != null ? true : false
                         });


            return new()
            {
                TotalCount = await query.CountAsync(),
                Orders = await data2.Select(o => new
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CreateDate = o.CreateDate,
                    UserName = o.Basket.User.UserName,
                    TotalPrice = o.Basket.BasketItems.Sum(bi => bi.Product.Price * bi.Quantity),
                    o.Completed

                }).ToListAsync()
            };

        }

        public async Task<SingleOrder> GetOrderByIdAsync(string id)
        {
            var data = _orderReadRepository.Table.Include(o => o.Basket)
                .ThenInclude(b => b.BasketItems)
                .ThenInclude(bi => bi.Product);
            //  .FirstOrDefaultAsync(o => o.Id == Guid.Parse(id));

            var data2 =await (from order in data
                         join CompletedOrder in _completedOrderReadRepository.Table on order.Id equals CompletedOrder.OrderId into co
                         from _co in co.DefaultIfEmpty()
                         select new
                         {
                             Id = order.Id,
                             CreateDate = order.CreateDate,
                             OrderCode = order.OrderCode,
                             Basket = order.Basket,
                             Description=order.Description,
                             Address=order.Address,
                             Completed = _co != null ? true : false
                         }).FirstOrDefaultAsync(o=>o.Id==Guid.Parse(id));


            return new()
            {
                Id = data2.Id.ToString(),
                OrderCode = data2.OrderCode,
                CreateDate = data2.CreateDate,
                Description = data2.Description,
                Address = data2.Address,
                Completed=data2.Completed,
                BasketItems = data2.Basket.BasketItems.Select(bi => new
                {

                    Name = bi.Product.Name,
                    Quantity = bi.Quantity,
                    Price = bi.Product.Price
                }).ToList()

            };
        }
        public async Task CompleteOrderAsync(string id)
        {
            Order order = await _orderReadRepository.GetByIdAsync(id);
            if (order != null)
            {
                await _completedOrderWriteRepository.AddAsync(new() { OrderId = Guid.Parse(id) });
                await _completedOrderWriteRepository.SaveAsync();

            }
        }
    }
}
