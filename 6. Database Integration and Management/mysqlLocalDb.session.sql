-- CREATE DATABASE technocorp;

-- DROP DATABASE app_db;
USE technocorp;
-- CREATE TABLE departments (
--     department_id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
--     department_name VARCHAR(50) NOT NULL UNIQUE
-- );


-- CREATE TABLE employees (
--         employee_id INT AUTO_INCREMENT PRIMARY KEY,
--     first_name VARCHAR(50) NOT NULL UNIQUE,
--     department_id INT
-- );

-- ALTER TABLE employees
--     ADD COLUMN last_name VARCHAR(50) NOT NULL;

-- ALTER TABLE employees 
-- ADD CONSTRAINT fkey_department 
-- FOREIGN KEY (department_id)
-- REFERENCES departments(department_id);

-- Examine table structure from terminal or script

-- DESCRIBE employees;

-- DESCRIBE departments;


-- INSERT INTO departments (department_name) VALUES ('HR'), ('Finance'), ('Engineering');

-- INSERT INTO employees (first_name, last_name, department_id) VALUES 
-- ('Alice', 'Smith', 1),
-- ('Bob', 'Johnson', 2),
-- ('Charlie', 'Brown', 3),
-- ('David', 'Wilson', 1),
-- ('Eve', 'Davis', 2);

-- 
-- SELECT employees.first_name, employees.last_name, departments.department_name
-- FROM employees
-- JOIN departments ON employees.department_id = departments.department_id;

-- 
SELECT employees.first_name, employees.last_name, departments.department_name
FROM departments
JOIN employees ON employees.department_id = departments.department_id;



SHOW DATABASES;

CREATE TABLE Employees (EmployeeID INT PRIMARY KEY AUTO_INCREMENT, FirstName VARCHAR(50), LastName VARCHAR(50), Department VARCHAR(50));



##########################################################################################

								--ANOTHER SESSION:--

CREATE TABLE Employees (
    EmployeeID INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10, 2),
    HireDate DATE
);

						

INSERT INTO EMPLOYEES (FirstName, LastName, Department, Salary, HireDate) VALUES
('Liam', 'Nguyen', 'Engineering', 85000.00, '2020-03-15'),
('Sophia', 'Smith', 'Marketing', 72000.00, '2019-05-22'),
('Raj', 'Patel', 'Sales', 64000.00, '2021-07-01'),
('Aisha', 'Khan', 'HR', 60000.00, '2020-09-12'),
('Carlos', 'Martinez', 'Engineering', 93000.00, '2018-12-01'),
('Chen', 'Zhao', 'Marketing', 77000.00, '2017-11-05'),
('Amara', 'Okafor', 'Sales', 67000.00, '2022-03-18');


SELECT  UPPER(CONCAT(FirstName,' ' , LastName)) AS FULLNAME FROM EMPLOYEES;


SELECT UPPER(Department) FROM EMPLOYEES;

SELECT FirstName, LOWER(LastName) AS LastName, Department, Salary, HireDate  FROM EMPLOYEES;



SELECT FirstName, LENGTH(FirstName) AS First_Name_Len, LOWER(LastName) AS LastName, Department, Salary, HireDate  FROM EMPLOYEES;


SELECT FirstName, LENGTH(FirstName) AS First_Name_Len, SUBSTRING(LastName, 3), LOWER(LastName) AS LastName, Department, Salary, HireDate  FROM EMPLOYEES;

SELECT SUBSTRING(LastName, 3) FROM EMPLOYEES;


SELECT SUM(Salary) FROM EMPLOYEES;

SELECT SUM(Salary) FROM EMPLOYEES WHERE Department = 'Engineering';

SELECT MIN(Salary) FROM EMPLOYEES;


SELECT MIN(Salary) FROM EMPLOYEES WHERE Department = 'Sales';


SELECT Department, SUM(Salary) FROM EMPLOYEES GROUP BY Department;

SELECT Department, AVG(Salary) FROM EMPLOYEES GROUP BY Department;

SELECT Department, COUNT(Salary) FROM EMPLOYEES GROUP BY Department;


SELECT CONCAT(FirstName, ' ', LastName) as FullName, LENGTH(CONCAT(FirstName, ' ', LastName)) FROM EMPLOYEES;  


SELECT HireDate, Count(*) FROM EMPLOYEES GROUP BY HireDate;

SELECT HireDate, SUM(Salary) FROM EMPLOYEES GROUP BY HireDate;





















