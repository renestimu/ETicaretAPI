using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.ViewModels.Baskets;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class BasketService : IBasketService
    {
        readonly IHttpContextAccessor _contextAccessor;
        readonly UserManager<AppUser> _userManager;
        readonly IOrderReadRepository _orderReadRepository;
        readonly IBasketWriteRepository _basketWriteRepository;
        readonly IBasketReadRepository _basketReadRepository;
        readonly IBasketItemWriteRepository _basketItemWriteRepository;
        readonly IBasketItemReadRepository _basketItemReadRepository;

        public BasketService(IHttpContextAccessor contextAccessor, UserManager<AppUser> userManager, IBasketWriteRepository basketWriteRepository, IBasketItemWriteRepository basketItemWriteRepository, IBasketItemReadRepository basketItemReadRepository, IBasketReadRepository basketReadRepository, IOrderReadRepository orderReadRepository)
        {
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _orderReadRepository = orderReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _basketItemWriteRepository = basketItemWriteRepository;
            _basketItemReadRepository = basketItemReadRepository;
            _basketReadRepository = basketReadRepository;
            _orderReadRepository = orderReadRepository;
        }

        private async Task<Basket?> ContextUser()
        {
            var username = _contextAccessor?.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                AppUser? user = await _userManager.Users.Include(u => u.Baskets).FirstOrDefaultAsync(x => x.UserName == username);
                var _basket = from basket in user?.Baskets
                              join order in _orderReadRepository.Table
                              on basket.Id equals order.Id into Basketorders
                              from ba in Basketorders.DefaultIfEmpty()
                              select new { Basket = basket, Order = ba };
                Basket? targetBasket = null;
                if (_basket.Any(b => b.Order is null))
                {
                    targetBasket = _basket.FirstOrDefault(b => b.Order is null)?.Basket;
                }
                else
                {
                    targetBasket = new();
                    user.Baskets.Add(targetBasket);
                }
                await _basketWriteRepository.SaveAsync();
                return targetBasket;
            }
            throw new Exception("Beklenmeyen bir hata ile karşılaşıldı");
        }

        public async Task AddItemToBasketAsync(VM_Create_BasketItem basketItem)
        {
            Basket? basket = await ContextUser();
            if (basket != null)
            {
                BasketItem _basketItem = await _basketItemReadRepository.GetSingleAsync(bi => bi.BasketId == basket.Id && bi.ProductId == Guid.Parse(basketItem.ProductId));
                if (_basketItem != null)
                {
                    _basketItem.Quantity++;

                }
                else
                    await _basketItemWriteRepository.AddAsync(new()
                    {
                        BasketId = basket.Id,
                        ProductId = Guid.Parse(basketItem.ProductId),
                        Quantity = basketItem.Quantity
                    });
                await _basketItemWriteRepository.SaveAsync();
            }
        }

        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            Basket? basket = await ContextUser();
            Basket? basketResult = await _basketReadRepository.Table
                .Include(b => b.BasketItems)
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(b => b.Id == basket.Id);

            return basketResult.BasketItems.ToList();
        }

        public async Task RemoveBasketItemAsync(string basketItemId)
        {
            BasketItem basketItem = await _basketItemReadRepository.GetByIdAsync(basketItemId);
            if (basketItem != null)
            {
                _basketItemWriteRepository.Remove(basketItem);
                await _basketItemWriteRepository.SaveAsync();
            }
        }

        public async Task UpdateQuantityAsync(VM_Update_BasketItem basketItem)
        {
            BasketItem item = await _basketItemReadRepository.GetByIdAsync(basketItem.BasketItemId);
            if (item != null)
            {
                item.Quantity = basketItem.Quantity;
                await _basketItemWriteRepository.SaveAsync();
            }
        }
    }
}
