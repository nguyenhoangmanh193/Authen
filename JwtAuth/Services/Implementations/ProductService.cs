using JwtAuth.Data;
using JwtAuth.Entities;
using JwtAuth.Models.ProductDTO;
using JwtAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        // Restriction - Where: Lấy đơn hàng của user theo Username
        public async Task<object> GetOrdersByUsernameAsync(string username)
        {
            return await _context.Orders
                .Where(o => o.User.Username == username)
                .Select(o => new
                {
                    o.Id,
                    Username = o.User.Username,
                    Total = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .ToListAsync();
        }

        // Projection - Select: Thống kê số đơn và tổng tiền mỗi user
        public async Task<object> GetUserOrdersSummaryAsync()
        {
            return await _context.Users
                .Select(u => new
                {
                    u.Username,
                    OrderCount = _context.Orders.Count(o => o.UserId == u.Id),
                    TotalSpent = _context.Orders
                        .Where(o => o.UserId == u.Id)
                        .Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice))
                })
                .ToListAsync();
        }

        // Partitioning - Take: Lấy N đơn hàng mới nhất
        public async Task<object> GetLatestOrdersAsync(int n)
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(n)
                .Select(o => new
                {
                    o.Id,
                    Username = o.User.Username,
                    o.OrderDate
                })
                .ToListAsync();
        }

        // Ordering - OrderByDescending: Lấy sản phẩm trong đơn hàng, sắp xếp theo giá
        public async Task<object> GetProductsInOrdersOrderedByPriceAsync()
        {
            return await _context.OrderItems
                .OrderByDescending(oi => oi.UnitPrice)
                .Select(oi => new
                {
                    oi.Product.Name,
                    oi.UnitPrice,
                    oi.Quantity,
                    Username = oi.Order.User.Username
                })
                .ToListAsync();
        }

        // Grouping - GroupBy: Tổng doanh thu theo user
        public async Task<object> GetTotalSalesGroupedByUserAsync()
        {
            return await _context.Orders
                .GroupBy(o => o.User.Username)
                .Select(g => new
                {
                    Username = g.Key,
                    TotalSales = g.Sum(o => o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice))
                })
                .ToListAsync();
        }

        // Set - Distinct: Lấy danh sách sản phẩm đã được đặt duy nhất
        public async Task<object> GetDistinctOrderedProductIdsAsync()
        {
            return await _context.OrderItems
                .Select(oi => oi.ProductId)
                .Distinct()
                .ToListAsync();
        }

        // Conversion - ToList: Lấy danh sách đơn hàng
        public async Task<object> GetOrdersAsListAsync()
        {
            return await _context.Orders
                .Select(o => new
                {
                    o.Id,
                    Username = o.User.Username,
                    o.OrderDate
                })
                .ToListAsync();
        }

        // Element - FirstOrDefault: Lấy đơn hàng đầu tiên của user
        public async Task<object?> GetFirstOrderOfUserAsync(string username)
        {
            return await _context.Orders
                .Where(o => o.User.Username == username)
                .OrderBy(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate
                })
                .FirstOrDefaultAsync();
        }

        // Quantifier - All: Kiểm tra tất cả đơn hàng có ít nhất 1 item
        public async Task<bool> AllOrdersHaveItemsAsync()
        {
            return await _context.Orders.AllAsync(o => o.OrderItems.Any());
        }

        // Aggregate - Sum: Tổng doanh thu
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.OrderItems.SumAsync(oi => oi.Quantity * oi.UnitPrice);
        }

        // Join / GroupJoin: Lấy chi tiết đơn hàng kèm user và sản phẩm
        public async Task<object> GetOrderDetailsWithUserAndProductsAsync()
        {
            return await _context.Orders
                .Select(o => new
                {
                    o.Id,
                    Username = o.User.Username,
                    Items = o.OrderItems.Select(oi => new
                    {
                        ProductName = oi.Product.Name,
                        oi.Quantity,
                        oi.UnitPrice
                    })
                })
                .ToListAsync();
        }



    }
}
