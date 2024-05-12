#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY ../fridgeapp.pfx /https/

ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/fridgeapp.pfx"

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fridge/Fridge.API.csproj", "Fridge/"]
COPY ["Fridge.DAL/Fridge.DAL.csproj", "Fridge.DAL/"]
COPY ["Fridge.Domain/Fridge.Domain.csproj", "Fridge.Domain/"]
COPY ["Fridge.Infrastructure/Fridge.Infrastructure.csproj", "Fridge.Infrastructure/"]
RUN dotnet restore "./Fridge/Fridge.API.csproj"
COPY . .
WORKDIR "/src/Fridge"
RUN dotnet build "./Fridge.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fridge.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fridge.API.dll"]