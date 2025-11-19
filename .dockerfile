FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
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

# Build Vue/Vite dashboard
FROM node:20-alpine AS spa-build
WORKDIR /src/SimpleRDBMSRestfulAPI/wwwroot/dashboard
COPY src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/package*.json ./
RUN npm ci
COPY src/SimpleRDBMSRestfulAPI/wwwroot/dashboard ./
RUN npm run build

# Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SimpleRDBMSRestfulAPI/SimpleRDBMSRestfulAPI.csproj", "SimpleRDBMSRestfulAPI/"]
COPY ["src/SimpleRDBMSRestfulAPI/nuget.config", "nuget.config"]
RUN dotnet restore "SimpleRDBMSRestfulAPI/SimpleRDBMSRestfulAPI.csproj"
COPY src/SimpleRDBMSRestfulAPI ./SimpleRDBMSRestfulAPI
WORKDIR "/src/SimpleRDBMSRestfulAPI"
RUN dotnet build "./SimpleRDBMSRestfulAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SimpleRDBMSRestfulAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
# Copy the Vue/Vite build output to the wwwroot directory
FROM publish AS copy-spa
COPY --from=spa-build /src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/dist /app/publish/wwwroot/dashboard/dist

FROM base AS final
WORKDIR /app
COPY --from=copy-spa /app/publish .

# Set the entrypoint to match the run-production.sh script
ENTRYPOINT ["dotnet", "SimpleRDBMSRestfulAPI.dll"]