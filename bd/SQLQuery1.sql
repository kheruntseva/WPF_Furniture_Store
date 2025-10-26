-- Create RectangleItems table
CREATE TABLE RectangleItems (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Price DECIMAL(18,2) NOT NULL,
    Color NVARCHAR(50) NOT NULL,
    Image NVARCHAR(255)
);

-- Create ItemTranslations table
CREATE TABLE ItemTranslations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RectangleItemId UNIQUEIDENTIFIER NOT NULL,
    LanguageCode NVARCHAR(2) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Category NVARCHAR(50),
    Availability NVARCHAR(50),
    FOREIGN KEY (RectangleItemId) REFERENCES RectangleItems(Id)
);

-- Create Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    IsAdmin BIT NOT NULL DEFAULT 0
);

-- Create PurchasedProducts junction table
CREATE TABLE PurchasedProducts (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RectangleItemId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (UserId, RectangleItemId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RectangleItemId) REFERENCES RectangleItems(Id)
);
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Alter RectangleItems table to match RectangleItem class
ALTER TABLE RectangleItems
ADD Name NVARCHAR(100) NOT NULL DEFAULT '',
    Description NVARCHAR(500),
    Category NVARCHAR(50),
    Availability NVARCHAR(50);

-- Drop ItemTranslations table (not used yet)
DROP TABLE ItemTranslations;

SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RectangleItems';
