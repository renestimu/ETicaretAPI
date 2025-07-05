
using Microsoft.EntityFrameworkCore;
using ETicaretAPI.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Persistence.Repositories;
using ETicaretAPI.Persistence.Repositories.File;
using ETicaretAPI.Domain.Entities.Identity;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Persistence.Services;
using ETicaretAPI.Application.Abstractions.Services.Authentications;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Persistence
{
    public  static class ServiceRegistration
         
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));

            services.AddIdentity<AppUser, AppRole>(options => {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase=false;
                options.Password.RequireUppercase=false;
                }).AddEntityFrameworkStores<ETicaretAPIDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<ICustomerReadRepository,CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository,CustomerWriteRepository>();
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository,OrderWriteRepository>();     
            
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddScoped<IFileWriteRepository, FileWriteRepository>();
            services.AddScoped<IProductImageFileReadRepository, ProductImageFileReadRepository>();
            services.AddScoped<IProductImageFileWriteRepository, ProductImageFileWriteRepository>();
            services.AddScoped<IInvoiceFileReadRepository , InvoiceFileReadRepository>();
            services.AddScoped<IInvoiceFileWriteRepository,InvoiceFileWriteRepository>();

            services.AddScoped<IBasketItemReadRepository, BasketItemReadRepository>();
            services.AddScoped<IBasketItemWriteRepository, BasketItemWriteRepository>();  
            services.AddScoped<IBasketReadRepository, BasketReadRepository>();
            services.AddScoped<IBasketWriteRepository, BasketWriteRepository>();
            services.AddScoped<ICompletedOrderWriteRepository, CompletedOrderWriteRepository>();
            services.AddScoped<ICompletedOrderReadRepository, CompletedOrderReadRepository>();
           
            services.AddScoped<IEndPointReadRepository, EndPointReadRepository>();
            services.AddScoped<IEndPointWriteRepository, EndPointWriteRepository>();
            services.AddScoped<IMenuReadRepository, MenuReadRepository>();
            services.AddScoped<IMenuWriteRepository, MenuWriteRepository>();




            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IInternalAuthentication,AuthService>();
            services.AddScoped<IExternalAuthentication,AuthService>();
            services.AddScoped<IBasketService,BasketService>();
            services.AddScoped<IOrderService,OrderService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthorizationEndPointService, AuthorizationEndPointService>();
            services.AddScoped<IProductService, ProductService>();

        }
            
    }
}
