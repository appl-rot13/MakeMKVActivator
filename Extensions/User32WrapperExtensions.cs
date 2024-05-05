
namespace MakeMKVActivator.Extensions
{
    using MakeMKVActivator.Interop;

    internal static class User32WrapperExtensions
    {
        public static void PressTab(this IntPtr windowHandle)
        {
            User32Wrapper.PressKey(windowHandle, 0x09);
        }

        public static void PressEnter(this IntPtr windowHandle)
        {
            User32Wrapper.PressKey(windowHandle, 0x0D);
        }

        public static void SendText(this IntPtr windowHandle, string text)
        {
            foreach (var c in text)
            {
                User32Wrapper.SendChar(windowHandle, c);
            }
        }
    }
}
