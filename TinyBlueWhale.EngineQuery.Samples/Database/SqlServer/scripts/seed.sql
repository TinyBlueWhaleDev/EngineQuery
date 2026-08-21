USE EngineQuerySample;
GO

INSERT INTO dbo.categories (parent_category_id, name)
VALUES
(NULL, 'Services'),
(1, 'Consulting'),
(2, 'Architecture'),
(1, 'Development');
GO

INSERT INTO dbo.customers (email, full_name, is_active, created_at)
VALUES
('admin@test.com', 'Admin User', 1, '2024-01-01T10:00:00'),
('reader@test.com', 'Reader User', 1, '2024-02-01T10:00:00'),
('inactive@test.com', 'Inactive User', 0, '2024-03-01T10:00:00'),
('noinvoices@test.com', 'No Invoices Customer', 1, '2024-04-01T10:00:00');
GO

INSERT INTO dbo.products (name, unit_price, is_active)
VALUES
('Architecture Review', 500.00, 1),
('Database Migration', 1200.00, 1),
('Performance Audit', 800.00, 1),
('Legacy Support', 300.00, 0);
GO

INSERT INTO dbo.invoices (customer_id, invoice_number, total, created_at)
VALUES
(1, 'INV-001', 500.00, '2024-05-01T10:00:00'),
(1, 'INV-002', 1500.00, '2024-05-02T10:00:00'),
(2, 'INV-003', 800.00, '2024-05-03T10:00:00'),
(3, 'INV-004', 300.00, '2024-05-04T10:00:00');
GO

INSERT INTO dbo.invoice_lines (invoice_id, product_id, quantity, line_total)
VALUES
(1, 1, 1, 500.00),
(2, 2, 1, 1200.00),
(2, 3, 1, 300.00),
(3, 3, 1, 800.00),
(4, 4, 1, 300.00);
GO
