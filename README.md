# INVEST APP
---

## Cấu trúc thư mục

```txt
INVEST-APP/
│
├── Code/
│   ├── backend/         # ASP.NET Backend API
│   ├── database/        # Docker SQL Server / script DB
│   ├── desktop/         # Desktop application
│   └── docs/            # Tài liệu kỹ thuật
│       └── BACKEND.md
│
├── DOCX/                # Báo cáo Word
├── PPTX/                # Slide thuyết trình
└── README.md
```

---

## Clone project

Clone repository:

```bash
git clone https://github.com/nqkha06/invest-app
```

Di chuyển:

```bash
cd INVEST-APP
```

---

Xem tài liệu backend:

```txt
Code/docs/BACKEND.md
```

Xem tài liệu desktop:
...
---

## Tài liệu

| Tài liệu | Mô tả |
|---------|------|
| docs/BACKEND.md | Hướng dẫn chạy backend |
| DOCX/ | Báo cáo đồ án |
| PPTX/ | Slide thuyết trình |

---

## Công nghệ sử dụng

Backend:

- ASP.NET Core
- Socket
- Redis

Desktop:

- Winform(C#)

Database:

- SQL Server
- Docker Compose

---

## Quy tắc commit

Ví dụ:

```txt
feat(auth): implement JWT login
fix(api): resolve CORS issue
refactor(database): optimize query
docs: update backend guide
chore: bootstrap project
```

---