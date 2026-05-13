# Multi-stage Dockerfile: .NET 8 SDK ile build, runtime imajıyla minik container
# Hosting panellerinde (Coolify/CapRover/Dokploy vb.) Git+Dockerfile ile çalışır.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Bağımlılıkları önce restore et (Docker cache'i için)
COPY MuhasebeApp.Web/MuhasebeApp.Web.csproj MuhasebeApp.Web/
RUN dotnet restore MuhasebeApp.Web/MuhasebeApp.Web.csproj

# Geri kalanı kopyala ve publish et
COPY MuhasebeApp.Web/ MuhasebeApp.Web/
RUN dotnet publish MuhasebeApp.Web/MuhasebeApp.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime stage — sadece aspnet runtime, ~200MB
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# SQLite veritabanı için kalıcı dizin
RUN mkdir -p /app/App_Data && chmod 755 /app/App_Data

COPY --from=build /app/publish .

# Container içinde 8080 portunu dinle (Coolify/CapRover varsayılanı)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

EXPOSE 8080

# Sağlık kontrolü için basit endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
  CMD wget -qO- http://localhost:8080/Identity/Account/Login || exit 1

ENTRYPOINT ["dotnet", "MuhasebeApp.Web.dll"]
