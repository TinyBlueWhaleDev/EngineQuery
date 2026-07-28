USE [TinyBlueWhaleEngineQueryLabs];
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Customers_Email')
    CREATE UNIQUE INDEX UX_Customers_Email ON dbo.Customers(Email);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Customers_Country_IsActive')
    CREATE INDEX IX_Customers_Country_IsActive ON dbo.Customers(Country, IsActive);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Products_Sku')
    CREATE UNIQUE INDEX UX_Products_Sku ON dbo.Products(Sku);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Products_CategoryId_IsActive')
    CREATE INDEX IX_Products_CategoryId_IsActive ON dbo.Products(CategoryId, IsActive) INCLUDE(Name);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Orders_OrderNumber')
    CREATE UNIQUE INDEX UX_Orders_OrderNumber ON dbo.Orders(OrderNumber);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Orders_Search')
    CREATE INDEX IX_Orders_Search ON dbo.Orders(OrderDateUtc, Status, CustomerId) INCLUDE(OrderNumber, TotalAmount);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_OrderItems_OrderId_ProductId')
    CREATE UNIQUE INDEX UX_OrderItems_OrderId_ProductId ON dbo.OrderItems(OrderId, ProductId) INCLUDE(Quantity);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_OrderItems_ProductId')
    CREATE INDEX IX_OrderItems_ProductId ON dbo.OrderItems(ProductId, OrderId);
GO
