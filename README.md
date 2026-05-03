# HotelCore HMS

Hotel Management System built with .NET 10 and React + TypeScript.

## Modules

| Module | Owner | Description |
|--------|-------|-------------|
| Reception | Person A | Check-In / Check-Out wizard, walk-in reservations |
| Cleaning | Person C | Task requests, queue management, supervisor verification |
| Staff Management | Person D | Work schedules, shift assignment, shift change requests |
| Restaurant | Person B | Food orders, menu management *(independent module)* |

## Tech Stack

**Backend**
- ASP.NET Core 10 Web API
- Clean Architecture (Domain → Application → Infrastructure → Api)
- CQRS with MediatR + FluentValidation pipeline behaviour
- Entity Framework Core with PostgreSQL (via .NET Aspire)
- ASP.NET Core Identity with JWT Bearer auth

**Frontend**
- React 18 + TypeScript + Vite
- React Router v6
- Zustand (auth + toast state)
- Axios with JWT interceptor
- Custom CSS design system (Inter font, CSS variables)

## Prerequisites

- .NET 10 SDK
- Node.js 20+
- PostgreSQL (or use the Aspire AppHost to spin one up)
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## Setup & Run

### 1. Backend

```bash
# Restore packages
dotnet restore backend/HotelCore.sln

# Apply database migrations
dotnet ef database update \
  --project backend/src/HotelCore.Infrastructure \
  --startup-project backend/src/HotelCore.Api

# Run the API (default: http://localhost:5000)
dotnet run --project backend/src/HotelCore.Api
```

On first run, seed data is applied automatically:
- 6 rooms (floors 1–3, various types)
- Admin account: `admin@hotelcore.com` / `Admin123!`

### 2. Frontend

```bash
cd frontend
npm install
npm run dev   # http://localhost:3000
```

The Vite dev server proxies `/api` requests to `http://localhost:5000` so no CORS issues during development.

## Environment Variables

| File | Variable | Default | Purpose |
|------|----------|---------|---------|
| `frontend/.env.local` | `VITE_API_URL` | *(empty — uses proxy)* | Override API base URL |
| `backend/src/HotelCore.Api/appsettings.Development.json` | `Jwt:Secret` | *(auto-generated)* | JWT signing key |

## Running Tests

```bash
dotnet test backend/HotelCore.sln
```

## Project Structure

```
HotelCore/
├── backend/
│   └── src/
│       ├── HotelCore.Domain/          # Entities, enums, domain exceptions
│       ├── HotelCore.Application/     # Commands, queries, handlers, interfaces
│       ├── HotelCore.Infrastructure/  # EF Core, repositories, migrations
│       └── HotelCore.Api/             # Controllers, Program.cs, appsettings
├── frontend/
│   └── src/
│       ├── api/        # Axios wrappers per module
│       ├── components/ # UI components (reception/, cleaning/, schedule/)
│       ├── pages/      # Page-level components
│       ├── stores/     # Zustand stores
│       └── types/      # TypeScript DTOs
└── docker-compose.yml
```

## API Endpoints

### Auth
| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/auth/login` | Authenticate and receive JWT token |
| POST | `/api/auth/refresh` | Refresh an expired JWT using refresh token |

### Reception
| Method | Path | Role | Description |
|--------|------|------|-------------|
| GET | `/api/reception/reservations` | Receptionist | Search reservations by guest name or confirmation number |
| POST | `/api/reception/check-in` | Receptionist | Complete guest check-in with identity verification and payment |
| POST | `/api/reception/check-out` | Receptionist | Complete guest check-out |
| POST | `/api/reception/walk-in` | Receptionist | Create and immediately check in a walk-in guest |

### Cleaning
| Method | Path | Role | Description |
|--------|------|------|-------------|
| POST | `/api/cleaning/request` | Receptionist, Guest | Submit a new cleaning request for a room |
| POST | `/api/cleaning/{id}/cancel` | Receptionist, Manager | Cancel a pending cleaning task |
| POST | `/api/cleaning/{id}/complete` | HousekeepingStaff | Mark a cleaning task as completed |
| POST | `/api/cleaning/{id}/verify` | HousekeepingSupervisor | Verify and approve completed cleaning |
| GET | `/api/cleaning/my-tasks` | HousekeepingStaff | Get task queue for the authenticated staff member |

### Staff Management (Schedule)
| Method | Path | Role | Description |
|--------|------|------|-------------|
| POST | `/api/schedule` | Manager | Create a new work schedule for a period |
| POST | `/api/schedule/{id}/publish` | Manager | Publish a draft schedule (notifies staff) |
| POST | `/api/schedule/{id}/draft` | Manager | Save schedule changes as draft |
| POST | `/api/schedule/{id}/shifts` | Manager | Assign a staff member to a shift |
| GET | `/api/schedule` | Manager | Get schedule overview with all shifts |
| GET | `/api/schedule/employees` | Manager | List employees with department and availability |
| GET | `/api/schedule/my-shifts` | Staff | Get personal shift list for authenticated user |
