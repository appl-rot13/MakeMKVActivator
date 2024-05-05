
namespace MakeMKVActivator.Extensions
{
    using System.Diagnostics;

    public static class ProcessExtensions
    {
        public static void WaitForWindowHandleAssigned(this Process process, int millisecondsTimeout = Timeout.Infinite)
        {
            SpinWait.SpinUntil(() =>
            {
                process.Refresh();
                return process.MainWindowHandle != IntPtr.Zero || process.HasExited;
            }, millisecondsTimeout);
        }
    }
}
