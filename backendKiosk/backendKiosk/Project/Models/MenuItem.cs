using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backendKiosk.Project.Models
{
    public enum MenuItemType { ENTREE, SIDE, DRINK, DESSERT, APPETIZER };
    public enum MenuSizeType { SMALL, MEDIUM, LARGE };
    
    public abstract class CartItem
    {
        private static int _idCounter = 0;
        private static int _id;
        public abstract decimal GetCost();

        public int ID {  get { return _id; } }

        public CartItem()
        {
            _id = _idCounter++;
        }
    }

    public class MenuItem: CartItem
    {
        public static decimal MEDIUM_RATE = 0.25M;
        public static decimal LARGE_RATE = 0.75M;
        public int DatabaseID { get; set; } = 0;
        public string ItemName { get; set; } = "NA";
        public decimal ItemPrice { get; set; } = 0.0M;
        public bool HasSize { get; set; } = false;
        public MenuSizeType Size{ get; set; } = MenuSizeType.SMALL;
        public MenuItemType Type { get; set; } = MenuItemType.ENTREE;
        public string Icon { get; set; } = "eagle.png";

        public override decimal GetCost()
        {
            if (!HasSize)
                return ItemPrice;
            else
            {
                if (Size == MenuSizeType.SMALL)
                    return ItemPrice;
                else if (Size == MenuSizeType.MEDIUM)
                    return ItemPrice + ItemPrice * MEDIUM_RATE;
                else
                    return ItemPrice + ItemPrice * LARGE_RATE;
            }
        }

        public MenuItem DeepCopy()
        {
            MenuItem menu = new MenuItem();
            menu.DatabaseID = DatabaseID;
            menu.ItemName = ItemName;
            menu.ItemPrice = ItemPrice;
            menu.HasSize = HasSize;
            menu.Size = Size;
            menu.Type = Type;
            menu.Icon = Icon;
            return menu;
        }
    }

    public class ComboItem: CartItem
    {
        public static decimal DiscountRate = 0.10M;
        public required MenuItem Entree { get; set; }
        public required MenuItem Side { get; set; }
        public required MenuItem Drink { get; set; }

        public decimal Cost
        {
            get
            {
                decimal sum = Entree.GetCost() + Side.GetCost() + Drink.GetCost();
                sum = sum - sum * DiscountRate;

                return sum;
            }
        }

        public override decimal GetCost()
        {
            return Cost;
        }
    }
}
