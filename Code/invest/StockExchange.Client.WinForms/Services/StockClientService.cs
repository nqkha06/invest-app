using System;
using System.Threading.Tasks;
using StockExchange.Shared.DTOs;

namespace StockExchange.Client.WinForms.Services
{
    /// <summary>
    /// Interface định nghĩa các chức năng lấy dữ liệu cổ phiếu ở Client.
    /// Nhóm trưởng hoặc thành viên khác dựa vào đây để cấu hình kết nối thực tế.
    /// </summary>
    public interface IStockClientService
    {
        // Hàm lấy thông tin chi tiết của một mã cổ phiếu được chọn
        Task<StockResponseDto?> GetStockDetailAsync(string stockCode);
    }

    /// <summary>
    /// Lớp triển khai dịch vụ lấy dữ liệu cổ phiếu cho Client.
    /// </summary>
    public class StockClientService : IStockClientService
    {
        public async Task<StockResponseDto?> GetStockDetailAsync(string stockCode)
        {
            if (string.IsNullOrWhiteSpace(stockCode))
                return null;

            try
            {
                // TODO: Chờ nhóm trưởng cấu hình luồng kết nối mạng thực tế (Socket/HTTP) ở đây.
                // Hiện tại trả về dữ liệu Mock tạm thời để tầng UI (MainForm/Controls) có thể chạy thử nghiệm.
                
                await Task.Delay(100); // Giả lập độ trễ mạng nhỏ

                return new StockResponseDto
                {
                    StockCode = stockCode.ToUpper(),
                    CompanyName = stockCode.ToUpper() == "FPT" ? "FPT Corporation" : $"{stockCode.ToUpper()} Joint Stock Company",
                    CurrentPrice = 115.50m,
                    PriceChange = 2.64m,
                    PercentChange = 2.34m,
                    TotalVolume = 2840000,
                    OpenPrice = 113.19m,
                    HighestPrice = 118.97m,
                    LowestPrice = 110.88m
                };
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu luồng truyền tải dữ liệu từ Server gặp sự cố
                Console.WriteLine($"Lỗi khi lấy dữ liệu Stock {stockCode}: {ex.Message}");
                return null;
            }
        }
    }
}