﻿using System;
using System.Diagnostics;
using Voodoo.Logging;

namespace Voodoo.Logging
{
    public class DebugLogger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public void Log(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}