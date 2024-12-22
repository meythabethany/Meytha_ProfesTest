using Meytha_ProfesTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Meytha_ProfesTest.Controllers
{
    public class SalesOrderController : Controller
    {
        private readonly IConfiguration _configuration;
        public SalesOrderController(IConfiguration ConnectionStrings)
        {
            _configuration = ConnectionStrings;
        }

        public IActionResult Index(string keyword, DateTime? orderDate)
        {
            var salesOrders = GetSalesOrders();

            if (!string.IsNullOrEmpty(keyword))
            {
                salesOrders = salesOrders.FindAll(x =>
                    x.SalesOrderNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.Customer.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            if (orderDate.HasValue)
            {
                salesOrders = salesOrders.FindAll(x => x.OrderDate.Date == orderDate.Value.Date);
            }

            return View(salesOrders);
        }

        private List<SalesOrder> GetSalesOrders()
        {
            var salesOrders = new List<SalesOrder>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("usp_sales_order_lists", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            salesOrders.Add(new SalesOrder
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                SalesOrderNumber = reader["SalesOrderNumber"].ToString(),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                Customer = reader["Customer"].ToString()
                            });
                        }
                    }
                }
            }

            return salesOrders;
        }

        public IActionResult AddEdit(int? id)
        {
            return View();

        }

    }
}
