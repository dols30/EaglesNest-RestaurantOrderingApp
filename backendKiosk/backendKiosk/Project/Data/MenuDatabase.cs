/*
I acknowledge the following statements:
That the code I provide below is my own work and NOT copied from any outside resource, which includes, but not limited to, an artificial intelligence program unless given explicit permission by the instructor.
That the code I provide below is my own work and NOT the work of my peers, tutors, or any other individual unless given explicit permission by the instructor.
That if the code below is in violation of statements 1 and 2 above, I may be reported to the Academic Integrity office and subject to penalties as described in the Academic Integrity Policy.
Your Name: Dol Raj Bashyal
Your Student ID: w10169035
*/
using backendKiosk.Project.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;
using SQLitePCL;

namespace backendKiosk.Project.Data
{
    public class MenuDatabase
    {
        // Initialize as nullable to address CS8618 warning
        SQLiteAsyncConnection? Database;
        // Add a semaphore to prevent multiple initialization attempts
        readonly SemaphoreSlim _initLock = new SemaphoreSlim(1, 1);

        public MenuDatabase()
        {
            // Initialize the database when the class is instantiated
            // but don't block the constructor
            _ = Init();
        }

        async Task Init()
        {
            try
            {
                // Ensure only one initialization process runs at a time
                await _initLock.WaitAsync();
                
                if (Database is not null)
                    return;

                // Make sure the directory exists
                string dbPath = DatabaseConstants.DatabasePath;
                string dbDir = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(dbDir) && !Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                }

                // Create the database connection
                Database = new SQLiteAsyncConnection(dbPath, DatabaseConstants.Flags);
                
                // Create the table if it doesn't exist
                await Database.CreateTableAsync<backendKiosk.Project.Models.MenuItem>();
                
                // Check if table is empty
                var count = await Database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM MenuItem");
                
                if (count == 0)
                {
                    await SeedInitialData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                // Re-throw to allow calling code to handle the error
                throw;
            }
            finally
            {
                // Always release the lock
                _initLock.Release();
            }
        }

        private async Task SeedInitialData()
        {
            // Use fully qualified name for MenuItem
            var initialItems = new List<backendKiosk.Project.Models.MenuItem>
            {
                // Entrees
                new backendKiosk.Project.Models.MenuItem("Cheeseburger", 8.99, MenuItemType.ENTREE, "cheeseburger.png"),
                new backendKiosk.Project.Models.MenuItem("Chicken Sandwich", 7.99, MenuItemType.ENTREE, "chickensandwich.png"),
                new backendKiosk.Project.Models.MenuItem("Veggie Burger", 8.49, MenuItemType.ENTREE, "veggieburger.png"),

                // Sides
                new backendKiosk.Project.Models.MenuItem("French Fries", 2.99, MenuItemType.SIDE, "fries.png", true),
                new backendKiosk.Project.Models.MenuItem("Onion Rings", 3.49, MenuItemType.SIDE, "onionrings.png", true),
                new backendKiosk.Project.Models.MenuItem("Salad", 4.99, MenuItemType.SIDE, "salad.png"),

                // Drinks
                new backendKiosk.Project.Models.MenuItem("Soda", 1.99, MenuItemType.DRINK, "soda.png", true),
                new backendKiosk.Project.Models.MenuItem("Iced Tea", 1.99, MenuItemType.DRINK, "icedtea.png", true),
                new backendKiosk.Project.Models.MenuItem("Water Bottle", 1.49, MenuItemType.DRINK, "water.png"),

                // Desserts
                new backendKiosk.Project.Models.MenuItem("Ice Cream", 3.99, MenuItemType.DESSERT, "icecream.png"),
                new backendKiosk.Project.Models.MenuItem("Brownie", 2.99, MenuItemType.DESSERT, "brownie.png"),

                // Appetizers
                new backendKiosk.Project.Models.MenuItem("Mozzarella Sticks", 5.99, MenuItemType.APPETIZER, "mozzarellasticks.png"),
                new backendKiosk.Project.Models.MenuItem("Chicken Wings", 7.49, MenuItemType.APPETIZER, "chickenwings.png")
            };
            await Database.InsertAllAsync(initialItems);
        }


        // Use fully qualified name for MenuItem
        public async Task<List<backendKiosk.Project.Models.MenuItem>> GetItemsAsync()
        {
            await Init();
            
            // Use direct SQL query instead of Table<T>() to avoid 'new()' constraint issue
            return await Database!.QueryAsync<backendKiosk.Project.Models.MenuItem>("SELECT * FROM MenuItem");
        }

        // Use fully qualified name for MenuItem
        public async Task<List<backendKiosk.Project.Models.MenuItem>> GetItemsByTypeAsync(MenuItemType type)
        {
            await Init();
            int typeInt = (int)type;
            
            // Use parameterized SQL query instead of Table<T>() to avoid 'new()' constraint issue
            return await Database!.QueryAsync<backendKiosk.Project.Models.MenuItem>(
                "SELECT * FROM MenuItem WHERE TypeInt = ?", typeInt);
        }

        // Use fully qualified name for MenuItem
        public async Task<backendKiosk.Project.Models.MenuItem> GetItemAsync(int id)
        {
            await Init();
            
            // Use parameterized SQL query instead of Table<T>() to avoid 'new()' constraint issue
            return await Database!.FindAsync<backendKiosk.Project.Models.MenuItem>(id);
        }

        // Use fully qualified name for MenuItem
        public async Task<int> SaveItemAsync(backendKiosk.Project.Models.MenuItem item)
        {
            await Init();
            if (item.DatabaseID != 0)
            {
                return await Database!.UpdateAsync(item);
            }
            else
            {
                return await Database!.InsertAsync(item);
            }
        }

        // Use fully qualified name for MenuItem
        public async Task<int> DeleteItemAsync(backendKiosk.Project.Models.MenuItem item)
        {
            await Init();
            return await Database!.DeleteAsync(item);
        }
    }
}
