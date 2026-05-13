# Multi-stage Dockerfile — Blazor Server .NET 8
# Dokploy / Coolify / CapRover gibi paneller bunu otomatik kullanir.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Önce sadece csproj (Docker katman önbelleği için)
COPY *.csproj ./
RUN dotnet restore

# Tüm kaynak kodu
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Runtime imajı — sadece ASP.NET Core runtime, ~210 MB
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# SQLite için kalıcı dizin
RUN mkdir -p /app/App_Data && chmod 777 /app/App_Data

COPY --from=build /app/publish .

# 0.0.0.0 ile bind — bazı reverse proxy'lerde IPv6-only sorunlarını önler
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost:8080/Identity/Account/Login || exit 1

ENTRYPOINT ["dotnet", "MuhasebeApp.Web.dll"]
