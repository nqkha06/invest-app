using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockExchange.Client.WinForms.Services;
using StockExchange.Client.WinForms.Controls;

namespace StockExchange.Client.WinForms.Forms
{
    public partial class MainForm : Form
    {
        // 1. Khai báo dịch vụ lấy dữ liệu Stock
        private readonly IStockClientService _stockClientService;

        // 2. Khai báo biến control hiển thị chi tiết cổ phiếu (Đây chính là chỗ thiếu làm ăn lỗi số 2 và 3!)
        private StockTableControl? stockTableControl1;

        public MainForm()
        {
            InitializeComponent(); 
            
            // Khởi tạo instance cho service
            _stockClientService = new StockClientService();

            // Nếu trên giao diện Designer của nhóm chưa tự sinh ra control này, mình tự tạo thủ công để chống sập app
            if (stockTableControl1 == null)
            {
                stockTableControl1 = new StockTableControl();
                // Ní có thể chỉnh vị trí hiển thị cho đẹp tùy thích trên Form
                stockTableControl1.Location = new System.Drawing.Point(250, 20); 
                this.Controls.Add(stockTableControl1);
            }
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Hàm xử lý sự kiện khi người dùng chọn (click) một mã cổ phiếu từ danh sách (Watchlist)
        /// </summary>
        public async Task OnStockTickerSelectedAsync(string stockCode)
        {
            if (string.IsNullOrWhiteSpace(stockCode)) return;

            try
            {
                // Bước 1: Gọi sang StockClientService bất đồng bộ để lấy gói dữ liệu từ Mock/Server
                var stockData = await _stockClientService.GetStockDetailAsync(stockCode);

                // Bước 2: Đẩy gói dữ liệu nhận được vào StockTableControl để cập nhật giao diện
                if (stockData != null && stockTableControl1 != null)
                {
                    stockTableControl1.BindingStockDetailData(stockData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi tải chi tiết mã {stockCode}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}