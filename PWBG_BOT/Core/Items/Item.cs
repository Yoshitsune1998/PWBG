using System;
using System.Collections.Generic;
using System.Text;

namespace PWBG_BOT.Core.Items
{
    public class Item
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public uint value { get; set; }
        public string Rarity { get; set; }
        public bool GiveBuffs { get; set; }
        public bool GiveDebuffs { get; set; }
    }
}
