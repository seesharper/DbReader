SELECT 
    e.EmployeeId,
	e.LastName,
	e.FirstName,
    o.OrderId AS Orders_OrderId,
	o.OrderDate as Orders_OrderDate,
	NULL as Territories_TerritoryId,
	NULL as Territories_TerritoryDescription
FROM 
    Employees e
INNER JOIN 
    Orders o 
ON 
    e.EmployeeId = o.employeeId AND
    e.EmployeeId = @EmployeeId 
UNION
SELECT 
	e.EmployeeId,
	e.LastName,
	e.FirstName,
	NULL AS Orders_OrderId,
	NULL AS Orders_OrderDate,
	t.TerritoryId as Territories_TerritoryId,
	t.TerritoryDescription as Territories_TerritoryDescription
FROM 
    Employees e
INNER JOIN 
    EmployeeTerritories et 
ON 
    e.employeeid = et.employeeid AND
    e.employeeid = @EmployeeId 
INNER JOIN 
    Territories t   
ON 
    et.TerritoryId = t.TerritoryId 