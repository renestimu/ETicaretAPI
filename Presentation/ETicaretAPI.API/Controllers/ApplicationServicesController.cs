using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]
    public class ApplicationServicesController : ControllerBase
    {
        readonly IApplicationService _applicationService;
        public ApplicationServicesController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }
        [HttpGet]
        [AuthorizeDefinition(Menu = "Application Services",ActionType = Application.Enums.ActionType.Reading,Definition = "GetAuthorizeDefinitionEndpoints")]
        public IActionResult GetAuthorizeDefinitionEndpoints()
        {
         
           var datas= _applicationService.GetAuthorizeDefinitionEndPoints(typeof(Program));
            return Ok(datas);
        }
    }
}