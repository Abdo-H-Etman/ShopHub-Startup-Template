# ShopHub System

A clean and extensible **ASP.NET Core MVC E-Commerce System** built with **.NET 10**, **Entity Framework Core**, **ASP.NET Core Identity**, and a layered architecture.

The project is designed to provide a solid foundation for building E-Commerce applications while demonstrating practical backend development concepts such as service-layer separation, repository/unit-of-work patterns, DTOs, AutoMapper, authentication, authorization, role management, database migrations, and initial data seeding.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
  - [Clone the Repository](#1-clone-the-repository)
  - [Configure the Database](#2-configure-the-database)
  - [Apply Database Migrations](#3-apply-database-migrations)
  - [Run the Application](#4-run-the-application)
- [Database](#-database)
- [Authentication & Authorization](#-authentication--authorization)
- [Default Admin Account](#-default-admin-account)
- [Application Modules](#-application-modules)
- [Troubleshooting](#-troubleshooting)
- [Future Extensions](#-future-extensions)

---

## 📖 Overview

**ShopHub Startup Template** is an educational ASP.NET Core MVC project intended to serve as a starting point for E-Commerce applications.

The project separates responsibilities across multiple layers:

- **Web Layer** — MVC controllers, views, view models, authentication, authorization, and application configuration.
- **BLL Layer** — Business logic, DTOs, AutoMapper profiles, and application services.
- **DAL Layer** — Entity Framework Core, database context, models, migrations, repositories, and Unit of Work.

The template can be extended with additional E-Commerce functionality such as shopping carts, orders, payments, reviews, wishlists, and analytics.

---

## ✨ Features

### Product Management

- Create products
- View products
- Edit products
- Delete products
- Upload product images
- Product/category relationships
- Server-side validation
- Pagination support

### Category Management

- Create categories
- View categories
- Edit categories
- Delete categories

### Authentication

- User registration
- User login
- User logout
- Email confirmation support
- Authentication using ASP.NET Core Identity
- Cookie-based authentication
- Access denied handling

### Authorization

- Role-based authorization
- `Admin` role
- `Customer` role
- `AdminOnly` authorization policy
- Protected administrative endpoints/pages

### User Management

- View users
- Manage user roles
- Manage account lockout status
- User administration through a dedicated service layer

### Database & Persistence

- Entity Framework Core
- SQL Server
- EF Core migrations
- Repository Pattern
- Unit of Work
- Automatic migration application on application startup

### Application Infrastructure

- Dependency Injection
- DTOs
- AutoMapper
- Service Layer
- ViewModels
- Session support
- Razor Runtime Compilation
- TempData notifications
- File upload support

### UI

- Bootstrap
- AdminLTE dashboard
- jQuery
- DataTables
- Toastr notifications
- SweetAlert2
- Font Awesome

---

## 🛠 Tech Stack

| Technology | Purpose |
|------------|---------|
| .NET 10 | Application framework |
| ASP.NET Core MVC | Web application framework |
| Entity Framework Core 10 | ORM / data access |
| SQL Server | Relational database |
| ASP.NET Core Identity | Authentication & authorization |
| AutoMapper | Object mapping |
| Bootstrap | UI framework |
| AdminLTE | Admin dashboard |
| jQuery | Client-side scripting |
| DataTables | Interactive tables |
| Toastr | Notifications |
| SweetAlert2 | Confirmation dialogs |
| X.PagedList | Pagination |
| Razor Runtime Compilation | Runtime Razor view compilation |

---

## 🏗 Architecture

The project follows a layered architecture:

```text
┌──────────────────────────────────────┐
│              Web Layer               │
│                                      │
│ Controllers / Views / ViewModels     │
│ Authentication / Authorization      │
│ Application Configuration            │
└──────────────────┬───────────────────┘
                   │
                   ▼
┌──────────────────────────────────────┐
│              BLL Layer               │
│                                      │
│ Services / DTOs / AutoMapper         │
│ Business Logic                       │
└──────────────────┬───────────────────┘
                   │
                   ▼
┌──────────────────────────────────────┐
│              DAL Layer               │
│                                      │
│ EF Core / DbContext / Repositories   │
│ Unit of Work / Entities / Migrations │
└──────────────────┬───────────────────┘
                   │
                   ▼
             ┌────────────┐
             │ SQL Server │
             └────────────┘
````

# 🚀 Getting Started
Instructions to run this project locally

## Prerequisites

Before running the project, make sure you have the following installed:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* SQL Server
* SQL Server Management Studio (SSMS) — recommended
* Git
* Visual Studio 2022/2026 or another .NET-compatible IDE

You can verify your .NET installation with:

```bash
dotnet --version
```

The project targets:

```text
net10.0
```

---

## 1. Clone the Repository

Clone the repository and switch to the development branch:

```bash
git clone https://github.com/Abdo-H-Etman/ShopHub-Startup-Template.git
cd ShopHub-Startup-Template
git checkout develop
```

---

## 2. Configure the Database

The application uses SQL Server and reads the connection string using:

```csharp
builder.Configuration.GetConnectionString("DefaultConnection")
```

Create or update the following configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=myshopDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### SQL Server Authentication

If your SQL Server instance uses SQL Server Authentication:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=myshopDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

> **Important:** Never commit real database credentials, passwords, API keys, or other secrets to source control.

For local development, prefer using:

* `appsettings.Development.json` with safe local credentials
* User Secrets
* Environment variables

---

## 3. Apply Database Migrations

The repository contains Entity Framework Core migrations.

From the solution directory, run:

```bash
dotnet ef database update
```

If the EF CLI tool is not installed:

```bash
dotnet tool install --global dotnet-ef
```

Then run:

```bash
dotnet ef database update
```

You can also apply migrations from Visual Studio Package Manager Console:

```powershell
Update-Database
```

### Automatic Migration

The application also applies pending migrations during startup:

```csharp
db.Database.Migrate();
```

Therefore, when the application starts, pending migrations are automatically applied to the configured database.

---

## 4. Run the Application

From the solution directory:

```bash
dotnet run --project myshop.Web
```

Or open:

```text
myshop.sln
```

in Visual Studio and run the `myshop.Web` project.

The application will display the HTTPS/HTTP URL in the console.

The default app route currently starts at:

```text
/Account/Login
```

---

# 🗄 Database

The application uses **SQL Server** with **Entity Framework Core** for data persistence.

The database schema is managed through **EF Core migrations**, while initial development data is created automatically through the application's data seeder.


The `ApplicationDbContext` inherits from:

```csharp
IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
```

The application contains Identity tables in addition to application-specific tables such as:

```text
Users
Categories
Products
AspNetRoles
AspNetUserRoles
AspNetUserClaims
AspNetUserLogins
AspNetUserTokens
AspNetRoleClaims
AspNetUserTokens
```

> The actual Identity table structure is generated and maintained through Entity Framework Core migrations.


# 🔐 Authentication & Authorization

ASP.NET Core Identity is used for authentication and authorization.

The application uses:

```csharp
ApplicationUser
IdentityRole<int>
```

Authentication is configured using Identity and Entity Framework Core.

The application also configures:

```text
Login Path: /Account/Login
Access Denied Path: /Account/AccessDenied
```

## Roles

Two roles are currently seeded:

```text
Admin
Customer
```

## Admin Authorization Policy

An `AdminOnly` policy is configured for authenticated users who belong to the `Admin` role.

Example:

```csharp
[Authorize(Policy = "AdminOnly")]
```

You can use this policy to protect administrative functionality.

---

# 👤 Default Admin Account

The application automatically creates an initial administrator when the database is initialized.

The current development seeder creates:

```text
Username: admin
Email: admin@myshop.com
Role: Admin
```

The current seeded development password is:

```text
Admin@123
```

> **Security Warning:** Change this password before using the application in any real or production environment I did that only for testing.

The seeder also creates the following roles if they do not already exist:

```text
Admin
Customer
```

The application executes the seeder during startup after applying pending migrations.

---

# 📦 Application Modules

## Category Management

The category module supports:

* Create
* Read
* Update
* Delete

Categories are used as part of the product management functionality.

---

## Product Management

The product module supports:

* Create products
* View products
* Edit products
* Delete products
* Product image uploads
* Category assignment
* Pagination

---

## Account Management

The account functionality includes:

* Registration
* Login
* Logout
* Authentication
* Email confirmation handling
* Access denied handling
* Account-related validation

---

## User Management

Administrative users can manage application users through the user management functionality.

The implementation uses:

```text
IUserManagementService
UserManagementService
```

The service layer keeps user-management business logic outside the MVC controllers.

---

# 🧩 Design Patterns & Practices

The project demonstrates several commonly used ASP.NET Core development practices.

### Repository Pattern

Data access is abstracted through repositories.

```text
IGenericRepository<T>
GenericRepository<T>
```

### Unit of Work

Multiple repositories can be coordinated through:

```text
IUnitOfWork
UnitOfWork
```

### Service Layer

Business logic is separated from controllers through services such as:

```text
IProductService
ICategoryService
IAccountService
IUserManagementService
```

### DTOs

DTOs are used to separate business/application data contracts from persistence entities.

DTOs are organized under:

```text
myshop.BLL/DTOs/
```

### AutoMapper

AutoMapper is used to map between:

```text
Entities
DTOs
ViewModels
```

---

# 🔧 Troubleshooting

## Database Connection Error

If the application cannot connect to SQL Server:

1. Make sure SQL Server is running.
2. Verify the SQL Server instance name.
3. Check the `DefaultConnection` connection string.
4. Verify SQL Server authentication credentials if using SQL authentication.
5. Make sure `TrustServerCertificate=True` is configured for local development if required.

---

## Migration Command Not Found

Install the EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

Then verify:

```bash
dotnet ef --version
```

---

# 📷 Images
* [Drive Link](https://drive.google.com/drive/folders/1MSif-Ar1ScIr6ptgS1DFipIAGeIPWQxs?usp=drive_link)

# 🚧 Future Extensions

The project can be extended with additional E-Commerce functionality, including:

* [ ] Shopping Cart
* [ ] Orders
* [ ] Order Items
* [ ] Checkout
* [ ] Payment Integration
* [ ] Product Reviews
* [ ] Wishlist
* [ ] Inventory Management
* [ ] Dashboard Analytics
* [ ] Product Search
* [ ] Advanced Filtering
* [ ] Email Notifications
* [ ] Password Reset
* [ ] External Authentication
* [ ] API Layer
* [ ] Automated Tests
* [ ] Docker Support
* [ ] CI/CD Pipeline

---
