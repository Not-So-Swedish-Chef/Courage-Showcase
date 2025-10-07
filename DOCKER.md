# Docker Setup Guide

This guide explains how to run the Courage Showcase application using Docker.

## Prerequisites

- Docker Desktop installed and running
- At least 4GB of RAM available for Docker
- Ports 1433, 5000, and 8081 available on your machine

## Quick Start

### 1. Build and Start All Services

From the root directory of the project, run:

```bash
docker-compose up --build
```

This will:
- Start SQL Server database
- Build and start the ASP.NET Core backend (including running tests)
- Build and start the Angular frontend with Nginx

### 2. Access the Application

- **Frontend**: http://localhost:8081
- **Backend API**: http://localhost:5000
- **Database**: localhost:1433
  - Username: `sa`
  - Password: `YourStrong!Passw0rd`

## Individual Service Commands

### Start services in detached mode (background)
```bash
docker-compose up -d
```

### View logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f database
```

### Stop all services
```bash
docker-compose down
```

### Stop and remove volumes (database data)
```bash
docker-compose down -v
```

### Rebuild a specific service
```bash
# Rebuild backend only
docker-compose build backend

# Rebuild and restart backend
docker-compose up -d --build backend
```

## Service Details

### Database Service
- **Container Name**: `courage_showcase_db`
- **Image**: SQL Server 2022
- **Port**: 1433
- **Data Persistence**: Uses named volume `mssql-data`
- **Health Check**: Automatically checks database availability before starting backend

### Backend Service
- **Container Name**: `courage_showcase_backend`
- **Port**: 5000 (host) → 8080 (container)
- **Build Context**: `./server`
- **Features**:
  - Runs unit tests during build (build fails if tests fail)
  - .NET 9.0 runtime
  - Auto-connects to database service
  - JWT authentication configured

### Frontend Service
- **Container Name**: `courage_showcase_frontend`
- **Port**: 8081 (host) → 80 (container)
- **Build Context**: `./frontend`
- **Server**: Nginx
- **Features**:
  - Production-optimized Angular build
  - Gzip compression
  - Static asset caching
  - SPA routing support

## Environment Variables

### Backend
- `ASPNETCORE_ENVIRONMENT`: Development
- `ConnectionStrings__DefaultConnection`: Database connection string
- `Jwt__Issuer`: JWT token issuer
- `Jwt__Audience`: JWT token audience
- `Jwt__Secret`: JWT signing secret
- `Jwt__TokenValidityMins`: Token validity duration

### Frontend
- `API_URL`: Backend API URL (for runtime configuration if needed)

## Troubleshooting

### Database connection issues
```bash
# Check database health
docker-compose ps database

# View database logs
docker-compose logs database

# Restart database
docker-compose restart database
```

### Backend build fails
```bash
# View build output
docker-compose build backend --no-cache

# Check if tests are failing
docker-compose logs backend
```

### Frontend not loading
```bash
# Check if nginx is running
docker-compose ps frontend

# View nginx logs
docker-compose logs frontend

# Rebuild frontend
docker-compose up -d --build frontend
```

### Clearing everything and starting fresh
```bash
# Stop all containers
docker-compose down

# Remove volumes (WARNING: This deletes database data)
docker-compose down -v

# Remove all images
docker-compose down --rmi all

# Rebuild everything
docker-compose up --build
```

## Development Workflow

### Making backend changes
1. Make your code changes
2. Rebuild and restart:
   ```bash
   docker-compose up -d --build backend
   ```

### Making frontend changes
1. Make your code changes
2. Rebuild and restart:
   ```bash
   docker-compose up -d --build frontend
   ```

### Database migrations
```bash
# Run migrations inside the backend container
docker-compose exec backend dotnet ef database update
```

## Network Architecture

All services are connected via the `courage-network` bridge network, allowing them to communicate using service names:
- Frontend can reach backend at `http://backend:8080`
- Backend can reach database at `database:1433`

## Security Notes

⚠️ **IMPORTANT**: The current configuration uses default credentials and secrets suitable for development only.

For production:
1. Change `SA_PASSWORD` to a strong, unique password
2. Update `Jwt__Secret` to a secure random string
3. Use environment-specific configuration files
4. Consider using Docker secrets or a secret management service
5. Enable HTTPS/TLS
6. Update CORS policies in backend

## Additional Commands

### Execute commands in containers
```bash
# Backend
docker-compose exec backend bash

# Database
docker-compose exec database /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -No -C
```

### View container resource usage
```bash
docker stats
```

### Inspect networks
```bash
docker network ls
docker network inspect courage-network
```

