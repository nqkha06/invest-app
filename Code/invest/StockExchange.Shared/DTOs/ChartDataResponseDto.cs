using System;
using System.Collections.Generic;

namespace StockExchange.Shared.DTOs
{
    public class ChartDataResponseDto
    {
        public string? StockCode { get; set; }
        public string? TimeFrame { get; set; } // "1D", "1W", "1M", "1Y"...
        public List<ChartPointDto> DataPoints { get; set; } = new List<ChartPointDto>();
    }

    public class ChartPointDto
    {
        public DateTime Time { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
    }
}