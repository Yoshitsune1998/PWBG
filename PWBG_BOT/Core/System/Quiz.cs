using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PWBG_BOT.Core.Items;

namespace PWBG_BOT.Core.System
{
    public class Quiz
    {
        public ulong ID { get; set; }
        public string Type { get; set; }
        public string Difficulty { get; set; }
        public string RightAnswer { get; set; }
        public int WordContainInCorrectAnswer
        {
            get
            {
                return RightAnswer.Split().Length;
            }
        }
        public string URL { get; set; }
        public List<string> Hints { get; set; }
        public List<Item> Drop{ get; set; }
    }
}
