using System;
using System.Collections.Generic;
using System.Text;

namespace PawnShop.Forms.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsDigit(this string text)
        {
            foreach (var ch in text)
            {
                if (char.IsDigit(ch))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
