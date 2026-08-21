USE EngineQuerySample;

DROP TABLE IF EXISTS categories;
DROP TABLE IF EXISTS invoice_lines;
DROP TABLE IF EXISTS invoices;
DROP TABLE IF EXISTS products;
DROP TABLE IF EXISTS customers;

CREATE TABLE categories
(
    category_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    parent_category_id INT NULL,
    name VARCHAR(200) NOT NULL,
    CONSTRAINT fk_categories_parent
        FOREIGN KEY (parent_category_id)
        REFERENCES categories(category_id)
);

CREATE TABLE customers
(
    customer_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    email VARCHAR(320) NOT NULL,
    full_name VARCHAR(250) NOT NULL,
    is_active BOOLEAN NOT NULL,
    created_at DATETIME NOT NULL
);

CREATE TABLE products
(
    product_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    unit_price DECIMAL(18,2) NOT NULL,
    is_active BOOLEAN NOT NULL
);

CREATE TABLE invoices
(
    invoice_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    customer_id INT NOT NULL,
    invoice_number VARCHAR(50) NOT NULL,
    total DECIMAL(18,2) NOT NULL,
    created_at DATETIME NOT NULL,
    CONSTRAINT fk_invoices_customers FOREIGN KEY (customer_id) REFERENCES customers(customer_id)
);

CREATE TABLE invoice_lines
(
    invoice_line_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    invoice_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    line_total DECIMAL(18,2) NOT NULL,
    CONSTRAINT fk_invoice_lines_invoices FOREIGN KEY (invoice_id) REFERENCES invoices(invoice_id),
    CONSTRAINT fk_invoice_lines_products FOREIGN KEY (product_id) REFERENCES products(product_id)
);
