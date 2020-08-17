/*
 * Qingqing Wu
 * 2020/08/15
 * 
 * Online Pizza Shop
 * main windows for ordering food, login, and check out
 * 
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DarioPizzaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Object for Order class
        Order da = new Order();
        private int cid, oid;
        public MainPage()
        {
            this.InitializeComponent();
        }

        // Window dialog for providing verified login
        private async void DisplayLoginDialog()
        {
            ContentDialog content = new ContentDialog
            {
                Title = "Message",
                Content = "Login verified successfully\nPlease go to check out!",
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await content.ShowAsync();
        }

        // Window dialog for providing order information(price)
        private async void DisplayTotalPriceDialog(double price, int oID)
        {
            ContentDialog content = new ContentDialog
            {
                Title = "Purchase Confirmation",
                Content = "Order#" + oID + " has been processed\nTotal Price: $" + price.ToString(),
                CloseButtonText = "Ok"
            };
            ContentDialogResult result = await content.ShowAsync();
            this.Frame.Navigate(typeof(OrderReceipts),oID);
        }

        // checking users login
        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string eaddr = tbemail.Text.ToString();
            string pwd = tbpwd.Text.ToString();
            
            string query1 = "Select EmailAddress, Password from AccountInfo";
            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query1;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                        DataSet dataSet = new DataSet();
                        sqlDataAdapter.Fill(dataSet, "AccountInfo");
                        DataTable tblAcc = dataSet.Tables["AccountInfo"];
                        var userPwd = new List<string>();
                        var userEmail = new List<string>();

                        foreach (DataRow row in tblAcc.Rows)
                        {
                            userEmail.Add($"{row["EmailAddress"]}".Trim());
                            userPwd.Add($"{row["Password"]}".Trim());
                        }
                        if (userEmail.Contains(eaddr) && userPwd.Contains(pwd))
                        {
                            lbMember.Text = "You got here";
                            DisplayLoginDialog();
                        }
                    }
                }
            }
            cid = da.GetCustomerID((App.Current as App).ConnectionString, eaddr, pwd);

           
        }

        // Create an account
        private void btnregister_Click(object sender, RoutedEventArgs e)
        {
            string newFirstName = tbnewfn.Text.ToString();
            string newLastName = tbnewln.Text.ToString();
            string newEmail = tbnewemail.Text.ToString();
            string newBillingAddr = tbnewbillingaddr.Text.ToString();
            string newShippingAddr = tbnewshippingaddr.Text.ToString();
            string newPassword = tbnewpassword.Text.ToString();
            string pwdConfirm = tbpasswordconfirm.Text.ToString();
            Debug.WriteLine("\n\n");
            Debug.WriteLine("Password valid---!" + String.Equals(newPassword, pwdConfirm));
            if (String.Equals(newPassword, pwdConfirm))
            {
                string query = "Insert into AccountInfo(FirstName, LastName, EmailAddress, BillingAddress, ShippingAddress, Password)" +
                    "values(@FirstName, @LastName, @EmailAddress, @BillingAddress, @ShippingAddress, @Password)";
                using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Parameters.AddWithValue("FirstName", newFirstName);
                            cmd.Parameters.AddWithValue("LastName", newLastName);
                            cmd.Parameters.AddWithValue("EmailAddress", newEmail);
                            cmd.Parameters.AddWithValue("BillingAddress", newBillingAddr);
                            cmd.Parameters.AddWithValue("ShippingAddress", newShippingAddr);
                            cmd.Parameters.AddWithValue("Password", newPassword);
                            cmd.CommandText = query;
                            int result = cmd.ExecuteNonQuery();
                            if (result > 0)
                            {
                                lbRegister.Text = "Registerd successfully and your order has been processed";
                            }
                        }
                    }

                }
            }
            cid = da.GetCustomerID((App.Current as App).ConnectionString, newEmail, newPassword);
        }

        // Go to feedback page to provide feedback
        private void btnprovidefeedback_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FeedBack));
        }

        // check out
        private void btncheckout_Click(object sender, RoutedEventArgs e)
        {
            // a collection for storing all selected products
            var pid = new List<int>();
            double totalPrice = 0;

            // Add price to total price based on the size chosen
            var index = cbxsize.SelectedIndex;
            switch(index)
            {
                case 0:
                    totalPrice += 6.99;
                    break;
                case 1:
                    totalPrice += 8.99;
                    break;
                case 2:
                    totalPrice += 12.99;
                    break;
                case 3:
                    totalPrice += 15.99;
                    break;
            }

            // Add price to total price based on the veg topping chosen
            if (p1.IsChecked == true)
            {
                pid.Add(1);
                totalPrice += 3.00;
            }
            if (p2.IsChecked == true)
            {
                pid.Add(2);
                totalPrice += 3.00;
            }
            if (p3.IsChecked == true)
            {
                pid.Add(3);
                totalPrice += 3.00;
            }
            if (p4.IsChecked == true)
            {
                pid.Add(4);
                totalPrice += 3.00;
            }
            if (p5.IsChecked == true)
            {
                pid.Add(5);
                totalPrice += 3.00;
            }
            if (p6.IsChecked == true)
            {
                pid.Add(6);
                totalPrice += 3.00;
            }
            if (p7.IsChecked == true)
            {
                pid.Add(7);
                totalPrice += 3.00;
            }
            if (p8.IsChecked == true)
            {
                pid.Add(8);
                totalPrice += 3.00;
            }
            if (p9.IsChecked == true)
            {
                pid.Add(9);
                totalPrice += 3.00;
            }

            // Add price to total price based on the meat topping chosen
            if (p10.IsChecked == true)
            {
                pid.Add(10);
                totalPrice += 5.00;
            }
            if (p11.IsChecked == true)
            {
                pid.Add(11);
                totalPrice += 5.00;
            }
            if (p12.IsChecked == true)
            {
                pid.Add(12);
                totalPrice += 5.00;
            }
            if (p13.IsChecked == true)
            {
                pid.Add(13);
                totalPrice += 5.00;
            }
            if (p14.IsChecked == true)
            {
                pid.Add(14);
                totalPrice += 5.00;
            }
            if (p15.IsChecked == true)
            {
                pid.Add(15);
                totalPrice += 5.00;
            }
            if (p16.IsChecked == true)
            {
                pid.Add(16);
                totalPrice += 5.00;
            }
            if (p17.IsChecked == true)
            {
                pid.Add(17);
                totalPrice += 5.00;
            }

            // Add price to total price based on the sea food topping chosen
            if (p18.IsChecked == true)
            {
                pid.Add(18);
                totalPrice += 5.00;
            }
            if (p19.IsChecked == true)
            {
                pid.Add(19);
                totalPrice += 5.00;
            }
            if (p20.IsChecked == true)
            {
                pid.Add(20);
                totalPrice += 5.00;
            }
            if (p21.IsChecked == true)
            {
                pid.Add(21);
                totalPrice += 5.00;
            }
            if (p22.IsChecked == true)
            {
                pid.Add(22);
                totalPrice += 5.00;
            }
            if (p23.IsChecked == true)
            {
                pid.Add(23);
                totalPrice += 5.00;
            }
            if (p24.IsChecked == true)
            {
                pid.Add(24);
                totalPrice += 5.00;
            }

            // Add price to total price based on the cheese topping chosen
            if (p25.IsChecked == true)
            {
                pid.Add(25);
                totalPrice += 3.00;
            }
            if (p26.IsChecked == true)
            {
                pid.Add(26);
                totalPrice += 3.00;
            }
            if (p27.IsChecked == true)
            {
                pid.Add(27);
                totalPrice += 3.00;
            }
            if (p28.IsChecked == true)
            {
                pid.Add(28);
                totalPrice += 3.00;
            }
            if (p29.IsChecked == true)
            {
                pid.Add(29);
                totalPrice += 3.00;
            }
            if (p30.IsChecked == true)
            {
                pid.Add(30);
                totalPrice += 3.00;
            }

            // Add price to total price if any combo is selected
            if (p31.IsChecked == true)
            {
                pid.Add(31);
                totalPrice += 15.99;
            }
            if (p32.IsChecked == true)
            {
                pid.Add(32);
                totalPrice += 15.99;
            }
            if (p33.IsChecked == true)
            {
                pid.Add(33);
                totalPrice += 15.99;
            }
            if (p34.IsChecked == true)
            {
                pid.Add(34);
                totalPrice += 15.99;
            }
            if (p35.IsChecked == true)
            {
                pid.Add(35);
                totalPrice += 15.99;
            }
            if (p36.IsChecked == true)
            {
                pid.Add(36);
                totalPrice += 15.99;
            }
            if (p37.IsChecked == true)
            {
                pid.Add(37);
                totalPrice += 15.99;
            }

            // Add price to total price if any side is selected
            if (p38.IsChecked == true)
            {
                pid.Add(38);
                totalPrice += 6.99;
            }
            if (p39.IsChecked == true)
            {
                pid.Add(39);
                totalPrice += 7.99;
            }
            if (p40.IsChecked == true)
            {
                pid.Add(40);
                totalPrice += 6.99;
            }
            if (p41.IsChecked == true)
            {
                pid.Add(41);
                totalPrice += 7.99;
            }
            if (p42.IsChecked == true)
            {
                pid.Add(42);
                totalPrice += 10.99;
            }
            if (p43.IsChecked == true)
            {
                pid.Add(43);
                totalPrice += 8.99;
            }
            if (p44.IsChecked == true)
            {
                pid.Add(44);
                totalPrice += 11.99;
            }
            if (p45.IsChecked == true)
            {
                pid.Add(45);
                totalPrice += 10.99;
            }
            if (p46.IsChecked == true)
            {
                pid.Add(46);
                totalPrice += 6.99;
            }

            // Add price to total price if any drink is selected
            if (p47.IsChecked == true)
            {
                pid.Add(47);
                totalPrice += 3.50;
            }
            if (p48.IsChecked == true)
            {
                pid.Add(48);
                totalPrice += 2.50;
            }
            if (p49.IsChecked == true)
            {
                pid.Add(49);
                totalPrice += 2.50;
            }
            if (p50.IsChecked == true)
            {
                pid.Add(50);
                totalPrice += 4.50;
            }
            if (p51.IsChecked == true)
            {
                pid.Add(51);
                totalPrice += 3.50;
            }
            if (p52.IsChecked == true)
            {
                pid.Add(52);
                totalPrice += 2.50;
            }
            DateTime odate = DateTime.Now.Date; // get the current order date
            string query = "Insert into Orders(OrderDate) values (@OrderDate)";
            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("OrderDate", odate);
                        cmd.CommandText = query;
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            oid = da.GetOrderID((App.Current as App).ConnectionString);
                            string query2 = "update AccountInfo set OrderID = @OrderID where CustomerID = @CustomerID;";
                            using (SqlCommand cmd2 = conn.CreateCommand())
                            {
                                cmd2.Parameters.AddWithValue("OrderID", oid);
                                cmd2.Parameters.AddWithValue("CustomerID", cid);
                                cmd2.CommandText = query2;
                                int result2 = cmd2.ExecuteNonQuery();
                                if (result2 > 0)
                                {
                                    totalPrice = totalPrice * 0.9; // customers in the system can get 10% discount from the order.
                                    foreach(var proid in pid)
                                    {
                                        string query3 = "Insert into OrderProducts(ProductID, OrderID) values (@ProductID, @OrderID)";
                                        using (SqlCommand cmd3 = conn.CreateCommand())
                                        {
                                            cmd3.Parameters.AddWithValue("ProductID", proid);
                                            cmd3.Parameters.AddWithValue("OrderID", oid);
                                            cmd3.CommandText = query3;
                                            int result3 = cmd3.ExecuteNonQuery();
                                            if (result3 > 0)
                                            {
                                                Debug.WriteLine("Insert products ordered successfully into the database.");
                                            }
                                        }
                                    }
                                    
                                    // pass price and order id for window dialog to display to user
                                    DisplayTotalPriceDialog(totalPrice, oid);
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
