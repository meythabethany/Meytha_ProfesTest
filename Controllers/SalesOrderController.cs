﻿using Meytha_ProfesTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

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
            var customers = GetCustomers();
            ViewBag.Customers = customers;
            return View();

        }

        private List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("usp_customer_lists", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    customers.Add(new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        CustomerName = reader.GetString(1)
                    });
                }
            }

            return customers;
        }

        [HttpPost]
        public IActionResult Save(SalesOrderheader salesOrder, string OrderDetailsJson)
        {
            //System.Diagnostics.Debug.WriteLine($"Sales Order Number: {salesOrder.SalesOrderNumber}");
            //System.Diagnostics.Debug.WriteLine($"Order Date: {salesOrder.OrderDate}");
            //System.Diagnostics.Debug.WriteLine($"Customer: {salesOrder.Customer}");
            //System.Diagnostics.Debug.WriteLine($"Address: {salesOrder.Address}");
            //var orderDetailss = JsonConvert.DeserializeObject<List<SalesOrderDetail>>(OrderDetailsJson);

            //foreach (var detail in orderDetailss)
            //{
            //    System.Diagnostics.Debug.WriteLine($"ProductID: {detail.Product}, Quantity: {detail.Qty}, Price: {detail.Price}, Total: {detail.Total}");
            //}

            try
            {
                var orderDetails = JsonConvert.DeserializeObject<List<SalesOrderDetail>>(OrderDetailsJson);

                SaveSalesOrder(salesOrder, orderDetails);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            ViewBag.Customers = GetCustomers();
            return View("AddEdit", salesOrder);
        }

        private void SaveSalesOrder(SalesOrderheader salesOrder, List<SalesOrderDetail> orderDetails)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("usp_sales_order_saving", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@SalesOrderNumber", salesOrder.SalesOrderNumber);
                    command.Parameters.AddWithValue("@OrderDate", salesOrder.OrderDate);
                    command.Parameters.AddWithValue("@CustomerID", salesOrder.Customer);
                    command.Parameters.AddWithValue("@Address", salesOrder.Address);

                    var orderDetailsTable = new DataTable();
                    orderDetailsTable.Columns.Add("ProductName", typeof(string));
                    orderDetailsTable.Columns.Add("Qty", typeof(int));
                    orderDetailsTable.Columns.Add("Price", typeof(decimal));
                    orderDetailsTable.Columns.Add("Total", typeof(decimal));

                    foreach (var detail in orderDetails)
                    {
                        orderDetailsTable.Rows.Add( detail.Product, detail.Qtys, detail.Price, detail.Total);
                    }

                    var orderDetailsParam = new SqlParameter("@OrderDetails", SqlDbType.Structured)
                    {
                        TypeName = "dbo.DetailItem",
                        Value = orderDetailsTable
                    };
                    command.Parameters.Add(orderDetailsParam);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
