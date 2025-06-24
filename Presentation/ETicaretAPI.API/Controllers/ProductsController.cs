
using ETicaretAPI.Application.Abstractions.Storage;
using ETicaretAPI.Application.Consts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.Product.CreateProduct;
using ETicaretAPI.Application.Features.Commands.Product.RemoveProduct;
using ETicaretAPI.Application.Features.Commands.Product.UpdateProduct;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.ChangeShowcaseImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.RemoveProductImage;
using ETicaretAPI.Application.Features.Commands.ProductImageFile.UploadProductImage;
using ETicaretAPI.Application.Features.Queries.GetAllProduct;
using ETicaretAPI.Application.Features.Queries.Product.GetByIdProduct;
using ETicaretAPI.Application.Features.Queries.ProductImageFile.GetProductImage;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.RequestParameters;

using ETicaretAPI.Application.ViewModels.Products;
using ETicaretAPI.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        readonly IMediator _mediator;

        public ProductsController( IMediator mediator)
        {            
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] GetAllProductQueryRequest getAllProductQueryRequest)
        {
            GetAllProductQueryResponse response = await _mediator.Send(getAllProductQueryRequest);

            return Ok(response);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult> Get([FromRoute] GetByIdProductQueryRequest request)
        {
            GetByIdProductQueryResponse response = await _mediator.Send(request);

            return Ok(response);

        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Writing, Definition = "CreateProduct")]
        public async Task<IActionResult> Post(CreateProductCommandRequest model)
        {
            await _mediator.Send(model);
            return StatusCode((int)(HttpStatusCode.Created));
        }
        [HttpPut]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Updating, Definition = "UpdateProduct")]
        public async Task<ActionResult> Put([FromBody] UpdateProductCommandRequest model)
        {
            await _mediator.Send(model);

            return Ok();
        }
        [HttpDelete("{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Deleting, Definition = "RemoveProduct")]
        public async Task<IActionResult> Delete([FromRoute] RemoveProductCommandRequest request)
        {
            await _mediator.Send(request);


            return Ok();
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Writing, Definition = "UploadProductImage")]
        public async Task<IActionResult> Upload([FromQuery] UploadProductImageCommandRequest request)
        {
            request.Files = Request.Form.Files;
            await _mediator.Send(request);
            return Ok();

        }

        [HttpGet("[action]/{id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Reading, Definition = "GetProductImages")]
        public async Task<IActionResult> GetImages([FromRoute] GetProductImageQueryRequest request)
        {
            List< GetProductImageQueryResponse> response = await _mediator.Send(request);

            return Ok(response);
        }
        [HttpDelete("[action]/{Id}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Deleting, Definition = "RemoveProductImage")]
        public async Task<IActionResult> DeleteProductImage([FromRoute] RemoveProductImageCommandRequest request, string imageId)
        {
            request.ImageId = imageId;
            await _mediator.Send(request);

            return Ok();
        }
        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Products, ActionType = ActionType.Updating, Definition = "ChangeShowcaseImage")]
        public async Task<IActionResult> ChangeShowcaseImage([FromQuery]ChangeShowcaseImageCommandRequest changeShowcaseImageCommandRequest)
        {
           ChangeShowcaseImageCommandResponse response=    await _mediator.Send(changeShowcaseImageCommandRequest);
            return Ok(response);
        }


    }
}
