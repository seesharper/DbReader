WITH RECURSIVE ctx (
    EmployeeId,  
    FirstName,
    LastName,
    ReportsTo,
    Level
)
AS (
    SELECT initial.EmployeeId,           
           initial.FirstName,
           initial.LastName,
           initial.ReportsTo,
           0
      FROM employees initial
     WHERE initial.ReportsTo IS NULL
    UNION
    SELECT emp.Employeeid,           
           emp.Firstname,
           emp.Lastname,
           emp.ReportsTo,
           ctx.Level + 1
      FROM employees emp
           INNER JOIN
           ctx ON ctx.EmployeeId = emp.ReportsTo
)
SELECT 
    EmployeeId,    
    FirstName,
    LastName,
    ReportsTo
FROM 
    ctx
     ORDER BY level;
