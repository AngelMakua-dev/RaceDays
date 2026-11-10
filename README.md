# RaceDays
RaceDay Event Management Systems

## Project Description

RaceDay is a race event management system designed to help organisers manage running events and allow participants to register for race categories and view their results.

The system will provide a central platform for managing users, organisers, participants, events, race categories, enrolments and results.

## Project Objectives

The main objectives of the RaceDay system are to:

- Allow users to register and log in.
- Allow organisers to create and manage race events.
- Allow organisers to create and manage race categories.
- Allow participants to view available events and categories.
- Allow participants to enrol in race categories.
- Allow organisers to view participant enrolments.
- Allow organisers to record and update race results.
- Allow participants to view their own enrolments and results.

## User Roles

### Organiser

Organisers can:

- Create and manage events.
- Create and manage event categories.
- View participant enrolments.
- Record and update race results.

### Participant

Participants can:

- Create an account and log in.
- Manage their profile.
- View available events and categories.
- Enrol in race categories.
- View their enrolments.
- View their race results.

## Database

The RaceDay database is designed using SQL Server.

The database contains the following main entities:

- Users
- Organisers
- Participants
- Events
- Categories
- Enrolments
- Results

The database uses primary keys and foreign keys to maintain relationships between the entities.

## Project Documentation

The `/docs` folder contains the planning and database documentation for the project:

- `RaceDay_ERD.png` – Entity Relationship Diagram
- `API_Endpoint_Plan.pdf` – API endpoint planning document
- `RaceDay_Database.sql` – SQL Server database creation and sample data script

## Technologies

- C#
- .NET
- SQL Server
- SQL Server Management Studio (SSMS)
- GitHub
- GitHub Actions

## Version Control

GitHub is used to manage the project's source code and documentation.

Meaningful commits are used throughout development to track changes and progress.

## Continuous Integration

GitHub Actions is used to automatically validate the project whenever changes are pushed to the repository.

## Project Status

Currently, the project is in the planning and database development stage. The ERD, API endpoint plan and SQL database script have been prepared before the API implementation.


## RaceDay API — Part 2

### Overview

The RaceDay API is an ASP.NET Core Web API built with .NET 10 and SQL Server. It provides backend functionality for user registration, login, profiles, events, categories, participant enrolments, and race results.

### Technologies

- C#
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- Microsoft SQL Server
- JWT authentication
- Swagger / OpenAPI
- xUnit unit testing
- GitHub Actions

### Main API Areas

| Area | Purpose |
|---|---|
| Authentication | Register and log in |
| Account | Authenticated account and role checks |
| Profile | View and update profile details |
| Events | Manage race events |
| Categories | Manage event categories |
| Enrolments | Register participants for categories |
| Results | Record and view race results |

### Running the Project

1. Install the .NET 10 SDK.
2. Ensure SQL Server is installed and running.
3. Configure the database connection string.
4. Configure JWT settings for development.
5. Open the solution in Visual Studio.
6. Restore NuGet packages and build the solution.
7. Run the API and open Swagger to explore the endpoints.

### Automated Testing

The project includes an xUnit test project. GitHub Actions is configured to build the solution and run automated tests when changes are pushed or pull requests are opened.

# RaceDay – Part 3

## 1. Project Overview

RaceDay is a sports event management system developed using ASP.NET Core MVC and ASP.NET Core Web API. The system allows organisers to manage sporting events and participants to discover events, enrol in competitions and view their results.

Part 3 focuses on developing the MVC user interface and connecting it to the existing RaceDay API.

## 2. Technologies Used

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- SQL Server
- JWT Authentication
- HTML
- CSS
- Bootstrap
- Visual Studio Community
- GitHub and GitHub Actions
- Docker
- Azure Blob Storage

## 3. System Structure

The RaceDay solution contains three main projects:

- **RaceDay.API:** Handles API requests, authentication, database operations and application functionality.
- **RaceDay.MVC:** Provides the user interface through which participants and organisers interact with the system.
- **RaceDay.API.Tests:** Contains automated tests for the API.

The MVC application communicates with the API using HTTP requests. Authentication tokens are stored in the MVC session and used when accessing protected API endpoints.

## 4. Authentication and User Roles

The system supports two user roles:

### Participants
Participants can access participant-related functionality.

### Organisers
Organisers can access organiser-related functionality.

The authentication system supports:
- User registration.
- User login and logout.
- Password hashing.
- JWT authentication.
- Role-based navigation.
- Session management.

## 5. MVC User Interfaces

### Home Page
The home page introduces RaceDay and provides navigation to the available system features.

### Participant Dashboard
The participant dashboard provides access to participant-related features.

### Organiser Dashboard
The organiser dashboard provides access to event management, participant management and results management.

### Profile
The profile section provides a user interface for displaying and managing account information.

## 6. API Integration

The MVC application uses a dedicated `RaceDayApiService` to communicate with the RaceDay API.

The service supports:
- GET requests.
- POST requests.
- Authenticated GET requests.
- Authenticated PUT requests.

JWT tokens are included in authenticated requests to access protected API endpoints.

## 7. User Interface Design

RaceDay uses a Premium Modern Sports design approach.

The interface includes:
- Burgundy as the primary brand colour.
- Warm white backgrounds.
- Dark headings and text.
- Gold accent details.
- Responsive Bootstrap layouts.
- Role-based navigation.
- Consistent page styling.

The design aims to provide a professional and user-friendly sports event management experience.

## 8. Running the Application

### Requirements

Before running the application, ensure that the following are installed:

- Visual Studio Community.
- .NET 10 SDK.
- SQL Server.
- SQL Server Management Studio (SSMS).

### Setup Instructions

1. Clone or download the RaceDay repository.
2. Open the RaceDay solution in Visual Studio Community.
3. Configure the SQL Server connection string in the API project's `appsettings.json`.
4. Ensure that the database is created and migrations have been applied.
5. Configure the API URL in the MVC project's `appsettings.json`.
6. Set `RaceDay.API` and `RaceDay.MVC` as startup projects.
7. Run the solution.

### Local Application URLs

API Swagger:
`https://localhost:7264/swagger`

MVC Application:
`https://localhost:7211`

These are the development URLs used in the project and may differ depending on the local launch settings.

## 9. Testing

The RaceDay solution includes a separate `RaceDay.API.Tests` project for automated API testing.

Testing should cover:
- User registration.
- User authentication.
- Protected API endpoints.
- Participant functionality.
- Organiser functionality.
- API responses and validation.

## 10. Future Improvements

Further development and integration can include:
- Complete event management functionality.
- Participant enrolment management.
- Event category management.
- Results entry and publishing.
- Azure Blob Storage integration for event banners and profile pictures.
- Docker containerisation of the MVC application.
- Expanded automated testing.
- Improved API and MVC integration.

## 11. Developer

Developed as part of the RaceDay software development project.

## HOW IT LOOKS LIKE

![alt text](<Screenshot 2026-11-10 174246.png>)