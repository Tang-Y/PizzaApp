/*
 * Qingqing Wu
 * 2020/08/15
 * 
 * Online Pizza Shop
 * Admin log in page
 */
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
    public sealed partial class AdminLogin : Page
    {
        public AdminLogin()
        {
            this.InitializeComponent();
        }

        private void btnlogin_Click(object sender, RoutedEventArgs e)
        {
            string user = tbusername.Text;
            string pwd = tbpwd.Text;
            if(user.Equals("PapaDario") && pwd.Equals("abcde"))
            {
                this.Frame.Navigate(typeof(Admin));
            }
            else
            {
                lbadmin.Text = "You don't have access to this point!";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(FeedBack));
        }
    }
}
