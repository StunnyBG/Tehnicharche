# Tehnicharche

> An open-source web platform for publishing and discovering hands-on technical service listings across Bulgaria.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-blue)](https://docs.microsoft.com/aspnet/core)
[![EF Core](https://img.shields.io/badge/EF_Core-8.0-orange)](https://docs.microsoft.com/ef/core)

---

## Overview

Tehnicharche is a free, open marketplace where skilled individuals can list practical services such as soldering, PCB repair, appliance maintenance, computer diagnostics, home electrical work, and more. Clients can browse listings by category, region, and city, and contact providers directly — no middlemen, no fees.

---

## Screenshots

<h3 align="center">Public Listings</h3>
<p align="center"><em>Filter by category, region, city, and price range</em></p>
<p align="center">
<img width="85%" alt="image" src="https://github.com/user-attachments/assets/1312b4e8-1c4f-432c-ad58-0aea4eca77c0" />
</p>

<h3 align="center">Listing Detail</h3>
<p align="center"><em>Full contact details and save/unsave</em></p>
<p align="center">
<img width="85%" alt="image" src="https://github.com/user-attachments/assets/60d33341-ed06-4714-b4b1-a254b934ec90" />
</p>

<h3 align="center">Admin Dashboard</h3>
<p align="center"><em>Stats, recent listings, and messages</em></p>
<p align="center">
  <img width="85%" alt="image" src="https://github.com/user-attachments/assets/17dd9322-f855-47ad-97d0-3d13964105e1" />
</p>

---

## Features

### For Users
- Browse service listings with filters: **category**, **region**, **city**, **price range**, and **keyword search**
- View full listing details including provider contact information (email, phone)
- Save and manage favourite listings
- Register, log in, and manage your account (username, email, phone, password)
- Create, edit, and soft-delete your own listings
- Optional two-factor authentication (TOTP) with recovery codes

### For Admins
- Admin dashboard with statistics (active/deleted listings, users, messages)
- Full listing management: soft delete, restore, hard delete
- User management: search, ban/unban (banning auto-soft-deletes all their listings)
- Contact message inbox with read/unread tracking
- Catalog management: categories, regions, and cities (create, edit, delete with in-use guards)

### Platform
- Global query filter hides soft-deleted listings from public views
- Banned users are signed out and blocked on every request via custom middleware
- Memory-cached category, region, and city lookups (6-hour TTL)
- Rate limiting on the contact form (3 requests per 5 minutes)
- JSON-driven database seeder (40+ users, 15 categories, 28 regions, 70 cities, 24 listings)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 |
| Web framework | ASP.NET Core MVC (Razor views) |
| Authentication | ASP.NET Core Identity |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Caching | `IMemoryCache` |
| Rate limiting | ASP.NET Core built-in rate limiter |
| Front-end | Vanilla HTML/CSS/JS, Bootstrap Icons |
| Tests | NUnit 4, Moq — unit + integration |

---

## Project Structure

```
Tehnicharche/
│
├── Tehnicharche.GCommon/           # Shared constants, validation attributes
├── Tehnicharche.Data.Models/       # EF Core entity classes (ApplicationUser, Listing, …)
├── Tehnicharche.Data/              # DbContext, repositories, JSON data seeder
├── Tehnicharche.ViewModels/        # DTOs used between controllers and views
├── Tehnicharche.Services.Core/     # Business logic services and interfaces
├── Tehnicharche.Web/               # ASP.NET Core MVC host (controllers, views, middleware)
├── Tehnicharche.Services.Tests/    # Unit tests (Moq + NUnit)
├── Tehnicharche.IntegrationTests/  # Integration tests (EF Core InMemory)
└── Tehnicharche.sln
```

### Key conventions

- **Repository pattern** — every aggregate has a dedicated repository interface in `Tehnicharche.Data`.
- **Service layer** — all business logic lives in `Tehnicharche.Services.Core`; controllers are thin.
- **Soft delete** — `Listing.IsDeleted` is enforced via an EF Core global query filter; admin views use `IgnoreQueryFilters()`.
- **Admin area** — separated into its own MVC Area (`/Areas/Admin`) with a distinct layout and role guard (`[Authorize(Roles = "Administrator")]`).

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (Express or Developer edition)
- Visual Studio 2022 / VS Code / Rider

### Installation

```bash
git clone https://github.com/StunnyBG/Tehnicharche.git
cd Tehnicharche
dotnet restore
```

### Configuration

Update `Tehnicharche.Web/appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TehnicharcheDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Database Setup

The application runs EF Core migrations and seeds all reference data automatically on startup. No manual migration step is required.

If you prefer to apply migrations manually:

```bash
cd Tehnicharche.Web
dotnet ef database update
```

### Run

```bash
dotnet run --project Tehnicharche.Web
```

The application will be available at `https://localhost:7277` (or the port shown in your terminal).

### Default Seed Accounts

| Role | Username | Password |
|---|---|---|
| Administrator | `admin` | `Admin_1!` |
| User | `ivan.tech` | `DevUser_1!` |

All 49 regular seed users follow the pattern `DevUser_N!` where `N` matches their account number.

---

## Running Tests

```bash
# Unit tests
dotnet test Tehnicharche.Services.Tests/

# Integration tests (uses EF Core InMemory — no database required)
dotnet test Tehnicharche.IntegrationTests/
```

### Test coverage areas

| Test project | What is covered |
|---|---|
| `Services.Tests` | `ListingService`, `SavedListingService`, `ContactService`, `AdminListingService`, `AdminMessageService`, `AdminUserService`, `AdminCategoryService`, `AdminRegionService`, `AdminCityService`, `AdminDashboardService` |
| `IntegrationTests` | `ListingRepository`, `AdminListingRepository`, `SavedListingRepository`, `ContactMessageRepository`, `BanMiddleware` |

---

## Architecture Notes

### Ban system

When an admin bans a user:
1. `IsBanned = true` is set on the `ApplicationUser`.
2. The security stamp is rotated, invalidating any active sessions.
3. All of the user's listings are soft-deleted.
4. On every subsequent request, `BanMiddleware` checks for a `"Banned"` claim and signs the user out with a `403` response — even if they somehow hold a valid cookie.

### Caching

Category, region, and city lookups are cached in-process for 6 hours. Admin write operations (add, edit, delete) explicitly evict the relevant cache key so public views always reflect the latest data.

### Seeder

`DataSeeder` seeds the database from JSON files inside `Tehnicharche.Data/Seeding/Seeds/`. Each seed method checks for existing data first and is idempotent. Invalid DTOs (validated with `DataAnnotations`) are skipped rather than throwing, making the seeder resilient to partial runs.

---

## License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## Author

**StunnyBG** — [github.com/StunnyBG](https://github.com/StunnyBG)
