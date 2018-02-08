using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchCalendarSkill
{
    /// <summary>
    /// Replaces some characters that throw off Alexa's SSML renderer.
    /// </summary>
    public static class SsmlSanitizer
    {
        public static string Sanitize(string input)
        {
            return input.Replace("&", "and");
        }
    }
}
