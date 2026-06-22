using System;
using System.Drawing;
using System.Windows.Forms;
using StockExchange.Shared.DTOs;

namespace StockExchange.Client.WinForms.Controls
{
    public partial class StockTableControl : UserControl
    {
        // Thêm dấu ? để báo cho trình biên dịch biết các biến này có thể null ban đầu
        private Label? lblStockCode;
        private Label? lblCompanyName;
        private Label? lblCurrentPrice;
        private Label? lblPercentChange;
        private Label? lblTotalVolume;

        public StockTableControl()
        {
            // Khởi tạo trực tiếp tại Constructor để chặn đứng lỗi Nullable của .NET 8
            lblStockCode = new Label();
            lblCompanyName = new Label();
            lblCurrentPrice = new Label();
            lblPercentChange = new Label();
            lblTotalVolume = new Label();

            InitControlLayout();
        }

        /// <summary>
        /// Hàm cấu hình vị trí, kích thước và font chữ cho các thành phần giao diện
        /// </summary>
        private void InitControlLayout()
        {
            SuspendLayout();

            // 1. Cấu hình Mã cổ phiếu
            lblStockCode!.Location = new Point(20, 20);
            lblStockCode.Text = "Mã: ---";
            lblStockCode.Font = new Font("Arial", 12, FontStyle.Bold);
            lblStockCode.AutoSize = true;

            // 2. Cấu hình Tên công ty
            lblCompanyName!.Location = new Point(20, 50);
            lblCompanyName.Text = "Tên công ty: ---";
            lblCompanyName.Size = new Size(300, 20);

            // 3. Cấu hình Giá hiện tại
            lblCurrentPrice!.Location = new Point(20, 80);
            lblCurrentPrice.Text = "Giá hiện tại: ---";
            lblCurrentPrice.AutoSize = true;

            // 4. Cấu hình Phần trăm thay đổi
            lblPercentChange!.Location = new Point(150, 80);
            lblPercentChange.Text = "0.00%";
            lblPercentChange.AutoSize = true;

            // 5. Cấu hình Khối lượng
            lblTotalVolume!.Location = new Point(20, 110);
            lblTotalVolume.Text = "Khối lượng: ---";
            lblTotalVolume.AutoSize = true;

            // Add các control vào UserControl
            Controls.Add(lblStockCode);
            Controls.Add(lblCompanyName);
            Controls.Add(lblCurrentPrice);
            Controls.Add(lblPercentChange);
            Controls.Add(lblTotalVolume);

            Name = "StockTableControl";
            Size = new Size(400, 200);
            
            ResumeLayout(false);
        }

        /// <summary>
        /// Hàm nhận dữ liệu từ Service đổ về để cập nhật giao diện trực quan
        /// </summary>
        public void BindingStockDetailData(StockResponseDto stock)
        {
            if (stock == null) return;

            if (InvokeRequired)
            {
                Invoke(new Action(() => BindingStockDetailData(stock)));
                return;
            }

            // Dùng dấu ! để khẳng định biến chắc chắn không null khi chạy hàm này
            lblStockCode!.Text = $"Mã: {stock.StockCode}";
            lblCompanyName!.Text = $"Tên công ty: {stock.CompanyName}";
            lblCurrentPrice!.Text = $"Giá hiện tại: {stock.CurrentPrice:N2}";
            lblTotalVolume!.Text = $"Khối lượng: {stock.TotalVolume:N0}";

            if (stock.PercentChange >= 0)
            {
                lblPercentChange!.Text = $"+{stock.PercentChange}%";
                lblPercentChange.ForeColor = Color.Green;
            }
            else
            {
                lblPercentChange!.Text = $"{stock.PercentChange}%";
                lblPercentChange.ForeColor = Color.Red;
            }
        }
    }
}