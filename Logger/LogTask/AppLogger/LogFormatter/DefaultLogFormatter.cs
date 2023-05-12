using System;
using System.Collections.Generic;
using System.Text;

namespace AppLogger
{
    public class DefaultLogFormatter : ILogFormatter
    {
        public string Format(string message)
        {
            var sb = new StringBuilder(27 + message.Length); // 27 is the length of the date format "yyyy-MM-dd HH:mm:ss.fff"

            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            sb.Append(" ");
            sb.Append(message);

            return sb.ToString();
        }
    }
}
