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

        public static Buff GetSpecificBuff(string name)
        {
            if (buffs == null) return null;
            var result = from i in buffs
                         where i.Name.ToLower().Equals(name.ToLower())
                         select i;
            if (result == null) return null;
            var item = result.FirstOrDefault();
            return item;
        }

        public static List<Buff> GetBuffs()
        {
            return buffs;
        }

        public static void SaveBuffs()
        {
            DataStorage.SaveBuffs(buffs, BuffsFile);
        }

        public static Buff CreatingBuff(string name, string tech, int value, int countdown)
        {
            ulong stId = MainStorage.GetValueOf("LatestBuffId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in buffs
                         where i.ID == Id
                         select i;
            var buf = result.FirstOrDefault();
            if (buf == null) buf = CreateBuff(Id, name, tech, value, countdown);
            return buf;
        }

        private static Buff CreateBuff(ulong id, string name, string tech, int value, int countdown)
        {
            var newBuff = new Buff()
            {
                ID = id,
                Name = name,
                Tech = tech,
                Value = value,
                Countdown = countdown
            };
            buffs.Add(newBuff);
            MainStorage.ChangeData("LatestBuffId", id);
            SaveBuffs();
            return newBuff;
        }
    }
}
