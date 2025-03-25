# Stage 1: Build and Test
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY back-end/*.csproj ./back-end/
COPY back-end-tests/*.csproj ./back-end-tests/

# Copy the full project folders
COPY back-end/ ./back-end/
COPY back-end-tests/ ./back-end-tests/

# Restore dependencies
RUN dotnet restore

# Build all projects
RUN dotnet build -c Release

# Run tests (will fail build if tests fail)
RUN dotnet test back-end-tests/back-end-tests.csproj --no-build --verbosity normal

# Publish the main app
WORKDIR /src/back-end
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "back-end.dll"]
