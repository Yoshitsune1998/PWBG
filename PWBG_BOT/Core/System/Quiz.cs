using System;
using System.Collections.Generic;
using System.Text;
using PWBG_BOT.Core.Items;

namespace PWBG_BOT.Core.System
{
    public class Quiz
    {
        public ulong ID { get; set; }
        public uint Number { get; set; }
        public string Type { get; set; }
        public string Diffulty { get; set; }
        public string RightAnswer { get; set; }
        public string ImageURL { get; set; }
        public List<string> Hints { get; set; }
        public List<Item> Drops{ get; set; }
    }
}
