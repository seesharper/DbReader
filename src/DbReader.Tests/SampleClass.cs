namespace DbReader.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection.Emit;

    public class SampleClass
    {
        public int Int32Property { get; set; }
    }

    public class SampleClassCollection : Collection<SampleClass>
    {
    }


    public class Order
    {
        [Key]
        public long OrderId { get; set; }   
        
        public DateTime? OrderDate { get; set; }     
    }


    public class CustomerWithOrders
    {
        public string CustomerWithOrdersId { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    public class Customer
    {
        public string CustomerId { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }
    }





   


    public class Employee
    {
        public long EmployeeID { get; set; }

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
}