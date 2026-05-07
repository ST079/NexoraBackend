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
mkdir ProductManagement
cd ProductManagement

# Create the solution file (think of it as the "folder" that holds all projects)
dotnet new sln -n NexoraBackend

# Create each project (each is its own .csproj file)
dotnet new classlib -n NexoraBackend.Domain        
dotnet new classlib -n NexoraBackend.Application   
dotnet new classlib -n NexoraBackend.Infrastructure 
dotnet new webapi   -n NexoraBackend.API           

# Add all projects to the solution
dotnet sln add src/NexoraBackend.Domain/NexoraBackend.Domain.csproj
dotnet sln add src/NexoraBackend.Application/NexoraBackend.Application.csproj
dotnet sln add src/NexoraBackend.Infrastructure/NexoraBackend.Infrastructure.csproj
dotnet sln add src/NexoraBackend.API/NexoraBackend.API.csproj 
```

### Step 2 — Set Up Project References
``` 
# Application needs to know about Domain
dotnet add src/ProductManagement.Application reference src/ProductManagement.Domain

# Infrastructure needs to know about Domain AND Application
dotnet add src/ProductManagement.Infrastructure reference src/ProductManagement.Domain
dotnet add src/ProductManagement.Infrastructure reference src/ProductManagement.Application

# API needs to know about Application (and Infrastructure for DI wiring only)
dotnet add src/ProductManagement.API reference src/ProductManagement.Application
dotnet add src/ProductManagement.API reference src/ProductManagement.Infrastructure
```