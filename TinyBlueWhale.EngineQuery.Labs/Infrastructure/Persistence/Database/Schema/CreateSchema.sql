USE [TinyBlueWhaleEngineQueryLabs];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.Customers', N'U') IS NULL
CREATE TABLE dbo.Customers
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
    FirstName nvarchar(80) NOT NULL,
    LastName nvarchar(80) NOT NULL,
    Email nvarchar(256) NOT NULL,
    Country nvarchar(80) NOT NULL,
    IsActive bit NOT NULL,
    CreatedAtUtc datetime2(0) NOT NULL
);

IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
CREATE TABLE dbo.Categories
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
    Name nvarchar(120) NOT NULL,
    Description nvarchar(500) NOT NULL,
    IsActive bit NOT NULL,
    CreatedAtUtc datetime2(0) NOT NULL
);

IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
CREATE TABLE dbo.Products
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
    CategoryId int NOT NULL,
    Name nvarchar(160) NOT NULL,
    Sku nvarchar(50) NOT NULL,
    UnitPrice decimal(18,2) NOT NULL CONSTRAINT CK_Products_UnitPrice CHECK (UnitPrice >= 0),
    IsActive bit NOT NULL,
    CreatedAtUtc datetime2(0) NOT NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id)
);

IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
CREATE TABLE dbo.Orders
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
    CustomerId int NOT NULL,
    OrderNumber varchar(30) NOT NULL,
    Status int NOT NULL CONSTRAINT CK_Orders_Status CHECK (Status BETWEEN 1 AND 4),
    OrderDateUtc datetime2(0) NOT NULL,
    TotalAmount decimal(18,2) NOT NULL CONSTRAINT CK_Orders_TotalAmount CHECK (TotalAmount >= 0),
    CreatedAtUtc datetime2(0) NOT NULL,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id)
);

IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NULL
CREATE TABLE dbo.OrderItems
(
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderItems PRIMARY KEY,
    OrderId int NOT NULL,
    ProductId int NOT NULL,
    Quantity int NOT NULL CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
    UnitPrice decimal(18,2) NOT NULL CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice >= 0),
    LineTotal decimal(18,2) NOT NULL CONSTRAINT CK_OrderItems_LineTotal CHECK (LineTotal >= 0),
    CreatedAtUtc datetime2(0) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);

COMMIT;
GO

CREATE OR ALTER VIEW dbo.Lab001OrderSearchText
AS
SELECT
    o.Id AS OrderId,
    CONCAT(o.OrderNumber, N'|', c.FirstName, N'|', c.LastName, N'|', c.Email) AS SearchText
FROM dbo.Orders AS o
INNER JOIN dbo.Customers AS c ON c.Id = o.CustomerId;
GO
