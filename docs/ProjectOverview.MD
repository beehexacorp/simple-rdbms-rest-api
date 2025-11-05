
# Simple RDBMS RESTful API

## 1. Introduction & Purpose
Simple RDBMS REST API provides a secure, scalable, and multi-tenant RESTful interface for interacting with relational database management systems (RDBMS) using standard HTTP methods and a HexaSync-defined Query DSL. Designed for SaaS vendors, enterprise IT, and integration partners, it enables seamless database operations with robust security and extensibility.

## 2. Supported RDBMS Systems
- Currently supports:
  - [PostgreSQL](/src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/PostgresDataHelper.cs)
  - [SQLAnywhere](/src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLAnywhereDataHelper.cs)
- Roadmap: MySQL, Oracle, SQL Server, and more.

## 3. API Features
- **CRUD Operations:** Well-defined endpoints for Create, Read, Update, Delete.
- **Query DSL:** HexaSync DSL for expressive, secure queries; prevents SQL injection.
- **Multi-Tenancy:** Tenant isolation, data partitioning, onboarding workflow.

## 4. Security
- **Authentication:** Supports JWT, OAuth2, API keys.
- **Authorization:** Role-based access, resource-level permissions.
- **SQL Injection Prevention:** Input validation, parameterized queries, DSL enforcement.
- **Data Encryption:** Encryption at-rest and in-transit; secure key management.

## 5. Scalability & Performance
- Handles multiple concurrent requests efficiently.
- Implements rate limiting, connection pooling, and caching strategies.
- Designed for load balancing and failover.

## 6. Configuration & Extensibility
- **Logging:** Configurable providers (console, file, database, Datadog, Hubspot, HexaSync Incident Service).
- **Settings:** Environment-based configuration, secrets management.
- **Extending RDBMS Support:** Adapter/plugin architecture for new databases.

## 7. Error Handling & Auditing
- Standardized error responses with meaningful messages and HTTP status codes.
- Incident logging and audit trails; integration with external services.
- Health checks, metrics, and alerting for monitoring.

## 8. Documentation & Developer Experience
- Comprehensive API documentation (Swagger/OpenAPI), code samples, and usage guides.
- Quickstart onboarding and sandbox environment.
- Support channels: community, ticketing, escalation.

## 9. Roadmap & Future Enhancements
- Planned features: Write operations, analytics endpoints, admin dashboard.
- Community contributions: guidelines and governance.