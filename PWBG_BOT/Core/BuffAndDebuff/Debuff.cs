using System;
using System.Collections.Generic;
using System.Text;

namespace PWBG_BOT.Core.BuffAndDebuff
{
    public class Debuff
    {
        public ulong ID { get; set; }
        public string Name { get; set; }
        public string Tech { get; set; }
        public int Value { get; set; }
        public int Countdown { get; set;}
    }
}
