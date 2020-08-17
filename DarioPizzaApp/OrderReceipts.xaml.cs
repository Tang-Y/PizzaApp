/*
 * Qingqing Wu
 * 2020/08/15
 * 
 * Online Pizza Shop
 * Receipt window for displaying user's order items
 * 
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class OrderReceipts : Page
    {
        public int oid;
        public OrderReceipts()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            oid = Int32.Parse(e.Parameter.ToString());
            Order order = new Order();
            // get the Order collection from the Order class and print them into list view.
            InventoryList.ItemsSource = order.GetOrders((App.Current as App).ConnectionString, oid);
        }
        
      
    }
}
