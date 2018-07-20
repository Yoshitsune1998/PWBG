using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PWBG_BOT;
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

        public static Item CreatingItem(string name, string type, bool active, uint value, string rarity, 
            string buffname= "", string debuffname="")
        {
            ulong stId = MainStorage.GetValueOf("LatestItemId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in items
                         where i.ID == Id
                         select i;
            var item = result.FirstOrDefault();
            if (item == null) item = CreateItem(Id, name, type, active, value, rarity, buffname, debuffname);
            return item;
        }

        private static Item CreateItem(ulong id, string name, string type, bool active, uint value, string rarity, 
            string buffname, string debuffname)
        {
            var newItem = new Item()
            {
                ID = id,
                Name = name,
                Type = type,
                Active = active,
                Value = value,
                Rarity = rarity,
                buffs = Buffs.BuffExist(buffname),
                debuffs = Debuffs.DebuffExist(debuffname)
            };
            items.Add(newItem);
            MainStorage.ChangeData("LatestItemId",id);
            SaveItems();
            return newItem;
        }
    }
}
