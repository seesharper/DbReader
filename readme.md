# DbReader #

* Is **NOT** an ORM
* Does **NOT** try to abstract the database.

### Simple ###

	public class Order
	{		
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
	}

### Code ###


	var orders = dbConnection.Read<Order>(SQL);

### SQL ###

	SELECT 
		OrderId,
		OrderDate
	FROM 
		Orders o 
	WHERE 
		o.OrderId = @OrderId


### Many To One ###

	public class Order
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public Customer Customer { get; set; }
	}

	public class Customer
	{
		public string CustomerId { get; set; }
		public string CompanyName { get; set; }
	}

### Code ###

	var orders = dbConnection.Read<Order>(SQL);


### SQL ###

	SELECT 
		o.OrderId,
		o.OrderDate,
		c.CustomerId,
		c.CompanyName
	FROM
		Orders o
	INNER JOIN 
		Customers c
	ON 
		o.CustomerId = c.CustomerId


### One To Many ###

	public class Order
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public OrderDetail[] OrderDetails { get; set; }
	}

	public class OrderDetails 
	{
		[Key]		
		public int OrderId { get ; set;}
		[Key]		
		public int ProductId { get; set; }
		public int Quantity	{ get; set; }
		public decimal UnitPrice { get; set; } 
	}

### Code ###

	var orders = dbConnection.Read<Order>(SQL);

### SQL ###

	SELECT 
		o.OrderId,	
		o.OrderDate,
		od.OrderId AS OrderDetails_OrderId,
		od.ProductId,
		od.Quantity,
		od.UnitPrice
	FROM
		Order o
	INNER JOIN 
		OrderDetails od
	ON 
		o.OrderId = od.OrderId

## Keys ##

The only requirement with regards to metadata is that the .Net types expose a "key" property. The default convention is to look for a property named ***Id*** or ***[classname]Id***.

	public class Order
	{
		public int Id { get; set; }
	}

	public class Order
	{
		public int OrderId { get; set; }
	}

The convention can easily be changed 

	DbReaderOption.KeyConvention = (property) => property.IsDefined(typeof(KeyAttribute));

The key properties can also be set per type overriding the default convention.

	DbReaderOptions.KeySelector<Order>(o => o.OrderId);


## Prefixes ##

A prefix is used to identify the target navigation property.
