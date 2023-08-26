#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Basket/Basket.API/Basket.API.csproj", "Services/Basket/Basket.API/"]
COPY ["Shared/EventBus.Messages/EventBus.Messages.csproj", "Shared/EventBus.Messages/"]
COPY ["Services/Basket/Basket.DataAccess/Basket.DataAccess.csproj", "Services/Basket/Basket.DataAccess/"]
COPY ["Services/Basket/Basket.Entities/Basket.Entities.csproj", "Services/Basket/Basket.Entities/"]
COPY ["Services/Basket/Basket.Utility/Basket.Utility.csproj", "Services/Basket/Basket.Utility/"]
COPY ["Services/Basket/Basket.Services/Basket.Services.csproj", "Services/Basket/Basket.Services/"]
RUN dotnet restore "Services/Basket/Basket.API/Basket.API.csproj"
COPY . .
WORKDIR "/src/Services/Basket/Basket.API"
RUN dotnet build "Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.API.dll"]