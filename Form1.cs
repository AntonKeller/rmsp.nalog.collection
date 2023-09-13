using System;
using System.Windows.Forms;
using rmsp.nalog.collection.scripts;
using System.Collections.Generic;

namespace rmsp.nalog.collection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Хранит в себе список скриптов -> отображаются в ListBox главного окна
        // Создайте скрипт -> Реализуйте IScript -> Добавьте в список ниже
        private readonly List<IScript> Scripts = new List<IScript>()
        {
            new MSP_LOADER(),
            new EGRUL_LOADER()
            // ...............................
            // ...............................
        };

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Scripts.ForEach(script => ListScripts.Items.Add(script.GetName()));
        }

        private async void ListScripts_DoubleClick(object sender, EventArgs e)
        {
            if (ListScripts.SelectedIndex != -1)
            {
                var selectedName = ListScripts.SelectedItem.ToString();
                var found = Scripts.Find(element => element.GetName() == selectedName);
                if (found != null)
                {
                    await found.Start();
                }
            }
        }

        private void ListScripts_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
