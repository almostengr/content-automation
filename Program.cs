namespace Almostengr.smautomation
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.LogMessage("Starting application");
            AutomationControl control = new AutomationControl();
            control.RunAutomation();
        }
    }
}