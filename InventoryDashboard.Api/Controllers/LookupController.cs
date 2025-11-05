namespace InventoryDashboard.Api.Controllers
{
    using InventoryDashboard.Api.Common.Extensions;
    using InventoryDashboard.Api.Extensions.Models.Product;
    using InventoryDashboard.Api.Messages.Product;
    using InventoryDashboard.Infrastructure.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("api/lookups")]
    [ApiController]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly IProductService productService;

        public LookupController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet("products")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductLookupWebResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(GetProductLookupWebResponse))]
        public async Task<IActionResult> GetProductLookupAsync()
        {
            var result = await this.productService.GetLookupsAsync();
            return this.CreateResponse(result.AsGetProductLookupWebResponse());
        }
    }
}