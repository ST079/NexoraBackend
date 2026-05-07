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
dotnet new classlib -n Core       
dotnet new classlib -n Config 
dotnet new classlib -n Common
dotnet new webapi   -n Adapter          

# Add all projects to the solution
dotnet sln add src/Core/Core.csproj
dotnet sln add src/Config/Config.csproj
dotnet sln add src/Common/Common.csproj
dotnet sln add src/Adapter/Adapter.csproj 
```

### Step 2 — Set Up Project References
``` 
# Application needs to know about Domain
dotnet add src/Application reference src/Domain

# Infrastructure needs to know about Domain AND Application
dotnet add src/Infrastructure reference src/Domain
dotnet add src/Infrastructure reference src/Application

# API needs to know about Application (and Infrastructure for DI wiring only)
dotnet add src/API reference src/Application
dotnet add src/API reference src/Infrastructure
```