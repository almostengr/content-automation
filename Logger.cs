using System;

namespace Almostengr.smautomation
{
    public static class Logger
    {
        public static void LogMessage(string message)
        {
            Console.WriteLine("{0} | {1}", DateTime.Now, message);
        }

        public static void DebugMessage(string message)
        {
            #if DEBUG
            LogMessage("DEBUG: " + message);
            #endif
        }
    }
}