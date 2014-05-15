using System;

namespace Infrastructure
{
    static class ExtensionMethods
    {
        private static readonly ArgumentOutOfRangeException sr_ArgumentOutOfRangeException = new ArgumentOutOfRangeException();
        
        public static bool IsInRange(this float s_TheNum, float i_Low, float i_High)
        {
            return s_TheNum <= i_High && s_TheNum >= i_Low;
        }

        public static void ThrowIfNotInRange(this float s_TheNum, float i_Low, float i_High)
        {
            if (s_TheNum > i_High || s_TheNum < i_Low)
            {
                throw sr_ArgumentOutOfRangeException;
            }
        }
    }
}
