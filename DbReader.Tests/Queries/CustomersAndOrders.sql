SELECT 
	c.CustomerID,
	o.OrderId
FROM 
	Customers c
INNER JOIN 
	Orders o 
ON 
	c.CustomerId =o.CustomerId