using System.Collections.Generic;
using PWBG_BOT.Core.BuffAndDebuff;
using PWBG_BOT.Core.PlayerInventory;

namespace PWBG_BOT.Core.UserAccounts
{
    public class UserAccount
    {
        public ulong ID { get; set; }
        public int HP { get; set; }
        public Inventory Inventory { get; set; }
        public int Points { get; set; }
        public uint Kills { get; set; }
        public List<Buff> Buffs { get; set; }
        public List<Debuff> Debuffs { get; set; }
        public int TempPoint = 0;
    }
}
