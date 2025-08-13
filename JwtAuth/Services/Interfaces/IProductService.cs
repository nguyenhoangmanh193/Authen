using JwtAuth.Entities;
using JwtAuth.Models.ProductDTO;

namespace JwtAuth.Services.Interfaces
{
    public interface IProductService
    {
        Task<object> GetOrdersByUsernameAsync(string username);       // Restriction - Where
        Task<object> GetUserOrdersSummaryAsync();                     // Projection - Select
        Task<object> GetLatestOrdersAsync(int n);                     // Partitioning - Take
        Task<object> GetProductsInOrdersOrderedByPriceAsync();        // Ordering - OrderByDescending
        Task<object> GetTotalSalesGroupedByUserAsync();              // Grouping - GroupBy
        Task<object> GetDistinctOrderedProductIdsAsync();            // Set - Distinct
        Task<object> GetOrdersAsListAsync();                          // Conversion - ToList
        Task<object?> GetFirstOrderOfUserAsync(string username);      // Element - FirstOrDefault
        Task<bool> AllOrdersHaveItemsAsync();                         // Quantifier - All
        Task<decimal> GetTotalRevenueAsync();                         // Aggregate - Sum
        Task<object> GetOrderDetailsWithUserAndProductsAsync();       // Join / GroupJoin

    }
}
