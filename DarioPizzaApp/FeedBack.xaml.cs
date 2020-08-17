/*
 * Qingqing Wu
 * 2020/08/15
 * 
 * Online Pizza Shop
 * Feedback page for customers to provide feedbacks
 * 
 */
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DarioPizzaApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedBack : Page
    {
        public FeedBack()
        {
            this.InitializeComponent();
        }

        private void btnfeedback_Click(object sender, RoutedEventArgs e)
        {
            string feedback = tbfeedback.Text.ToString();
            string query = "Insert into Feedback(Feedback) values (@Feedback)";
            using (SqlConnection conn = new SqlConnection((App.Current as App).ConnectionString))
            {
                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.Parameters.AddWithValue("Feedback", feedback);
                        cmd.CommandText = query;
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            tblkfeedback.Text = "Feedback was submitted successfully!";
                            tbfeedback.Text = "";
                        }
                    }
                }

            }
        }

        // Navigate back to main page
        private void btnmain_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        // event only can be processed by adminn user
        private void btnadminLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AdminLogin));
        }
    }
}
