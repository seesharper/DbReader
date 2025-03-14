SELECT 
	c.CustomerID as CustomerWithOrdersId,
	o.OrderId as O_OrderId
FROM 
	Customers c
INNER JOIN 
	Orders o 
ON 
	c.CustomerId =o.CustomerId