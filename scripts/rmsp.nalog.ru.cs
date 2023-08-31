using Microsoft.Playwright;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rmsp.nalog.collection.scripts
{

    interface IScript
    {
        string GetName();
        Task Start(IPage page);
    }

    public class TLoadPDFFileFromRegisterByINN : IScript
    {
        public string GetName() => "Загрузить документ из реестра rmsp.nalog.ru";

        public async Task Start(IPage page)
        {
            //string INN = "7707738620";
            string INN = "";
            
            if (InputBox("Загрузка документа", "Введите ИНН:", ref INN) == DialogResult.OK)
            {
                if (INN.Length > 0)
                {
                    await page.GotoAsync("https://rmsp.nalog.ru/");
                    string url = await page.EvaluateAsync<string>(@"
                    async (inn) => {
                            return await fetch('https://rmsp.nalog.ru/search-proc.json', {
                                'headers': {
                                    'accept': 'application/json, text/javascript, */*; q=0.01',
                                    'accept-language': 'ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7',
                                    'cache-control': 'no-cache',
                                    'content-type': 'application/x-www-form-urlencoded; charset=UTF-8',
                                    'pragma': 'no-cache',
                                    'sec-ch-ua': `'Chromium';v='116', 'Not)A;Brand';v='24', 'Google Chrome';v='116'`,
                                    'sec-ch-ua-mobile': '?1',
                                    'sec-ch-ua-platform': `'Android'`,
                                    'sec-fetch-dest': 'empty',
                                    'sec-fetch-mode': 'cors',
                                    'sec-fetch-site': 'same-origin',
                                    'x-requested-with': 'XMLHttpRequest',
                                    'cookie': '_ym_uid=169236167051720986; _ym_d=1692361670; _ym_isad=2; _ym_visorc=b; JSESSIONID=7F095CA58D9B1D7480E0166F3B04E040',
                                    'Referer': 'https://rmsp.nalog.ru/',
                                    'Referrer-Policy': 'strict-origin-when-cross-origin'
                                },
                                'body': `mode=quick&page=1&query=${inn}&pageSize=10&sortField=NAME_EX&sort=ASC`,
                                'method': 'POST'
                            }).then(async resp => 'https://rmsp.nalog.ru/excerpt.pdf?token=' + (await resp.json())?.data[0].token)
                        }
                    ", INN);

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
                            string filePath = saveFileDialog.FileName;

                            using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                            {
                                message.Headers.Add("Accept", "*/*");
                                message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");
                                message.Headers.Add("X-Requested-With", "XMLHttpRequest");

                                using (var client = new HttpClient())
                                {
                                    var response = await client.SendAsync(message);
                                    response.EnsureSuccessStatusCode();
                                    //var stream = File.Create("result.pdf");
                                    var stream = File.Create(filePath);
                                    await response.Content.CopyToAsync(stream);
                                    stream.Close();
                                }
                            }
                        }
                    }
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

   
}
