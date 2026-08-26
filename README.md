# ShopHub E-Commerce System

A modern, robust, and extensible **ASP.NET Core MVC E-Commerce Platform** built with **.NET 10**, **C# 13**, **Entity Framework Core 10**, and **ASP.NET Core Identity**, adhering to a clean **3-Tier Layered Architecture**.

ShopHub serves as an enterprise-ready template and educational blueprint demonstrating real-world backend and full-stack patterns: Generic Repository & Unit of Work (with transaction safety), Service-Layer Business Isolation, Data Transfer Objects (DTOs), AutoMapper, In-Memory Caching, Session-based Shopping Cart with Guest Migration, AJAX-driven filtering/pagination with History API synchronization, and an AdminLTE-powered management portal.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture & Solution Structure](#-architecture--solution-structure)
- [Tech Stack](#-tech-stack)
- [Key Features](#-key-features)
  - [1. Customer Storefront & Catalog](#1-customer-storefront--catalog)
  - [2. Shopping Cart & Guest Migration](#2-shopping-cart--guest-migration)
  - [3. Category Management & Caching](#3-category-management--caching)
  - [4. Product Management & Image Uploads](#4-product-management--image-uploads)
  - [5. User & Role Administration](#5-user--role-administration)
  - [6. Authentication & Security](#6-authentication--security)
  - [7. Checkout, Orders & Stripe Sandbox Payments](#7-checkout-orders--stripe-sandbox-payments)
  - [8. Email Notifications](#8-email-notifications)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [1. Clone Repository](#1-clone-repository)
  - [2. Database Configuration](#2-database-configuration)
  - [3. Stripe Sandbox Configuration](#3-stripe-sandbox-configuration)
  - [4. Email Configuration](#4-email-configuration)
  - [5. Run Migrations & Seed Data](#5-run-migrations--seed-data)
  - [6. Launch Application](#6-launch-application)
- [Default Seeded Accounts & Data](#-default-seeded-accounts--data)
- [Application Routing & Endpoints](#-application-routing--endpoints)
- [Design Patterns & Engineering Practices](#-design-patterns--engineering-practices)
- [Troubleshooting](#-troubleshooting)
- [Project Screenshots](#-project-screenshots)
- [Roadmap & Future Extensions](#-roadmap--future-extensions)

---

## 📖 Overview

**ShopHub** is organized into three decoupled layers to enforce separation of concerns, testability, and maintainability:

- **`myshop.Web` (Presentation Layer)**: ASP.NET Core MVC controllers, Razor views, view models, cookie authentication, session state management, AdminLTE dashboard, and client-side scripts.
- **`myshop.BLL` (Business Logic Layer)**: Service implementations, business validation, DTO contracts, AutoMapper configuration profiles, and caching policies.
- **`myshop.DAL` (Data Access Layer)**: Entity Framework Core `ApplicationDbContext`, database entities, EF Core migrations, Generic Repositories, and the Unit of Work pattern with transaction boundaries.

---

## 🏗 Architecture & Solution Structure

```text
ShopHub-Startup-Template/
├── myshop.sln
│
├── myshop.Web/                             # Presentation Layer (MVC & UI)
│   ├── Areas/
│   │   └── Admin/                         # Admin Management Area
│   │       ├── Controllers/               # CategoryController, ProductController, UserController
│   │       └── Views/                     # AdminLTE Razor views (Category, Product, User)
│   ├── Controllers/                       # AccountController, CartController, HomeController, ProductController
│   ├── Mapping/                           # WebMappingProfile (ViewModel <-> DTO)
│   ├── Seed/                              # InitialDataSeeder (Roles, Users, Categories, Products)
│   ├── Services/                          # LocalFileService, CartService (Session-backed)
│   ├── ViewModels/                        # Account (Login, Register), Product (Index, Edit, Create)
│   ├── Views/                             # Storefront Views (Home, Product, Cart, Account, Shared)
│   ├── wwwroot/                           # CSS, JS (products.js, category.js, users.js, cart.js), uploads
│   └── Program.cs                         # Application startup, DI container, middleware pipeline
│
├── myshop.BLL/                             # Business Logic Layer
│   ├── DTOs/
│   │   ├── Account/                       # LoginDto, RegisterDto
│   │   ├── Cart/                          # CartItem
│   │   ├── Category/                      # CategoryListDto, CreateCategoryDto, UpdateCategoryDto
│   │   ├── Common/                        # PagedResultDto<T>
│   │   ├── Product/                       # ProductListDto, CreateProductDto, UpdateProductDto
│   │   └── User/                          # UserManagementDto
│   ├── Mapping/                           # MappingProfile (Entity <-> DTO)
│   └── Services/                          # IProductService, ICategoryService, ICartService, etc.
│
└── myshop.DAL/                             # Data Access Layer
    ├── Data/                              # ApplicationDbContext (IdentityDbContext<ApplicationUser, IdentityRole<int>, int>)
    ├── Migrations/                        # Entity Framework Core Migrations
    ├── Models/                            # ApplicationUser, Category, Product, ShoppingCart, OrderHeader, OrderDetail
    └── Repositories/                      # GenericRepository<T>, UnitOfWork (Transaction-managed)
```

```text
┌────────────────────────────────────────────────────────────────────────┐
│                        Presentation (myshop.Web)                       │
│  Controllers  │  Razor Views  │  ViewModels  │  AdminLTE  │  Sessions  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ Calls DTO-based Services
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                       Business Logic (myshop.BLL)                      │
│   Services   │   DTOs   │   AutoMapper   │   IMemoryCache Caching      │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ Coordinates Repositories
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                        Data Access (myshop.DAL)                        │
│   Generic Repositories   │   Unit of Work   │   EF Core DbContext      │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
                             ┌──────────────┐
                             │  SQL Server  │
                             └──────────────┘
```

---

## 🛠 Tech Stack

| Component | Technology | Description |
|-----------|------------|-------------|
| **Runtime & Language** | .NET 10 / C# 13 | High-performance modern .NET ecosystem |
| **Framework** | ASP.NET Core MVC | Web application framework with Razor View Engine |
| **ORM / Data Access** | Entity Framework Core 10 | Object-Relational Mapper targeting SQL Server |
| **Database** | Microsoft SQL Server | Relational Database Management System |
| **Identity & Security** | ASP.NET Core Identity | Membership, role-based authorization, cookie auth (`int` PKs) |
| **Object Mapping** | AutoMapper 16.2.0 | Automated entity-to-DTO and DTO-to-ViewModel transformations |
| **Caching** | `IMemoryCache` | In-memory caching for high-frequency queries (Categories) |
| **Session State** | Distributed Memory Cache | Session storage for guest and authenticated shopping carts |
| **Compilation** | Razor Runtime Compilation | Instant Razor view updates without project rebuilds |
| **Pagination** | Custom `PagedResultDto<T>` & X.PagedList | Server-side queryable pagination |
| **Admin Dashboard** | AdminLTE 3 + Bootstrap 5 | Modern, responsive dashboard layout with responsive sidebar |
| **Interactive Tables** | jQuery DataTables | Client-side/AJAX data tables with sorting, filtering, and paging |
| **UI Components** | SweetAlert2, Toastr, FontAwesome 6 | Interactive alerts, toast messages, and scalable iconography |
| **Payments** | Stripe.net + Stripe.js | Stripe Sandbox payment processing using PaymentIntents and Stripe Payment Element |
| **Email** | MailKit | SMTP-based order confirmation emails |

---

## ✨ Key Features

### 1. Customer Storefront & Catalog
- **Live Debounced Search**: Fast product lookup with a 300ms debounce threshold ($\ge 3$ characters with instant fallback on clear).
- **Dynamic Sorting & Pagination**: Sort by Name (A-Z, Z-A) or Price (Low-High, High-Low) with configurable page sizes (8, 12, 24 products per page).
- **AJAX Partial Loading**: Seamless browsing without full-page reloads using `_ProductListPartial.cshtml`.
- **Browser History Synchronization**: Preserves filter/search state in URL parameters (`history.replaceState`) for bookmarkable and shareable search URLs.
- **Hero Showcase Landing**: Modern hero section displaying featured products with responsive image fallbacks (`/Images/no-image.png`).

### 2. Shopping Cart & Guest Migration
- **Session-Driven Cart**: Lightweight and decoupled cart state stored via `ICartService` and `ISession`.
- **Guest-to-User Cart Migration**: Items added by unauthenticated guests (`ShoppingCart_Guest`) are automatically merged into the user's permanent cart (`ShoppingCart_User_{id}`) upon login (`MigrateGuestCart`).
- **Real-Time AJAX Cart Actions**:
  - Add to cart with visual notification feedback.
  - Increase / Decrease quantity dynamically recalculating line-item totals and grand total.
  - Remove item with interactive SweetAlert2 confirmation.
  - Clear entire cart with automated empty-state rendering.
- **CSRF Protected**: Every mutation request enforces `[ValidateAntiForgeryToken]` and token headers.

### 3. Category Management & Caching
- **Full CRUD Operations**: Admin area category management (`/Admin/Category`).
- **In-Memory Caching Strategy**: Categories are cached using `IMemoryCache` with a 30-minute absolute expiration to reduce database round-trips.
- **Automatic Cache Invalidation**: Create, Update, and Delete operations automatically purge the `categories` cache key to maintain fresh data.

### 4. Product Management & Image Uploads
- **Product Catalog CRUD**: Admin product management (`/Admin/Product`) with category relationships.
- **Secure File Handling**: `LocalFileService` manages image uploads under `wwwroot/uploads/products/`.
  - Allowed extensions: `.jpg`, `.jpeg`, `.png`, `.webp`.
  - File size validation: Maximum 2 MB per image.
  - Automatic orphan cleanup: Deletes old physical image files when a product is updated or deleted.
- **Interactive DataTables**: DataTables integration with server-sourced JSON (`/Admin/Product/GetData`) and AJAX deletion.

### 5. User & Role Administration
- **User Management Dashboard**: Dedicated administrative view (`/Admin/User`) displaying user details, assigned roles, and lockout states.
- **Instant Role Modification**: Promote or demote users between `Admin` and `Customer` via AJAX.
- **Account Lockout Control**: Instantly lock or unlock accounts by setting `LockoutEndDate`.
- **Self-Protection Guardrails**: Built-in backend safeguards prevent logged-in administrators from changing their own role, locking their own account, or deleting themselves.

### 6. Authentication & Security
- **Identity with Integer Keys**: Configured with `ApplicationUser` and `IdentityRole<int>` for optimal relational indexing.
- **Account Lockout Policy**: Configured to lock accounts for 10 minutes after 5 consecutive failed login attempts.
- **Cookie Authentication**: Explicit paths configured for `/Account/Login` and `/Account/AccessDenied`.
- **Role-Based Authorization Policy**: `AdminOnly` policy requiring authentication and the `Admin` role (`[Authorize(Policy = "AdminOnly")]`).

### 7. Checkout, Orders & Stripe Sandbox Payments
- **Checkout Flow**: Customers provide delivery information and review their cart before payment.
- **Stripe PaymentIntent**: The server creates a Stripe PaymentIntent using the server-calculated order total.
- **Stripe Payment Element**: Secure card/payment information is collected through Stripe.js without ShopHub storing card details.
- **Payment Verification**: Orders are created only after Stripe confirms that the PaymentIntent succeeded.
- **PaymentIntent Ownership**: PaymentIntents contain the authenticated user's ID in Stripe metadata and are verified before finalizing an order.
- **Payment Amount Verification**: The server verifies the Stripe PaymentIntent amount against the calculated cart total to prevent client-side price manipulation.
- **Payment Status Verification**: The backend checks the PaymentIntent status before creating the order.
- **Order Finalization**: Successful payments trigger order creation, cart clearing, and order confirmation processing.
- **Sandbox Testing**: Stripe test cards can be used during development without real charges.

### 8. Email Notifications
- **SMTP Integration**: MailKit is used to send application emails through an SMTP server.
- **Email Service Abstraction**: Email functionality is exposed through an `IEmailService` interface to keep email infrastructure isolated from business logic.
- **Order Confirmation Emails**: Customers receive an email after a successful payment and order creation.
- **HTML Email Templates**: Order confirmation emails contain structured order information including order number, products, quantities, prices, and total amount.
- **Asynchronous Sending**: Emails are sent asynchronously to avoid blocking the main request thread.
- **Configuration-Based SMTP**: SMTP credentials and server settings are provided through application configuration/User Secrets rather than hard-coded values.

---

## 🚀 Getting Started

### Prerequisites
Ensure the following tools are installed:
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (verify with `dotnet --version`)
- **SQL Server** (LocalDB, Express, or Full instance)
- **SQL Server Management Studio (SSMS)** or **Azure Data Studio** (optional)
- **Visual Studio 2022 / 2026** or **VS Code** with C# Dev Kit

---

### 1. Clone Repository

```bash
git clone https://github.com/Abdo-H-Etman/ShopHub-Startup-Template.git
cd ShopHub-Startup-Template
git checkout develop
```

---

### 2. Database Configuration

The application reads the connection string `DefaultConnection` from `appsettings.json` or `appsettings.Development.json`.

Update `myshop.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=myshopDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

*If using SQL Server Authentication:*
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=myshopDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

> **Security Note:** Never commit production database credentials or sensitive secrets to version control. Use .NET User Secrets (`dotnet user-secrets`) or environment variables in production.

---
### 3. Stripe Sandbox Configuration

ShopHub uses Stripe Sandbox/Test Mode for payment processing.

You must configure Stripe API keys before using the checkout and payment functionality.

#### Create Stripe Test Keys

Create a Stripe account and enable Test/Sandbox mode.

From the Stripe Dashboard, obtain:

* Publishable key (`pk_test_...`)
* Secret key (`sk_test_...`)

> ⚠️ Never commit your Stripe Secret Key to Git or expose it in client-side JavaScript.

#### Configure .NET User Secrets

The application uses .NET User Secrets to store Stripe credentials during local development.

Navigate to the web project:

```bash
cd myshop.Web
```
Initialize User Secrets if they have not already been initialized:

```bash
dotnet user-secrets init
```

Configure the Stripe settings:

```bash
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_your_publishable_key"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_your_secret_key"
dotnet user-secrets set "Stripe:Currency" "usd"
```

Verify the configured secrets:

```bash
dotnet user-secrets list
```

### 4. Email Configuration

ShopHub uses **MailKit** with SMTP to send order confirmation emails.

You must have account on **MailTrap** to get the email setting from it

Email configuration should be stored securely using .NET User Secrets during local development.

Navigate to the web project:

```bash
cd myshop.Web
```

Configure your SMTP settings:

```bash
dotnet user-secrets set "EmailSettings:Username" "your_email@example.com"
dotnet user-secrets set "EmailSettings:Password" "your_email_password"
```

### 5. Run Migrations & Seed Data

The application includes automatic migration and data seeding on startup:

```csharp
// Program.cs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<InitialDataSeeder>();
    await seeder.SeedAsync();
}
```

If you prefer applying migrations manually via the CLI:

```bash
# Install EF tool if not already present
dotnet tool install --global dotnet-ef

# Apply migrations
dotnet ef database update --project myshop.DAL --startup-project myshop.Web
```

---

### 6. Launch Application

Run the application using the .NET CLI:

```bash
dotnet run --project myshop.Web
```

Navigate to the displayed local HTTPS address (e.g., `https://localhost:7001` or `http://localhost:5000`).

---

## 👤 Default Seeded Accounts & Data

When the database is initialized, `InitialDataSeeder` automatically populates default roles, administrative accounts, customer test users, categories, and products:

### Seeded User Accounts

| Username | Email | Password | Role | Description |
|----------|-------|----------|------|-------------|
| `admin` | `admin@myshop.com` | `Admin@123` | **Admin** | Full administrative privileges (Dashboard, Products, Categories, Users) |
| `user1` | `user1@myshop.com` | `User@123` | **Customer** | Standard customer test account |
| `user2` | `user2@myshop.com` | `User@123` | **Customer** | Standard customer test account |

> ⚠️ **Warning:** The seeded passwords are for development and testing only. Change default credentials prior to deploying to any production environment.

### Seeded Categories & Sample Products
- **Categories**: `Electronics`, `Books`, `Clothing`, `Home & Kitchen`
- **Products**: Smartphone ($699.99), Laptop ($1,299.99), Barca-Shirt ($14.99) with sample images.

---

## 🛣 Application Routing & Endpoints

### Storefront & Customer Routes
| Route / URL | Controller | Action | Description |
|-------------|------------|--------|-------------|
| `/` or `/Home/Index` | `HomeController` | `Index` | Storefront landing page & featured items |
| `/Product/Index` | `ProductController` | `Index` | Product catalog with live search, sorting, and pagination |
| `/Cart/Index` | `CartController` | `Index` | Shopping cart overview |
| `/Cart/Add` | `CartController` | `Add` (POST) | Add product item to cart |
| `/Cart/Increase` | `CartController` | `Increase` (POST) | Increment quantity (AJAX-supported) |
| `/Cart/Decrease` | `CartController` | `Decrease` (POST) | Decrement quantity (AJAX-supported) |
| `/Cart/Remove` | `CartController` | `Remove` (POST) | Remove item from cart (AJAX-supported) |
| `/Cart/Clear` | `CartController` | `Clear` (POST) | Remove all items from cart |

### Account & Authentication Routes
| Route / URL | Controller | Action | Description |
|-------------|------------|--------|-------------|
| `/Account/Login` | `AccountController` | `Login` (GET/POST) | User authentication & guest cart migration |
| `/Account/Register` | `AccountController` | `Register` (GET/POST) | New customer account registration |
| `/Account/Logout` | `AccountController` | `Logout` (POST) | Sign out and clear active session |
| `/Account/AccessDenied` | `AccountController` | `AccessDenied` | Unauthorized access landing page |

### Admin Area Routes (`[Area("Admin")]`, `[Authorize(Policy = "AdminOnly")]`)
| Route / URL | Controller | Action | Description |
|-------------|------------|--------|-------------|
| `/Admin/Category` | `CategoryController` | `Index` | Category listing with DataTables |
| `/Admin/Category/Create` | `CategoryController` | `Create` (GET/POST) | Create a new category |
| `/Admin/Category/Edit/{id}` | `CategoryController` | `Edit` (GET/POST) | Update an existing category |
| `/Admin/Category/DeleteAjax` | `CategoryController` | `DeleteAjax` (DELETE) | AJAX category deletion |
| `/Admin/Product` | `ProductController` | `Index` | Product management data table |
| `/Admin/Product/GetData` | `ProductController` | `GetData` (GET) | JSON endpoint for DataTables |
| `/Admin/Product/Create` | `ProductController` | `Create` (GET/POST) | Create product with image upload |
| `/Admin/Product/Edit/{id}` | `ProductController` | `Edit` (GET/POST) | Update product details & image replacement |
| `/Admin/Product/DeleteAjax` | `ProductController` | `DeleteAjax` (DELETE) | AJAX product deletion with file cleanup |
| `/Admin/User` | `UserController` | `Index` | User administration portal |
| `/Admin/User/GetData` | `UserController` | `GetData` (GET) | JSON endpoint for user table |
| `/Admin/User/ChangeRole` | `UserController` | `ChangeRole` (POST) | Switch user role (`Admin` / `Customer`) |
| `/Admin/User/Lock` | `UserController` | `Lock` (POST) | Lock user account |
| `/Admin/User/Unlock` | `UserController` | `Unlock` (POST) | Unlock user account |
| `/Admin/User/DeleteAjax` | `UserController` | `DeleteAjax` (DELETE) | Delete user account |

---

## 🧩 Design Patterns & Engineering Practices

### 1. Generic Repository Pattern (`IGenericRepository<T>`)
Data access operations are abstracted behind a generic repository interface providing type-safe querying, projection, include navigation, and pagination:

```csharp
Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null);
Task<T?> GetByIdAsync(int id);
Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null);
Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
    int pageNumber,
    int pageSize,
    Expression<Func<T, bool>>? predicate = null,
    Func<IQueryable<T>, IQueryable<T>>? include = null,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
```

### 2. Unit of Work with Transaction Handling (`IUnitOfWork`)
Coordinates multiple entity repositories and encapsulates `SaveChangesAsync()` within an explicit transaction boundary with automatic rollback on error:

```csharp
public async Task<int> SaveChangesAsync()
{
    if (_context.Database.CurrentTransaction is not null)
        return await _context.SaveChangesAsync();

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        var result = await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return result;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

### 3. Layered Service Abstraction
Controllers remain lean and free of database logic. All business rules, caching, file manipulations, and transactions are encapsulated within dedicated services (`ProductService`, `CategoryService`, `UserManagementService`, `AccountService`, `CartService`).

### 4. DTO Pattern & AutoMapper
Database entities never leak directly to the client views. Dedicated DTOs represent business contracts, while ViewModels represent UI contracts. AutoMapper profiles handle object-to-object mapping cleanly across boundaries.

---

## 🔧 Troubleshooting

### Database Connection Failure
- Verify that SQL Server / SQLEXPRESS is actively running in Windows Services (`services.msc`).
- Confirm that `TrustServerCertificate=True` is present in your connection string for local development certificates.
- Check that the database user credentials have `db_owner` or `CREATE DATABASE` permissions.

### Missing `dotnet ef` Tool
Install or update the global EF CLI tool:
```bash
dotnet tool install --global dotnet-ef
dotnet ef --version
```

### Image Upload Issues
- Ensure `wwwroot/uploads/products/` has write permissions.
- Validate that the uploaded image format is `.jpg`, `.jpeg`, `.png`, or `.webp` and size does not exceed 2 MB.

---

## 📷 Project Screenshots

Screenshots, demos, and assets can be referenced in the repository media resources:
* [Project Images & Assets Drive Link](https://drive.google.com/drive/folders/1MSif-Ar1ScIr6ptgS1DFipIAGeIPWQxs?usp=drive_link)

---

## 🚧 Roadmap & Future Extensions

- [x] Layered 3-Tier Architecture (.NET 10)
- [x] ASP.NET Core Identity (`int` Keys & Role Management)
- [x] Product & Category Full CRUD with AdminLTE
- [x] Product Image Uploads & File Cleanup (`IFileService`)
- [x] In-Memory Caching for High-Frequency Queries (`IMemoryCache`)
- [x] User Management (Role Promotion, Account Lockout, Self-Protection Guards)
- [x] Session-Based Shopping Cart (`ICartService`)
- [x] Guest-to-User Cart Migration on Login
- [x] Storefront Debounced Live Search, Sorting & AJAX Pagination
- [x] Interactive SweetAlert2 & Toastr Client Feedback
- [ ] Checkout & Order Placement Flow
- [ ] Stripe Payment Gateway Integration (`Stripe.net`)
- [ ] Order Management & Tracking (`OrderHeader` / `OrderDetail`)
- [ ] Product Reviews and Customer Ratings
- [ ] Customer Wishlist
- [ ] Email Notifications (Order Confirmation, Password Reset via SMTP / SendGrid)
- [ ] RESTful API Endpoints with Swagger / OpenAPI
- [ ] Automated Unit & Integration Tests (xUnit, Moq, Testcontainers)
- [ ] Docker & Containerization Support
- [ ] CI/CD Pipeline via GitHub Actions

---

## 📄 License

This project is licensed under the terms of the MIT license.
