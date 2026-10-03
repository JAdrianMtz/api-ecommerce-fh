# ApiEcommerce

RESTful API for a simple e-commerce application built with ASP.NET Core 10 and Entity Framework Core. The project provides product and category management, user authentication, and purchasing functionality, with an emphasis on backend development practices and the .NET ecosystem.

## Overview

ApiEcommerce is a backend application developed to explore and apply common patterns and technologies used in modern .NET Web API development.

The application uses SQL Server for data persistence, Entity Framework Core for database access, and ASP.NET Core Identity with JWT Bearer authentication for user management and access control. It also includes API versioning, pagination, filtering, caching, centralized exception handling, and Swagger documentation.

## Technologies

| Technology                     | Purpose                                                              |
| ------------------------------ | -------------------------------------------------------------------- |
| C# / .NET 10                   | Application development                                              |
| ASP.NET Core Web API           | HTTP endpoints and request handling                                  |
| Entity Framework Core          | Object-relational mapping and database access                        |
| SQL Server                     | Relational database                                                  |
| ASP.NET Core Identity          | User and role management                                             |
| JWT Bearer                     | Authentication and token validation                                  |
| AutoMapper                     | Object and DTO mapping                                               |
| BCrypt                         | Password hashing in the initial manual authentication implementation |
| Asp.Versioning.Mvc             | API versioning                                                       |
| Asp.Versioning.Mvc.ApiExplorer | Versioned API exploration and documentation                          |
| Swagger / OpenAPI              | API documentation                                                    |
| ASP.NET Core Output Cache      | Response caching                                                     |

## Features

* Product and category management with CRUD operations.
* Product pagination, category filtering, and search.
* Product purchasing endpoint.
* User registration and login.
* JWT-based authentication and role-based authorization.
* User and role management with ASP.NET Core Identity.
* Database migrations and automatic data seeding.
* DTOs and object mapping with AutoMapper.
* API versioning with separate Swagger documentation.
* Centralized exception handling using ASP.NET Core middleware.
* CORS configuration.
* Output caching.
* Repository-based data access.
* Dependency injection.

## Authentication and Authorization

The API uses ASP.NET Core Identity for user management and JWT Bearer tokens for authentication.

Protected endpoints require a valid access token, which must be included in the HTTP `Authorization` header using the Bearer scheme.

```http
Authorization: Bearer <access_token>
```

The application defines two user roles:

* **Admin** — administrative role.
* **User** — standard user role.

User registration and login are available through the Users endpoints. Authorization is applied to protected operations.

During the initial development of the project, manual user management and BCrypt password hashing were explored before adopting ASP.NET Core Identity.

## API Endpoints

All endpoints listed below belong to API version `v1`. Protected endpoints require authentication.

### Categories

| Method | Endpoint                  | Description               | Authentication |
| ------ | ------------------------- | ------------------------- | -------------- |
| GET    | `/api/v1/Categories`      | Retrieve all categories   | Public         |
| GET    | `/api/v1/Categories/{id}` | Retrieve a category by ID | Public         |
| POST   | `/api/v1/Categories`      | Create a category         | Required       |
| PUT    | `/api/v1/Categories/{id}` | Update a category         | Required       |
| DELETE | `/api/v1/Categories/{id}` | Delete a category         | Required       |

### Products

| Method | Endpoint                           | Description                   | Authentication |
| ------ | ---------------------------------- | ----------------------------- | -------------- |
| GET    | `/api/v1/Products`                 | Retrieve products             | Public         |
| GET    | `/api/v1/Products/{id}`            | Retrieve a product by ID      | Public         |
| GET    | `/api/v1/Products/paged`           | Retrieve paginated products   | Public         |
| GET    | `/api/v1/Products/by-category`     | Retrieve products by category | Public         |
| GET    | `/api/v1/Products/search`          | Search products               | Public         |
| POST   | `/api/v1/Products`                 | Create a product              | Required       |
| PUT    | `/api/v1/Products/{id}`            | Update a product              | Required       |
| DELETE | `/api/v1/Products/{id}`            | Delete a product              | Required       |
| POST   | `/api/v1/Products/{productId}/buy` | Purchase a product            | Required       |

### Users

| Method | Endpoint                 | Description           | Authentication |
| ------ | ------------------------ | --------------------- | -------------- |
| GET    | `/api/v1/Users`          | Retrieve users        | Required       |
| GET    | `/api/v1/Users/{id}`     | Retrieve a user by ID | Required       |
| POST   | `/api/v1/Users/login`    | Authenticate a user   | Public         |
| POST   | `/api/v1/Users/register` | Register a new user   | Public         |

## Database

The application uses SQL Server and Entity Framework Core for data persistence.

`ApplicationDbContext` manages database access and is configured through dependency injection. Entity Framework Core migrations are used to manage schema changes.

The project also includes a data seeding process that initializes the database with predefined data. The seeder is integrated into the application startup process.

The repository pattern is used to encapsulate data access operations and separate persistence concerns from HTTP request handling.

## API Versioning and Documentation

API versioning is implemented using `Asp.Versioning.Mvc` and `Asp.Versioning.Mvc.ApiExplorer`.

The application supports versioned routes and provides separate Swagger documentation for the available API versions. Versioning configuration includes a default API version, supported-version reporting, and API version substitution in routes.

Swagger UI allows developers to explore available endpoints, inspect request and response schemas, and test API operations. Bearer authentication is configured to support testing protected endpoints through the interface.

When running in the Development environment, Swagger UI is available at:

`/swagger/index.html`

## Error Handling

The application uses ASP.NET Core's `UseExceptionHandler` middleware to centralize the handling of unhandled exceptions.

When an unexpected exception occurs, the exception handler:

* Captures the exception from the HTTP request context.
* Records the exception message, stack trace, and UTC timestamp in the database.
* Returns a generic HTTP 500 Internal Server Error response to the client.

The API returns a consistent error response without exposing internal exception details to consumers.

Example response:

```json
{
  "Type": "error",
  "Message": "Ha ocurrido un error inesperado",
  "StatusCode": 500
}
```

This approach separates unexpected application failures from normal API responses and provides persisted error information for troubleshooting.

## Caching

The API uses ASP.NET Core Output Cache to cache HTTP responses.

A default cache expiration of 60 seconds is configured, providing a framework-based approach to response caching.

## CORS

Cross-Origin Resource Sharing is configured through the application's `AllowedOrigins` configuration.

This allows the API to explicitly control which origins are permitted to make cross-origin requests.

## Project Structure

The application is organized into directories that separate HTTP request handling, data access, models, configuration, and API documentation.

```text
ApiEcommerce/
├── Configurations/
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Repository/
│   └── IRepository/
├── Swagger/
├── Program.cs
└── ApiEcommerce.csproj
```

## Getting Started

### Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* SQL Server
* Git
* An IDE or code editor with .NET support

### Installation

Clone the repository:

```bash
git clone https://github.com/JAdrianMtz/api-ecommerce-fh.git
cd api-ecommerce-fh
```

Navigate to the application directory:

```bash
cd ApiEcommerce
```

### Configuration

Configure the database connection string and JWT settings in your local application configuration.

Keep credentials, signing keys, and other sensitive configuration values out of source control.

### Database Setup

Apply the Entity Framework Core migrations:

```bash
dotnet ef database update
```

Ensure that the configured SQL Server instance is running and accessible before executing the command.

The application includes a data seeder that initializes the database during startup.

### Run the Application

Start the API:

```bash
dotnet run
```

Once the application is running, use the URL displayed in the console to access the API and its Swagger documentation.

Swagger UI is available at:

`/swagger/index.html`

## Development

This project was developed as a practical exercise in backend development with ASP.NET Core, covering API design, persistence, authentication, authorization, and application configuration.

The implementation explores different approaches to user management, data access, API versioning, exception handling, and request processing within the .NET ecosystem.

## Author

**Jorge Adrian Martínez González**

[GitHub](https://github.com/JAdrianMtz)
