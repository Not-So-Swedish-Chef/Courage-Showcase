# Backend - ASP.NET Core (NET 9)
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files and restore as separate layers for caching
COPY back-end.sln ./
COPY back-end/back-end.csproj back-end/
COPY back-end-tests/back-end-tests.csproj back-end-tests/
RUN dotnet restore back-end.sln

# Copy the rest of the source
COPY back-end/ back-end/
COPY back-end-tests/ back-end-tests/

# Build the solution
RUN dotnet build back-end.sln -c Release

# Run tests (fail the build if tests fail)
RUN dotnet test back-end-tests/back-end-tests.csproj -c Release --no-build --logger "trx;LogFileName=test_results.trx"

# Publish the API
WORKDIR /src/back-end
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
# Listen on 8080 internally
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "back-end.dll"]

