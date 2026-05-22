using Invest.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Invest.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options
    ) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}