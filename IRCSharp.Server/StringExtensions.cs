using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IRCSharp.Server
{
    public static class StringExtensions
    {
        public static bool PatternMatch(this string pattern, string subject)
        {
            if (pattern == subject)
            {
                return true;
            }
            if (pattern == "*" || subject == "*")
            {
                return true;
            }
            if (pattern.Length == 0)
            {
                return subject.Length == 0;
            }
            else if (subject.Length == 0)
            {
                return false;
            }

            if (pattern[0] == '*')
            {
                for (int i = 0; i < subject.Length; i++)
                {
                    if (pattern.Substring(1).PatternMatch(subject.Substring(i)))
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (subject[0] == '*')
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    if (pattern.Substring(i).PatternMatch(subject.Substring(1)))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return pattern[0] == subject[0] && pattern.Substring(1).PatternMatch(subject.Substring(1));
            }
        }
    }
}
