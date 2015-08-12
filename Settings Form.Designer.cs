namespace GSM
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ModemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PhoneNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DomainName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ServicePhoneId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.PasswordBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ImapServerBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ImapPortBox = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.SmtpServerBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SmtpPortBox = new System.Windows.Forms.NumericUpDown();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.ImapSSLBox = new System.Windows.Forms.CheckBox();
            this.SmtpSSLBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.emailIntervalBox = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.modemIntervalBox = new System.Windows.Forms.NumericUpDown();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImapPortBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SmtpPortBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emailIntervalBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.modemIntervalBox)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ModemId,
            this.ModemName,
            this.PortName,
            this.PhoneNumber,
            this.DomainName,
            this.ServicePhoneId});
            this.dataGridView1.Location = new System.Drawing.Point(12, 120);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(810, 302);
            this.dataGridView1.TabIndex = 40;
            // 
            // ModemId
            // 
            this.ModemId.DataPropertyName = "ModemId";
            this.ModemId.HeaderText = "Column1";
            this.ModemId.Name = "ModemId";
            this.ModemId.Visible = false;
            // 
            // ModemName
            // 
            this.ModemName.DataPropertyName = "ModemName";
            this.ModemName.HeaderText = "Модем";
            this.ModemName.Name = "ModemName";
            // 
            // PortName
            // 
            this.PortName.DataPropertyName = "PortName";
            this.PortName.HeaderText = "Порт";
            this.PortName.Name = "PortName";
            // 
            // PhoneNumber
            // 
            this.PhoneNumber.DataPropertyName = "PhoneNumber";
            this.PhoneNumber.HeaderText = "Телефон";
            this.PhoneNumber.Name = "PhoneNumber";
            // 
            // DomainName
            // 
            this.DomainName.DataPropertyName = "DomainName";
            this.DomainName.HeaderText = "Компьютер";
            this.DomainName.Name = "DomainName";
            // 
            // ServicePhoneId
            // 
            this.ServicePhoneId.DataPropertyName = "ServicePhoneId";
            this.ServicePhoneId.HeaderText = "Column1";
            this.ServicePhoneId.Name = "ServicePhoneId";
            this.ServicePhoneId.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(751, 428);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 39);
            this.button1.TabIndex = 7;
            this.button1.Text = " Сохранить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Пароль:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Почтовый ящик:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // PasswordBox
            // 
            this.PasswordBox.Location = new System.Drawing.Point(120, 34);
            this.PasswordBox.Name = "PasswordBox";
            this.PasswordBox.Size = new System.Drawing.Size(179, 20);
            this.PasswordBox.TabIndex = 4;
            this.PasswordBox.Text = "+79191226027";
            this.PasswordBox.TextChanged += new System.EventHandler(this.phNum_TextChanged_1);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(120, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(179, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "+79191226027";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(319, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Imap Сервер:";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // ImapServerBox
            // 
            this.ImapServerBox.Location = new System.Drawing.Point(398, 6);
            this.ImapServerBox.Name = "ImapServerBox";
            this.ImapServerBox.Size = new System.Drawing.Size(179, 20);
            this.ImapServerBox.TabIndex = 4;
            this.ImapServerBox.Text = "+79191226027";
            this.ImapServerBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(331, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Imap Порт:";
            this.label4.Click += new System.EventHandler(this.label3_Click);
            // 
            // ImapPortBox
            // 
            this.ImapPortBox.Location = new System.Drawing.Point(398, 36);
            this.ImapPortBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.ImapPortBox.Name = "ImapPortBox";
            this.ImapPortBox.Size = new System.Drawing.Size(179, 20);
            this.ImapPortBox.TabIndex = 41;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(606, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Smtp Сервер:";
            this.label5.Click += new System.EventHandler(this.label3_Click);
            // 
            // SmtpServerBox
            // 
            this.SmtpServerBox.Location = new System.Drawing.Point(686, 6);
            this.SmtpServerBox.Name = "SmtpServerBox";
            this.SmtpServerBox.Size = new System.Drawing.Size(179, 20);
            this.SmtpServerBox.TabIndex = 4;
            this.SmtpServerBox.Text = "+79191226027";
            this.SmtpServerBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(618, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Smtp Порт:";
            this.label6.Click += new System.EventHandler(this.label3_Click);
            // 
            // SmtpPortBox
            // 
            this.SmtpPortBox.Location = new System.Drawing.Point(686, 36);
            this.SmtpPortBox.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.SmtpPortBox.Name = "SmtpPortBox";
            this.SmtpPortBox.Size = new System.Drawing.Size(179, 20);
            this.SmtpPortBox.TabIndex = 41;
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::GSM.Properties.Resources.blog_add_24x24;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button2.Location = new System.Drawing.Point(829, 128);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(58, 58);
            this.button2.TabIndex = 42;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackgroundImage = global::GSM.Properties.Resources.blog_compose_24x24;
            this.button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.Location = new System.Drawing.Point(828, 202);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(58, 58);
            this.button3.TabIndex = 42;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::GSM.Properties.Resources.blog_delete_24x24;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button4.Location = new System.Drawing.Point(829, 276);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(58, 58);
            this.button4.TabIndex = 42;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(829, 350);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(58, 58);
            this.button5.TabIndex = 42;
            this.button5.Text = "button2";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            // 
            // ImapSSLBox
            // 
            this.ImapSSLBox.AutoSize = true;
            this.ImapSSLBox.Location = new System.Drawing.Point(398, 62);
            this.ImapSSLBox.Name = "ImapSSLBox";
            this.ImapSSLBox.Size = new System.Drawing.Size(46, 17);
            this.ImapSSLBox.TabIndex = 43;
            this.ImapSSLBox.Text = "SSL";
            this.ImapSSLBox.UseVisualStyleBackColor = true;
            // 
            // SmtpSSLBox
            // 
            this.SmtpSSLBox.AutoSize = true;
            this.SmtpSSLBox.Location = new System.Drawing.Point(686, 62);
            this.SmtpSSLBox.Name = "SmtpSSLBox";
            this.SmtpSSLBox.Size = new System.Drawing.Size(46, 17);
            this.SmtpSSLBox.TabIndex = 43;
            this.SmtpSSLBox.Text = "SSL";
            this.SmtpSSLBox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Период чтения почты:";
            this.label7.Click += new System.EventHandler(this.label3_Click);
            // 
            // emailIntervalBox
            // 
            this.emailIntervalBox.Location = new System.Drawing.Point(120, 63);
            this.emailIntervalBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.emailIntervalBox.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.emailIntervalBox.Name = "emailIntervalBox";
            this.emailIntervalBox.Size = new System.Drawing.Size(179, 20);
            this.emailIntervalBox.TabIndex = 41;
            this.emailIntervalBox.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.emailIntervalBox.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 94);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Период чтения СМС:";
            this.label8.Click += new System.EventHandler(this.label3_Click);
            // 
            // modemIntervalBox
            // 
            this.modemIntervalBox.Location = new System.Drawing.Point(120, 92);
            this.modemIntervalBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.modemIntervalBox.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.modemIntervalBox.Name = "modemIntervalBox";
            this.modemIntervalBox.Size = new System.Drawing.Size(179, 20);
            this.modemIntervalBox.TabIndex = 41;
            this.modemIntervalBox.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "СМС-Сервис";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 474);
            this.Controls.Add(this.SmtpSSLBox);
            this.Controls.Add(this.ImapSSLBox);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.SmtpPortBox);
            this.Controls.Add(this.modemIntervalBox);
            this.Controls.Add(this.emailIntervalBox);
            this.Controls.Add(this.ImapPortBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SmtpServerBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ImapServerBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PasswordBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "СМС-Сервис";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImapPortBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SmtpPortBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emailIntervalBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.modemIntervalBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox PasswordBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ImapServerBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown ImapPortBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SmtpServerBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown SmtpPortBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox ImapSSLBox;
        private System.Windows.Forms.CheckBox SmtpSSLBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown emailIntervalBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown modemIntervalBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PortName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PhoneNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn DomainName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServicePhoneId;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

