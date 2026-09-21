# StockFlow

這是一個使用 .NET 10 和 PostgreSQL 建構的小型庫存管理後端。

## Tech Stack

- .NET 10 / ASP.NET Core
- EF Core
- PostgreSQL
- JWT
- xUnit / Moq
- Docker
- Render
- Swagger

## Architecture

API
 ↓
Application
 ↓
Infrastructure
 ↓
EF Core
 ↓
PostgreSQL

## Features

- Product CRUD
- Inventory Stock In / Stock Out
- Inventory Movement
- JWT Authentication / Authorization
- Pagination / Search / Sorting
- Global Exception Handling
- Health Check

## Concurrency

Stock Out uses a database-level atomic update:

UPDATE ... WHERE Quantity >= @quantity

可以防止在並發請求下庫存變為負數

測試結果:

- Initial inventory: 50
- Concurrent requests: 100
- Success: 50
- Conflict: 50
- Server errors: 0
- Final inventory: 0

## Testing

- Unit Tests with xUnit / Moq
- Integration Tests with WebApplicationFactory
- PostgreSQL integration testing
- Concurrent Stock Out testing

## Deployment

該 API 部署在 Render 上，並使用 PostgreSQL

Swagger:
https://stockflow-api-u5t5.onrender.com/Swagger/index.html

Health Check:
https://stockflow-api-u5t5.onrender.com/health

## What I Learned

這個專案是我從 Unity 前端開發轉型為 .NET 後端開發過程的一部分

主要用來理解：

- REST API design
- Database transactions
- Concurrency control
- Authentication / Authorization
- Integration testing
- Docker
- Cloud deployment
