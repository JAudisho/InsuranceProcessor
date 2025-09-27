# Insurance Transaction Processor

Minimal claims intake and validation API built with .NET 8, EF Core, and SQL Server. Includes SQL indexes, views, and a paged search stored procedure.

## Stack
.NET 8, ASP.NET Core, EF Core, SQL Server, Swagger

## Prerequisites
- .NET 8 SDK
- Docker Desktop (for SQL Server)

## Run
1. Start SQL Server  
   `docker compose -f docker/docker-compose.yml up -d`
2. Configure connection string  
   Set `ConnectionStrings:Sql` via User Secrets or `appsettings.json`.
3. Create database  
   `dotnet ef database update --project Insurance.Infrastructure --startup-project Insurance.Api`
4. Run API  
   `dotnet run --project Insurance.Api`  
   Open Swagger at the printed URL.

## SQL pack
Run in SSMS or sqlcmd in this order:
1) `sql/01_indexes.sql`  
2) `sql/02_views.sql`  
3) `sql/03_proc_search.sql`  
4) `sql/04_seed.sql` (optional demo data)  
5) `sql/05_perf_check.sql` (shows IO and time)

## Quick test (curl)
Seed a policy:
```bash
curl -X POST http://localhost:5000/api/policies/seed
