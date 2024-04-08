#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV TZ=America/Sao_Paulo
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MotoRental.API/MotoRental.API.csproj", "MotoRental.API/"]
COPY ["MotoRental.Core/MotoRental.Core.csproj", "MotoRental.Core/"]
COPY ["MotoRental.Infrastructure/MotoRental.Infrastructure.csproj", "MotoRental.Infrastructure/"]
COPY ["MotoRental.Service/MotoRental.Service.csproj", "MotoRental.Service/"]
RUN dotnet restore "./MotoRental.API/./MotoRental.API.csproj"
COPY . .
WORKDIR "/src/MotoRental.API"
RUN dotnet build "./MotoRental.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MotoRental.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MotoRental.API.dll"]