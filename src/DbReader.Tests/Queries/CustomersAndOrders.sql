SELECT 
	c.CustomerID as CustomerWithOrdersId,
	o.OrderId
FROM 
	Customers c
INNER JOIN 
	Orders o 
ON 
	c.CustomerId =o.CustomerId