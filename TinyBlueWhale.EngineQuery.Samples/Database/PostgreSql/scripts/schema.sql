DROP TABLE IF EXISTS invoice_lines;
DROP TABLE IF EXISTS invoices;
DROP TABLE IF EXISTS products;
DROP TABLE IF EXISTS customers;

CREATE TABLE customers
(
    customer_id SERIAL PRIMARY KEY,
    email VARCHAR(320) NOT NULL,
    full_name VARCHAR(250) NOT NULL,
    is_active BOOLEAN NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE products
(
    product_id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    unit_price NUMERIC(18,2) NOT NULL,
    is_active BOOLEAN NOT NULL
);

CREATE TABLE invoices
(
    invoice_id SERIAL PRIMARY KEY,
    customer_id INTEGER NOT NULL REFERENCES customers(customer_id),
    invoice_number VARCHAR(50) NOT NULL,
    total NUMERIC(18,2) NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE TABLE invoice_lines
(
    invoice_line_id SERIAL PRIMARY KEY,
    invoice_id INTEGER NOT NULL REFERENCES invoices(invoice_id),
    product_id INTEGER NOT NULL REFERENCES products(product_id),
    quantity INTEGER NOT NULL,
    line_total NUMERIC(18,2) NOT NULL
);
