using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(StockExchangeDbContext context)
    {
        if (await context.Users.AnyAsync())
            return; // Đã có dữ liệu thì bỏ qua

        // Lưu ý: Trong thực tế hãy dùng thư viện băm mật khẩu như BCrypt. 
        // Đây là chuỗi hash giả định cho mật khẩu "123456" để phục vụ Demo.
        var defaultPasswordHash = "AQAAAAEAACcQAAAAE... (thay bằng hash thật nếu có hàm tạo hash, tạm dùng 1 chuỗi ngẫu nhiên hoặc mã hóa đơn giản)";

        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                Email = "admin@investapp.local",
                PasswordHash = "123456", // Hãy hash trước khi lưu ở production
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Username = "demo_user",
                Email = "user@investapp.local",
                PasswordHash = "123456", // Hãy hash trước khi lưu ở production
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);
    }
}