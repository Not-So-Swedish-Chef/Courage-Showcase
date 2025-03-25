# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only the main project files
COPY back-end/*.csproj ./back-end/
COPY back-end/. ./back-end/

WORKDIR /src/back-end
RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000
ENTRYPOINT ["dotnet", "back-end.dll"]
