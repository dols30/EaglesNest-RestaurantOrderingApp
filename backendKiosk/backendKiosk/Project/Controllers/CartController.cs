using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using backendKiosk.Project.Models;

namespace backendKiosk.Project.Controllers
{
    public class CartController
    {
        public string CustomerName { get; set; } = "";

        private List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            CartItems.Add(item);
        }

        public void RemoveItem(int ID)
        {
            var item = CartItems.Find(x => x.ID == ID);
            
            if(item != null) 
                CartItems.Remove(item);
        }

        public ObservableCollection<CartItem> GetCartItems()
        {
            return new ObservableCollection<CartItem>(CartItems);
        }
        
        // Add ClearCart method to clear all items from the cart
        public void ClearCart()
        {
            CartItems.Clear();
            CustomerName = "";
        }
    }
}
