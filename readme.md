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

| CustomerId    | CompanyName   |
| ------------- |-------------- | 
| ALFKI         | Alfreds Futterkiste |
| ...           | ...      |
| ...           | ...      |

> It is important that each column in the result set matches the property name.  (Case insensitive)

With both the Customer class and the SQL in place, we are ready to query the database.

```
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
| CustomerId    | CompanyName   |
| ------------- |-------------- | 
| ALFKI         | Alfreds Futterkiste |

DbReader makes passing an argument value very easy.

```
dbConnection.Read<Customer>(sql, new {CustomerId = "ALFKI"});
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

| CustomerId    | Name   |
| ------------- |-------------- | 
| ALFKI         | Alfreds Futterkiste |
| ...           | ...      |
| ...           | ...      |




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

| CustomerId|CompanyName|OrderId|OrderDate
| ----------|-----------|-------|---------
|ALFKI|Alfreds Futterkiste|10643|1997-08-25
|ALFKI|Alfreds Futterkiste|10692|1997-10-03
|ALFKI|Alfreds Futterkiste|10702|1997-10-13
|ALFKI|Alfreds Futterkiste|10835|1998-01-15
|ALFKI|Alfreds Futterkiste|10952|1998-03-16
|ALFKI|Alfreds Futterkiste|11011|1998-04-09

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

The only requirement with regards to metadata is that a class must declare a property that uniquely identifies an instance of this class. This information is used to determine if we should create a new instance of a class or retrieve it from the query cache. The query cache makes sure that we don't eagerly create new instances of classes that has already been read. The default convention here is that each class must declare a property named *Id* or *[classname]Id*.
For instance there might be desirable to be more explicit about the key properties and there also might be needed to define composite keys. One solution here is to take advantage of the *KeyAttribute* defined in the System.Components.DataAnnotations namespace.

```
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
```
DbReaderOptions.KeyConvention = (property) => property.IsDefined(typeof(KeyAttribute));
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
DbReaderOptions.WhenPassing<Guid>().Use((parameter, guid) => parameter.Value = guid.ToByArray());
```
















