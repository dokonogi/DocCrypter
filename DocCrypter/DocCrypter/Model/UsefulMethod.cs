using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocCrypter.Model
{
    public static class UsefulMethod
    {
        /// <summary>
        /// クヌース・モリス・プラット法でバイトデータの探索を行う。
        /// </summary>
        /// <param name="pattern">検索されるバイト列</param>
        /// <param name="target">検索するバイト列</param>
        /// <returns></returns>
        public static int byteSearchFromEnd(byte[] target, byte[] pattern)
        {
            var table = CreateTable(pattern);
            int i = 0, p = 0;
            int ret = -2;

            while (i < target.Length)
            {
                while (i < target.Length && p < pattern.Length)
                {
                    if (target[i] == pattern[p])
                    {
                        i++; p++;
                    }
                    else if (p == 0)
                    {
                        i++;
                    }
                    else
                    {
                        p = table[p];
                    }
                    if (p == pattern.Length)
                    {
                        ret = i - p;
                        p = 0;
                    }
                }
            }
            return ret;
        }

        private static int[] CreateTable(byte[] pattern)
        {
            var table = new int[pattern.Length];
            table[0] = 0;

            var j = 0;
            for(int i = 1;i<pattern.Length; i++)
            {
                if(pattern[i] == pattern[j])
                {
                    table[i] = j++;
                }
                else
                {
                    table[i] = j;
                    j = 0;
                }
            }
            return table;
        }
    }
}
