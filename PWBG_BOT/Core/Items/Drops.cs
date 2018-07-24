using System;
using System.Collections.Generic;
using System.Linq;
using PWBG_BOT.Core.UserAccounts;
using PWBG_BOT.Core.BuffAndDebuff;

namespace PWBG_BOT.Core.Items
{
    public static class Drops
    {
        private static List<Item> items;
        private static string ItemsFile = "Resources/items.json";

        static Drops()
        {
            if (DataStorage.SaveExist(ItemsFile))
            {
                items = DataStorage.LoadItem(ItemsFile).ToList();
            }
            else
            {
                items = new List<Item>();
                SaveItems();
            }
        }

        public static List<Item> GetItems()
        {
            return items;
        }

        public static Item GetSpecificItem(string name)
        {
            var result = from i in items
                         where i.Name == name
                         select i;
            var item = result.FirstOrDefault();
            return item;
        }

        public static Item GetSpecificItem(ulong id)
        {
            var result = from i in items
                         where i.ID == id
                         select i;
            var item = result.FirstOrDefault();
            return item;
        }

        public static List<Item> PackageOfItem(string name)
        {
            List<Item> newInvent = new List<Item>();
            newInvent.Add(GetSpecificItem(name));
            return newInvent;
        }

        public static List<Item> PackageOfItem(ulong id)
        {
            List<Item> newInvent = new List<Item>();
            newInvent.Add(GetSpecificItem(id));
            return newInvent;
        }

        public static void SaveItems()
        {
            DataStorage.SaveItems(items, ItemsFile);
        }

        public static Item CreatingItem(string name, string type, bool active, int value, string rarity, 
            string description)
        {
            ulong stId = MainStorage.GetValueOf("LatestItemId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in items
                         where i.ID == Id
                         select i;
            var item = result.FirstOrDefault();
            if (item == null) item = CreateItem(Id, name, type, active, value, rarity,description);
            return item;
        }

        private static Item CreateItem(ulong id, string name, string type, bool active, int value, string rarity, string description,
            string buffname="", string debuffname="")
        {
            var newItem = new Item()
            {
                ID = id,
                Name = name,
                Type = type,
                Active = active,
                Value = value,
                Rarity = rarity,
                Buffs = Buffs.BuffExist(buffname),
                Debuffs = Debuffs.DebuffExist(debuffname),
                Description = description
            };
            items.Add(newItem);
            MainStorage.ChangeData("LatestItemId",id);
            SaveItems();
            return newItem;
        }

        public static bool UseTargetItem(Item used, UserAccount user)
        {
            if (user == null && used == null && !used.Active) return false;
            Console.WriteLine("masuk drops");
            ItemTech.UseDecreasingHPItem(used,user);
            return true;
        }

        public static bool UseRandomItem(Item used, UserAccount user)
        {
            if (used == null && !used.Active) return false;

            return true;
        }

        public static bool UseSelfItem(Item used, UserAccount user)
        {
            if (used == null && !used.Active) return false;
            ItemTech.UseIncreasingHPItem(used,user);
            return true;
        }

        public static void PassiveItem(Item used)
        {
            if (used == null && used.Active) return;
        }   

    }
}
