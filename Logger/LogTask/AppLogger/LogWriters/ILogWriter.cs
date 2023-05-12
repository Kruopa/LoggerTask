using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger
{
    public interface ILogWriter
    {
        Task WriteAsync(string logMessage);
    }
}
