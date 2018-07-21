using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PWBG_BOT
{
    public static class Tasking
    {
        public static void Sleep(double countdown)
        {
            Thread.Sleep((int)countdown);
        }
    }
}
