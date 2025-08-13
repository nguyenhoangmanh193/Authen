using JwtAuth.Entities;
using JwtAuth.Models.ProductDTO;
using JwtAuth.Services.Implementations;
using JwtAuth.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }


        [HttpGet("orders-by-username/{username}")]
        public async Task<IActionResult> GetOrdersByUsername(string username)
            => Ok(await _service.GetOrdersByUsernameAsync(username));

        [HttpGet("user-orders-summary")]
        public async Task<IActionResult> GetUserOrdersSummary()
            => Ok(await _service.GetUserOrdersSummaryAsync());

        [HttpGet("latest-orders/{count}")]
        public async Task<IActionResult> GetLatestOrders(int count)
            => Ok(await _service.GetLatestOrdersAsync(count));

        [HttpGet("products-in-orders-ordered-by-price")]
        public async Task<IActionResult> GetProductsInOrdersOrderedByPrice()
            => Ok(await _service.GetProductsInOrdersOrderedByPriceAsync());

        [HttpGet("total-sales-grouped-by-user")]
        public async Task<IActionResult> GetTotalSalesGroupedByUser()
            => Ok(await _service.GetTotalSalesGroupedByUserAsync());

        [HttpGet("distinct-ordered-products")]
        public async Task<IActionResult> GetDistinctOrderedProductIds()
            => Ok(await _service.GetDistinctOrderedProductIdsAsync());

        [HttpGet("orders-list")]
        public async Task<IActionResult> GetOrdersAsList()
            => Ok(await _service.GetOrdersAsListAsync());

        [HttpGet("first-order-of-user/{username}")]
        public async Task<IActionResult> GetFirstOrderOfUser(string username)
            => Ok(await _service.GetFirstOrderOfUserAsync(username));

        [HttpGet("all-orders-have-items")]
        public async Task<IActionResult> AllOrdersHaveItems()
            => Ok(await _service.AllOrdersHaveItemsAsync());

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue()
            => Ok(await _service.GetTotalRevenueAsync());

        [HttpGet("order-details")]
        public async Task<IActionResult> GetOrderDetailsWithUserAndProducts()
            => Ok(await _service.GetOrderDetailsWithUserAndProductsAsync());


    }
}
