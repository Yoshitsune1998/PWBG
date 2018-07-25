using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PWBG_BOT.Core.BuffAndDebuff
{
    public static class Debuffs
    {
        private static List<Debuff> debuffs;
        private static string DebuffsFile = "Resources/debuffs.json";

        static Debuffs()
        {
            if (DataStorage.SaveExist(DebuffsFile))
            {
                debuffs = DataStorage.LoadDebuff(DebuffsFile).ToList();
            }
            else
            {
                debuffs = new List<Debuff>();
                SaveDebuffs();
            }
        }

        public static Debuff DebuffExist(ulong id = 0)
        {
            if (id != 0)
            {
                var result = from i in debuffs
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

        public static Debuff DebuffExist(string name = "")
        {
            if (name != "")
            {
                var result = from i in debuffs
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

        public static Debuff GetSpecificDebuff(string name)
        {
            if (debuffs == null) return null;
            var result = from i in debuffs
                         where i.Name.ToLower().Equals(name.ToLower())
                         select i;
            if (result == null) return null;
            var item = result.FirstOrDefault();
            return item;
        }

        public static List<Debuff> GetDebuffs()
        {
            return debuffs;
        }

        public static void SaveDebuffs()
        {
            DataStorage.SaveDebuffs(debuffs, DebuffsFile);
        }

        public static Debuff CreatingDebuff(string name, string tech, int value, int countdown)
        {
            ulong stId = MainStorage.GetValueOf("LatestDebuffId");
            ulong Id = (ulong)Convert.ToInt32(stId) + 1;
            var result = from i in debuffs
                         where i.ID == Id
                         select i;
            var deb = result.FirstOrDefault();
            if (deb == null) deb = CreateDebuff(Id, name, tech, value,countdown);
            return deb;
        }

        private static Debuff CreateDebuff(ulong id, string name, string tech, int value, int countdown)
        {
            var newDebuffs = new Debuff()
            {
                ID = id,
                Name = name,
                Tech = tech,
                Value = value,
                Countdown = countdown
            };
            debuffs.Add(newDebuffs);
            MainStorage.ChangeData("LatestDebuffId", id);
            SaveDebuffs();
            return newDebuffs;
        }
    }
}
