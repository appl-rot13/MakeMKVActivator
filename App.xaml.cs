
namespace MakeMKVActivator
{
    using System.Diagnostics;
    using System.Net.Http;
    using System.Windows;

    using AngleSharp.Html.Dom;
    using AngleSharp.Html.Parser;

    using MakeMKVActivator.Extensions;
    using MakeMKVActivator.Interop;

    public partial class App
    {
        private static readonly string ProgramPath = "C:\\Program Files (x86)\\MakeMKV\\makemkv.exe";

        private static readonly string CommonPopupTitle = "MakeMKV ベータポップアップ";
        private static readonly string RegistrationPopupTitle = "公認コードを入力してください";

        private readonly Task<string> fetchActivationKey = FetchActivationKey();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Task.Run(this.Activate);
        }

        private void Activate()
        {
            // MakeMKV 開始
            var process = Process.Start(ProgramPath);
            process.Exited += (_, _) =>
            {
                this.Dispatcher.Invoke(this.Shutdown);
            };

            process.EnableRaisingEvents = true;
            process.WaitForInputIdle();

            // ポップアップ対応
            var commonPopup = WaitForVisibleWindow(CommonPopupTitle, 1000);
            if (commonPopup == IntPtr.Zero)
            {
                // 一定時間内にポップアップが表示されない場合、有効期限内と判断し終了する
                this.Dispatcher.Invoke(this.Shutdown);
                return;
            }

            if (process.MainWindowHandle == IntPtr.Zero)
            {
                // メインウィンドウが表示されていない場合、初回のみ表示される
                // [アクティベーションキーの期限切れ]ポップアップと判断する
                commonPopup.PressEnter();
                process.WaitForWindowHandleAssigned();

                commonPopup = WaitForVisibleWindow(CommonPopupTitle);
            }

            commonPopup.PressTab();
            commonPopup.PressEnter();

            // アクティベーションキー入力
            var registrationPopup = WaitForVisibleWindow(RegistrationPopupTitle);
            registrationPopup.SendText(this.fetchActivationKey.Result);
            registrationPopup.PressEnter();

            // ポップアップ対応 (正常動作の確認のため、自動では閉じない)
            // commonPopup = WaitForVisibleWindow(CommonPopupTitle);
            // commonPopup.PressEnter();
        }

        private static IntPtr WaitForWindow(string title, int millisecondsTimeout = Timeout.Infinite)
        {
            var window = IntPtr.Zero;
            var ret = SpinWait.SpinUntil(
                () =>
                {
                    window = User32Wrapper.FindWindow(title);
                    return window != IntPtr.Zero;
                },
                millisecondsTimeout);

            return ret ? window : IntPtr.Zero;
        }

        private static IntPtr WaitForVisibleWindow(string title, int millisecondsTimeout = Timeout.Infinite)
        {
            var window = WaitForWindow(title, millisecondsTimeout);
            if (window == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var ret = SpinWait.SpinUntil(() => User32Wrapper.IsVisible(window), millisecondsTimeout);
            return ret ? window : IntPtr.Zero;
        }

        private static async Task<IHtmlDocument> FetchDocumentAsync(string uri)
        {
            using (var client = new HttpClient())
            using (var stream = await client.GetStreamAsync(uri))
            {
                return await new HtmlParser().ParseDocumentAsync(stream);
            }
        }

        private static async Task<string> FetchActivationKey()
        {
            var document = await FetchDocumentAsync("https://forum.makemkv.com/forum/viewtopic.php?f=5&t=1053");
            var codeNode = document.QuerySelector("#post_content3548 code");

            return codeNode?.TextContent ?? string.Empty;
        }
    }
}
