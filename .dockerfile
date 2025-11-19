FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://*:5000

RUN apt-get update \
    && apt-get install -y --no-install-recommends wget gnupg lsb-release \
    && echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" \
       > /etc/apt/sources.list.d/pgdg.list \
    && wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - \
    && apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client-15 postgresql-client-16 postgresql-client-17 postgresql-client-18\
    && rm -rf /var/lib/apt/lists/*

# Build React SPA
FROM node:20-alpine AS spa-build
WORKDIR /src
# Create the directory structure that matches the project layout
RUN mkdir -p /src/ClientApp /src/HexaSyncDatabaseDirectory/wwwroot
WORKDIR /src/ClientApp
COPY ClientApp/package.json ClientApp/package-lock.json* ./
RUN npm ci
COPY ClientApp/ ./
# Build the React SPA
RUN npm run build
# Debug: List the contents of various possible build output locations
RUN echo "Checking build output locations:" && \
    echo "1. /src/SimpleRDBMSRestfulAPI/wwwroot:" && \
    ls -la /src/SimpleRDBMSRestfulAPI/wwwroot || echo "SimpleRDBMSRestfulAPI/wwwroot not found" && \
    echo "2. Current directory:" && \
    ls -la && \
    echo "3. Parent directory:" && \
    ls -la /src && \
    echo "4. All wwwroot directories:" && \
    find /src -type d -name "wwwroot" | xargs -I{} ls -la {} || echo "No wwwroot directories found"

# Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SimpleRDBMSRestfulAPI/SimpleRDBMSRestfulAPI.csproj", "SimpleRDBMSRestfulAPI/"]
COPY ["nuget.config", "nuget.config"]
RUN dotnet restore "SimpleRDBMSRestfulAPI/SimpleRDBMSRestfulAPI.csproj"
COPY ./SimpleRDBMSRestfulAPI ./SimpleRDBMSRestfulAPI
WORKDIR "/src/SimpleRDBMSRestfulAPI"
RUN dotnet build "./SimpleRDBMSRestfulAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SimpleRDBMSRestfulAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
# Copy the React build output to the wwwroot directory
FROM publish AS copy-spa
COPY --from=spa-build /src/SimpleRDBMSRestfulAPI/wwwroot /app/publish/wwwroot

FROM base AS final
WORKDIR /app
COPY --from=copy-spa /app/publish .

# Set the entrypoint to match the run-production.sh script
ENTRYPOINT ["dotnet", "SimpleRDBMSRestfulAPI.dll"]