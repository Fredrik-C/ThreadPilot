# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Vehicles/ThreadPilot.Vehicles.Api/ThreadPilot.Vehicles.Api.csproj", "src/Vehicles/ThreadPilot.Vehicles.Api/"]
COPY ["src/Vehicles/ThreadPilot.Vehicles.Application/ThreadPilot.Vehicles.Application.csproj", "src/Vehicles/ThreadPilot.Vehicles.Application/"]
COPY ["src/Vehicles/ThreadPilot.Vehicles.Domain/ThreadPilot.Vehicles.Domain.csproj", "src/Vehicles/ThreadPilot.Vehicles.Domain/"]
COPY ["src/Vehicles/ThreadPilot.Vehicles.Infrastructure/ThreadPilot.Vehicles.Infrastructure.csproj", "src/Vehicles/ThreadPilot.Vehicles.Infrastructure/"]
RUN dotnet restore "./src/Vehicles/ThreadPilot.Vehicles.Api/ThreadPilot.Vehicles.Api.csproj"
COPY . .
WORKDIR "/src/src/Vehicles/ThreadPilot.Vehicles.Api"
RUN dotnet build "./ThreadPilot.Vehicles.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ThreadPilot.Vehicles.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default for Release configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ThreadPilot.Vehicles.Api.dll"]