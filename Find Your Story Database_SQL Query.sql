CREATE DATABASE Find_Your_Story_DB;
USE	Find_Your_Story_DB;

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	DOB DATE NOT NULL,
	Email NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(100) NOT NULL
);

CREATE TABLE Product (
    ProductId INT PRIMARY KEY IDENTITY(101,1),
	BookCoverImage VARBINARY(MAX),
    Title NVARCHAR(100) NOT NULL,
    Author NVARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
	InStock BIT NOT NULL 
);

CREATE TABLE Cart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
	ProductID INT NOT NULL,
	Quantity INT,
	FOREIGN KEY (UserID) REFERENCES Users(UserID),
	FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
);


INSERT INTO Product (BookCoverImage, Title, Author, Price, InStock)
VALUES
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\The Great Gatsby_Cover.jpg', SINGLE_BLOB) AS Image), 'The Great Gatsby', 'F.Scott Fitzgerald', 129, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\One Hundred Years of Solitude_Cover.jpg', SINGLE_BLOB) AS Image), 'One Hundred Years of Solitude', 'Gabriel Garcia Marquez', 215, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\Don Quixote_Cover.jpg', SINGLE_BLOB) AS Image), 'Don Quixote', 'Miguel de Cervantes', 199, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\To Kill A Mockingbird_Cover.jpg', SINGLE_BLOB) AS Image), 'To Kill A Mockingbird', 'Harper Lee', 229, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\1984_Cover.jpg', SINGLE_BLOB) AS Image), '1984', 'George Orwell', 409, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\The Lord of the Rings_Cover.jpg', SINGLE_BLOB) AS Image), 'The Lord of the Rings', 'J. R. R. Tolkien', 1515, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\Anna Karenina_Cover.jpg', SINGLE_BLOB) AS Image), 'Anna Karenina', 'Leo Tolstoy', 295, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\Moby Dick_Cover.jpg', SINGLE_BLOB) AS Image), 'Moby Dick', 'Herman Melville', 149, 1),
((SELECT * FROM OPENROWSET(BULK N'C:\Users\Rachael\Documents\BCAD YEAR 3\PROG7311 YEAR 3\SPRINT 1 - FIND YOUR STORY\Book Cover Images\Pride and Prejudice_Cover.jpg', SINGLE_BLOB) AS Image), 'Pride and Prejudice', 'Jane Austen', 159, 1)

SELECT * FROM Product;