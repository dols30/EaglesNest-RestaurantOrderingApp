using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace backendKiosk.Project.Models
{
    public enum MenuItemType { ENTREE, SIDE, DRINK, DESSERT, APPETIZER };
    public enum MenuSizeType { SMALL, MEDIUM, LARGE };
    
    public abstract class CartItem
    {
        private static int _idCounter = 0;
        private readonly int _id;
        public abstract decimal GetCost();

        [Ignore]
        public int ID => _id;

        public CartItem()
        {
            _id = System.Threading.Interlocked.Increment(ref _idCounter);
        }
    }

    public class MenuItem : CartItem 
    {
        [PrimaryKey, AutoIncrement] 
        public int DatabaseID { get; set; }

        // Remove 'required' modifier to make SQLite happy
        public string ItemName { get; set; } = string.Empty;
        public double ItemPrice { get; set; }
        public bool HasSize { get; set; }

        public int SizeInt { get; set; }
        public int TypeInt { get; set; }

        [Ignore] 
        public MenuSizeType Size
        {
            get => (MenuSizeType)SizeInt;
            set => SizeInt = (int)value;
        }

        [Ignore] 
        public MenuItemType Type
        {
            get => (MenuItemType)TypeInt;
            set => TypeInt = (int)value;
        }

        // Remove 'required' modifier and initialize with empty string
        public string Icon { get; set; } = string.Empty;

        public static decimal MEDIUM_RATE = 0.25M;
        public static decimal LARGE_RATE = 0.75M;

        public override decimal GetCost()
        {
            if (!HasSize)
                return (decimal)ItemPrice;
            else
            {
                if (Size == MenuSizeType.SMALL)
                    return (decimal)ItemPrice;
                else if (Size == MenuSizeType.MEDIUM)
                    return (decimal)ItemPrice + (decimal)ItemPrice * MEDIUM_RATE;
                else
                    return (decimal)ItemPrice + (decimal)ItemPrice * LARGE_RATE;
            }
        }

        // Constructor
        public MenuItem(string name, double price, MenuItemType type, string icon, bool hasSize = false)
        {
            ItemName = name;
            ItemPrice = price;
            Type = type;
            Icon = icon;
            HasSize = hasSize;
            Size = MenuSizeType.SMALL;
        }

        // Default constructor needed by SQLite
        public MenuItem() 
        { 
            // Initialization already done via property initializers
        }


        // DeepCopy method
        public MenuItem DeepCopy()
        {
            return new MenuItem
            {
                DatabaseID = this.DatabaseID,
                ItemName = this.ItemName,
                ItemPrice = this.ItemPrice,
                HasSize = this.HasSize,
                SizeInt = this.SizeInt,
                TypeInt = this.TypeInt,
                Icon = this.Icon
            };
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
