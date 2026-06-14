namespace StockExchange.Shared.DTOs
{
    public class StockResponseDto
    {
        public string? StockCode { get; set; }
        public string? CompanyName { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PercentChange { get; set; }
        
        // Thông tin tóm tắt giao dịch bên dưới biểu đồ
        public long TotalVolume { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal LowestPrice { get; set; }
    }
}