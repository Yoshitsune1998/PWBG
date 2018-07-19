using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PWBG_BOT.Core.Items;

namespace PWBG_BOT.Core.PlayerInventory
{
    public static class Inventories
    {
        private static List<Inventory> invs;
        private static string InventFile = "Resources/inventories.json";

        static Inventories()
        {
            if (DataStorage.SaveExist(InventFile))
            {
                invs = DataStorage.LoadInventory(InventFile).ToList();
            }
            else
            {
                invs = new List<Inventory>();
                SaveInvent();
            }
        }

        public static Item GetItems(ulong id)
        {
            var savedInvent = GetOrCreateInventory(id);

        }

        public static Inventory GetOrCreateInventory(ulong id)
        {
            var result = from i in invs
                         where i.IDInvent == id
                         select i;
            var inventory = result.FirstOrDefault();
            if (inventory == null) inventory = CreateInventory(id);
            return inventory;
        }

        private static Inventory CreateInventory(ulong id)
        {
            var newInvent = new Inventory()
            {
                IDInvent = id,
                Items = new List<Items.Item>()
            };
            invs.Add(newInvent);
            SaveInvent();
            return newInvent;
        }

        public static void SaveInvent()
        {
            DataStorage.SaveInventory(invs, InventFile);
        }

    }
}
