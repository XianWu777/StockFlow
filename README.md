# StockFlow

## Start PostgreSQL
docker compose up -d

## Create migration
dotnet ef migrations add InitialCreate \
  --project src/StockFlow.Infrastructure \
  --startup-project src/StockFlow.Api

## Update database
dotnet ef database update \
  --project src/StockFlow.Infrastructure \
  --startup-project src/StockFlow.Api

## Run API
dotnet run --project src/StockFlow.Api