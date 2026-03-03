## Foor ordering & Restaurent/Customer registration services

## Project Overview

- ** Customer Registration/Login **
- ** Restaurant Registration **
- ** Place Order via Restaurant **
- ** Payments (In progress) **

## Architecture

### System Design
- **RESTful API** with JWT authentication
- **Frontend**: Shared React + TypeScript + Bootstrap CSS (SPA) (planned for later)
- **Backend**: ASP.NET Core 8 Web API (this repo)
- **Database**: Microsoft SQL (schema designed for reuse across stacks)
- **Caching**: Redis
- **Message Queue**: RabbitMQ (planned for later)

### Design Principles

- **Clean Architecture**: Separation of concerns with distinct layers
- **SOLID Principles**: Maintainable and extensible code
- **DRY**: Don’t Repeat Yourself
- **KISS**: Keep It Simple, Stupid
- **Security First**: Authentication, authorization, input validation
- **Test-Driven**: Comprehensive test coverage

**Prerequisites**
- **.NET 8 SDK**
- **Microsoft SQL**
- **Redis**
- **Git**