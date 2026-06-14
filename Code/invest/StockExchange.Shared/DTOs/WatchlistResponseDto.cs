using System.Collections.Generic;

namespace StockExchange.Shared.DTOs
{
    public class WatchlistResponseDto
    {
        public int WatchlistId { get; set; }
        public string? WatchlistName { get; set; }
        
        // Danh sách các cổ phiếu nằm trong danh mục theo dõi này
        public List<StockWatchlistSummaryDto>? SampleStocks { get; set; }
    }

    public class StockWatchlistSummaryDto
    {
        public string? StockCode { get; set; }      // Mã cổ phiếu (FPT, VNM...)
        public decimal CurrentPrice { get; set; }   // Giá hiện tại
        public decimal PercentChange { get; set; }  // Phần trăm tăng giảm để hiện màu xanh/đỏ
    }
}