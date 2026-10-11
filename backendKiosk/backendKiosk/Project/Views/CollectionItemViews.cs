using backendKiosk.Project.Models;
using MenuItem = backendKiosk.Project.Models.MenuItem;

namespace backendKiosk.Project.Views;

public class ComboItemView : BindableObject
{
    public MenuItem? Item { get; set; }
    public string Name => Item?.ItemName ?? "";
    public string Price => Item is null ? "" : $"${Item.GetCost():F2}";
}

public class ItemSelectView : ComboItemView
{
    public string Description => Item?.HasSize == true ? "Available in small, medium, or large." : "A favorite, made part of your meal.";
}

public class SizeTypeView : BindableObject
{
    public string Text { get; set; } = "";
    public string Rate { get; set; } = "";
}

public class CartPopupItemView : BindableObject
{
    public CartItem? Item { get; set; }
    private static string ItemDetail(MenuItem item) => item.HasSize ? $"{item.ItemName} ({item.Size.ToString().ToLowerInvariant()})" : item.ItemName;
    public string Detail => Item switch
    {
        MenuItem item => ItemDetail(item),
        ComboItem combo => $"Combo · 10% off\n{ItemDetail(combo.Entree)}\n{ItemDetail(combo.Side)}\n{ItemDetail(combo.Drink)}",
        _ => ""
    };
    public string ItemCost => $"${Item?.GetCost() ?? 0:F2}";
}
