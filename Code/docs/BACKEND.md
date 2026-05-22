# Backend Guide

Tài liệu hướng dẫn cài đặt và chạy Backend.

---

# 1. Vị trí thư mục làm việc

Các lệnh bên dưới được chạy trong:

```txt
INVEST-APP/
└── Code/
    └── backend/
        └── Invest.Api/
```

Di chuyển:

```bash
cd Code/backend/Invest.Api
```

---

# 2. Restore package

```bash
dotnet restore
dotnet build
```

---

# 3. Setup Database

### Cách 1 — Docker (Khuyến nghị)

Khởi động SQL Server:

```bash
cd Code/database
docker compose up -d
```

Connection String:

```json
"ConnectionStrings": {
  "DefaultConnection":
  "Server=localhost,1433;
   Database=InvestDB;
   User Id=sa;
   Password=Invest@123456;
   TrustServerCertificate=True;"
}
```

---

### Cách 2 — Cài SQL Server riêng

Tạo database:

```txt
InvestDB
```

Connection String:

```json
"ConnectionStrings": {
  "DefaultConnection":
  "Server=localhost;
   Database=InvestDB;
   Trusted_Connection=True;
   TrustServerCertificate=True;"
}
```

Hoặc:

```json
"ConnectionStrings": {
  "DefaultConnection":
  "Server=localhost;
   Database=InvestDB;
   User Id=sa;
   Password=your_password;
   TrustServerCertificate=True;"
}
```

---

# 4. Migration Database

Quay lại thư mục backend:

```bash
cd Code/backend/Invest.Api
```

Áp dụng migration:

```bash
dotnet ef database update
```

Kiểm tra:

```bash
dotnet ef migrations list
```

---

# 5. Chạy Backend

Run:

```bash
dotnet run
```

Hot reload:

```bash
dotnet watch run
```

Swagger:

```txt
https://localhost:5108/swagger
```

---

# Chạy nhanh (Khuyến nghị)

Dùng Docker:

```bash
cd Code/database
docker compose up -d

cd ../backend/Invest.Api
dotnet restore
dotnet ef database update
dotnet watch run
```