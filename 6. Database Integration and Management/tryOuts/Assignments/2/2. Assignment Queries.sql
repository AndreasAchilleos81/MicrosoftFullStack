-- Step2: Write multi-table JOIN queries with Copilot

USE [SmartShopDB];
SELECT p.Name AS ProductName, s.Date AS SalesDate, st.Location AS StoreLocation, s.Quantity AS UnitsSold
FROM Product p 
JOIN dbo.Sales s ON s.ProductId = p.Id
JOIN dbo.Store st ON s.StoreId = st.Id;


--Step 3: Implement nested queries and aggregation

-- Calculate total sales for each product.

USE [SmartShopDB];
SELECT p.Name AS ProductName, Sum(s.Quantity) as TotalSales
FROM Product p 
JOIN dbo.Sales s ON s.ProductId = p.Id
JOIN dbo.Store st ON s.StoreId = st.Id
Group by p.Name
ORDER by TotalSales DESC;



-- Identify suppliers with the most delayed deliveries.
SELECT s.Name , MAX(td.TimeToDelivery) AS MaxDeliveryTime, Sum(td.TimeToDelivery) AS TotalDeliveryHours FROM dbo.TimeToDelivery td
JOIN dbo.Supplier s ON s.Id = td.SupplierId
Group BY s.Name
Order By TotalDeliveryHours DESC;



-- Use aggregate functions (e.g., SUM, AVG, MAX) to analyze trends and summarize data

SELECT s.Name , MAX(td.TimeToDelivery) AS MaxDeliveryTime, Avg(td.TimeToDelivery) AS AverageTimeToDeliver, Sum(td.TimeToDelivery) AS TotalDeliveryHours FROM dbo.TimeToDelivery td
JOIN dbo.Supplier s ON s.Id = td.SupplierId
Group BY s.Name;
