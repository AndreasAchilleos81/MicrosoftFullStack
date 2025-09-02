Copilot helped with the below:
 
 -  Located errors in getting the latest price of a product
 
 - Add a non-clustered index on PriceInTime(ProductId, Date)

 - Add indexes on ProductStock(ProductId) and Product(CategoryId)
 
 - Add index Sales(ProductId, Quantity)
 
 - Add index Sales(ProductId, Quantity)
 
 - Add index TimeToDelivery(SupplierId, TimeToDelivery)
 
 - Add index Supplier(Id, Name)
 
 - Add index on ProductStock.Stock — for the WHERE clause
	- You might also consider a filtered index on ProductStock
	
		```
			CREATE NONCLUSTERED INDEX IX_ProductStock_LowStock
				ON dbo.ProductStock (ProductId, Stock)
					WHERE Stock < 20;
		```

 - Encouraged the use of Inner Joins where applicable to explicitly show the querys intentions
 
 - Mentioned how to filter on INNER JOIN and not with a WHERE Clause later
 
 - Add some Order By clauses
 
 - Recommened to use OFFSET
 
 - Recommended to add more date filters and store location filters
 
 - Recommended to use ORDER BY s.Date DESC, p.Name ASC to see trends
 
 - Located my redundant join 

-- Debugging and Optimizing queries

----  From Assignment 1 query debugging and optimization in Assignment 3

-- Step 2: Generate basic SELECT queries with Copilot - ProductName, Category, Price, and StockLevel.

SELECT 
    p.Name AS ProductName,
    c.Name AS Category,
    pit.Price,
    ps.Stock
FROM dbo.Product p
JOIN dbo.Category c ON p.CategoryId = c.Id
JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
OUTER APPLY (
    SELECT TOP 1 pt.Price
    FROM dbo.PriceInTime pt
    WHERE pt.ProductId = p.Id
    ORDER BY CAST(pt.[Date] AS datetime) DESC
) pit;


-- Products in a specific category


USE [SmartShopDB];
SELECT p.Name as ProductName, c.Name as Category, ps.Stock From dbo.Product p  
INNER JOIN dbo.Category c ON p.CategoryId = c.Id
INNER JOIN dbo.ProductStock ps ON ps.ProductId = p.Id AND p.CategoryId = 1
Order BY p.Name
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;


-- Products with low stock levels

USE [SmartShopDB];
SELECT p.Name AS ProductName, c.Name AS Category, ps.Stock
FROM dbo.Product p
INNER JOIN dbo.Category c ON p.CategoryId = c.Id
INNER JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
WHERE ps.Stock < 20
ORDER BY ps.Stock ASC;



-- Add sorting to display products by Price in ascending order

SELECT 
    p.Name AS ProductName, 
    c.Name AS Category, 
    lp.Price, 
    ps.Stock 
FROM dbo.Product p  
	INNER JOIN dbo.Category c ON p.CategoryId = c.Id
	INNER JOIN dbo.ProductStock ps ON ps.ProductId = p.Id
	OUTER APPLY (
		SELECT TOP 1 pt.Price
		FROM dbo.PriceInTime pt
		WHERE pt.ProductId = p.Id
		ORDER BY CAST(pt.[Date] AS datetime) DESC
	) lp
	ORDER BY lp.Price ASC;
	

---- From Assingment 2 optimizing queries in Assignment 3

-- Step2: Write multi-table JOIN queries with Copilot

USE [SmartShopDB];
SELECT p.Name AS ProductName, s.Date AS SalesDate, st.Location AS StoreLocation, s.Quantity AS UnitsSold
FROM Product p 
JOIN dbo.Sales s ON s.ProductId = p.Id
JOIN dbo.Store st ON s.StoreId = st.Id
WHERE s.Date >= '2024-01-01' AND st.Location = 'Store3'
ORDER BY s.Date DESC, p.Name ASC;

--Step 3: Implement nested queries and aggregation

-- Calculate total sales for each product.

SELECT 
    p.Name AS ProductName, 
    SUM(s.Quantity) AS TotalSales
FROM Product p 
JOIN dbo.Sales s ON s.ProductId = p.Id
GROUP BY p.Id, p.Name
ORDER BY TotalSales DESC;

-- Identify suppliers with the most delayed deliveries.

SELECT 
    s.Id,
    s.Name, 
    MAX(td.TimeToDelivery) AS MaxDeliveryTime, 
    SUM(td.TimeToDelivery) AS TotalDeliveryHours 
FROM dbo.TimeToDelivery td
JOIN dbo.Supplier s ON s.Id = td.SupplierId
GROUP BY s.Id, s.Name
ORDER BY TotalDeliveryHours DESC;


-- Use aggregate functions (e.g., SUM, AVG, MAX) to analyze trends and summarize data
SELECT 
    s.Id,
    s.Name,
    MAX(td.TimeToDelivery) AS MaxDeliveryTime, 
    AVG(td.TimeToDelivery) AS AverageTimeToDeliver, 
    SUM(td.TimeToDelivery) AS TotalDeliveryHours,
    COUNT(td.ProductId) AS DeliveryCount
FROM dbo.TimeToDelivery td 
JOIN dbo.Supplier s ON s.Id = td.SupplierId
GROUP BY s.Id, s.Name
ORDER BY TotalDeliveryHours DESC;
