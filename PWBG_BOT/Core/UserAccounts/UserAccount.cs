using System.Collections.Generic;
using PWBG_BOT.Core.BuffAndDebuff;
using PWBG_BOT.Core.PlayerInventory;

namespace PWBG_BOT.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong ID { get; set; }
        public string name { get; set; }
        public uint HP { get; set; }
        public Inventory Inventory { get; set; }
        public uint Points { get; set; }
        public uint Kills { get; set; }
        public List<Buff> Buffs { get; set; }
        public List<Debuff> Debuffs { get; set; }
    }
}
