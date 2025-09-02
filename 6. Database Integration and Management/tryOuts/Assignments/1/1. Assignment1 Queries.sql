-- Step 2: Generate basic SELECT queries with Copilot - ProductName, Category, Price, and StockLevel.

USE [SmartShopDB];
SELECT p.Name as ProductName, c.Name as Category, lp.Price, ps.Stock From dbo.Product p  
JOIN dbo.Category c ON p.CategoryId = c.Id
JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
JOIN (SELECT ProductId, MAX(Price) AS Price FROM dbo.PriceInTime GROUP BY ProductId) lp ON p.Id = lp.ProductId; 

-- Step 3: Implement filtering and sorting

-- Products in a specific category

USE [SmartShopDB];
SELECT p.Name as ProductName, c.Name as Category, ps.Stock From dbo.Product p  
JOIN dbo.Category c ON p.CategoryId = c.Id
JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
WHERE p.CategoryId = 1;


-- Products with low stock levels

USE [SmartShopDB];
SELECT p.Name as ProductName, c.Name as Category, ps.Stock From dbo.Product p  
JOIN dbo.Category c ON p.CategoryId = c.Id
JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
WHERE ps.Stock < 20;


-- Add sorting to display products by Price in ascending order

USE [SmartShopDB];
SELECT p.Name as ProductName, c.Name as Category, lp.Price, ps.Stock From dbo.Product p  
JOIN dbo.Category c ON p.CategoryId = c.Id
JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
JOIN (SELECT ProductId, MAX(Price) AS Price FROM dbo.PriceInTime GROUP BY ProductId) lp ON p.Id = lp.ProductId
Order By lp.Price ASC;