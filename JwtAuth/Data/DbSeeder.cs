using JwtAuth.Data;
using JwtAuth.Entities; 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // 2. Products
        if (!context.Products.Any())
        {
            var products = new[]
            {
                    new Product { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001"), Name = "Laptop", Price = 1000, Stock = 10 },
                    new Product { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002"), Name = "Smartphone", Price = 500, Stock = 20 },
                    new Product { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003"), Name = "Headphones", Price = 100, Stock = 50 },
                    new Product { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"), Name = "Keyboard", Price = 50, Stock = 30 },
                    new Product { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005"), Name = "Mouse", Price = 25, Stock = 40 }
                };
            context.Products.AddRange(products);
            context.SaveChanges();
        }

        // 3. Carts
        if (!context.Carts.Any())
        {
            var carts = new[]
            {
                    new Cart { Id = Guid.NewGuid(), UserId = Guid.Parse("535F027C-5607-43DC-28A3-08DDD27F6A90") },
                    new Cart { Id = Guid.NewGuid(), UserId = Guid.Parse("48A1B737-CB3F-401D-5910-08DDD64D274D") },
                    new Cart { Id = Guid.NewGuid(), UserId = Guid.Parse("4BD4FB60-5FA4-412C-5911-08DDD64D274D") },
                    new Cart { Id = Guid.NewGuid(), UserId = Guid.Parse("535F027C-5607-43DC-28A3-08DDD27F6A90") },
                    new Cart { Id = Guid.NewGuid(), UserId = Guid.Parse("671B4995-862C-4501-5913-08DDD64D274D") }
                };
            context.Carts.AddRange(carts);
            context.SaveChanges();
        }

        // 4. CartItems
        if (!context.CartItems.Any())
        {
            var carts = context.Carts.ToArray(); 
            var products = context.Products.ToArray();

            var cartItems = new[]
            {
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[0].Id, ProductId = products[0].Id, Quantity = 1 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[0].Id, ProductId = products[1].Id, Quantity = 2 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[1].Id, ProductId = products[2].Id, Quantity = 1 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[2].Id, ProductId = products[3].Id, Quantity = 2 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[2].Id, ProductId = products[4].Id, Quantity = 1 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[3].Id, ProductId = products[0].Id, Quantity = 1 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[4].Id, ProductId = products[1].Id, Quantity = 1 },
                    new CartItem { Id = Guid.NewGuid(), CartId = carts[4].Id, ProductId = products[2].Id, Quantity = 2 }
                };
            context.CartItems.AddRange(cartItems);
            context.SaveChanges();
        }

        // 5. Orders
        if (!context.Orders.Any())
        {
            var orders = new[]
            {
                    new Order { Id = Guid.NewGuid(), UserId = Guid.Parse("535F027C-5607-43DC-28A3-08DDD27F6A90"), OrderDate = DateTime.Parse("2025-08-13 12:00"), Status = "Pending" },
                    new Order { Id = Guid.NewGuid(), UserId = Guid.Parse("48A1B737-CB3F-401D-5910-08DDD64D274D"), OrderDate = DateTime.Parse("2025-08-13 13:00"), Status = "Pending" },
                    new Order { Id = Guid.NewGuid(), UserId = Guid.Parse("4BD4FB60-5FA4-412C-5911-08DDD64D274D"), OrderDate = DateTime.Parse("2025-08-13 14:00"), Status = "Completed" },
                    new Order { Id = Guid.NewGuid(), UserId = Guid.Parse("535F027C-5607-43DC-28A3-08DDD27F6A90"), OrderDate = DateTime.Parse("2025-08-13 15:00"), Status = "Pending" },
                    new Order { Id = Guid.NewGuid(), UserId = Guid.Parse("671B4995-862C-4501-5913-08DDD64D274D"), OrderDate = DateTime.Parse("2025-08-13 16:00"), Status = "Pending" }
                };
            context.Orders.AddRange(orders);
            context.SaveChanges();

            // 6. OrderItems
            var products = context.Products.ToArray(); 
            var orderItems = new[]
            {
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[0].Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = products[0].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[0].Id, ProductId = products[1].Id, Quantity = 2, UnitPrice = products[1].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[1].Id, ProductId = products[2].Id, Quantity = 1, UnitPrice = products[2].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[2].Id, ProductId = products[3].Id, Quantity = 2, UnitPrice = products[3].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[2].Id, ProductId = products[4].Id, Quantity = 1, UnitPrice = products[4].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[3].Id, ProductId = products[0].Id, Quantity = 1, UnitPrice = products[0].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[4].Id, ProductId = products[1].Id, Quantity = 1, UnitPrice = products[1].Price },
                    new OrderItem { Id = Guid.NewGuid(), OrderId = orders[4].Id, ProductId = products[2].Id, Quantity = 2, UnitPrice = products[2].Price }
                };
            context.OrderItems.AddRange(orderItems);
            context.SaveChanges();
        }
    }
}

