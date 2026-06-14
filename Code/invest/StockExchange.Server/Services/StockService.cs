using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockExchange.Server.Services;

public class StockService
{
    private readonly StockExchangeDbContext _context;

    public StockService(StockExchangeDbContext context)
    {
        _context = context;
    }

    // 1. Logic lấy toàn bộ danh sách cổ phiếu từ bảng Stocks
    public async Task<IEnumerable<Stock>> GetAllStocksAsync()
    {
        return await _context.Stocks
            .AsNoTracking()
            .OrderBy(stock => stock.Symbol)
            .ToListAsync();
    }

    // 2. Logic tìm kiếm cổ phiếu (Kiểm tra lại xem trong file Stock.cs thuộc tính tên là gì)
    // Nếu file Stock.cs dùng tên khác (ví dụ: Name thay vì CompanyName), bạn hãy sửa lại cho đúng nhé!
    public async Task<IEnumerable<Stock>> SearchStocksAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllStocksAsync();
        }

        var lowerKeyword = keyword.ToLower();

        return await _context.Stocks
            .Where(s => s.Symbol.ToLower().Contains(lowerKeyword) || 
                        s.CompanyName.ToLower().Contains(lowerKeyword)) 
            .ToListAsync();
    }

    public async Task<Stock> CreateStockAsync(
        long userId,
        StockUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        await RequireAdminAsync(userId, cancellationToken);
        var values = await ValidateAsync(request, null, cancellationToken);
        var stock = new Stock
        {
            Symbol = values.Symbol,
            CompanyName = values.CompanyName,
            Sector = values.Sector,
            CurrentPrice = values.CurrentPrice,
            IsActive = values.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync(cancellationToken);
        return stock;
    }

    public async Task<Stock> UpdateStockAsync(
        long userId,
        StockUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        await RequireAdminAsync(userId, cancellationToken);
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Stock was not found.");
        var values = await ValidateAsync(request, stock.Id, cancellationToken);

        stock.Symbol = values.Symbol;
        stock.CompanyName = values.CompanyName;
        stock.Sector = values.Sector;
        stock.CurrentPrice = values.CurrentPrice;
        stock.IsActive = values.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        return stock;
    }

    public async Task DeleteStockAsync(
        long userId,
        long stockId,
        CancellationToken cancellationToken = default)
    {
        await RequireAdminAsync(userId, cancellationToken);
        var stock = await _context.Stocks
            .FirstOrDefaultAsync(item => item.Id == stockId, cancellationToken)
            ?? throw new InvalidOperationException("Stock was not found.");

        _context.Stocks.Remove(stock);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task RequireAdminAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User account was not found.");
        if (!user.IsActive || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Administrator permission is required.");
        }
    }

    private async Task<StockUpdateDto> ValidateAsync(
        StockUpdateDto request,
        long? currentStockId,
        CancellationToken cancellationToken)
    {
        var symbol = request?.Symbol.Trim().ToUpperInvariant() ?? string.Empty;
        var companyName = request?.CompanyName.Trim() ?? string.Empty;
        var sector = request?.Sector?.Trim();

        if (symbol.Length is < 1 or > 20)
        {
            throw new InvalidOperationException("Stock symbol must contain between 1 and 20 characters.");
        }
        if (companyName.Length is < 1 or > 255)
        {
            throw new InvalidOperationException("Company name must contain between 1 and 255 characters.");
        }
        if (sector?.Length > 100)
        {
            throw new InvalidOperationException("Sector cannot exceed 100 characters.");
        }
        if (request!.CurrentPrice < 0)
        {
            throw new InvalidOperationException("Current price cannot be negative.");
        }

        var symbolExists = await _context.Stocks.AnyAsync(
            stock => stock.Symbol == symbol && stock.Id != currentStockId,
            cancellationToken);
        if (symbolExists)
        {
            throw new InvalidOperationException($"Stock symbol {symbol} already exists.");
        }

        return new StockUpdateDto
        {
            Id = request.Id,
            Symbol = symbol,
            CompanyName = companyName,
            Sector = string.IsNullOrWhiteSpace(sector) ? null : sector,
            CurrentPrice = request.CurrentPrice,
            IsActive = request.IsActive
        };
    }
}
