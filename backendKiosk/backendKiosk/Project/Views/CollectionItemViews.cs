using backendKiosk.Project.Models;
using MenuItem = backendKiosk.Project.Models.MenuItem;

namespace backendKiosk.Project.Views
{
    public class ComboItemView : BindableObject
    {
        // Make Item nullable to fix CS8618 warning
        public MenuItem? Item { get; set; }

        public string ItemName
        {
            get
            {
                // Add null check to fix CS8602 warning
                return Item != null ? $"{Item.ItemName}\n${Item.GetCost():F2}" : string.Empty;
            }
        }

        public string Icon
        {
            get { return Item?.Icon ?? string.Empty; }
        }
    }

    class ItemSelectView : BindableObject
    {
        // Make Item nullable to fix CS8618 warning
        public MenuItem? Item { get; set; }

        public string ItemName
        {
            get
            {
                // Add null check to fix CS8602 warning
                return Item != null ? $"{Item.ItemName}\n${Item.GetCost():F2}" : string.Empty;
            }
        }

        public string Icon
        {
            get { return Item?.Icon ?? string.Empty; }
        }
    }

    public class SizeTypeView : BindableObject
    {
        public string Text { get; set; } = "";
        public string Rate { get; set; } = "";
    }

    public partial class CartPopupItemView : BindableObject
    {
        // Make Item nullable to fix CS8618 warning
        public CartItem? Item { get; set; }
        
        public string Detail
        {
            get
            {
                if (Item == null)
                    return string.Empty;
                    
                if (Item is MenuItem menuItem)
                {
                    return menuItem.ItemName;
                }
                else if (Item is ComboItem comboItem)
                {
                    string ss = "";
                    ss += "** Combo **\n";
                    ss += " --> ";
                    ss += comboItem.Entree?.ItemName ?? "No entree";
                    ss += "\n";
                    ss += " --> ";
                    ss += comboItem.Side?.ItemName ?? "No side";
                    ss += "\n";
                    ss += " --> ";
                    ss += comboItem.Drink?.ItemName ?? "No drink";
                    ss += "\n";

                    return ss;
                }
                else
                    return "";
            }
        }

        public string ItemCost
        {
            get
            {
                return Item != null ? $"${Item.GetCost():F2}" : "$0.00";
            }
        }
    }
}
