# Simple RDBMS RESTful API

A lightweight RESTful API designed to help integrate legacy systems with RDBMS databases. This API provides a simple way to query legacy databases that don't have an internet connection or a built-in API, enabling easy communication with modern applications.

## Table of Contents

- [Simple RDBMS RESTful API](#simple-rdbms-restful-api)
  - [Table of Contents](#table-of-contents)
  - [Introduction](#introduction)
  - [Pre-requisites](#pre-requisites)
  - [Key Features](#key-features)
  - [Installation](#installation)
    - [Windows (IIS or Windows Service)](#windows-iis-or-windows-service)
    - [Linux/MacOS (nginx)](#linuxmacos-nginx)
  - [API Endpoints](#api-endpoints)
    - [1. Entity Query](#1-entity-query)
    - [2. SQL Query](#2-sql-query)
    - [3. Dashboard](#3-dashboard)
  - [Security Considerations](#security-considerations)
  - [Contributing](#contributing)
  - [License](#license)

## Introduction

Legacy systems often rely on RDBMS databases with no internet connectivity or APIs, making them difficult to integrate with modern applications. This project provides a simple RESTful API that allows users to interact with these legacy databases over the internet securely, without exposing them directly.

**Why this API exists:**
- Exposing databases directly to the internet can lead to security risks.
- Legacy apps without built-in APIs can still interact with databases using this API.
- Complex database configurations or OS security settings often make database connections challenging.
- Exposing raw database commands like `DROP DATABASE` or `ALTER TABLE` can be risky, and this API prevents such operations.

## Pre-requisites

Before you can run the Simple RDBMS RESTful API, ensure the following are installed:

1. **Install Node.js**
   - Go to the [official Node.js website](https://nodejs.org/).
   - Download the latest LTS version.
   - Run the installer and follow the on-screen instructions to complete the installation.
   - Verify the installation by opening a terminal or command prompt and running:
     ```bash
     node -v
     npm -v
     ```

2. **Install .NET Core 8.0 SDK and Hosting Bundle**
   - Download the .NET Core 8.0 SDK and Hosting Bundle from the [official .NET website](https://dotnet.microsoft.com/download/dotnet).
   - Run the installer and follow the instructions.
   - Verify the installation by running the following command in a terminal or command prompt:
     ```bash
     dotnet --version
     ```

3. **Install IIS (If using Windows IIS)**
   - Open "Control Panel" > "Programs" > "Turn Windows features on or off".
   - Check "Internet Information Services" (IIS) and click "OK" to install IIS.
   - Verify that IIS is running by opening a browser and navigating to `http://localhost`. You should see the IIS welcome page.

## Key Features

1. **Entity Query**: Query your RDBMS tables with a simple API. You can specify the table name, fields, filter conditions, order, and more.
2. **SQL Query**: Execute raw SQL queries to fetch data from the database using a simple endpoint.
3. **Dashboard**: Provides a user-friendly dashboard for setting up database connections, viewing real-time logs, monitoring history logs, and accessing documentation for tables, fields, and API usage examples.

## Installation

### Windows (IIS or Windows Service)

1. Download the release for Windows.
2. Install the application on your server.
3. Configure it to run as an IIS web application or Windows Service.
4. Access the API via the configured URL.

### Linux/MacOS (nginx)

1. Download the release for Linux/MacOS.
2. Extract the files and configure the API to run through `nginx` for hosting.
3. Ensure your firewall settings allow connections only from trusted sources.
4. Access the API through your domain.

## API Endpoints

### 1. Entity Query

**Method**: `POST /api/{connectionId}/entity`

This endpoint allows users to query data from a specific table in the RDBMS. You can specify the `connectionId`, which corresponds to a database connection set up in the Dashboard. You can also specify the following parameters:

- **tableName**: The name of the table to query.
- **fields**: A comma-separated list of fields to retrieve.
- **filter**: Conditions to filter the results (e.g., `columnName = 'value'`).
- **orderBy**: Specify columns to order the results by (e.g., `columnName ASC`).

**Request Body Format**:
```json
{
  "tableName": "users",
  "fields": ["id", "name", "email"],
  "conditions": ["age > 30"],
  "orderBy": "name ASC",
  "params": {
    "age": 30
  }
}
```

**Controller**: `EntityController.cs`

This controller handles all entity-related queries. It validates the user inputs, sanitizes the query to prevent SQL injections, and executes the database query to return the result in a structured format.

### 2. SQL Query

**Method**: `POST /api/{connectionId}/sql`

This endpoint allows users to execute raw SQL queries against the database. It is useful when more complex queries are required that cannot be achieved using the entity query endpoint.

- **query**: A raw SQL query string.

**Request Body Format**:
```json
{
  "query": "SELECT * FROM users WHERE age > @age",
  "params": {
    "age": 30
  }
}
```

**Controller**: `SqlController.cs`

The `SqlController` is responsible for handling raw SQL queries. It also performs security checks to prevent SQL injection attacks. This endpoint gives users the flexibility to run custom queries directly against the database.

### 3. Dashboard

**Method**: `GET /api/{connectionId}/dashboard`

This endpoint serves the dashboard UI, which allows users to:

- **Setup Database Connection**: Configure the connection to the legacy system’s database.
- **Real-Time Logs**: View logs of the API’s activities in real time.
- **History Logs**: Access historical logs of past API interactions.
- **Documentation**: Access detailed documentation, including the schema of tables and fields and example API calls.

**Controller**: `DashboardController.cs`

This controller handles the logic for displaying the dashboard, reading logs, and providing documentation for available tables and fields. The dashboard serves as a centralized page for users to manage their API interactions and monitor the system’s status.

## Security Considerations

This API has basic security checks for SQL injections, but it is not robust enough to be exposed directly to the internet. We recommend using a tunneling service (such as [Cloudflared Tunnel](https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/)) to secure the API endpoint and only share it with trusted parties who are aware of the risks involved.

Please note that certain database operations like `DROP DATABASE`, `DROP TABLE`, and `ALTER TABLE` are intentionally restricted to prevent accidental data loss.

## Contributing

This is an open-source project, and we welcome contributions. To get started:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Submit a pull request with a description of your changes.

Please ensure that your contributions align with the project's goals and maintain the simplicity of the API.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.