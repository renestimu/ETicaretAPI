using ETicaretAPI.Application.Consts;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Enums;
using ETicaretAPI.Application.Features.Commands.Order.CompleteOrder;
using ETicaretAPI.Application.Features.Commands.Order.CreateOrder;
using ETicaretAPI.Application.Features.Queries.Order.GetAllOrders;
using ETicaretAPI.Application.Features.Queries.Order.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class OrdersController : ControllerBase
    {
        readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Orders, ActionType = ActionType.Reading, Definition = "GetAllOrders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrderQueryRequest request)
        {
            GetAllOrderQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }
        [HttpGet("{id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Orders, ActionType = ActionType.Reading, Definition = "GetOrderById")]
        public async Task<IActionResult> GetOrderById([FromRoute] GetOrderByIdQueryRequest request)
        {
            GetOrderByIdQueryResponse response = await _mediator.Send(request);
            return Ok(response);
        }



        [HttpPost]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Orders, ActionType = ActionType.Writing, Definition = "CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderCommandRequest request)
        {
            CreateOrderCommandResponse response = await _mediator.Send(request);
            return Ok(response);
        }
        [HttpGet("complete-order/{Id}")]
        [AuthorizeDefinition(Menu = AuthorizeDefinationConstans.Orders, ActionType = ActionType.Updating, Definition = "CompleteOrder")]
        public async Task<ActionResult> CompleteOrder([FromRoute]CompleteOrderCommandRequest completeOrderCommandRequest)
        {
            CompleteOrderCommandResponse completeOrderCommandResponse =await _mediator.Send(completeOrderCommandRequest);
            return Ok(completeOrderCommandResponse);
        }
    }
}
