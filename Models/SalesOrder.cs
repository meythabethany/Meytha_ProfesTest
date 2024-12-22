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
        public int Qty { get; set; }
        public int Qtys { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
