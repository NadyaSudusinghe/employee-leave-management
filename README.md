# Leave Management API

A backend leave management system built with ASP.NET Core Web API, PostgreSQL, and Entity Framework Core.
The API manages employees, leave requests, authentication, approval workflows, and leave balances while enforcing role-based access control and user-specific security.

The goal of this project is to demonstrate clean backend architecture, secure authorization rules, and real-world business logic in a production-style API.

---

# Features

## Authentication & Authorization

* JWT-based authentication
* User registration and login
* Role-based authorization
* Two supported roles:

  * `Admin`
  * `User`
* Role constants used instead of hardcoded strings
* Claims-based employee identification through JWT

---

## Employee Management

* Create employee
* Get employee by ID
* Update employee
* Delete employee
* Get all employees

### Employee Security Rules

* Admins can access all employees
* Normal users can only view and update their own employee profile
* Delete and list-all operations are restricted to admins only

---

## Leave Request Management

* Create leave request
* Update leave request
* Delete leave request
* Get leave request by ID
* Get leave requests by employee
* Get all leave requests
* Filter leave requests by status
* Admin approval / rejection workflow

### Supported Leave Types

* Annual Leave
* Casual Leave
* Sick Leave

### Supported Leave Request Statuses

* Pending
* Approved
* Rejected

---

# Security & Business Rules

One of the main goals of this project was to go beyond basic CRUD and implement real-world restrictions.

### User-Specific Access

A normal user:

* Can only create leave requests for themselves
* Can only update their own leave requests
* Cannot access another employee's leave requests
* Cannot modify another employee by changing an `employeeId` in the request body

To prevent this vulnerability, the API does not trust the `employeeId` sent from the client. Instead, the employee identity is taken directly from the authenticated JWT token.

### Admin Permissions

Admins can:

* View all employees
* Delete employees
* View all leave requests
* Delete leave requests
* Approve or reject leave requests
* Filter leave requests by status

---

# Leave Approval Workflow

All newly created leave requests start with:

```text
Pending
```

Only an admin can later change the status to:

```text
Approved
```

or

```text
Rejected
```

The approval endpoint uses a dedicated PATCH route and a strongly typed enum for status handling.

Example:

```http
PATCH /api/leaverequests/{id}/status
```

Request body:

```json
{
  "status": "Approved"
}
```

---

# Leave Balance System

The project includes a leave balance validation system.

Current leave limits:

* Annual Leave: 20 days
* Casual Leave: 10 days
* Sick Leave: unlimited

The API automatically:

* Calculates how many approved leave days an employee has already used
* Prevents creating or updating a leave request if the remaining balance would be exceeded
* Returns a meaningful error message showing requested and remaining days

Example:

```text
Insufficient annual leave balance. Requested: 5 day(s), Remaining: 2 day(s).
```

There is also a dedicated endpoint for viewing an employee's current leave balance.

Example:

```http
GET /api/leaverequests/employee/{employeeId}/balance
```

---

# Database

The project uses PostgreSQL running inside Docker.

Database schema is managed through Entity Framework Core migrations.

Current main tables:

* `Employees`
* `LeaveRequests`
* `Users`

Relationships:

* `LeaveRequests.EmployeeId` → `Employees.Id`
* `Users.EmployeeId` → `Employees.Id` (nullable)

---

# Architecture

The project uses a service-layer architecture.

```text
Controller -> Service -> DbContext
```

### Current Structure

* Controllers only handle HTTP concerns
* Services contain business logic
* Entity Framework Core handles database access
* DTOs are used for all request and response models
* Global exception middleware centralizes error handling

---

# Technologies Used

* ASP.NET Core Web API
* Entity Framework Core
* PostgreSQL
* JWT Authentication
* BCrypt Password Hashing
* Role-Based Authorization
* Middleware
* DTO Pattern
* Service Layer Pattern

---

# Example API Endpoints

## Authentication

```http
POST /api/auth/register
POST /api/auth/login
```

## Employees

```http
GET    /api/employees
GET    /api/employees/{id}
POST   /api/employees
PUT    /api/employees/{id}
DELETE /api/employees/{id}
```

## Leave Requests

```http
GET    /api/leaverequests
GET    /api/leaverequests?status=Pending
GET    /api/leaverequests/{id}
GET    /api/leaverequests/employee/{employeeId}
GET    /api/leaverequests/employee/{employeeId}/balance
POST   /api/leaverequests
PUT    /api/leaverequests/{id}
PATCH  /api/leaverequests/{id}/status
DELETE /api/leaverequests/{id}
```

---

# Tech Decisions

A few design choices made in this project:

* Used enums instead of magic strings for leave types and statuses
* Used role constants to avoid hardcoded authorization values
* Centralized exception handling through middleware
* Avoided trusting client-provided employee IDs
* Added business-rule validation before writing to the database
* Separated domain models from DTOs

---

# Project Status

The project is still actively being developed and expanded with additional backend features and improvements.
