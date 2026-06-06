using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(StockExchangeDbContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        // Example test user
        const string username = "testuser";
        const string email = "testuser@example.com";
        const string password = "Password123"; // plaintext for seeding only

        var exists = await context.Users.AnyAsync(u => u.Username == username || u.Email == email);
        if (exists) return;

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}

