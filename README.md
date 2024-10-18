# Angular-18-User-Registration-with-Asp.Net-Core-Web-API---BE

## Overview

This project is the backend part of an authentication system using ASP.NET Core 8 and JWT for secure user registration and authentication. The frontend is built with Angular 18.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies](#technologies)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Contributing](#contributing)
- [License](#license)

## Features

- User Registration
- User Authentication with JWT
- Secure API Endpoints
- Integration with Angular frontend

## Technologies

- ASP.NET Core 8
- JWT (JSON Web Tokens)
- Entity Framework Core
- SQL Server
- Angular 18

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Node.js](https://nodejs.org/) (for Angular frontend)

### Installation

1. Clone the repository:

   ```sh
   git clone https://github.com/yourusername/Angular-18-User-Registration-with-Asp.Net-Core-Web-API---BE.git
   cd Angular-18-User-Registration-with-Asp.Net-Core-Web-API---BE/AuthECBackend
   ```

2. Restore the .NET dependencies:

   ```sh
   dotnet restore
   ```

3. Update the database:

   ```sh
   dotnet ef database update
   ```

4. Configure the `appsettings.json` file with your SQL Server connection string.

### Running the Application

1. Build and run the backend:

   ```sh
   dotnet run
   ```

2. Navigate to `http://localhost:5000` to access the API.

## API Endpoints

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate a user and return a JWT
- `GET /api/weatherforecast` - Example secured endpoint

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
