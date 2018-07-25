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
        public int Value { get; set; }
        public string Rarity { get; set; }
        public List<Buff> Buffs { get; set; }
        public List<Debuff> Debuffs { get; set; }
        public string Description { get; set; }
        public int Countdown { get; set; }
    }
}
