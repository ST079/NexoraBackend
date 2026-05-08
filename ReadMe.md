## What the Heck Is Hexagonal Architecture?
Imagine you are building a restaurant.
The kitchen (Domain) doesn't care if the order came from a waiter, a phone app, or a drive-through window. It just knows how to cook.
The manager (Application) takes orders from any source, tells the kitchen what to cook, and coordinates everything.
The delivery staff, cashiers, storage rooms (Infrastructure) do the physical work — they fetch ingredients, save receipts, call suppliers.
The front door and windows (API) are how the outside world interacts with your restaurant.

The golden rule is: inner layers NEVER know about outer layers.

## Setting Up the Solution Structure
### Step 1 — Create the Solution
Open your terminal and run these commands one by one:
```
# Create the root folder
mkdir NexoraBack
cd NexoraBack
```

# Create the solution file (think of it as the "folder" that holds all projects)
```
dotnet new sln -n NexoraBackend
mkdir src
cd src

mkdir Core
mkdir Common
mkdir Config
mkdir Adapters

dotnet new classlib -n NexoraBackend.Core -o Core
dotnet new classlib -n NexoraBackend.Common -o Common
dotnet new classlib -n NexoraBackend.Config -o Config
dotnet new classlib -n NexoraBackend.Application -o Adapters/Application
dotnet new classlib -n NexoraBackend.Infrastructure -o Adapters/Infrastructure
dotnet new webapi -n NexoraBackend.API -o Adapters/API
cd ..

dotnet sln add src/Core/NexoraBackend.Core.csproj
dotnet sln add src/Common/NexoraBackend.Common.csproj
dotnet sln add src/Config/NexoraBackend.Config.csproj
dotnet sln add src/Adapters/Application/NexoraBackend.Application.csproj
dotnet sln add src/Adapters/Infrastructure/NexoraBackend.Infrastructure.csproj
dotnet sln add src/Adapters/API/NexoraBackend.API.csproj
dotnet add src/Adapters/Application reference src/Core
dotnet add src/Adapters/Application reference src/Common
dotnet add src/Adapters/Infrastructure reference src/Adapters/Application
dotnet add src/Core
dotnet add src/Common
dotnet add src/Adapters/API reference src/Adapters/Application
dotnet add src/Adapters/API reference src/Adapters/Infrastructure
dotnet add src/Adapters/API reference src/Config
dotnet add src/Adapters/API reference src/Common
dotnet add src/Config reference src/Adapters/Application
dotnet add src/Config reference src/Adapters/Infrastructure
dotnet add src/Config reference src/Common
```

### flow
```
Frontend
   ↓
DTO (ProductDto)
   ↓
Mapper
   ↓
Domain (Product)
   ↓
Business Logic
   ↓
Mapper
   ↓
Entity (ProductEntity)
   ↓
EF Core
   ↓
Database
```

### Return flow:
```

Database
   ↓
Entity
   ↓
Mapper
   ↓
Domain
   ↓
Mapper
   ↓
Response DTO
   ↓
Frontend
```