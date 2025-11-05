# Simple RDBMS RESTful API – Project Documentation

## 1. Introduction & Purpose
Simple RDBMS REST API is a secure, scalable, multi-tenant RESTful service for interacting with RDBMS systems. It uses standard HTTP methods and a HexaSync-defined Query DSL to enable safe, expressive database operations for SaaS vendors, enterprise IT, and integration partners.

## 2. Current Implementation

### Supported RDBMS Systems
- **PostgreSQL**: Implemented in `Helpers/RDBMS/PostgresDataHelper.cs`
- **SQLAnywhere**: Implemented in `Helpers/RDBMS/SQLAnywhereDataHelper.cs`
- **Planned/Stubbed**: MySQL (`Helpers/RDBMS/MySQLDataHelper.cs`), Oracle (`Helpers/RDBMS/OracleDataHelper.cs`), SQL Server (`Helpers/RDBMS/SQLServerDataHelper.cs`)

### API Features
- **CRUD Endpoints**: Controllers for connection (`ConnectionController.cs`), entity (`EntityController.cs`), SQL (`SqlController.cs`), and history (`HistoryController.cs`)
- **Query DSL**: HexaSync DSL parsing and validation (see `SqlInjectionHelper.cs`)
- **Multi-Tenancy**: Tenant isolation and connection info managed via DTOs and ViewModels (`Settings/ConnectionInfoDTO.cs`, `Settings/ConnectionInfoViewModel.cs`)
- **Security**:
  - Authentication/Authorization: Interfaces and stubs present, extensible for JWT/OAuth2/API keys
  - SQL Injection Prevention: `SqlInjectionHelper.cs` and DSL enforcement
  - Data Encryption: AES encryption utilities (`Libs/AESEncryptorExtensions.cs`, `Libs/MachineAESEncryptor.cs`)
- **Scalability & Performance**:
  - Concurrency: ASP.NET Core controllers, stateless design
  - Connection pooling: Supported via Dapper and native drivers
  - Caching: Not yet implemented
- **Configuration & Extensibility**:
  - Logging: Pluggable via Serilog and SignalR (`Libs/SignalRLoggerConfigurationExtensions.cs`, `Libs/SignalRSink.cs`, `Libs/NotificationHub.cs`)
  - Settings: `appsettings.json`, `appsettings.Development.json`, environment-based config
  - RDBMS Extensibility: Adapter pattern in `BaseDataHelper.cs` and derived helpers
- **Error Handling & Auditing**:
  - Standardized error responses in controllers
  - Logging middleware (`Middleware/RequestResponseLoggingMiddleware.cs`)
  - Audit trails: Not yet fully implemented
- **Documentation & Developer Experience**:
  - API documentation: Not yet auto-generated (Swagger/OpenAPI recommended)
  - Frontend dashboard: Vue-based SPA in `wwwroot/dashboard/`
  - Quickstart and onboarding: Not yet present
  - Support channels: Not yet integrated

## 3. Roadmap & Future Enhancements

### Short-Term (Next Release)
- Complete CRUD support for all planned RDBMS (MySQL, Oracle, SQL Server)
- Implement Swagger/OpenAPI documentation
- Add caching layer for query results
- Expand authentication/authorization (JWT, OAuth2)
- Enhance error handling and auditing (full incident logging, audit trails)
- Improve onboarding and developer guides

### Medium-Term
- Add analytics endpoints and admin dashboard
- Integrate external logging providers (Datadog, Hubspot, HexaSync Incident Service)
- Implement advanced multi-tenancy features (tenant onboarding, billing hooks)
- Expand support for additional RDBMS (e.g., SQLite, MariaDB)
- Add sandbox environment for API testing

### Long-Term
- Community contribution guidelines and governance
- Pluggable architecture for custom query DSLs and RDBMS adapters
- Advanced monitoring, metrics, and alerting integrations
- Support for write operations with transactional safety
- Enterprise features: SSO, RBAC, compliance modules

## 4. File Structure Overview
- `src/SimpleRDBMSRestfulAPI/Controllers/`: API endpoints
- `src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/`: RDBMS adapters
- `src/SimpleRDBMSRestfulAPI/Libs/`: Encryption, logging, SignalR
- `src/SimpleRDBMSRestfulAPI/Middleware/`: Logging and request/response middleware
- `src/SimpleRDBMSRestfulAPI/Settings/`: Configuration, DTOs, ViewModels
- `src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/`: Frontend SPA (Vue)
- `src/SimpleRDBMSRestfulAPI/appsettings*.json`: Application configuration

## 5. Key Gaps & Recommendations
- **Documentation**: Add Swagger/OpenAPI and developer guides
- **Security**: Implement full authentication/authorization flows
- **Auditing**: Expand incident logging and audit trail support
- **Extensibility**: Document adapter/plugin architecture for new RDBMS
- **Developer Experience**: Add onboarding, quickstart, and sandbox features
