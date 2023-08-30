using Microsoft.Playwright;
using System;
using System.Windows.Forms;

namespace rmsp.nalog.collection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void ExecuteButton_Click(object sender, EventArgs e)
        {
            var browser = await PlaywrightEngine.OpenBrowser();
            IPage page = await browser.NewPageAsync(new BrowserNewPageOptions() { });
            await page.GotoAsync("https://rmsp.nalog.ru/");
            await page.QuerySelectorAsync("#query");
            string fileLink = await page.EvaluateAsync<string>(@"
                await fetch(""https://rmsp.nalog.ru/search-proc.json"", {
                        ""headers"": {
                            ""accept"": ""application/json, text/javascript, */*; q=0.01"",
                            ""accept-language"": ""ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7"",
                            ""cache-control"": ""no-cache"",
                            ""content-type"": ""application/x-www-form-urlencoded; charset=UTF-8"",
                            ""pragma"": ""no-cache"",
                            ""sec-ch-ua"": ""\""Chromium\"";v=\""116\"", \""Not)A;Brand\"";v=\""24\"", \""Google Chrome\"";v=\""116\"""",
                            ""sec-ch-ua-mobile"": ""?1"",
                            ""sec-ch-ua-platform"": ""\""Android\"""",
                            ""sec-fetch-dest"": ""empty"",
                            ""sec-fetch-mode"": ""cors"",
                            ""sec-fetch-site"": ""same-origin"",
                            ""x-requested-with"": ""XMLHttpRequest"",
                            ""cookie"": ""_ym_uid=169236167051720986; _ym_d=1692361670; _ym_isad=2; _ym_visorc=b; JSESSIONID=7F095CA58D9B1D7480E0166F3B04E040"",
                            ""Referer"": ""https://rmsp.nalog.ru/"",
                            ""Referrer-Policy"": ""strict-origin-when-cross-origin""
                        },
                        ""body"": ""mode=quick&page=1&query=7707738620&pageSize=10&sortField=NAME_EX&sort=ASC"",
                        ""method"": ""POST""
                }).then(async resp => ""https://rmsp.nalog.ru/excerpt.pdf?token="" + (await resp.json())?.data[0].token)
            }");
            Console.WriteLine(fileLink);
        }
    }
}
