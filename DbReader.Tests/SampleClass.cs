namespace DbReader.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SampleClass
    {
        public int Int32Property { get; set; }
    }

    public class Order
    {
        [Key]
        public long OrderId { get; set; }

       
    }


    public class Customer
    {
        public string CustomerId { get; set; }

        public ICollection<Order> Orders { get; set; }
    }

    
}