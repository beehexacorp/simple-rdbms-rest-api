# Story Drafts: MSSQL Integration for Simple RDBMS RESTful API

---
## Story 1: CRUD Operations for MSSQL via REST Endpoints
**As an API consumer, I want to perform CRUD operations on MSSQL databases via REST endpoints so that I can manage my data securely and efficiently.**
- Acceptance Criteria:
  - API supports Create, Read, Update, Delete for MSSQL
  - All operations use parameterized queries
  - Unit and integration tests cover all CRUD endpoints

### Dev Agent Record
#### Tasks / Subtasks
- [x] Implement MSSQL CRUD logic in SQLServerDataHelper.cs
- [x] Ensure EntityController.cs and SqlController.cs route MSSQL requests
- [ ] Validate frontend entityService.ts integration
- [ ] Add/extend unit and integration tests for MSSQL CRUD endpoints
- [ ] Run tests and verify acceptance criteria

#### Debug Log
- CRUD logic implemented in SQLServerDataHelper.cs using Dapper and parameterized queries.
- EntityController.cs and SqlController.cs reviewed for MSSQL routing.
- Frontend entityService.ts integration pending validation.
- No test files found; recommend adding under src/SimpleRDBMSRestfulAPI/Tests/.

#### Completion Notes
- MSSQL CRUD logic implemented and ready for API routing and frontend validation.

#### File List
- src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLServerDataHelper.cs
- src/SimpleRDBMSRestfulAPI/Controllers/EntityController.cs
- src/SimpleRDBMSRestfulAPI/Controllers/SqlController.cs
- src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/src/services/entityService.ts

---
## Story 2: HexaSync DSL Support for MSSQL Syntax
**As a developer, I want the HexaSync Query DSL to support MSSQL-specific syntax so that I can write expressive and optimized queries for MSSQL.**
- Acceptance Criteria:
  - DSL supports MSSQL features (TOP, OUTPUT, identity columns)
  - Documentation is available for MSSQL DSL usage
  - DSL queries are validated and tested

### Dev Agent Record
#### Tasks / Subtasks
- [ ] Extend HexaSync DSL parser to support MSSQL-specific syntax (TOP, OUTPUT, identity columns)
- [ ] Update SQLServerDataHelper.cs to handle new DSL features
- [ ] Write documentation for MSSQL DSL usage
- [ ] Add/extend unit tests for DSL validation and MSSQL queries
- [ ] Validate DSL queries against MSSQL

#### Debug Log
- HexaSync DSL parser requires extension for MSSQL features.
- SQLServerDataHelper.cs will need updates for new DSL constructs.
- Documentation and tests pending.

#### Completion Notes
- MSSQL DSL support planned; parser and helper updates required.

#### File List
- src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLServerDataHelper.cs
- src/SimpleRDBMSRestfulAPI/Helpers/SqlInjectionHelper.cs
- src/SimpleRDBMSRestfulAPI/Models/EntityRequestMetadata.cs
- docs/MSSQL/MSSQL-DSL-usage.md (to be created)

---
## Story 3: Multi-Tenancy Isolation for MSSQL
**As a tenant admin, I want my MSSQL connections and queries to be isolated from other tenants so that my data remains secure and private.**
- Acceptance Criteria:
  - Tenant connection info and queries are isolated
  - Multi-tenancy enforced at API and data access layers
  - Security tests verify isolation

### Dev Agent Record
#### Tasks / Subtasks
- [ ] Ensure tenant connection info is isolated in ConnectionInfoDTO.cs and ConnectionInfoViewModel.cs
- [ ] Enforce multi-tenancy in EntityController.cs, SqlController.cs, and SQLServerDataHelper.cs
- [ ] Add/extend security tests for tenant isolation
- [ ] Validate isolation at API and data access layers

#### Debug Log
- Multi-tenancy enforcement requires updates to connection info models and controllers.
- Security tests for isolation pending.

#### Completion Notes
- Multi-tenancy isolation planned; model and controller updates required.

#### File List
- src/SimpleRDBMSRestfulAPI/Settings/ConnectionInfoDTO.cs
- src/SimpleRDBMSRestfulAPI/Settings/ConnectionInfoViewModel.cs
- src/SimpleRDBMSRestfulAPI/Controllers/EntityController.cs
- src/SimpleRDBMSRestfulAPI/Controllers/SqlController.cs
- src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLServerDataHelper.cs
- src/SimpleRDBMSRestfulAPI/Tests/SecurityTests.cs (to be created)

---
## Story 4: Secure, Audited MSSQL API Operations
**As a security officer, I want all MSSQL API operations to be authenticated, authorized, and audited so that compliance requirements are met.**
- Acceptance Criteria:
  - API requires JWT/OAuth2/API key authentication for MSSQL endpoints
  - All operations are logged and auditable
  - Audit logs are accessible for review

### Dev Agent Record
#### Tasks / Subtasks
- [ ] Implement authentication/authorization for MSSQL endpoints (JWT/OAuth2/API key)
- [ ] Ensure all MSSQL operations are logged and auditable (RequestResponseLoggingMiddleware.cs, SignalRLoggerConfigurationExtensions.cs)
- [ ] Provide access to audit logs via API or dashboard
- [ ] Add/extend tests for authentication, authorization, and auditing

#### Debug Log
- Authentication/authorization mechanisms need to be enforced for MSSQL endpoints.
- Logging and audit trail integration pending.

#### Completion Notes
- Secure, audited MSSQL API operations planned; middleware and logging updates required.

#### File List
- src/SimpleRDBMSRestfulAPI/Middleware/RequestResponseLoggingMiddleware.cs
- src/SimpleRDBMSRestfulAPI/Libs/SignalRLoggerConfigurationExtensions.cs
- src/SimpleRDBMSRestfulAPI/Libs/SignalRSink.cs
- src/SimpleRDBMSRestfulAPI/Libs/NotificationHub.cs
- src/SimpleRDBMSRestfulAPI/Controllers/EntityController.cs
- src/SimpleRDBMSRestfulAPI/Controllers/SqlController.cs
- src/SimpleRDBMSRestfulAPI/Tests/AuthAuditTests.cs (to be created)

---
## Story 5: Monitoring & Error Handling for MSSQL Endpoints
**As an operations engineer, I want to monitor MSSQL API performance and errors so that I can ensure reliability and quickly resolve issues.**
- Acceptance Criteria:
  - Requests, responses, and errors are logged
  - MSSQL-specific metrics are available for monitoring
  - Alerts are configured for critical errors

### Dev Agent Record
#### Tasks / Subtasks
- [ ] Integrate logging for MSSQL requests, responses, and errors (RequestResponseLoggingMiddleware.cs)
- [ ] Expose MSSQL-specific metrics via API or dashboard
- [ ] Configure alerts for critical MSSQL errors
- [ ] Add/extend tests for error handling and monitoring

#### Debug Log
- Logging and metrics integration for MSSQL endpoints pending.
- Alerting and error handling require configuration and tests.

#### Completion Notes
- Monitoring and error handling planned; middleware and dashboard updates required.

#### File List
- src/SimpleRDBMSRestfulAPI/Middleware/RequestResponseLoggingMiddleware.cs
- src/SimpleRDBMSRestfulAPI/Libs/SignalRLoggerConfigurationExtensions.cs
- src/SimpleRDBMSRestfulAPI/Libs/SignalRSink.cs
- src/SimpleRDBMSRestfulAPI/Libs/NotificationHub.cs
- src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/src/components/RealtimeLog.vue
- src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/src/components/HistoryLogs.vue
- src/SimpleRDBMSRestfulAPI/Tests/MonitoringTests.cs (to be created)

---
## Story 6: Extensible MSSQL Integration
**As a developer, I want the MSSQL integration to be extensible so that future features (stored procedures, triggers, cloud integration) can be added with minimal refactoring.**
- Acceptance Criteria:
  - MSSQL support follows the adapter pattern
  - New features can be added via modular extensions
  - Codebase is documented for extensibility

### Dev Agent Record
#### Tasks / Subtasks
- [ ] Ensure MSSQL integration follows adapter pattern in SQLServerDataHelper.cs and BaseDataHelper.cs
- [ ] Refactor for modular extension support
- [ ] Document codebase for MSSQL extensibility
- [ ] Add/extend tests for extensibility and future features

#### Debug Log
- Adapter pattern and modular extension support for MSSQL integration pending review and refactor.
- Documentation and extensibility tests required.

#### Completion Notes
- Extensible MSSQL integration planned; refactor and documentation required.

#### File List
- src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLServerDataHelper.cs
- src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/BaseDataHelper.cs
- docs/MSSQL/MSSQL-extensibility.md (to be created)
- src/SimpleRDBMSRestfulAPI/Tests/ExtensibilityTests.cs (to be created)

---
## Story 7: Documentation for MSSQL API Usage
**As a user, I want clear documentation for MSSQL API usage so that I can onboard quickly and use the features effectively.**
- Acceptance Criteria:
  - API documentation for MSSQL endpoints and DSL is published and accessible
  - Example queries and usage guides are provided

### Dev Agent Record
#### Tasks / Subtasks
- [ ] Write and publish API documentation for MSSQL endpoints and DSL
- [ ] Provide example queries and usage guides
- [ ] Validate documentation accessibility and clarity
- [ ] Add/extend tests for documentation accuracy

#### Debug Log
- API documentation and usage guides for MSSQL endpoints and DSL pending creation.
- Validation and tests required.

#### Completion Notes
- MSSQL API documentation planned; guides and examples to be created.

#### File List
- docs/MSSQL/MSSQL-API-usage.md (to be created)
- docs/MSSQL/MSSQL-DSL-usage.md
- src/SimpleRDBMSRestfulAPI/wwwroot/dashboard/src/views/Documentation.vue
- src/SimpleRDBMSRestfulAPI/Tests/DocumentationTests.cs (to be created)

---
