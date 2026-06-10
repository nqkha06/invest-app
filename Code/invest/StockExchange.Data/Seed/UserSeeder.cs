using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(StockExchangeDbContext context)
    {
        const string defaultPassword = "123456";
        var defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
        var existingUsers = await context.Users
            .Where(user =>
                user.Username == "admin"
                || user.Username == "demo_user"
                || user.Email == "admin@investapp.local"
                || user.Email == "user@investapp.local")
            .ToListAsync();

        var users = new List<User>
        {
            new()
            {
                Username = "admin",
                Email = "admin@investapp.local",
                PasswordHash = defaultPasswordHash,
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Username = "demo_user",
                Email = "user@investapp.local",
                PasswordHash = defaultPasswordHash,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        foreach (var user in users)
        {
            var existingUser = existingUsers.FirstOrDefault(existing =>
                string.Equals(existing.Username, user.Username, StringComparison.OrdinalIgnoreCase)
                || string.Equals(existing.Email, user.Email, StringComparison.OrdinalIgnoreCase));

            if (existingUser is null)
            {
                await context.Users.AddAsync(user);
                continue;
            }

            // Upgrade databases created by the old seeder, which stored this demo password as plain text.
            if (existingUser.PasswordHash == defaultPassword)
            {
                existingUser.PasswordHash = defaultPasswordHash;
                existingUser.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
