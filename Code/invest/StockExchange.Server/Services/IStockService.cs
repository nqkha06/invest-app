using StockExchange.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockExchange.Server.Services;

public interface IStockService
{
    // Hàm lấy toàn bộ danh sách cổ phiếu chuẩn thực thể Stock của nhóm
    Task<IEnumerable<Stock>> GetAllStocksAsync();

    // Hàm tìm kiếm cổ phiếu chuẩn thực thể Stock của nhóm
    Task<IEnumerable<Stock>> SearchStocksAsync(string keyword);
}