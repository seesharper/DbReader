namespace DbReader.Tests
{
    using System.ComponentModel.DataAnnotations;

    public class SampleClass
    {
        public int Int32Property { get; set; }
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public Customer Customer { get; set; }
    }


    public class Customer 
    {
        public string CustomerId { get; set; }
    }

    
}