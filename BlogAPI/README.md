# BlogAPI Project

## Introduction

Welcome to the BlogAPI project! This project is a simple yet robust blog API built with .NET 8, Entity Framework Core, and ASP.NET Core Identity.
The API allows users to register, login, create blog posts, comment on blog posts and other comments, and like blog posts and comments.
This project demonstrates a clean architecture and best practices in building a RESTful API.

## Features

- User registration and authentication using ASP.NET Core Identity
- CRUD operations for blog posts
- Nested comments on blog posts and other comments
- Like functionality for blog posts and comments with unique constraints
- Role-based authorization for managing blog content

## Technologies

- **ASP.NET Core:** ASP.NET Core is a cross-platform framework for building modern, cloud-based, and internet-connected applications.

- **Entity Framework:** Entity Framework (EF) is an ORM for .NET that simplifies data access by allowing developers to work with database objects using .NET objects.

- **Identity Framework:** ASP.NET Core Identity is a membership system that supports user authentication, authorization, and management.

- **MsSQL:** Microsoft SQL Server (MsSQL) is a relational database management system that provides high performance, security, and data integrity.

- **Swagger:** Swagger is a tool for designing, building, and documenting RESTful APIs with a user-friendly interface for testing endpoints.

## Project Structure

```maths
    BlogAPI-SoftITO/
    ├── Controllers/
    ├── Data/
    ├── Entities/
    │ ├── Models/
    │ ├── DTOs/
    ├── Migrations/
    ├── Properties/
```

- **Controllers:** Contains the API controllers for handling HTTP requests.
- **DTOs:** Data Transfer Objects for transferring data between client and server.
- **Models:** Entity models representing the database schema.
- **Data:** Database context and configuration for Entity Framework Core.

## Getting Started

### Prerequisites

- .NET 8 SDK
- MsSQL Server (or any other compatible database)
- Visual Studio or VS Code

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/denizciMert/BlogAPI.git
   cd BlogAPI
   ```

2. Set up the database connection string in appsettings.json:

   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*",
     "ConnectionStrings": {
       "ApplicationDbContext": "YourConnectionString"
     }
   }
   ```

3. Build and run the project:

   ```bash
   dotnet build
   ```

   ```bash
   dotnet run
   ```

## API Endpoints

### Account Management

<table>
<thead>
<tr>
<th>Method</th>
<th>Endpoint</th>
<th>Description</th>
</tr>
</thead>
<tbody>
<tr>
<td>POST</td>
<td>/api/account/register</td>
<td>Register a new user</td>
</tr>
<tr>
<td>POST</td>
<td>/api/account/login</td>
<td>Login a user</td>
</tr>
<tr>
<td>POST</td>
<td>/api/account/logout</td>
<td>Logout the current user</td>
</tr>
</tbody>
</table>

### Blog Posts

<table>
<thead>
<tr>
<th>Method</th>
<th>Endpoint</th>
<th>Description</th>
</tr>
</thead>
<tbody>
<tr>
<td>GET</td>
<td>/api/blogpost</td>
<td>Get all blog posts</td>
</tr>
<tr>
<td>GET</td>
<td>/api/blogpost/{id}</td>
<td>Get a single blog post by ID</td>
</tr>
<tr>
<td>POST</td>
<td>/api/blogpost</td>
<td>Create a new blog post (authorized users only)</td>
</tr>
<tr>
<td>PUT</td>
<td>/api/blogpost/{id}</td>
<td>Update a blog post (authorized users only)</td>
</tr>
<tr>
<td>DELETE</td>
<td>/api/blogpost/{id}</td>
<td>Soft delete a blog post (authorized users only)</td>
</tr>
</tbody>
</table>

### Comments

<table>
<thead>
<tr>
<th>Method</th>
<th>Endpoint</th>
<th>Description</th>
</tr>
</thead>
<tbody>
<tr>
<td>GET</td>
<td>/api/comments</td>
<td>Get all comments</td>
</tr>
<tr>
<td>GET</td>
<td>/api/comments/{id}</td>
<td>Get a single comment by ID</td>
</tr>
<tr>
<td>POST</td>
<td>/api/comments</td>
<td>Create a new comment (authorized users only)</td>
</tr>
<tr>
<td>PUT</td>
<td>/api/comments/{id}</td>
<td>Update a comment (authorized users only)</td>
</tr>
<tr>
<td>DELETE</td>
<td>/api/comments/{id}</td>
<td>Soft delete a comment (authorized users only)</td>
</tr>
</tbody>
</table>

### Likes

<table>
<thead>
<tr>
<th>Method</th>
<th>Endpoint</th>
<th>Description</th>
</tr>
</thead>
<tbody>
<tr>
<td>POST</td>
<td>/api/likes</td>
<td>Like a blog post or comment (authorized users only)</td>
</tr>
<tr>
<td>DELETE</td>
<td>/api/likes</td>
<td>Unlike a blog post or comment (authorized users only)</td>
</tr>
</tbody>
</table>

## Acknowledgements

- This project was developed as part of the backend program at <a href="https://softito.com.tr/index.php" rel="nofollow">Softito Yazılım - Bilişim Akademisi</a>.
- Special thanks to the instructors and peers who provided valuable feedback and support throughout the development process.
