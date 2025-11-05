# Simple RDBMS RESTful API

## Overview

* Simple RDBMS REST API project provides a RESTful interface to interact with a relational database management system (RDBMS) using standard HTTP methods and a specific HexaSync Defined Query DSL Language with high security support that prevent SQL Injection attacks.
* The project currently supports (READ or QUERY operations) the following RDBMS systems:
    * [PostgreSQL](/src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/PostgresDataHelper.cs)
    * [SQLAnywhere](/src/SimpleRDBMSRestfulAPI/Helpers/RDBMS/SQLAnywhereDataHelper.cs)
* The API allows clients to perform CRUD (Create, Read, Update, Delete) operations on database tables through well-defined endpoints.
* This API Application should be architectured as Multiple-Tenant SaaS application with robust security measures, including authentication and authorization mechanisms to ensure secure access to database resources.
* The API should be designed for scalability and performance, capable of handling multiple concurrent requests efficiently.
* The connection information provided by the clients must be securely stored using encryption techniques to protect sensitive data.
* The API must authorize requests and ensure that clients can only access the database resources they are permitted to interact with.
* The API endpoints should be well-documented, providing clear instructions on how to use the API effectively.
* The API endpoints should be designed to handle errors gracefully, providing meaningful error messages and appropriate HTTP status codes in response to client requests and log the incidents for auditing and debugging purposes.
  * The Logging Provider must be configurable to support different logging mechanisms (e.g., console, file, database, datadog, Hubspot Tickets, HexaSync Incident Service etc.) based on application settings.