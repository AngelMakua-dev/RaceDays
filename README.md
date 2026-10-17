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

