CREATE DATABASE MusicStore;
GO

USE MusicStore;
GO
CREATE TABLE Artists (
    ArtistID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Genre NVARCHAR(50),
    Biography NVARCHAR(MAX)
);
CREATE TABLE Albums (
    AlbumID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    ArtistID INT FOREIGN KEY REFERENCES Artists(ArtistID),
    ReleaseDate DATE,
    Price DECIMAL(10, 2),
    CoverImage NVARCHAR(255)
);
CREATE TABLE Tracks (
    TrackID INT PRIMARY KEY IDENTITY(1,1),
    AlbumID INT FOREIGN KEY REFERENCES Albums(AlbumID),
    Title NVARCHAR(100) NOT NULL,
    Duration TIME,
    TrackNumber INT,
    FilePath NVARCHAR(255),  
    CoverImage NVARCHAR(255)  
);
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(15),
    Address NVARCHAR(255)
);
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2)
);
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    AlbumID INT FOREIGN KEY REFERENCES Albums(AlbumID),
    Quantity INT,
    Price DECIMAL(10, 2)
);
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    AlbumID INT FOREIGN KEY REFERENCES Albums(AlbumID),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE()
);
CREATE TABLE Genres (
    GenreID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);
CREATE TABLE AlbumGenres (
    AlbumID INT FOREIGN KEY REFERENCES Albums(AlbumID),
    GenreID INT FOREIGN KEY REFERENCES Genres(GenreID),
    PRIMARY KEY (AlbumID, GenreID)
);
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,  -- Store hashed password
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    CreatedAt DATETIME DEFAULT GETDATE()
);

