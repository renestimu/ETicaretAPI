
using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;

using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        readonly private IProductWriteRepository _productWriteRepository;

        readonly private IProductReadRepository _productReadRepository;
        readonly private IWebHostEnvironment _webHostEnvironment;

        readonly private IFileWriteRepository _fileWriteRepository;
        readonly private IFileReadRepository _fileReadRepository;
        readonly private IProductImageFileWriteRepository _productImageFileWriteRepository;
        readonly private IProductImageFileReadRepository _productImageFileReadRepository;
        readonly private IInvoiceFileReadRepository _invoiceFileReadRepository;
        readonly private IInvoiceFileWriteRepository _invoiceFileWriteRepository;
        readonly private IStorageService _storageService;
        readonly private IConfiguration _configuration;

        public ProductsController(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IWebHostEnvironment webHostEnvironment, IFileWriteRepository fileWriteRepository, IFileReadRepository fileReadRepository, IProductImageFileWriteRepository productImageFileWriteRepository, IProductImageFileReadRepository productImageFileReadRepository, IInvoiceFileReadRepository invoiceFileReadRepository, IInvoiceFileWriteRepository invoiceFileWriteRepository, IStorageService storageService, IConfiguration configuration)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _webHostEnvironment = webHostEnvironment;

            _fileWriteRepository = fileWriteRepository;
            _fileReadRepository = fileReadRepository;
            _productImageFileWriteRepository = productImageFileWriteRepository;
            _productImageFileReadRepository = productImageFileReadRepository;
            _invoiceFileReadRepository = invoiceFileReadRepository;
            _invoiceFileWriteRepository = invoiceFileWriteRepository;
            _storageService = storageService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] Pagination pagination)
        {
            var totalCount = _productReadRepository.GetAll(false).Count();

            var products = _productReadRepository.GetAll(false).Select(x => new
            {
                x.Id,
                x.Name,
                x.Stock,
                x.Price,
                x.CreateDate,
                x.UpdateDate
            }).Skip(pagination.Size * pagination.Page).Take(pagination.Size);

            return Ok(new
            {
                products,
                totalCount
            });
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {

            return Ok(await _productReadRepository.GetByIdAsync(id, false));
        }
        [HttpPost]
        public async Task<IActionResult> Post(VM_Create_Product model)
        {
            await _productWriteRepository.AddAsync(new()
            {
                Name = model.Name,
                Price = model.Price,
                Stock = model.Stock
            });
            await _productWriteRepository.SaveAsync();
            return Ok();
        }
        [HttpPut]
        public async Task<ActionResult> Put(VM_Update_Product model)
        {
            Product product = await _productReadRepository.GetByIdAsync(model.Id);
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            await _productWriteRepository.SaveAsync();

            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _productWriteRepository.RemoveAsync(id);
            await _productWriteRepository.SaveAsync();

            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Upload(string id)
        {
            //var datas = await _fileService.UploadAsync("resource/Product-images", Request.Form.Files);
            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.path
            //}).ToList()) ;
            //await _productImageFileWriteRepository.SaveAsync();

            //    var datas = await _storageService.UploadAsync("resource/Product-images", Request.Form.Files);//local için
            //var datas = await _storageService.UploadAsync("files", Request.Form.Files);
            //await _productImageFileWriteRepository.AddRangeAsync(datas.Select(d => new ProductImageFile()
            //{
            //    FileName = d.fileName,
            //    Path = d.path,
            //    Storage=_storageService.StorageName
            //}).ToList());
            //await _productImageFileWriteRepository.SaveAsync();



            List<(string fileName, string pathOrContainerName)> result = await _storageService.UploadAsync("photo-images", Request.Form.Files);

            Product product=await _productReadRepository.GetByIdAsync(id);
            
            
            await _productImageFileWriteRepository.AddRangeAsync(result.Select(d => new ProductImageFile()
            {
                FileName = d.fileName,
                Path = d.pathOrContainerName,
                Storage = _storageService.StorageName,
                Products=new List<Product>() { product}
            }).ToList());
            await _productImageFileWriteRepository.SaveAsync();

            return Ok();

        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetImages(string id)
        {
            Product? product= await _productReadRepository.Table.Include(p => p.ProductImageFiles)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));




            return Ok(product.ProductImageFiles.Select(x => new
            {
                Path = $"{_configuration["BaseStorageUrl"]}/{x.Path}",
                FileName= x.FileName,
                Id=x.Id

            }));
        }
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> DeleteProductImage(string id,string imageId)
        {
            Product? product = await _productReadRepository.Table.Include(p => p.ProductImageFiles)
               .FirstOrDefaultAsync(p => p.Id == Guid.Parse(id));

            ProductImageFile productImageFile= product.ProductImageFiles.FirstOrDefault(p => p.Id == Guid.Parse(imageId));

            product.ProductImageFiles.Remove(productImageFile);
            await _productWriteRepository.SaveAsync(); ;

            return Ok  ();
        }
    
    
    }
}
