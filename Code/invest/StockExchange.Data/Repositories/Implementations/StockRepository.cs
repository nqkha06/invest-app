using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.Models;

namespace StockExchange.Data.Repositories.Implementations;

public class StockRepository : IStockRepository
{
    private readonly StockExchangeDbContext _context;

    public StockRepository(StockExchangeDbContext context)
    {
        _context = context;
    }

    public Task<Stock?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _context.Stocks
            .AsNoTracking()
            .FirstOrDefaultAsync(stock => stock.Id == id, cancellationToken);
    }

    public Task<Stock?> GetBySymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        return _context.Stocks
            .AsNoTracking()
            .FirstOrDefaultAsync(stock => stock.Symbol == symbol, cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stocks
            .AsNoTracking()
            .OrderBy(stock => stock.Symbol)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Stocks
            .AsNoTracking()
            .Where(stock => stock.IsActive)
            .OrderBy(stock => stock.Symbol)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> SearchAsync(string keyword, int limit = 50, CancellationToken cancellationToken = default)
    {
        var normalizedKeyword = keyword.Trim();
        var query = _context.Stocks.AsNoTracking().Where(stock => stock.IsActive);

        if (!string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            query = query.Where(stock =>
                stock.Symbol.Contains(normalizedKeyword) ||
                stock.CompanyName.Contains(normalizedKeyword) ||
                (stock.Sector != null && stock.Sector.Contains(normalizedKeyword)));
        }

        return await query
            .OrderBy(stock => stock.Symbol)
            .Take(Math.Max(1, limit))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Stock>> GetBySectorAsync(string sector, CancellationToken cancellationToken = default)
    {
        return await _context.Stocks
            .AsNoTracking()
            .Where(stock => stock.IsActive && stock.Sector == sector)
            .OrderBy(stock => stock.Symbol)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Stock stock, CancellationToken cancellationToken = default)
    {
        return _context.Stocks.AddAsync(stock, cancellationToken).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<Stock> stocks, CancellationToken cancellationToken = default)
    {
        return _context.Stocks.AddRangeAsync(stocks, cancellationToken);
    }

    public void Update(Stock stock)
    {
        _context.Stocks.Update(stock);
    }

    public void Remove(Stock stock)
    {
        _context.Stocks.Remove(stock);
    }
}
