using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rmsp.nalog.collection.scripts
{
    class MyMethod
    {
        public static async Task<HttpResponseMessage> GET(string url, Dictionary<string, string> headers)
        {
            using (var client = new HttpClient())
            {
                foreach (var element in headers)
                {
                    client.DefaultRequestHeaders.Add(element.Key, element.Value);
                }
                return await client.GetAsync(url);
            }
        }

        public static async Task<HttpResponseMessage> POST(string url, string content, string typeContent, Dictionary<string, string> headers)
        {
            using (var msg = new HttpRequestMessage(HttpMethod.Post, url))
            {
                foreach (var element in headers)
                {
                    msg.Headers.Add(element.Key, element.Value);
                }

                msg.Content = new StringContent(content, System.Text.Encoding.UTF8, typeContent);
                
                using (var client = new HttpClient())
                {
                    return await client.SendAsync(msg);
                }
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new System.Drawing.Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new System.Drawing.Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }

    interface IScript
    {
        string GetName();
        Task Start();
    }

    public class MSP_LOADER : IScript
    {
        private class InfoElement
        {
            public string token { get; set; }
        }

        private class MspResponseType
        {
            public InfoElement[] data { get; set; }
        }

        // Скачать документ по токену
        private async Task Load_PDF(string url, string filePath)
        {
            HttpResponseMessage response = await MyMethod.GET(url, new Dictionary<string, string>()
            {
                {"Accept", "*/*"},
                {"Upgrade-Insecure-Requests", "1"},
                {"User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36"}
            });

            response.EnsureSuccessStatusCode();
            var stream = File.Create(filePath);
            await response.Content.CopyToAsync(stream);
            stream.Close();
        }

        string IScript.GetName() => "Загрузить выписку МСП (реестр: rmsp.nalog.ru)";

        async Task IScript.Start()
        {
            string INN = "7707738620";
            
            if (MyMethod.InputBox("Загрузка документа", "Введите ИНН:", ref INN) == DialogResult.OK)
            {
                if (INN.Length > 0)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Title = "Директория сохранения";
                        saveFileDialog.InitialDirectory = "c:\\";
                        saveFileDialog.FileName = $"{INN}.pdf";
                        saveFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
                        saveFileDialog.FilterIndex = 2;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            var typeContent = "application/x-www-form-urlencoded";
                            var content = $"mode=quick&page=1&query={INN}&pageSize=10&sortField=NAME_EX&sort=ASC";
                            var response = await MyMethod.POST(
                                "https://rmsp.nalog.ru/search-proc.json", 
                                content, 
                                typeContent, 
                                new Dictionary<string, string>()
                                {
                                    {"Accept", "application/json, text/javascript, */*"},
                                    {"User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36"},
                                    {"X-Requested-With", "XMLHttpRequest"}
                                });
                            response.EnsureSuccessStatusCode();
                            var responseString = await response.Content.ReadAsStringAsync();
                            var json = JsonSerializer.Deserialize<MspResponseType>(responseString);
                            var urlA = "https://rmsp.nalog.ru/excerpt.pdf?token=" + json.data[0].token;
                            await Load_PDF(urlA, saveFileDialog.FileName);
                        }
                    }
                }
            }

            return;
        }
    }

    public class EGRUL_LOADER : IScript
    {
        private class EgrilResponseType
        {
            public string t { get; set; }
            public bool captchaRequired { get; set; }
        }

        private class EgrilResponseTypeArray
        {
            public EgrilResponseType[] rows { get; set; }
        }

        string IScript.GetName() => "Загрузить выписку ЕГРЮЛ (реестр: rmsp.nalog.ru)";

        private async Task<string> GetTokenByINN(string inn)
        {
            var url = "https://egrul.nalog.ru";
            var content = "vyp3CaptchaToken=&page=&query=" + inn + "&region=&PreventChromeAutocomplete=";
            var contentType = "application/x-www-form-urlencoded";
            var response = await MyMethod.POST(url, content, contentType, new Dictionary<string, string>()
                {
                    {"Accept", "application/json, text/javascript, */*"},
                    {"User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36"},
                    {"X-Requested-With", "XMLHttpRequest"}
                });
            if (!response.IsSuccessStatusCode) new Exception("Не удалось выполнить POST запрос на выгрузку ЕГРЮЛ");
            var responseString = await response.Content.ReadAsStringAsync();
            var deserializeJSON = JsonSerializer.Deserialize<EgrilResponseType>(responseString);
            return deserializeJSON.t;
        }

        private async Task<string> GetTokenByToken(string token)
        {
            var headers = new Dictionary<string, string>()
                {
                    {"Accept", "application/json, text/javascript, */*"},
                    {"User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36"},
                    {"X-Requested-With", "XMLHttpRequest"}
                };

            string url = "https://egrul.nalog.ru/search-result/" + token + "?r=1558805340510&_=1558805340511";
            var response = await MyMethod.GET(url, headers);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var deserializeJSON = JsonSerializer.Deserialize<EgrilResponseTypeArray>(responseString);
            var newToken = deserializeJSON.rows[0].t;
            var s1 = await MyMethod.GET("https://egrul.nalog.ru/vyp-request/" + newToken + "?r=&_=1558807064014", headers);
            var s2 = await MyMethod.GET("https://egrul.nalog.ru/vyp-status/" + newToken + "?r=1558807064155&_=1558807064155", headers);

            if (!s1.IsSuccessStatusCode || !s2.IsSuccessStatusCode)
            {
                new Exception("Не удалось запросить статус подготовки файла. Повторите попытку.");
            }
            return deserializeJSON.rows[0].t;
        }

        // Скачать документ по токену
        private async Task Load_PDF(string url, string filePath)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");
                HttpResponseMessage response = null;
                int counter = 0;
                do
                {
                    response = await client.GetAsync(url);
                    counter++;
                } while (!response.IsSuccessStatusCode || counter <= 50);

                if (response.IsSuccessStatusCode)
                {
                    var stream = File.Create(filePath);
                    await response.Content.CopyToAsync(stream);
                    stream.Close();
                }
                else new Exception("Не удалось загрузить файл. Повторите попытку.");
            }
        }

        async Task IScript.Start()
        {
            string INN = "7707738620";

            if (MyMethod.InputBox("Загрузка документа", "Введите ИНН:", ref INN) == DialogResult.OK)
            {
                if (INN.Length > 0)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Title = "Директория сохранения";
                        saveFileDialog.InitialDirectory = "c:\\";
                        saveFileDialog.FileName = $"{INN}.pdf";
                        saveFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
                        saveFileDialog.FilterIndex = 2;
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string token = await GetTokenByToken(await GetTokenByINN(INN));
                            await Load_PDF("https://egrul.nalog.ru/vyp-download/" + token, saveFileDialog.FileName);
                        }
                    }
                }
            }
            return;
        }
    }
}
