# 🏫 Multi-Tenant School Management RESTful API — ASP.NET Core | Domain-Driven Design

This project is a **Multi-Tenant School Management RESTful API** built using **ASP.NET Core** and structured using **Domain-Driven Design (DDD)** principles. The API serves educational institutions (schools, colleges, universities) by providing modular and scalable endpoints to manage students, staff, classes, attendance, exams, and more — while supporting tenant isolation and security.

---

## ✨ Features

- ✅ **Multi-Tenant Architecture** (via database or schema per tenant)
- 🎯 **Domain-Driven Design** layered structure
- 🔒 **Authentication & Authorization** with JWT
- 🗃️ **EF Core** with flexible persistence strategies
- 🔄 **CQRS & Mediator pattern** via MediatR
- 📦 **Modular Domains** for Students, Teachers, Courses, etc.
- 🧪 Integrated Unit & Integration Tests
- 📄 Swagger UI for API documentation

---

## 📁 Project Structure

```bash
src/
├── School.Core                 # Entry point (controllers, middlewares, DI, config)
├── School.Infrastructure      # Application layer (DTOs, CQRS, interfaces)
├── School.WebApi            # Domain layer (Entities, Aggregates, Value Objects, Interfaces)
├── School.Infrastructure     # Infrastructure layer (EF Core, Repositories, external services)
├── School.Persistence        # EF Core DbContext, migrations, seeders
└── School.Tests              # Unit & Integration Tests
```


---

## 🏗️ Technologies Used

| Technology         | Description                                |
|-------------------|--------------------------------------------|
| ASP.NET Core       | Web API Framework                         |
| Entity Framework Core | ORM for persistence                  |
| MediatR            | Implements CQRS/mediator pattern          |
| AutoMapper         | DTO ↔️ Domain mapping                     |
| JWT Auth           | Secure multi-tenant access                |
| Swagger / NSwag    | API documentation & testing               |
| Serilog            | Structured logging                        |

---

## 🏛️ Domain Modules & Modeling Strategy

This solution follows **Domain-Driven Design (DDD)** and organizes the system into **Bounded Contexts**, each representing a business domain.

---

### ✅ Core Domain Modules

#### 📘 Student Management
- **Entities**: `Student`, `Guardian`, `Enrollment`
- **Value Objects**: `Address`, `FullName`

#### 👨‍🏫 Teacher & Staff
- **Entities**: `Teacher`, `Staff`, `Role`

#### 🗓️ Courses & Scheduling
- **Entities**: `Course`, `Classroom`, `Timetable`

#### 🧪 Subjects & Exams
- **Entities**: `Subject`, `Exam`, `Result`

#### 🕒 Attendance
- **Entities**: `AttendanceRecord`, `DailyLog`

#### 📝 Grading
- **Entities**: `Grade`, `Transcript`, `Term`

#### 🏢 Tenant Management
- **Entities**: `Tenant`, `SchoolProfile`
- Multi-tenant isolation via `TenantId`

---

### 📦 DDD Principles Applied

- **Aggregates & Root Entities** ensure transactional consistency.
- **Value Objects** are used for immutables like `Email`, `PhoneNumber`.
- **Domain Services** encapsulate domain-specific logic outside entities.
- **Repositories** abstract persistence logic per aggregate.
- **Interfaces** define contracts across layers (infrastructure implements them).
- **CQRS** with `MediatR` is used for clear separation of reads and writes.

---

## 🌐 Multi-Tenancy Strategy

The application supports multiple tenants (schools or campuses) through:

- 🔑 **Tenant Identification**: via HTTP header (`X-Tenant-ID`) or subdomain
- 🧩 **Data Isolation**: either schema-per-tenant or database-per-tenant
- 🛡️ **Scoped Authorization**: ensures tenant isolation and access control

Multi-tenancy is implemented using **middleware** that injects the current tenant into the request pipeline and resolves services per tenant.

---

## 🛠️ Contributing

Contributions are welcome!

1. Fork the repo
2. Create your feature branch
   ```bash
   git checkout -b feature/my-feature


## 🚀 Getting Started

### 🔧 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server or PostgreSQL (configurable)
- Visual Studio / VS Code
- (Optional) Docker for containerization

### 📦 Setup

```bash
# Clone the repository
git clone https://github.com/hmuwonge/school-api.git
cd school-api

# Restore dependencies
dotnet restore

# Apply database migrations (specify tenant if needed)
dotnet ef database update --project School.Persistence

# Run the API
dotnet run --project School.API


# Run unit tests
dotnet test School.Tests

```
