/*
I acknowledge the following statements:
That the code I provide below is my own work and NOT copied from any outside resource, which includes, but not limited to, an artificial intelligence program unless given explicit permission by the instructor.
That the code I provide below is my own work and NOT the work of my peers, tutors, or any other individual unless given explicit permission by the instructor.
That if the code below is in violation of statements 1 and 2 above, I may be reported to the Academic Integrity office and subject to penalties as described in the Academic Integrity Policy.
Your Name: Dol Raj Bashyal
Your Student ID: w10169035
*/
using backendKiosk.Project.Data;
using backendKiosk.Project.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace backendKiosk.Project.Controllers
{
    public class MenuController
    {
        private readonly MenuDatabase _database;
        private ObservableCollection<backendKiosk.Project.Models.MenuItem> _menuItems = new ObservableCollection<backendKiosk.Project.Models.MenuItem>();

        public MenuController(MenuDatabase database)
        {
            _database = database;
        }

        private async Task EnsureItemsLoaded()
        {
            if (!_menuItems.Any())
            {
                await LoadMenuItemsAsync();
            }
        }

        public async Task LoadMenuItemsAsync()
        {
            var items = await _database.GetItemsAsync();
            _menuItems.Clear();
            foreach (var item in items)
            {
                _menuItems.Add(item);
            }
        }

        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetMenuItemsAsync()
        {
            await EnsureItemsLoaded();
            return _menuItems;
        }

        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetItemsByTypeAsync(MenuItemType type)
        {
            await EnsureItemsLoaded();
            return new ObservableCollection<backendKiosk.Project.Models.MenuItem>(_menuItems.Where(item => item.Type == type));
        }

        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetEntrees() => await GetItemsByTypeAsync(MenuItemType.ENTREE);
        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetSides() => await GetItemsByTypeAsync(MenuItemType.SIDE);
        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetDrinks() => await GetItemsByTypeAsync(MenuItemType.DRINK);
        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetDesserts() => await GetItemsByTypeAsync(MenuItemType.DESSERT);
        public async Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> GetAppetizers() => await GetItemsByTypeAsync(MenuItemType.APPETIZER);


        public async Task AddItemAsync(backendKiosk.Project.Models.MenuItem item)
        {
            await _database.SaveItemAsync(item);
            _menuItems.Add(item);
        }

        public async Task UpdateItemAsync(backendKiosk.Project.Models.MenuItem item)
        {
            await _database.SaveItemAsync(item);
            var existingItem = _menuItems.FirstOrDefault(mi => mi.DatabaseID == item.DatabaseID);
            if (existingItem != null)
            {
                int index = _menuItems.IndexOf(existingItem);
                _menuItems[index] = item;
            }
        }

        public async Task DeleteItemAsync(backendKiosk.Project.Models.MenuItem item)
        {
            await _database.DeleteItemAsync(item);
            _menuItems.Remove(item);
        }

         public async Task<backendKiosk.Project.Models.MenuItem> GetItemByIdAsync(int id)
        {
             return await _database.GetItemAsync(id);
        }

        // Add backward compatibility methods for existing code
        public ObservableCollection<backendKiosk.Project.Models.MenuItem> GetItems(MenuItemType type)
        {
            // Create a task and wait for it to complete to convert async to sync
            Task<ObservableCollection<backendKiosk.Project.Models.MenuItem>> task = GetItemsByTypeAsync(type);
            task.Wait();
            return task.Result;
        }

        // For compatibility with existing code
        public ObservableCollection<backendKiosk.Project.Models.MenuItem> GetEntreeItems() => GetItems(MenuItemType.ENTREE);
        public ObservableCollection<backendKiosk.Project.Models.MenuItem> GetSideItems() => GetItems(MenuItemType.SIDE);
        public ObservableCollection<backendKiosk.Project.Models.MenuItem> GetDrinkItems() => GetItems(MenuItemType.DRINK);
        public ObservableCollection<backendKiosk.Project.Models.MenuItem> GetDessertItems() => GetItems(MenuItemType.DESSERT);
        public ObservableCollection<backendKiosk.Project.Models.MenuItem> GetAppetizerItems() => GetItems(MenuItemType.APPETIZER);
    }
}
