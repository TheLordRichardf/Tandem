using System;
using System.Text.RegularExpressions;

namespace TandemAPI.Helpers
{
    public static class StringExtensions
    {
        public static bool IsGuidValid(this string guidString)
        {
            if (!string.IsNullOrEmpty(guidString))
            {
                if (Guid.TryParse(guidString.ToString(), out _))
                {
                    return true;
                }
                else
                    return false;
            }

            return false;
        }

        public static bool IsEmailValid(this string emailString)
        {
            return Regex.IsMatch(emailString,
                    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

        }

    }
}