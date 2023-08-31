namespace rmsp.nalog.collection
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ListScripts = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ListScripts
            // 
            this.ListScripts.BackColor = System.Drawing.Color.White;
            this.ListScripts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListScripts.Font = new System.Drawing.Font("PT Sans Bold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ListScripts.ForeColor = System.Drawing.Color.Black;
            this.ListScripts.FormattingEnabled = true;
            this.ListScripts.ItemHeight = 21;
            this.ListScripts.Location = new System.Drawing.Point(0, 0);
            this.ListScripts.Name = "ListScripts";
            this.ListScripts.Size = new System.Drawing.Size(509, 450);
            this.ListScripts.TabIndex = 0;
            this.ListScripts.UseTabStops = false;
            this.ListScripts.DoubleClick += new System.EventHandler(this.ListScripts_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(509, 450);
            this.Controls.Add(this.ListScripts);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Скрипты";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ListScripts;
    }
}

