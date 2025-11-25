# School Management System API (.NET 9 Web API + SQL Server)

A backend API built with **ASP.NET Core Web API (.NET 9)** + **Entity Framework Core** + **SQL Server** to manage:
- Users (Admin / Teacher / Student)
- Departments
- Courses
- Classes (Batches)
- Student enrollments
- Attendance
- Assignments & Submissions
- Grading
- Notifications (bonus)

The system supports **JWT authentication** and **role-based authorization**.

---

## Table of Contents
1. [Tech Stack](#tech-stack)
2. [Features](#features)
3. [Project Structure](#project-structure)
4. [Setup Instructions](#setup-instructions)
5. [Database Migration Commands](#database-migration-commands)
6. [Authentication Flow](#authentication-flow)
7. [Sample API Requests](#sample-api-requests)
   - [Auth](#auth)
   - [Admin](#admin)
   - [Teacher](#teacher)
   - [Student](#student)
8. [Swagger / API Docs](#swagger--api-docs)
9. [Caching & Pagination](#caching--pagination)
10. [Logging](#logging)
11. [Demo Video](#demo-video)
12. [Notes & Validation Rules](#notes--validation-rules)

---

## Tech Stack
- **Backend:** ASP.NET Core Web API **.NET 9**
- **Database:** **SQL Server**
- **ORM:** Entity Framework Core (EF Core)
- **Auth:** JWT Bearer Tokens (Access + Refresh)
- **Validation:** DataAnnotations / FluentValidation (optional)
- **Mapping:** AutoMapper (optional)
- **Logging:** Serilog / NLog (bonus)
- **Caching:** IMemoryCache (bonus)

---

## Features
### Core
- JWT authentication with refresh tokens
- Role-based access:
  - **Admin:** manage users, departments, courses  
  - **Teacher:** manage classes, attendance, assignments, grading  
  - **Student:** view classes, attendance, submit assignments, view grades  
- Relational database with EF Core navigation properties
- Async DB operations throughout

### Bonus
- In-memory caching for Departments & Courses listing
- Pagination + filtering for Classes, Students, Assignments
- Soft delete for Users & Courses (`IsActive = false`)
- Notifications for students/classes
- Optional file upload for submissions (`IFormFile`)

---

## Setup Instructions

### 1) Prerequisites
Install:
- **.NET 9 SDK**
- **SQL Server** (LocalDB / Express / Developer / Docker / Hosted)
- **EF Core CLI Tools**
- Optional IDE: Visual Studio 2022+ / Rider / VS Code

Verify .NET:
```bash
dotnet --version
dotnet tool install --global dotnet-ef
git clone https://github.com/Zer0bytte/SchoolMS.git
cd SchoolMS

