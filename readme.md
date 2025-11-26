# School Management System API

A comprehensive backend API built with .NET Core Web API for managing students, teachers, courses, classes, attendance, and grading with role-based access control.

## 📋 Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Testing Live](#testing-live)
- [Prerequisites](#prerequisites)
- [Installation & Setup](#installation--setup)
- [Database Migration](#database-migration)
- [API Documentation](#api-documentation)
- [Authentication](#authentication)
- [Sample API Requests](#sample-api-requests)
- [Testing](#testing)
- [Demo Video](#demo-video)

## Features

- **Role-Based Access Control**: Admin, Teacher, and Student roles with specific permissions
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Department Management**: Organize courses by departments with head of department assignments
- **Course Management**: Create and manage courses with credits and descriptions
- **Class Management**: Teachers can create and manage class batches for courses
- **Student Enrollment**: Automated enrollment system for students in classes
- **Attendance Tracking**: Mark and view attendance with multiple status options
- **Assignment System**: Create, submit, and grade assignments with file uploads
- **Notification System**: Send notifications to students and teachers
- **Data Validation**: Comprehensive validation rules for all operations
- **Soft Delete**: Maintain data integrity with soft delete functionality
- **Caching**: In-memory caching for improved performance
- **Logging**: Structured logging with Serilog
- **Pagination & Filtering**: Efficient data retrieval with cursor pagination support

## Tech Stack

- **.NET Core 9.0** - Web API
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **JWT Bearer** - Authentication
- **FluentValidation** - Input validation
- **Serilog/Seq** - Structured logging
- **Scalar/OpenAPI** - API documentation

## Testing Live
You can skip the setup steps below and test the API directly using this live URL: https://schoolms.markomedhat.com/scalar

## Prerequisites

- .NET 9.0 SDK
- SQL Server
- Visual Studio
- Git
- Seq (Logging Dashboard)



## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Zer0bytte/SchoolMS.git
cd SchoolMS
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Update Connection String

Open `appsettings.json` and update the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SchoolManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For SQL Server Authentication, use:
```json
"DefaultConnection": "Server=localhost;Database=SchoolManagementDB;User Id=your_username;Password=your_password;TrustServerCertificate=True;"
```

### 4. Configure JWT Settings

Update JWT configuration in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyWithAtLeast32Characters!",
    "Issuer": "SchoolManagementAPI",
    "Audience": "SchoolManagementClients",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

## Database Migration

The database migrated automatically when application starts.

### Seed Initial Data

Run the application for the first time to seed initial admin user:

```bash
cd src/SchoolMS.Api
dotnet run
```

Default admin credentials:
- **Email**: admin@email.com
- **Password**: Password@123


## API Documentation

Access Scalar UI after running the application:

```
https://localhost:port/scalar
```

## Authentication

### Register a New User

```http
POST /api/v1/auth/register
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@school.com",
  "password": "SecurePass@123",
  "role": "Student"
}
```

### Login As Student

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "email": "john.doe@school.com",
  "password": "SecurePass@123"
}
```

**Response:**
```json
{
    "userId": "019aa68c-266f-71a6-af99-16b28c7122de",
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.....",
    "refreshToken": "lKJu0sm",
    "role": "Student",
    "expiresOnUtc": "expirationTime"
}
```

### Using the JWT Token

Include the token in the Authorization header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Sample API Requests
You can test through postman with already made collection instead
The Postman collection included in the repository folder called "Project Submission"
### 1. Admin: Create Department

```http
POST /api/v1/admin/departments
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Computer Science",
  "description": "Department of Computer Science and Engineering",
  "headOfDepartmentId": "teacherId"
}
```

### 2. Admin: Create Course

```http
POST /api/v1/admin/courses
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "name": "Data Structures and Algorithms",
  "code": "CS201",
  "description": "Fundamental data structures and algorithms",
  "departmentId": "{departmentId}",
  "credits": 4
}
```

### 3. Teacher: Create Class

```http
POST /api/v1/teacher/classes
Authorization: Bearer {teacher_token}
Content-Type: application/json

{
  "name": "CS201-Fall2024-A",
  "courseId": "{courseId}",
  "semester": "Fall 2024",
  "startDate": "2024-09-01",
  "endDate": "2024-12-15",
}
```

### 4. Teacher: Mark Attendance

```http
POST /api/v1/teacher/attendance
Authorization: Bearer {teacher_token}
Content-Type: application/json

{
  "classId": "classId",
  "students": [
    {
      "studentId": "{studentId}",
      "status": "Present"
    },
    {
      "studentId": "{student2Id}",
      "status": "Absent"
    }
  ]
}
```

### 5. Teacher: Create Assignment

```http
POST /api/v1/teacher/assignments
Authorization: Bearer {teacher_token}
Content-Type: application/json

{
  "ClassId": "{classId}",
  "title": "First Assignment",
  "description": "Read chapter 1 and answer the questions at the end. Submit as a PDF.",
  "dueDate": "2025-12-12"
}

```

### 6. Student: Submit Assignment

```http
POST /api/v1/student/assignments/{assignmentId}/submit
Authorization: Bearer {student_token}
Content-Type: multipart/form-data

file: [assignment_file.pdf]
```

### 7. Teacher: Grade Submission

```http
POST /api/v1/teacher/assignments/{submissionId}/grade
Authorization: Bearer {teacher_token}
Content-Type: application/json

{
    "grade":5.5,
    "remarks":"Very good student"
}
```

### 8. Pagination Example

```http
GET /api/v1/admin/courses?Limit=10&Cursor=nextCursor
Authorization: Bearer {admin_token}
```

## Testing

### Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

### Test with Postman

The Postman collection included in the repository folder called "Project Submission"

## Demo Video

A demonstration video showcasing the complete API flow is available:

https://youtu.be/yoVY62xfiYU

**Demo Flow:**
1. Admin login and department/course creation
2. Teacher login and class management
3. Teacher marking attendance and creating assignments
4. Student login, viewing classes, and submitting assignments
5. Teacher grading submissions
6. Notification system demonstration

---

**Note**: This is a demonstration project for educational purposes. For production use, implement additional security measures, comprehensive testing, and proper error handling.