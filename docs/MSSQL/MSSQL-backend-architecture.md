# MSSQL Backend Architecture for Simple RDBMS RESTful API

## 1. Overview
This document describes the backend architecture for integrating Microsoft SQL Server (MSSQL) into the Simple RDBMS RESTful API. It synthesizes current codebase structure and brainstormed design considerations to guide robust, scalable, and secure MSSQL support.

## 2. Architectural Components

### 2.1. Data Access Layer
- **SQLServerDataHelper.cs**: Implements MSSQL-specific logic for CRUD operations, leveraging Dapper for efficient, parameterized queries.
- **BaseDataHelper.cs**: Abstract base class for RDBMS helpers, enforces adapter pattern for extensibility.
- **IDataHelper.cs**: Interface for data helpers, ensuring consistent API for all RDBMS types.

### 2.2. API Layer
- **EntityController.cs**: Routes DSL-based requests to the appropriate data helper (MSSQL, PostgreSQL, etc.). Handles entity CRUD operations.
- **SqlController.cs**: Handles raw SQL requests, enforcing parameterization and security.
- **ConnectionController.cs**: Manages connection info, supports multi-tenancy and secure storage.
- **HistoryController.cs**: Provides access to query and change history for auditing.

### 2.3. Query DSL & Security
- **HexaSync Query DSL**: Extended to support MSSQL-specific syntax (TOP, OUTPUT, identity columns).
- **SqlInjectionHelper.cs**: Validates queries, enforces parameterization, prevents SQL injection.
- **Authentication/Authorization**: Integrates with existing mechanisms (JWT, OAuth2, API keys).
- **Data Encryption**: Utilizes AES encryption for sensitive data at rest and in transit.

### 2.4. Multi-Tenancy & Configuration
- **ConnectionInfoDTO.cs / ConnectionInfoViewModel.cs**: Models for tenant-specific connection info.
- **AppSettings.cs / appsettings.json**: Centralized configuration, supports environment-based settings.

### 2.5. Logging & Auditing
- **RequestResponseLoggingMiddleware.cs**: Middleware for logging requests and responses.
- **SignalRLoggerConfigurationExtensions.cs / SignalRSink.cs / NotificationHub.cs**: Real-time logging and notifications via SignalR.

### 2.6. Frontend Integration
- **wwwroot/dashboard/**: Vue-based SPA for managing connections, entities, and logs.

## 3. Key Design Principles
- **Adapter Pattern**: Each RDBMS is supported via a dedicated helper class, enabling easy extension and maintenance.
- **Parameterization**: All queries use parameters to prevent SQL injection.
- **Extensibility**: MSSQL support is modular, allowing future enhancements (stored procedures, triggers, cloud features).
- **Scalability**: Connection pooling, stateless controllers, and efficient query handling ensure performance.
- **Security**: Defense in depth via authentication, authorization, encryption, and auditing.
- **Multi-Tenancy**: Tenant isolation at connection and query levels.

## 4. Implementation Roadmap
### Short-Term
- Complete CRUD logic in SQLServerDataHelper.cs
- Extend DSL for MSSQL features
- Add unit/integration tests for MSSQL endpoints
- Document MSSQL API usage

### Medium-Term
- Optimize query performance
- Support advanced MSSQL features (stored procedures, triggers, views)
- Enhance error handling and diagnostics
- Integrate MSSQL-specific monitoring

### Long-Term
- Support Azure SQL and Managed Instances
- Advanced security (row-level security, Always Encrypted)
- Community-driven extensions

## 5. Risks & Mitigations
- **SQL Injection**: Strict parameterization and DSL enforcement
- **Complexity**: Modular design, comprehensive documentation
- **Performance**: Connection pooling, query optimization
- **Security**: Encryption, RBAC, audit logging

## 6. References
- [SQLServerDataHelper.cs](../../src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLServerDataHelper.cs)
- [EntityController.cs](../../src/SimpleRDBMSRestfulAPI/Controllers/EntityController.cs)
- [SqlController.cs](../../src/SimpleRDBMSRestfulAPI/Controllers/SqlController.cs)
- [BaseDataHelper.cs](../../src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/BaseDataHelper.cs)
- [ProjectOverview.MD](../ProjectOverview.MD)
- [MSSQL-brainstorm.md](MSSQL-brainstorm.md)
