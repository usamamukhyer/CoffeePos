# Coffee Ordering App - .NET MAUI Prototype

## Overview

This solution contains a .NET MAUI coffee ordering mobile app plus a layered ASP.NET Core API aligned to the provided `Cafe.sql` SQL Server schema.

## How to Run

```powershell
dotnet restore .\DBCafeteria.sln
dotnet build .\DBCafeteria.sln -f net10.0-windows10.0.19041.0
```

For mobile targets, open `DBCafeteria.sln` in Visual Studio with the MAUI workload installed, select an Android emulator or paired iOS device, then run the `DBCafeteria` project.

For the API:

```powershell
dotnet run --project .\src\DBCafeteria.Api\DBCafeteria.Api.csproj
```

Update `src/DBCafeteria.Api/appsettings.json` with your SQL Server connection string before using live database endpoints.

## Screen Flow

```text
Auth Landing
-> Login / Signup / Continue as Guest
-> Order Setup
   -> For Me -> Shop by Category -> Our Menu -> Customize Your Coffee -> My Basket -> Order Success
   -> Send Gift -> Gift Details -> Shop by Category -> Our Menu -> Customize Your Coffee -> My Basket -> Order Success
```

Bottom navigation is shown on the main app screens:

- Order -> Order Setup
- Menu -> Shop by Category
- Cart -> My Basket
- Profile -> Profile placeholder

## Project Structure

- `Views/` - XAML pages for each screen
- `ViewModels/` - screen state and commands
- `Models/` - dummy domain models
- `Services/` - dummy data and order session state
- `Controls/` - reusable UI controls such as `GoldButton`, `SelectionCard`, `BottomNavigationBar`, `ProductCard`, `QuantityStepper`, and `SectionTitle`
- `Resources/Styles/` - theme colors and shared styles
- `Resources/Images/` - placeholder coffee and category images
- `Images/` - original reference screenshots supplied for design matching
- `src/DBCafeteria.Api/` - ASP.NET Core API controllers and host
- `src/DBCafeteria.Application/` - DTOs and service interfaces
- `src/DBCafeteria.Domain/` - entities mapped to `Cafe.sql`
- `src/DBCafeteria.Infrastructure/` - EF Core DbContext, SQL Server mappings, auth/order/menu services
- `Database/Cafe.RequiredExtensions.sql` - optional refresh-token table extension for production auth

## Dummy Data

Dummy categories and products live in `Services/DummyDataService.cs`.

Auth state, guest mode, customer details, order state, selected location, gift details, selected product, and basket items live in `Services/OrderSessionService.cs`.

The MAUI app also includes `Services/CafeApiClient.cs` and API request/response contracts so screens can be moved from dummy data to API calls incrementally.

## API Endpoints

- `POST /auth/login`
- `POST /auth/signup`
- `POST /auth/guest`
- `GET /categories`
- `GET /products?categoryId=1`
- `GET /menu/milks`
- `GET /menu/beans`
- `GET /menu/cups`
- `GET /menu/toppings`
- `POST /cart`
- `POST /payment`
- `POST /orders/create`
- `POST /gifts/create`
- `GET /gifts/{id}`

## Database

Run `Cafe.sql` against SQL Server to create `dbcafeteria`. The API maps to the existing tables, including:

- `Clientes`
- `Pedidos`
- `PedidosProductos`
- `PedidosProductosToppings`
- `PedidosPagos`
- `Clasificaciones`
- `SubClasificaciones`
- `Toppings`
- `Leches`
- `TiposGranos`
- `Vasos`
- `Regalos`

`Cafe.sql` already includes the requested auth/order/gift fields such as `PasswordHash`, `FechaRegistro`, `Activo`, `EsInvitado`, `TipoPedido`, `TipoPickup`, `Ubicacion`, and linked `Regalos`.

## Later API / Database Integration

Replace `DummyDataService` with an API-backed service for categories, products, menu options, and prices. Replace or persist `OrderSessionService` when user accounts, basket persistence, checkout, notifications, and database storage are added.

## Replacing Images

Add production assets to `Resources/Images/` using lowercase file names. Update the `Image` values in `DummyDataService.cs` or the future API response to point at the new asset names.
