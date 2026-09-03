-- =============================================
-- RaceDay Database
-- Section C - SQL Database Script
-- =============================================

USE master;
GO

IF DB_ID('RaceDay') IS NULL
BEGIN
    CREATE DATABASE RaceDay;
END;
GO

USE RaceDay;
GO


-- =============================================
-- 1. USERS TABLE
-- =============================================

CREATE TABLE Users
(
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT GETDATE()
);
GO


-- =============================================
-- 2. ORGANISERS TABLE
-- =============================================

CREATE TABLE Organisers
(
    organiser_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    organisation_name VARCHAR(100) NOT NULL,

    CONSTRAINT FK_Organisers_Users
        FOREIGN KEY (user_id)
        REFERENCES dbo.Users(user_id)
);
GO


-- =============================================
-- 3. PARTICIPANTS TABLE
-- =============================================

CREATE TABLE Participants
(
    participant_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    date_of_birth DATE NOT NULL,
    emergency_contact_name VARCHAR(100) NOT NULL,
    emergency_contact_phone VARCHAR(20) NOT NULL,

    CONSTRAINT FK_Participants_Users
        FOREIGN KEY (user_id)
        REFERENCES dbo.Users(user_id)
);
GO


-- =============================================
-- 4. EVENTS TABLE
-- =============================================

CREATE TABLE Events
(
    event_id INT IDENTITY(1,1) PRIMARY KEY,
    organiser_id INT NOT NULL,
    event_name VARCHAR(100) NOT NULL,
    event_date DATE NOT NULL,
    location VARCHAR(150) NOT NULL,
    description VARCHAR(500),
    status VARCHAR(20) NOT NULL DEFAULT 'Upcoming',

    CONSTRAINT FK_Events_Organisers
        FOREIGN KEY (organiser_id)
        REFERENCES dbo.Organisers(organiser_id)
);
GO


-- =============================================
-- 5. CATEGORIES TABLE
-- =============================================

CREATE TABLE Categories
(
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    event_id INT NOT NULL,
    category_name VARCHAR(100) NOT NULL,
    distance_km DECIMAL(5,2) NOT NULL,
    entry_fee DECIMAL(10,2) NOT NULL DEFAULT 0.00,

    CONSTRAINT FK_Categories_Events
        FOREIGN KEY (event_id)
        REFERENCES dbo.Events(event_id),

    CONSTRAINT UQ_Categories_Event_Name
        UNIQUE (event_id, category_name)
);
GO


-- =============================================
-- 6. ENROLMENTS TABLE
-- =============================================

CREATE TABLE Enrolments
(
    enrolment_id INT IDENTITY(1,1) PRIMARY KEY,
    participant_id INT NOT NULL,
    category_id INT NOT NULL,
    enrolment_date DATETIME NOT NULL DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'Registered',

    CONSTRAINT FK_Enrolments_Participants
        FOREIGN KEY (participant_id)
        REFERENCES dbo.Participants(participant_id),

    CONSTRAINT FK_Enrolments_Categories
        FOREIGN KEY (category_id)
        REFERENCES dbo.Categories(category_id),

    CONSTRAINT UQ_Enrolments_Participant_Category
        UNIQUE (participant_id, category_id)
);
GO


-- =============================================
-- 7. RESULTS TABLE
-- =============================================

CREATE TABLE Results
(
    result_id INT IDENTITY(1,1) PRIMARY KEY,
    enrolment_id INT NOT NULL UNIQUE,
    finish_time TIME NOT NULL,
    position INT,
    result_status VARCHAR(20) NOT NULL DEFAULT 'Finished',

    CONSTRAINT FK_Results_Enrolments
        FOREIGN KEY (enrolment_id)
        REFERENCES dbo.Enrolments(enrolment_id),

    CONSTRAINT UQ_Results_Position
        UNIQUE (enrolment_id)
);
GO


-- =============================================
-- SAMPLE DATA
-- =============================================


-- =============================================
-- USERS
-- 2 Organisers + 2 Participants
-- =============================================

INSERT INTO Users
    (first_name, last_name, email, password, role)
VALUES
    ('Thabo', 'Mokoena', 'thabo@raceday.co.za', 'Password123!', 'Organiser'),
    ('Lerato', 'Dlamini', 'lerato@raceday.co.za', 'Password123!', 'Organiser'),
    ('Sipho', 'Nkosi', 'sipho@email.com', 'Password123!', 'Participant'),
    ('Amahle', 'Khumalo', 'amahle@email.com', 'Password123!', 'Participant');
GO


-- =============================================
-- ORGANISERS
-- =============================================

INSERT INTO Organisers
    (user_id, organisation_name)
VALUES
    (1, 'Johannesburg Running Club'),
    (2, 'Cape Town Race Events');
GO


-- =============================================
-- PARTICIPANTS
-- =============================================

INSERT INTO Participants
    (user_id, date_of_birth, emergency_contact_name, emergency_contact_phone)
VALUES
    (3, '2002-05-14', 'Nomsa Nkosi', '0825551234'),
    (4, '2001-11-22', 'Sibusiso Khumalo', '0835555678');
GO


-- =============================================
-- EVENTS
-- =============================================

INSERT INTO Events
    (organiser_id, event_name, event_date, location, description, status)
VALUES
    (1, 'Johannesburg City Run', '2026-10-10', 'Johannesburg', 
     'Annual city running event for beginner and experienced runners.', 'Upcoming'),

    (2, 'Cape Town Coastal Run', '2026-11-07', 'Cape Town', 
     'Scenic running event along the Cape Town coastline.', 'Upcoming'),

    (1, 'Pretoria Spring Marathon', '2026-11-21', 'Pretoria', 
     'Spring marathon event featuring multiple running categories.', 'Upcoming');
GO


-- =============================================
-- CATEGORIES
-- Each event has multiple categories
-- =============================================

INSERT INTO Categories
    (event_id, category_name, distance_km, entry_fee)
VALUES
    -- Johannesburg City Run
    (1, '5 KM Fun Run', 5.00, 100.00),
    (1, '10 KM Race', 10.00, 180.00),
    (1, '21 KM Half Marathon', 21.00, 300.00),

    -- Cape Town Coastal Run
    (2, '5 KM Coastal Run', 5.00, 120.00),
    (2, '10 KM Coastal Race', 10.00, 200.00),

    -- Pretoria Spring Marathon
    (3, '21 KM Half Marathon', 21.00, 300.00),
    (3, '42 KM Marathon', 42.00, 450.00);
GO


-- =============================================
-- ENROLMENTS
-- =============================================

INSERT INTO Enrolments
    (participant_id, category_id, status)
VALUES
    (1, 1, 'Registered'),
    (1, 3, 'Registered'),
    (1, 6, 'Registered'),
    (2, 2, 'Registered'),
    (2, 4, 'Registered'),
    (2, 7, 'Registered');
GO


-- =============================================
-- RESULTS
-- =============================================

INSERT INTO Results
    (enrolment_id, finish_time, position, result_status)
VALUES
    (1, '00:32:15', 12, 'Finished'),
    (2, '01:48:30', 8, 'Finished');
GO


-- =============================================
-- END OF RACE DAY DATABASE SCRIPT
-- =============================================