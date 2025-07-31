# URL Shortener

### Test assignment for **Inforce**

A simple URL shortener built with ASP.NET MVC, React, and PostgreSQL. It follows the principles of Clean Architecture and is structured for scalability, separation of concerns, and maintainability.

---

## 🚀 Getting Started

### 🔧 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js + npm](https://nodejs.org/)
- PostgreSQL 15+
- (Optional) JetBrains Rider or Visual Studio 2022+

---

### 🔄 Clone and Run

```bash
git clone https://github.com/k1tbyte/URLShortener.git
cd urlshortener
```

- Open the solution in Rider or Visual Studio
- Setup the database (see Database Setup section below)
- Apply migrations to the database
  - Migration project: `URLShortener.Infrastructure`, path: `Data/Migrations`
  - Startup project: `URLShortener.Server`
- Update the database
- Select URLShortener.Server as the startup project
- Press Run

This will start both the ASP.NET Core backend and React frontend using SpaProxy

---
## Architecture
| Project                       | Responsibility                                  |
|-------------------------------| ----------------------------------------------- |
| `URLShortener.Domain`         | Core domain models and logic (entities) |
| `URLShortener.Infrastructure` | Persistence logic, EF Core DbContext, Repositories |
| `URLShortener.Server`         | API endpoints, configuration  |
| `URLShortener.Client`         | React frontend, components, services |

---

## Database Setup

The project uses PostgreSQL. Here's how you can set it up manually (e.g. on Windows):

```
CREATE USER urlshortener;
ALTER USER urlshortener CREATEDB;
ALTER SCHEMA public OWNER TO urlshortener;
CREATE DATABASE "urlshortener" WITH OWNER "urlshortener" ENCODING 'UTF8' LC_COLLATE = 'en_US.UTF-8' LC_CTYPE = 'en_US.UTF-8';
```

## Environment Configuration

Copy the example environment file .env.example to .env.production in release

In development, the application uses a local appsettings.Development.json with a test DB_CONNECTION_STRING

## Features

🗃️ Code-first migrations with EF Core

🔐 JWT-based authentication (access - refresh, Authorization header)

🔗 Shortens URLs using a hash

🔄 Redirects short URLs back to the original

📦 React frontend with vite

📄 About page served via Razor Pages

🧪 Ready for unit/integration testing (just a little bit :p)


