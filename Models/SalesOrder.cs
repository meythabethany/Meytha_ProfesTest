namespace Meytha_ProfesTest.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string SalesOrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Customer { get; set; }
    }
}
