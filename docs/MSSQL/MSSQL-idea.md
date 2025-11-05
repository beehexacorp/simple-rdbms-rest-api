# MSSQL

## Overview
This document outlines the design ideas and considerations for integrating Microsoft SQL Server (MSSQL) support into the Simple RDBMS Restful API project. It covers key features, security measures, scalability aspects, configuration options, error handling, documentation, and a roadmap for future enhancements.

## References

* [SQLServerDataHelper](src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLServerDataHelper.cs): Implement detail logic for MS SQL Server dialect that support query, update, insert, delete operations.
* [HexaSync Query DSL Endpoint](/src/SimpleRDBMSRestfulAPI/Controllers/EntityController.cs): The main API endpoint that will utilize the generic/dynamic data helper for processing requests that will transformed to specific database query/command based on the Connection ID & Type.
* [HexaSync Raw Query Endpoint](/src/SimpleRDBMSRestfulAPI/Controllers/SqlController.cs): The main API endpoint that will utilize the generic/dynamic data helper for processing raw SQL requests that will be executed directly on the target database based on the Connection ID & Type.

## Key Features

- Should use Dapper for database interactions to ensure performance and security.
- Must avoid SQL Injection vulnerabilities by leveraging parameterized queries and the existing HexaSync Query DSL.
- Should support CRUD operations (Create, Read, Update, Delete) for MSSQL databases.
- Must enforce user to provide parameters instead of concatenating raw SQL strings.
- Should handle connection pooling for efficient resource management.
