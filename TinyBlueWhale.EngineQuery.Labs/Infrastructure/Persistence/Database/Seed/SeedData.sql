USE [TinyBlueWhaleEngineQueryLabs];
GO
SET XACT_ABORT ON;
SET NOCOUNT ON;
BEGIN TRANSACTION;

DELETE FROM dbo.OrderItems;
DELETE FROM dbo.Orders;
DELETE FROM dbo.Products;
DELETE FROM dbo.Categories;
DELETE FROM dbo.Customers;
SET IDENTITY_INSERT dbo.Categories ON;
INSERT dbo.Categories(Id, Name, Description, IsActive, CreatedAtUtc)
VALUES
(1,N'Electronics',N'Electronic devices',1,'2024-01-01'),(2,N'Office',N'Office supplies',1,'2024-01-01'),
(3,N'Home',N'Home products',1,'2024-01-01'),(4,N'Sports',N'Sports equipment',1,'2024-01-01'),
(5,N'Books',N'Technical books',1,'2024-01-01'),(6,N'Legacy',N'Discontinued catalog',0,'2024-01-01');
SET IDENTITY_INSERT dbo.Categories OFF;

DECLARE @n int = 1;
SET IDENTITY_INSERT dbo.Products ON;
WHILE @n <= 30
BEGIN
    INSERT dbo.Products(Id, CategoryId, Name, Sku, UnitPrice, IsActive, CreatedAtUtc)
    VALUES (@n, ((@n - 1) % 6) + 1, CONCAT(N'Product ', FORMAT(@n,'00')), CONCAT('SKU-', FORMAT(@n,'000')),
            CAST(5 + (@n * 3.25) AS decimal(18,2)), IIF(@n IN (6,12,18),0,1), '2024-01-02');
    SET @n += 1;
END;
SET IDENTITY_INSERT dbo.Products OFF;

SET @n = 1;
SET IDENTITY_INSERT dbo.Customers ON;
WHILE @n <= 20
BEGIN
    INSERT dbo.Customers(Id, FirstName, LastName, Email, Country, IsActive, CreatedAtUtc)
    VALUES (@n, CONCAT(N'First', FORMAT(@n,'00')), CONCAT(N'Last', FORMAT(@n,'00')),
            CONCAT('customer', FORMAT(@n,'00'), '@example.test'),
            CHOOSE(((@n - 1) % 5) + 1, N'Mexico', N'United States', N'Canada', N'Spain', N'Brazil'),
            IIF(@n = 20,0,1), '2024-01-03');
    SET @n += 1;
END;
SET IDENTITY_INSERT dbo.Customers OFF;

SET @n = 1;
WHILE @n <= 120
BEGIN
    SET IDENTITY_INSERT dbo.Orders ON;
    INSERT dbo.Orders(Id, CustomerId, OrderNumber, Status, OrderDateUtc, TotalAmount, CreatedAtUtc)
    VALUES (@n, ((@n - 1) % 18) + 1, CONCAT('ORD-', FORMAT(@n,'0000')), ((@n - 1) % 4) + 1,
            DATEADD(day, @n - 1, CONVERT(datetime2(0),'2024-02-01')), 0,
            DATEADD(minute, 5, DATEADD(day, @n - 1, CONVERT(datetime2(0),'2024-02-01'))));
    SET IDENTITY_INSERT dbo.Orders OFF;
    DECLARE @j int = 1;
    SET IDENTITY_INSERT dbo.OrderItems ON;
    WHILE @j <= 3
    BEGIN
        DECLARE @productId int = (((@n * 3 + @j * 5) - 1) % 28) + 1;
        DECLARE @quantity int = ((@n + @j) % 4) + 1;
        DECLARE @price decimal(18,2) = (SELECT UnitPrice FROM dbo.Products WHERE Id = @productId);
        INSERT dbo.OrderItems(Id, OrderId, ProductId, Quantity, UnitPrice, LineTotal, CreatedAtUtc)
        VALUES (((@n - 1) * 3) + @j, @n, @productId, @quantity, @price, @quantity * @price,
                DATEADD(minute, 10 + @j, DATEADD(day, @n - 1, CONVERT(datetime2(0),'2024-02-01'))));
        SET @j += 1;
    END;
    SET IDENTITY_INSERT dbo.OrderItems OFF;
    SET @n += 1;
END;

UPDATE o SET TotalAmount = x.TotalAmount
FROM dbo.Orders o
INNER JOIN (SELECT OrderId, SUM(LineTotal) TotalAmount FROM dbo.OrderItems GROUP BY OrderId) x ON x.OrderId = o.Id;

COMMIT;
GO
