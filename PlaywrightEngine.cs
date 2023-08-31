using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightExtraSharp.Models;
using PlaywrightExtraSharp;
using PlaywrightExtraSharp.Plugins.ExtraStealth;

namespace rmsp.nalog.collection
{
    internal class PlaywrightEngine
    {
        public static async Task<PlaywrightExtra> OpenBrowser(bool headless = true)
        {
            var playwright = new PlaywrightExtra(BrowserTypeEnum.Chromium);
            playwright.Install();
            playwright.Use(new StealthExtraPlugin());
            await playwright.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = headless
            });
            return playwright;
        }
    }
}
