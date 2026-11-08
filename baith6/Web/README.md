# web-vue

SPA application built with Vue.js and Bootstrap for Vue.

## Run locally without Docker

The local development configuration uses SQL Server Express (`.\SQLEXPRESS`) for
products and policies. Dashboard and policy search read policies from the same
SQL Server database. RabbitMQ, Elasticsearch, and Eureka are not required for
the local UI workflows.

1. Create the local databases once, using SQL Server Management Studio or:

   ```powershell
   sqlcmd -S .\SQLEXPRESS -E -i .\sql\setup-local.sql
   ```

2. From the `baith6` directory, start each service in its own terminal:

   ```powershell
   dotnet run --project PricingService.csproj
   dotnet run --project ProductService\ProductService.csproj
   dotnet run --project PolicyService\PolicyService.csproj
   dotnet run --project PolicySearchService\PolicySearchService.csproj
   dotnet run --project DashboardService\DashboardService.csproj
   dotnet run --project AuthService\AuthService.csproj
   dotnet run --project ChatService\ChatService.csproj
   dotnet run --project ApiGateway\ApiGateway.csproj
   ```

3. In `baith6\Web`, install the JavaScript dependencies and start Vue:

   ```powershell
   npm install
   npm run serve
   ```

   Open the local URL printed by Vue CLI (normally `http://localhost:8080`).

Product tables and demo products are created on the first ProductService run.
Policy tables are created by NHibernate when PolicyService first connects. The
dashboard will show sales after policies have been created. Realtime chat works
without RabbitMQ; policy-created notifications delivered through RabbitMQ are
not available in this local mode.

## Prerequisites

Node.js/npm and a running SQL Server Express instance are required. No `.env`
file or Docker installation is needed for local development.

## Project setup
```
yarn install
```

### Compiles and hot-reloads for development
```
yarn run serve
```

### Compiles and minifies for production
```
yarn run build
```

### Lints and fixes files
```
yarn run lint
```
