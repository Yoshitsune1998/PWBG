using System;
using System.Collections.Generic;
using System.Text;
using PWBG_BOT.Core.Items;

namespace PWBG_BOT.Core.SurvivorInventory
{
    public class Inventory
    {
        public ulong IDInvent { get; set; }
        public List<Item> Items { get; set; }
    }
}
