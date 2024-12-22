namespace Meytha_ProfesTest.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Customer { get; set; }
    }

    public class SalesOrderheader
    {
        public string SalesOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public int Customer { get; set; }
        public string Address { get; set; }
        public List<SalesOrderDetail> OrderDetails { get; set; } 
    }

    public class SalesOrderDetail
    {
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
