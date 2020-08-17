/*
 * Qingqing Wu
 * 2020/08/15
 * 
 * Online Pizza Shop
 * Order class for storing and retrieving all order products
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace DarioPizzaApp
{
    class Order: INotifyPropertyChanged
    {
        // Properties declaration
        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string Password { get; set; }
        public int OrderID { get; set; }
        public string ToppingName { get; set; }
        public string SideName { get; set; }
        public string BeverageName { get; set; }
        public int ComboID { get; set; }
        public int id, orderid;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Order> GetOrders(string connectionString, int orderID)
        {
            const string GetOrderQuery = "select o.OrderID, a.FirstName as'Customer Name', a.ShippingAddress, " +
                "isnull(p.ToppingName, '****') as topping, isnull(s.SideName, '****') as side, isnull(b.BeverageName, '****') as drink, isnull(c.ComboID, 0) as combo " +
                "from OrderProducts op join AccountInfo a on a.OrderID = op.OrderID " +
                "join Orders o on o.OrderID = op.OrderID " +
                "join Products pr on pr.ProductID = op.ProductID " +
                "full outer join Pizza p on p.ToppingID = pr.ProductID " +
                "full outer join Sides s on s.SideID = pr.ProductID " +
                "full outer join Beverages b on b.BeverageID = pr.ProductID " +
                "full outer join Combos c on c.ComboID = pr.ProductID where op.OrderID = @OrderID";
            Debug.WriteLine("Exception: ");
            var orders = new ObservableCollection<Order>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Parameters.AddWithValue("OrderID", orderID);
                            cmd.CommandText = GetOrderQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                // Iteerating through thee order collection to get all data
                                while(reader.Read())
                                {
                                    var order = new Order();
                                    order.OrderID = reader.GetInt32(0);
                                    order.FirstName = reader.GetString(1);
                                    order.ShippingAddress = reader.GetString(2);
                                    order.ToppingName = reader.GetString(3);
                                    order.SideName = reader.GetString(4);
                                    order.BeverageName = reader.GetString(5);
                                    order.ComboID = reader.GetInt32(6);
                                    // Adding into list
                                    orders.Add(order);
                                }
                                // Returning the object 
                                return orders;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            return null;
        }

        // Method to get customer id
        public int GetCustomerID(string connectionString, string eaddress, string password)
        {
            const string GetCIDQuery = "select CustomerID from AccountInfo where EmailAddress = @EmailAddress " +
                "and Password = @Password";
            Debug.WriteLine("Exception: ");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Parameters.AddWithValue("EmailAddress", eaddress);
                            cmd.Parameters.AddWithValue("Password", password);
                            cmd.CommandText = GetCIDQuery;
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                string cid = result.ToString();
                                id = Int32.Parse(cid);
                            }
                        }
                    }

                }
                return id;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            return -1;
        }

        // Method to get order id
        public int GetOrderID(string connectionString)
        {
            const string GetOIDQuery = "select TOP 1 OrderID from Orders order by OrderID desc";
            Debug.WriteLine("Exception: ");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetOIDQuery;
                            var result = cmd.ExecuteScalar();
                            if (result != null)
                            {
                                string oid = result.ToString();
                                orderid = Int32.Parse(oid);
                            }
                        }
                    }

                }
                return orderid;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
            return -1;
        }
    }
}
