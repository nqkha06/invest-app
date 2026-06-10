using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
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
        return await _context.Stocks.ToListAsync();
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
}