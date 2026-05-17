USE EngineQuerySample
GO

IF OBJECT_ID('dbo.invoice_lines', 'U') IS NOT NULL DROP TABLE dbo.invoice_lines;
IF OBJECT_ID('dbo.invoices', 'U') IS NOT NULL DROP TABLE dbo.invoices;
IF OBJECT_ID('dbo.products', 'U') IS NOT NULL DROP TABLE dbo.products;
IF OBJECT_ID('dbo.customers', 'U') IS NOT NULL DROP TABLE dbo.customers;
GO

CREATE TABLE dbo.customers
(
    customer_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    email NVARCHAR(320) NOT NULL,
    full_name NVARCHAR(250) NOT NULL,
    is_active BIT NOT NULL,
    created_at DATETIME2 NOT NULL
);
GO

CREATE TABLE dbo.products
(
    product_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    unit_price DECIMAL(18,2) NOT NULL,
    is_active BIT NOT NULL
);
GO

CREATE TABLE dbo.invoices
(
    invoice_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    customer_id INT NOT NULL,
    invoice_number NVARCHAR(50) NOT NULL,
    total DECIMAL(18,2) NOT NULL,
    created_at DATETIME2 NOT NULL,
    CONSTRAINT fk_invoices_customers FOREIGN KEY (customer_id) REFERENCES dbo.customers(customer_id)
);
GO

CREATE TABLE dbo.invoice_lines
(
    invoice_line_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    invoice_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    line_total DECIMAL(18,2) NOT NULL,
    CONSTRAINT fk_invoice_lines_invoices FOREIGN KEY (invoice_id) REFERENCES dbo.invoices(invoice_id),
    CONSTRAINT fk_invoice_lines_products FOREIGN KEY (product_id) REFERENCES dbo.products(product_id)
);
GO