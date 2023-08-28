#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiGateways/Shopping.Aggregator/Shopping.Aggregator.csproj", "ApiGateways/Shopping.Aggregator/"]
RUN dotnet restore "ApiGateways/Shopping.Aggregator/Shopping.Aggregator.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Shopping.Aggregator"
RUN dotnet build "Shopping.Aggregator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "Shopping.Aggregator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shopping.Aggregator.dll"]