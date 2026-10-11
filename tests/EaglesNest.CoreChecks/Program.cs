using System.Globalization;
using backendKiosk.Project.Controllers;
using backendKiosk.Project.Data;
using backendKiosk.Project.Models;
using MenuItem = backendKiosk.Project.Models.MenuItem;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
var passed = 0;
void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
    passed++;
    Console.WriteLine($"PASS {message}");
}

var entree = new MenuItem("Burger", 8.99, MenuItemType.ENTREE, "");
var side = new MenuItem("Fries", 2.99, MenuItemType.SIDE, "", true);
var drink = new MenuItem("Soda", 1.99, MenuItemType.DRINK, "", true);
var originalId = entree.ID;
Check(entree.ID != side.ID && side.ID != drink.ID && entree.ID == originalId, "Cart IDs remain unique and stable after more items are created");
var cart = new CartController();
cart.AddItem(entree); cart.AddItem(side); cart.AddItem(drink);
cart.RemoveItem(side.ID);
Check(cart.GetCartItems().Count == 2 && cart.GetCartItems().Contains(entree) && cart.GetCartItems().Contains(drink), "Removing a selected item keeps the other cart items");

var copy = side.DeepCopy(); copy.Size = MenuSizeType.LARGE;
Check(side.Size == MenuSizeType.SMALL && copy.ID != side.ID && copy.GetCost() == 5.2325m, "Size changes affect the order copy, not the menu source");
copy.Size = MenuSizeType.MEDIUM;
Check(copy.GetCost() == 3.7375m, "Medium adds 25% to sized items");
var combo = new ComboItem { Entree = entree.DeepCopy(), Side = side.DeepCopy(), Drink = drink.DeepCopy() };
Check(combo.GetCost() == 12.573m, "A combo applies the existing 10% discount");
var total = OrderTotals.Calculate(combo.GetCost(), 2);
Check(total.Subtotal == 12.57m && total.Tax == 0.88m && total.Total == 15.45m, "Checkout rounds currency and applies 7% tax plus tip");
Check(OrderTotals.TryParseTip("", out var tip) && tip == 0, "Clearing a tip resets it to zero");
Check(OrderTotals.TryParseTip("2.50", out tip) && tip == 2.50m, "Decimal tips parse correctly");
Check(!OrderTotals.TryParseTip("-1", out _) && !OrderTotals.TryParseTip("abc", out _), "Invalid and negative tips are rejected");
Check(!OrderTotals.TryParseTip("999999999999999999999999999999999", out _), "Overflowing tip input is rejected");
Check(OrderTotals.Calculate(0, 0).Total == 0, "An empty order has a zero total");
cart.CustomerName = "Alex"; cart.ClearCart();
Check(cart.GetCartItems().Count == 0 && cart.CustomerName == "", "Completing an order clears the cart and customer name");

var scratch = Path.Combine(Path.GetTempPath(), "eaglesnest-checks-" + Guid.NewGuid().ToString("N"));
Environment.SetEnvironmentVariable("EAGLESNEST_DATA_DIR", scratch);
SQLitePCL.Batteries_V2.Init();
try
{
    var database = new MenuDatabase();
    var reads = await Task.WhenAll(Enumerable.Range(0, 6).Select(_ => database.GetItemsAsync()));
    Check(reads.All(items => items.Count == 13), "Concurrent first loads seed the database exactly once");
    var controller = new MenuController(database);
    var appetizers = await controller.GetAppetizers();
    Check(appetizers.Count == 2 && appetizers.All(i => i.Type == MenuItemType.APPETIZER), "The appetizer category is available");
    var added = new MenuItem("Test dessert", 3.25, MenuItemType.DESSERT, "");
    await controller.AddItemAsync(added);
    Check(added.DatabaseID > 0 && (await database.GetItemsAsync()).Count == 14, "Menu items persist in SQLite");
    added.ItemPrice = 4.25;
    await controller.UpdateItemAsync(added);
    Check((await controller.GetItemByIdAsync(added.DatabaseID)).ItemPrice == 4.25, "Menu price edits persist");
    await controller.DeleteItemAsync(added);
    Check((await database.GetItemsAsync()).Count == 13, "Menu deletion removes only the requested item");
}
finally
{
    SQLite.SQLiteAsyncConnection.ResetPool();
    if (Directory.Exists(scratch)) Directory.Delete(scratch, recursive: true);
}
Console.WriteLine($"{passed} regression checks passed.");
