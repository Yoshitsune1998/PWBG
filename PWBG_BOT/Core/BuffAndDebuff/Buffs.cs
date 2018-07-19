using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWBG_BOT.Core.BuffAndDebuff
{
    public static class Buffs
    {
        private static List<Buff> buffs;
        private static string BuffsFile = "Resources/buffs.json";

        static Buffs()
        {
            if (DataStorage.SaveExist(BuffsFile))
            {
                buffs = DataStorage.LoadBuff(BuffsFile).ToList();
            }
            else
            {
                buffs = new List<Buff>();
                SaveBuffs();
            }
        }

        public static Buff BuffExist(ulong id=0)
        {
            if (id!=0)
            {
                var result = from i in buffs
                             where i.ID == id
                             select i;
                var buf = result.FirstOrDefault();
                if (buf != null)
                {
                    return buf;
                }
            }
            return null;
        }

        public static Buff BuffExist(string name="")
        {
            if (name != "")
            {
                var result = from i in buffs
                             where i.Name == name
                             select i;
                var buf = result.FirstOrDefault();
                if (buf != null)
                {
                    return buf;
                }
            }
            return null;
        }

        public static List<Buff> GetItems()
        {
            return buffs;
        }

        public static void SaveBuffs()
        {
            DataStorage.SaveBuffs(buffs, BuffsFile);
        }

        private static Buff CreatingBuff(string name, string tech, uint value)
        {
            ulong stId = MainStorage.GetValueOf("LatestBuffId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in buffs
                         where i.ID == Id
                         select i;
            var buf = result.FirstOrDefault();
            if (buf == null) buf = CreateBuff(Id, name, tech, value);
            return buf;
        }

        private static Buff CreateBuff(ulong id, string name, string tech, uint value)
        {
            var newBuff = new Buff()
            {
                ID = id,
                Name = name,
                Tech = tech,
                Value = value
            };
            buffs.Add(newBuff);
            MainStorage.ChangeData("LatestBuffId", id);
            SaveBuffs();
            return newBuff;
        }
    }
}
