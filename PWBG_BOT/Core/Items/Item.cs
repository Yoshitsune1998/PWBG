using System;
using System.Collections.Generic;
using System.Text;
using PWBG_BOT.Core.BuffAndDebuff;

namespace PWBG_BOT.Core.Items
{
    public class Item
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public uint Value { get; set; }
        public string Rarity { get; set; }
        public Buff buffs { get; set; }
        public Debuff debuffs { get; set; }
    }
}
