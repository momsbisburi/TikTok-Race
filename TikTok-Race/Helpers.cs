using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTok_Race
{
    public static class Helpers
    {
        public static uint ComputeStringHash(string s)
        {
            uint num=default;
            if (s != null)
            {
                num = 2166136261U;
                for (int i = 0; i < s.Length; i++)
                {
                    num = ((uint)s[i] ^ num) * 16777619U;
                }
            }
            return num;
        }
    }
}
