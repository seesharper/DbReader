## What is DbReader

**DbReader** is first and foremost **NOT** an **ORM**. **DbReader** simply maps rows into classes.
These classes are nothing but representation of the rows returned by the query in the context of .Net. 
They are not entities, not business objects, they are just rows represented as .Net objects.
No Magic.



## The grocery store philosophy

When going to the grocery store (database), make a list (sql) of all the things you need and bring it back in one go rather than driving (quering)
back and forth for every item on the list. Makes sense, right?


## Why not Dapper?

Most notably, Dapper lacks the ability to map master-detail relationships at least without relying on a third party library such as AutoMapper.
If your needs are simple (and they usually are) with just single level objects, you should propably just stick with Dapper as it is an excellent library with great community support.

If you need to map master-detail relationships (eg Order -> OrderLines), you might want to check out this library.

## Do you speak Northwindian?

All the following samples are based on the good old Northwind database since

* It's simple
* It has been around for ages (literally)
* Everybody understands the domain 

![Schema](https://github.com/seesharper/Presentations/blob/gh-pages/SqlPerformance/Northwind.png)

## Database agnostic

Most of the functionality in DbReader is implemented on top of the IDataReader/IDataRecord interface. In fact, DbReader can actually be used without any database at all since there is nothing that states that an IDataReader needs to get its data from a relational database. 


## Single level 

First we create a class that will represent each row returned by the query.

```csharp
public class Customer
{
	public string CustomerId { get; set; }
	public string CompanyName { get; set; }
}
```

Next we define the SQL that will produce a result set that matches the Customer class.

```sql
SELECT
	CustomerId,
	CompanyName
FROM 
	Customers	
```

| CustomerId | CompanyName         |
| ---------- | ------------------- |
| ALFKI      | Alfreds Futterkiste |
| ...        | ...                 |
| ...        | ...                 |

> It is important that each column in the result set matches the property name.  (Case insensitive)

With both the Customer class and the SQL in place, we are ready to query the database.

```sql
var customers = dbConnection.Read<Customer>(sql)
```

## Async
**DbReader** provides the *ReadAsync* method that uses the DbCommand.ExecuteReaderAsync method to fetch the result set. 

```
var result =  await dbConnection.ReadAsync<Customer>(sql)
```

> The code will run synchronously or asynchronously depending on the support for async in the underlying database driver. For instance, Oracle does not implement async properly and hence the query will run synchronously.

## Parameterized Queries 

A very common scenario is for a SQL statement to define a parameter for which an argument value must be passed from the client. 

```sql
SELECT
	CustomerId,
	CompanyName
FROM 
	Customers
WHERE 
	CustomerId = @CustomerId		
```
| CustomerId | CompanyName         |
| ---------- | ------------------- |
| ALFKI      | Alfreds Futterkiste |

DbReader makes passing an argument value very easy.

```
dbConnection.Read<Customer>(sql, new {CustomerId = "ALFKI"});
```
**DbReader** will create an *IDataParameter* for each property of the anonymous type passed in as the argument object.

If we need more control with the regards to the parameters, we can simply assign an *IDataParameter* instance to a property of the argument object.

```C#
 var parameter = new SQLiteParameter(DbType.String) { Value = "ALFKI" }
 
 connection.Read<Customer>(sql, new { CustomerId = parameter });

```



## ArgumentsBuilder

The `ArgumentsBuilder` can be used to dynamically create the arguments object passed to the query. 

```c#
var args = new ArgumentsBuilder().Add("CustomerId", "ALFKI").Build();
connection.Read<Customer>("SELECT * FROM Customers WHERE CustomerId = @CustomerId", args);
```

We can even base an `ArgumentsBuilder` upon an existing object and keep adding arguments.

```c#
var args = new ArgumentsBuilder()
                .From(new { Country = "UK" })
                .Add("City", "London")
                .Build();
connection.Read<Customer>("SELECT * FROM Customers WHERE Country = @Country AND City = @City",
```

## Record types

**DbReader** supports [C# records](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record) meaning that we can ensure immutability and it also makes it possible to create a class representing the result of a query with minimum effort.

```c#
public record Customer(string CustomerId, string CompanyName)
```

To define key "properties", we can decorate the constructor arguments with the `KeyAttribute`  

```c#
public record Customer([attribute:Key]string CustomerId, string CompanyName)
```

> Note: The `[attribute:Key]` syntax simply tells the compiler to add the `KeyAttribute` to the generated property

## List Parameters

A pretty typical scenario is that we have a list of values o the .Net side that we want to pass into a SQL statement.

```c#
var customers = connection.Read<Customer>(sql, new { Id1 = "ALFKI", Id2 = "BLAUS" });
```

The SQL would look something like this

```SQL
SELECT
	CustomerId,
	CompanyName
FROM 
	Customers
WHERE 
	CustomerId IN ( @Id1, @Id2)
```

The problem with this approach is that it is not very dynamic as we would have to change the SQL and the arguments object if we wanted to add another value to the list. 

**DbReader** solves this by letting us pass an  `IEnumerable<T>` that will  expand the parameter list based on the number  of items in the list that we pass in. Note that this will only work for IN clauses in the SQL.

```SQL
SELECT
	CustomerId,
	CompanyName
FROM 
	Customers
WHERE 
	CustomerId IN ( @Ids )
```

We can now supply a list of customer ids like this.

```c#
var customers = connection.Read<Customer>(sql, new { Ids = new[] { "ALFKI", "BLAUS" } });
```

What happens under the hood here is that the SQL will expand the parameter list and the SQL will look like this. 

```SQL
SELECT
	CustomerId,
	CompanyName
FROM 
	Customers
WHERE 
	CustomerId IN (@Ids0, @Ids1)
```




## Aliasing

Sometimes we want give a property a different name than the target column returned from the query. The easiest way of doing this is simply to take advantage of column aliasing in the SQL statement.

```csharp
public class Customer
{
	public string CustomerId { get; set; }
	public string Name { get; set; } 
}
```

The property "Name" no longer matches the column "CompanyName". 
So instead of providing metadata in the client that describes the mapping between a property and a column, we can simply modify the SQL so that it matches the target property name.  It does not get much simpler than that.

```sql
SELECT
	CustomerId,
	CompanyName AS Name
FROM 
	Customers	
```

| CustomerId | Name                |
| ---------- | ------------------- |
| ALFKI      | Alfreds Futterkiste |
| ...        | ...                 |
| ...        | ...                 |




## Master-Detail

Now we need to create a class that can represent the rows returned from the query as a master-detail relationship.

```csharp
public class Customer
{
    public string CustomerId { get; set; }
    public string CompanyName { get; set; }			
    public ICollection<Order> Orders { get; set; }
}

public class Order
{
	public int OrderId { get; set; }
	public DateTime OrderDate { get; set; }
} 	
```

Now we need to create a query that brings back both *Customers* and *Orders*.

```sql
SELECT
	c.CustomerId,
	c.CompanyName,
	o.OrderId,
	o.OrderDate
FROM 
	Customers c
INNER JOIN 
	Orders o
ON
	c.CustomerId = o.CustomerId	AND
	c.CustomerId = @CustomerID
```

| CustomerId | CompanyName         | OrderId | OrderDate  |
| ---------- | ------------------- | ------- | ---------- |
| ALFKI      | Alfreds Futterkiste | 10643   | 1997-08-25 |
| ALFKI      | Alfreds Futterkiste | 10692   | 1997-10-03 |
| ALFKI      | Alfreds Futterkiste | 10702   | 1997-10-13 |
| ALFKI      | Alfreds Futterkiste | 10835   | 1998-01-15 |
| ALFKI      | Alfreds Futterkiste | 10952   | 1998-03-16 |
| ALFKI      | Alfreds Futterkiste | 11011   | 1998-04-09 |

As we can see from the result we now have six rows. One for each *Order* and the *Customer* columns are duplicated for each *Order*. 

This is where the true power of DbReader comes into play as we don't have to do anything special to map these rows into our class representing the master-detail relationship. 

```
var customers = dbConnection.Read<Customer>(sql, new {CustomerId = "ALFKI"});
```

> **DbReader** makes sure that only one instance of the *Customer* class is ever instantiated even if the customer information is "duplicated" six times in the result set.

There is no limitation as to the number of levels **DbReader** can handle meaning that we could create a query that represents a master-detail-subdetail query.

## Many To One

If we look at the query we used in the master-detail example, we have already established that there is a one-to-many relationship between the *Customers* table and the *Orders* table. Seen from the *Orders* table's "point of view", there is also a many-to-one relationship between the Orders table and the *Customers* table. 

So if we wanted to get a list of orders with their related customers, we would need to create a class that models this.

```
public class Order
{
	public int OrderId { get; set; }
	public DateTime OrderDate { get; set; }
	public Customer Customer { get; set; }
}
```

The SQL is exactly the same and the only difference is that we ask **DbReader** to map the result into a list of *Order* instances.

```sql
SELECT
	c.CustomerId,
	c.CompanyName,
	o.OrderId,
	o.OrderDate
FROM 
	Customers c
INNER JOIN 
	Orders o
ON
	c.CustomerId = o.CustomerId	AND
	o.ShipCity = "London"
```

```
var orders = dbConection.Read<Order>(sql)
```



## Multiple "parallell" collections

If we look at the *Employees* table we see that there is a "one-to-many" relationship between the *Employees* table and the *Orders* table. There is also another "one-to-many" relationship between the Employees table and the EmployeeTerritories which in turn has a "many-to-one" relationship to the Territories table. 

```
Employees -< Orders
Employees -< EmployeeTerritories >- Territories
```

> Tables such as the *EmployeeTerritories* table, are often referred to as junction tables. Their main purpose is to allow for "many-to-many" relationships, in this case between *Employees* and *Territories*.

```
public class Employee
{
    public int EmployeeID { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public ICollection<Order> Orders { get; set; }
    public ICollection<Territory> Territories { get; set; }
}

public class Territory
{
    public string TerritoryId { get; set; }
    public string TerritoryDescription { get; set; }
}

public class Order
{   
    public long OrderId { get; set; }    
    public DateTime OrderDate { get; set; }            
}
```

The following SQL uses the *UNION* keyword that basically allows separate result sets to be merged together before being returned to the client.

```sql
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
    e.EmployeeId = 7 
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
    e.employeeid = 7 
INNER JOIN 
    Territories t   
ON 
    et.TerritoryId = t.TerritoryId
```

> Most databases implement some form of TDS (Tabular Data Stream) that is highly optimized with regards to duplicate column values and null values.   



## Keys

The only requirement with regards to metadata is that a class must declare a property that uniquely identifies an instance of this class. This information is used to determine if we should create a new instance of a class or retrieve it from the query cache. The query cache makes sure that we don't eagerly create new instances of classes that has already been read. The default convention here is that each class must declare a property named *Id*, *[classname]Id* or decorate the key property with the `KeyAttribute` from the `System.ComponentModel.DataAnnotations` namespace.


```c#
public class OrderDetail
{
	[Key]
	public int OrderId { get; set; }
	[Key]
	public int ProductId { get; set ; }
	public decimal UnitPrice { get; set; }
}
```
The convention can easily be changes through the DbReaderOptions class.
```c#
DbReaderOptions.KeyConvention = (property) => property.IsDefined(typeof(KeyAttribute));
```

Another possibility is to supply this information using the DbReaderOptions`KeySelector` method.

```c#
DbReaderOptions.KeySelector<Customer>(c => c.CustomerId)
```

## Prefixes

All the previous examples assumes that column names are unique in the result set.  This might not always be the case and prefixes are used to specify the target property for a given column.
Consider the following query.
```sql
SELECT
	c.CustomerId,
	c.CompanyName,
	c.ModifiedBy,
	o.OrderId,
	o.OrderDate,
	o.ModifiedBy
FROM 
	Customers c
INNER JOIN 
	Orders o
ON
	c.CustomerId = o.CustomerId	AND
	c.CustomerId = @CustomerID
```

```csharp
public class Customer
{
    public string CustomerId { get; set; }
    public string CompanyName { get; set; }			
    public ICollection<Order> Orders { get; set; }
    public string ModifiedBy { get; set; }
}

public class Order
{
	public int OrderId { get; set; }
	public DateTime OrderDate { get; set; }
    public string ModifiedBy { get; set; }
} 	
```

The *Orders* table and the *Customers* table both define the *ModifiedBy* column and we need a way to tell **DbReader** which *ModifiedBy* column goes into which *ModifiedBy* property.  

The solution is really quite simple. We just need to prefix the ModifiedBy column that originates from the Orders table with the name of the navigation property.

> The term *NavigationProperty* means a property that either navigates downwards (one-to-many) or navigates upwards (many-to-one). 

Syntax: [NavigationPropertyName]_[propertyName] 

```sql
SELECT
	c.CustomerId,
	c.CompanyName,
	c.ModifiedBy,
	o.OrderId,
	o.OrderDate,
	o.ModifiedBy AS Orders_ModifiedBy
FROM 
	Customers c
INNER JOIN 
	Orders o
ON
	c.CustomerId = o.CustomerId	AND
	c.CustomerId = @CustomerID
```

The length of the alias name might actually be a problem since we can nest these properties indefinitely.  Consider the following prefix/alias.
```
ol.ModifiedBy AS FirstLevelProperty_SecondLevelProperty_ThirdLevelProperty_ModifiedBy
```
Some database engines might not allow for such long identifiers and **DbReader** allows for CamelHumps (A ReSharper term) that basically means that we compress the property name into its capital letters.

```
ol.ModifiedBy AS FLP_SLP_TLP_ModifiedBy
```

## Stored Procedures

**DbReader** makes no attempt to generalize calling stored procedures as this in most cases requires code that is specific to the database engine. **DbReader** allows custom initialization of an IDbCommand through the DbReaderOptions.CommandInitializer property.

This is an extension point where we can plug in support for features that are specific to the database engine. The following example shows how to add support for calling an Oracle procedure.

```
DbReaderOptions.CommandInitializer = InitializeCommand;

private static void InitializeCommand(IDbCommand command)
{
	if (!command.CommandText.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase))
	{
		command.CommandType = CommandType.StoredProcedure;
		command.Parameters.Insert(0, new OracleParameter
		{
			OracleDbType = OracleDbType.RefCursor,
			Direction = ParameterDirection.ReturnValue
		});
	}
}
```



## Custom Conversions

Sometimes there is a "mismatch" between the .Net type system and the types exposed by the underlying database.
For instance, Oracle does not provide a Guid datatype and the most common way of storing a Guid is by using a byte array (raw[16]).

Since we probably don't want to represent a Guid as a byte array in our classes, we need to register a custom delegate to handle the conversion from a byte array to a Guid.

```
DbReaderOptions.WhenReading<Guid>().Use((datarecord, ordinal) => new Guid(dataRecord.ReadBytes(1,16)));
```

This instructs **DbReader** to use our custom read delegate whenever it encounters a *Guid* property.

The *Guid* also needs to be converted back into a byte array when passing a *Guid* value as a parameter to a query.

```
DbReaderOptions.WhenPassing<Guid>().Use((parameter, guid) => parameter.Value = guid.ToByArray()
```

> Note that the conversion function is only invoked if the value from the database is NOT `DBNull`

### Default values

When using a custom conversion function (WhenReading) it is possible to define a default for the value to be used when value from the database is `DBNull`.

Say that we have a class with a property of type `CustomValueType`

```c#

public class CustomValueType
{
	public CustomValueType(int value)
	{
		Value = value;
	}

	public int Value { get; }
}

public class MyClass
{
	public CustomValueType SomeProperty { get; set; }
}
```

When the property `SomeProperty` we can specify what to return in the case of the field (SomeProperty) being `DBNull` from the `IDataRecord`
 
```C#
DbReaderOptions.WhenReading<CustomValueType>().Use((dr, i) => new CustomValueType(dr.GetInt32(i))).WithDefaultValue(new CustomValueType(42));
```


## Simple Types

Sometimes we just want to get a list of a simple types such as `string` or maybe an `integer`

```c#
var customerIds = connection.Read<string>("SELECT CustomerID FROM Customers");
```

This will simply return a list of strings representing the customer ids.



## Hierarchical/Recursive Queries 

Self referencing tables represents an infinite parent-child relationship and the following example shows how to write a query that recursively finds employees and their managers. This example uses SQLite, but most relational databases have a similar way of dealing with recursive queries.

First lets have a look at how the employees table looks like

| EmployeeID | FirstName | LastName  | ReportsTo |
| ---------- | --------- | --------- | --------- |
| 1          | Nancy     | Davolio   | 2         |
| 2          | Andrew    | Fuller    | NULL      |
| 3          | Janet     | Leverling | 2         |
| 4          | Margaret  | Peacock   | 2         |
| 5          | Steven    | Buchanan  | 2         |
| 6          | Michael   | Suyama    | 5         |
| 7          | Robert    | King      | 5         |
| 8          | Laura     | Callahan  | 2         |
| 9          | Anne      | Dodsworth | 5         |



The *ReportsTo* column refers back to the *Employees* table and indicates the manager for each employee. For instance we can see that *Margaret* reports directly to Andrew who again reports to nobody (He is the boss), where as Laura reports to Margaret that in turn reports to Andrew. 

Using what is known as a [Common Table Expression](https://www.sqlite.org/syntax/common-table-expression.html) we can create a query that shows the relationship using a level column that we in turn will use to map our result into a class.

```sql
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
```



This will give us the following result.

| EmployeeId | FirstName | LastName  | ReportsTo |
| ---------- | --------- | --------- | --------- |
| 2          | Andrew    | Fuller    | NULL      |
| 5          | Steven    | Buchanan  | 2         |
| 8          | Laura     | Callahan  | 2         |
| 1          | Nancy     | Davolio   | 2         |
| 3          | Janet     | Leverling | 2         |
| 4          | Margaret  | Peacock   | 2         |
| 9          | Anne      | Dodsworth | 5         |
| 7          | Robert    | King      | 5         |
| 6          | Michael   | Suyama    | 5         |

The result is now ordered by level which greatly simplifies the mapping code on the C# side.

```c#
var employees = connection.Read<Employee>(SQL.EmployeesHierarchy).ToArray();
var map = new Dictionary<long?, Employee>();
foreach (var employee in employees)
{                    
  if (employee.ReportsTo != null)
  {
  	map[employee.ReportsTo].Employees.Add(employee);
  }
  map.Add(employee.EmployeeId, employee);
}

var initialEmployee = map.First().Value;
```
The *Employee* class looks like this 

```c#
public class Employee
{
    public long EmployeeId { get; set; }

    public string LastName { get; set; }

    public string FirstName { get; set; }
    
    public long? ReportsTo { get; set; }
    
    public ICollection<Employee> Employees { get; set; }
}
```


> Note: The *ReportsTo* property might not actually be needed later on and we could for instance in a Web application decorate this property with the *JsonIgnoreAttribute* to avoid that this property is exposed to the client. 



## Command Initialization

The `DbReaderOptions` class exposes a `CommandInitializer` property that can be used to initialize an `IDbCommand` after it has been created by `DbReader`.  

The following example uses an anonymous PL/SQL block to retrieve a list of employees from the SCOTT schema.

> The SCOTT schema contains a few simple tables that if often used as examples when working with Oracle. 



````plsql
BEGIN
	OPEN :p_cursor FOR
	SELECT 
		e.EMPNO,
		e.ENAME
	FROM 
		SCOTT.EMP e
	WHERE 
    	e.ENAME LIKE :p_name;
END;	
````

In order to return a result set from a PL/SQL block, the first parameter needs to be a `OracleDbType.RefCursor` and its direction has to be `ParameterDirection.ReturnValue`. 

We could do this by adding the cursor parameter to the arguments object like this.

```c#
connection.ReadAsync<Employee>(sql, new {p_cursor = new OracleParameter{OracleDbType = OracleDbType.RefCursor, Direction = ParameterDirection.ReturnValue}, p_name = "A%"});
```

If we have multiple queries such as this one, it might make more sense to define a convention that always adds the needed cursor parameter. 

```c#
DbReaderOptions.CommandInitializer = (command) => {
    if (command.CommandText.Contains("p_cursor"))
    {
         command.Parameters.Insert(0, new OracleParameter
         {
         	OracleDbType = OracleDbType.RefCursor,
         	Direction = ParameterDirection.ReturnValue,
         	ParameterName = "p_cursor"                        
         });
    }
} 	
```

We can now execute the query without specifying the `p_cursor` parameter.

```c#
connection.ReadAsync<Employee>(SQL.emp, new {p_name = "A%"});
```

## Tracking

As stated previously, `DbReader` is NOT an ORM and the support for simple object tracking does not change that
although it opens up a couple of doors for more dynamic SQL, specially when doing updates in the database.

First of all what do we mean by tracking? Let's illustrate with an example. 

Say that we have an WebAPI application that manages customers. We will focus on the update part here. 

The table looks something like the following (SQLite)

```sql
CREATE TABLE Customers (
    CustomerID TEXT NOT NULL,
    CompanyName TEXT NOT NULL,
    ContactName TEXT,
    ContactTitle TEXT,
    Address TEXT,
    City TEXT,
    Region TEXT,
    PostalCode TEXT,
    Country TEXT,
    Phone TEXT,
    Fax TEXT,
    PRIMARY KEY (CustomerID)
);
```

For updating the Customer we could imagine a class or a record like this

```c#
public record Customer(
    string CustomerID,
    string CompanyName,
    string? ContactName = null,
    string? ContactTitle = null,
    string? Address = null,
    string? City = null,
    string? Region = null,
    string? PostalCode = null,
    string? Country = null,
    string? Phone = null,
    string? Fax = null
);
```
The update statement for this table would be 

```sql
UPDATE Customers
SET
    CompanyName = @CompanyName,
    ContactName = @ContactName,
    ContactTitle = @ContactTitle,
    Address = @Address,
    City = @City,
    Region = @Region,
    PostalCode = @PostalCode,
    Country = @Country,
    Phone = @Phone,
    Fax = @Fax
WHERE
    CustomerID = @CustomerID;
```	

So in `DbReader`this would be as simple as

```c# 
await dbConnection.ExecuteAsync(sql, customer);
```
> sql` is the update statement and `customer` is an instance of the `Customer` record.
 
Everything looks and works pretty much as expected so again....
What is `Tracking` and how could it be useful for updating a customer?

Let's imagine that we wanted to update ONLY the `PostalCode` for a given customer?
What are our options here?

* Create a new update statement and the plumbing (data access) in C# to update just the `PostalCode`

* Use the existing update statement supplying the new `PostalCode` along with ALL the other values that hasn't changed
  Note that the `Customer`record has a lot of nullable properties. We need to supply them as well.

* Use `Tracking` to determine which of the properties of the `Customer` record has actually been set and therefore eligible for update. 

Back to the WebAPI application where we have an endpoint for patching a customer

```
	PATCH api/customers
```

where the body of the request is a JSON mapping fully or partially to the `Customer` record.

> Note: To keep things simple we assume that everything is provided in the body of the request, including the `CustomerID` which normally would be part of the URL.


So it could be 

```json
{
  "CustomerID": "ALFKI",
  "CompanyName": "Alfreds Futterkiste",
  "ContactName": "Maria Anders",
  "ContactTitle": "Sales Representative",
  "Address": "Obere Str. 57",
  "City": "Berlin",
  "Region": null,
  "PostalCode": "12209",
  "Country": "Germany",
  "Phone": "030-0074321",
  "Fax": "030-0076545"
}
```
or it could be just the `PostalCode` which is what we are looking to to update in this example.

```json
{
  "CustomerID": "ALFKI",
  "PostalCode": "12209",  
}
```

The goal here is to use the same endpoint and the same C# data access code when doing a full update and also 
when doing a partial update for the `PostalCode`

When the JSON comes into the endpoint, Asp.Net will deserialize the request into an instance of the `Customer` record populating the record with the properties found in the JSON. 
A C# record is just a regular class with "readonly" properties. They can still be set using reflection which is how `System.Text.Json.JsonSerializer`sets these properties.

The key takeaway here is that it will only set the properties found in the JSON.
So what if we could keep track of which properties that was actually set?

Lets make some changes to the `Customer` record. In fact, for simplicity lets just make it a class. 

But first an interface to let us retrieve the properties that has been set/modified. 

```c#
/// <summary>
/// This interface is implemented by classes that are tracked for changes.
/// </summary>
public interface ITrackedObject
{
    /// <summary>
    /// Gets a list of the properties that have been modified.
    /// </summary>
    /// <returns></returns>
    HashSet<string> GetModifiedProperties();
}
```
Then our new Customer record

```c#
public record Customer : ITrackedObject
{
	private readonly HashSet<string> modifiedProperties = [];
	private string _companyName;
	private string _customerId;

	public string CustomerId
	{
		get => _customerId;
		set
		{
			_customerId = value;
			modifiedProperties.Add(nameof(CustomerId));
		}
	}

	public string CompanyName
	{
		get => _companyName;
		set
		{
			_companyName = value;
			modifiedProperties.Add(nameof(CompanyName));
		}
	}

	// The rest of the properties .............

	public HashSet<string> GetModifiedProperties()
	{
		return modifiedProperties;
	}
}
```
This now gives us the ability to figure out which properties has changed and this information can be used when building the arguments object that is passed to `DbReader`when executing the query. 

```c#
var arguments = new ArgumentsBuilder().From(customer).Build();
```

This arguments object will now be an object with properties from the `customer` instance, but since `Customer`implements `ITrackedObject` it will ONLY contain the properties that has been set/modified.

At this point we have the arguments object to be passed to `DbReader`, but in order for this to work we also need to build the SQL that represents our update statement. 

```c#
var modifiedProperties = ((ITrackedObject)positionalRecord).GetModifiedProperties();
string sql = $"""
	UPDATE Customers
	SET {string.Join("\n", modifiedProperties.Select(prop => $"{prop} = @{prop}"))}
	WHERE Id = @Id
""";
```

So in the example where we just provided the `PostalCode` the resulting SQL would be 

```sql
UPDATE Customers
SET PostalCode = @PostalCode
WHERE CustomerId = @CustomerId
```

We are finally ready to actually perform the update in the database

```c#
await dbConnection.ExecuteAsync(sql, arguments);
```

### DbReader.Tracking

As we learned previously we can leverage this functionally by rewriting our `Customer` record from 

```c#
public record Customer(
    string CustomerID,
    string CompanyName,
    string? ContactName = null,
    string? ContactTitle = null,
    string? Address = null,
    string? City = null,
    string? Region = null,
    string? PostalCode = null,
    string? Country = null,
    string? Phone = null,
    string? Fax = null
);
```

to a version that supports tracking changes to properties 

```c#
public record Customer : ITrackedObject
{
	private readonly HashSet<string> modifiedProperties = [];
	private string _companyName;
	private string _customerId;

	public string CustomerId
	{
		get => _customerId;
		set
		{
			_customerId = value;
			modifiedProperties.Add(nameof(CustomerId));
		}
	}

	public string CompanyName
	{
		get => _companyName;
		set
		{
			_companyName = value;
			modifiedProperties.Add(nameof(CompanyName));
		}
	}

	// The rest of the properties .............

	public HashSet<string> GetModifiedProperties()
	{
		return modifiedProperties;
	}
}
```

This would require a lot of tedious plumbing code every time we would want this type of behavior. 

`DbReader.Tracking` makes this very easy by just requiring an attribute to be set on the class/record that we want to track.

```c#
[Tracked]
public record Customer(
    string CustomerID,
    string CompanyName,
    string? ContactName = null,
    string? ContactTitle = null,
    string? Address = null,
    string? City = null,
    string? Region = null,
    string? PostalCode = null,
    string? Country = null,
    string? Phone = null,
    string? Fax = null
);
```

By applying the `TrackedAttribute`, `DbReader.Tracking` will automatically implement the `ITrackedObject` interface and modify each property setter to track changes when properties are being set.
This happens at compile time using Mono.Cecil


