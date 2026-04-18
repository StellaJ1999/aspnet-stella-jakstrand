# CoreFitness Gym App

CoreFitness is a .NET 10 web application for managing a gym’s public site, member accounts, and admin workflows. It uses ASP.NET Core MVC with Razor views and ASP.NET Core Identity for authentication. The solution is split into clear layers so UI, business logic, and infrastructure concerns stay separated.

## Features
- Public pages for fitness centers, memberships, training, and support
- Member account area for profile management and bookings
- Admin area for operations like schedule management and member administration
- Role-based access with `Admin` and `Member`
- Local sign-in and optional Google external login

## Tech Stack
- ASP.NET Core MVC (Razor views)
- .NET 10
- Entity Framework
- ASP.NET Core Identity
- SQLite in development and SQL Server in production
- xUnit tests

## Project Structure
- `Presentation.WebApp` - Web UI (controllers, views, static assets)
- `Application` - Application services and abstractions
- `Infrastructure` - EF Core, Identity, repositories, and external integrations
- `Tests` - Unit and integration tests

## Getting Started
You can run the app locally with the .NET 10 SDK installed. Configuration is split between production and development settings, and external login settings can be supplied via user secrets.

### Prerequisites
- .NET 10 SDK
- EF Core tooling (optional, for migrations)
- SQLite or SQL Server depending on environment

### Configuration
- Connection strings:
  - `Presentation.WebApp/appsettings.json`
  - `Presentation.WebApp/appsettings.Development.json`
- Google login (optional) uses:
  - `Authentication:Google:ClientId`
  - `Authentication:Google:ClientSecret`

### Run



