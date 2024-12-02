-- Create the MusicStore Database (if not already created)
CREATE DATABASE MusicStore;
GO

USE MusicStore;
GO

-- Artists table
CREATE TABLE Artists (
    ArtistID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Genre NVARCHAR(50),
    Biography NVARCHAR(MAX)
);

-- Albums table
CREATE TABLE Albums (
    AlbumID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    ArtistID INT FOREIGN KEY REFERENCES Artists(ArtistID),
    ReleaseDate DATE,
    CoverImage NVARCHAR(255)
);

-- Tracks table
CREATE TABLE Tracks (
    TrackID INT PRIMARY KEY IDENTITY(1,1),
    AlbumID INT FOREIGN KEY REFERENCES Albums(AlbumID),
    Title NVARCHAR(100) NOT NULL,
    Duration TIME,
    Path NVARCHAR(MAX),
    CoverImage NVARCHAR(255)
);

-- Customers table (no changes)
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Phone NVARCHAR(15),
    Address NVARCHAR(255)
);

-- Memberships table (replacing Orders table)
CREATE TABLE Memberships (
    MembershipID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    StartDate DATETIME DEFAULT GETDATE(),
    EndDate DATETIME,
    MembershipPlan NVARCHAR(50), -- E.g., "Monthly", "Annual"
    Amount DECIMAL(10, 2), -- Subscription fee
    Status NVARCHAR(20) -- E.g., "Active", "Cancelled", "Expired"
);

-- Users table (no changes)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Roles table (no changes)
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

-- UserRoles table (no changes)
CREATE TABLE UserRoles (
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID),
    PRIMARY KEY (UserID, RoleID)
);

-- Playlists table (new table for managing playlists)
CREATE TABLE Playlists (
    PlaylistID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    PlaylistName NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsPublic BIT DEFAULT 0 -- Can be public or private
);

-- PlaylistTracks table (relationship table for adding tracks to playlists)
CREATE TABLE PlaylistTracks (
    PlaylistID INT FOREIGN KEY REFERENCES Playlists(PlaylistID),
    TrackID INT FOREIGN KEY REFERENCES Tracks(TrackID),
    PRIMARY KEY (PlaylistID, TrackID)
);

-- Optionally, you can also create a table to track users' preferences or listening history
CREATE TABLE ListeningHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    PlaylistID INT FOREIGN KEY REFERENCES Playlists(PlaylistID),
    ListenDate DATETIME DEFAULT GETDATE()
);

