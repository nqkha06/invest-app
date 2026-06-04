# Stock Exchange App

Ứng dụng mẫu gồm 4 project:

- `StockExchange.Server`: app console server.
- `StockExchange.Client.WinForms`: app giao diện WinForms.
- `StockExchange.Data`: thư viện dữ liệu.
- `StockExchange.Shared`: thư viện dùng chung.

## Yêu cầu

- Windows
- .NET SDK 10.0 hoặc mới hơn

Kiểm tra SDK đã cài:

```powershell
dotnet --list-sdks
```

## Cài đặt

Mở PowerShell tại thư mục solution:

```powershell
cd C:\Users\KHA\Documents\lap-trinh-mang\invest-app\code\invest
```

Khôi phục package:

```powershell
dotnet restore
```

Build toàn bộ solution:

```powershell
dotnet build
```

## Database

Migration:

```powershell
dotnet ef database update --project .\StockExchange.Data\StockExchange.Data.csproj --startup-project .\StockExchange.Server\StockExchange.Server.csproj
```

## Chạy ứng dụng

Chạy server:

```powershell
dotnet run --project .\StockExchange.Server\StockExchange.Server.csproj
```

Chạy WinForms client:

```powershell
dotnet run --project .\StockExchange.Client.WinForms\StockExchange.Client.WinForms.csproj
```

Hiện tại app mới có entry point cơ bản để kiểm tra project chạy được. Server sẽ hiện thông báo trong console, còn WinForms client sẽ mở một cửa sổ đơn giản.

## Ghi chú

Không chạy trực tiếp `dotnet run` trong thư mục solution nếu chưa chỉ rõ project, vì solution có nhiều project. Hãy dùng `--project` như các lệnh phía trên.
