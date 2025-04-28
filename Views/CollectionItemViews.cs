using RestaurantAppFullImp.Project.Models;
using MenuItem = RestaurantAppFullImp.Project.Models.MenuItem;

namespace RestaurantAppFullImp.Project.Views
{

    public class ComboItemView : BindableObject
    {
        public MenuItem Item { get; set; }

        public string ItemName
        {
            get
            {
                return $"{Item.ItemName}\n${Item.GetCost():F2}";
            }
        }

        public string Icon
        {
            get { return Item.Icon; }
        }
    }

    class ItemSelectView : BindableObject
    {
        public MenuItem Item { get; set; }

        public string ItemName
        {
            get
            {
                return $"{Item.ItemName}\n${Item.GetCost():F2}";
            }
        }

        public string Icon
        {
            get { return Item.Icon; }
        }
    }

    public class SizeTypeView : BindableObject
    {
        public string Text { get; set; } = "";
        public string Rate { get; set; } = "";
    }

    public partial class CartPopupItemView : BindableObject
    {
        public CartItem Item { get; set; }
        public string Detail
        {

            get
            {
                if ((Item as RestaurantAppFullImp.Project.Models.MenuItem) != null)
                {
                    //output just name of item.
                    return (Item as RestaurantAppFullImp.Project.Models.MenuItem).ItemName;

                }
                else if ((Item as RestaurantAppFullImp.Project.Models.ComboItem) != null)
                {
                    //output combo details
                    string ss = "";
                    ss += "** Combo **\n";
                    ss += " --> ";
                    ss += (Item as RestaurantAppFullImp.Project.Models.ComboItem).Entree.ItemName;
                    ss += "\n";
                    ss += " --> ";
                    ss += (Item as RestaurantAppFullImp.Project.Models.ComboItem).Side.ItemName;
                    ss += "\n";
                    ss += " --> ";
                    ss += (Item as RestaurantAppFullImp.Project.Models.ComboItem).Drink.ItemName;
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
                return $"${Item.GetCost():F2}";
            }
        }
    }
}
