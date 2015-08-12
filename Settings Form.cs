using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using GSM;
using GSM.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using System.Configuration;
using NLog;

namespace GSM
{

    public partial class Form1 : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static MessageCenter gsmModule = new MessageCenter();
        public SMSContext context = new SMSContext();
        public List<DisplayedModem> users;
        public BindingSource source;
        public Form1()
        {

            InitializeComponent();       
            textBox1.Text = GSM.Settings.Default.Email;
            PasswordBox.Text = GSM.Settings.Default.Password;
            ImapServerBox.Text = GSM.Settings.Default.ImapServer;
            ImapPortBox.Value = GSM.Settings.Default.ImapPort;
            SmtpServerBox.Text = GSM.Settings.Default.SmtpServer;
            SmtpPortBox.Value = GSM.Settings.Default.SmtpPort;
            ImapSSLBox.Checked = GSM.Settings.Default.ImapSSL;
            SmtpSSLBox.Checked = GSM.Settings.Default.SmtpSSL;
            emailIntervalBox.Value = GSM.Settings.Default.EmailInterval / 1000;
            modemIntervalBox.Value = GSM.Settings.Default.ModemInterval / 1000;
            string Domain = Environment.MachineName;

            try
            {
                users = context.Modem.Select(y => new DisplayedModem { ModemName = y.ModemName, PortName = y.PortName, ModemId = y.ModemId, PhoneNumber = y.ServicePhone.PhoneNumber, ServicePhoneId = y.ServicePhoneId, DomainName = y.DomainName }).ToList();
                source = new BindingSource();
                source.DataSource = users;
                dataGridView1.DataSource = source;
            }
            catch (Exception e)
            {
                Error(e);
            }

            gsmModule.Init(this);

        }
        public void Error(Exception e)
        {
            logger.Error("Ошибка чтения базы данных. Не удалось загрузить список модемов. Проверьте подключение к сети Интернет. Детали ошибки:", e);
            MessageBox.Show("Ошибка чтения базы данных. Не удалось загрузить список модемов. Проверьте подключение к сети Интернет. ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }

        private void phNum_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void phNum_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ModemSettingsForm modemSettingsForm = new ModemSettingsForm(this);
            modemSettingsForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int modemId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ModemId"].Value);

            ModemSettingsForm modemSettingsForm = new ModemSettingsForm(this,modemId);
            modemSettingsForm.Show();
        }

        public void RefreshTable()
        {
            users = context.Modem.Select(y => new DisplayedModem { ModemName = y.ModemName, PortName = y.PortName, ModemId = y.ModemId, PhoneNumber = y.ServicePhone.PhoneNumber, ServicePhoneId = y.ServicePhoneId, DomainName = y.DomainName }).ToList();
            source.ResetBindings(false);
            source.DataSource = users;
            dataGridView1.Refresh();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            int modemId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ModemId"].Value);
            var model = context.Modem.FirstOrDefault(c => c.ModemId == modemId);
            context.Modem.Remove(model);
            context.SaveChanges();
            RefreshTable();
            ModifidMessage();
        }
        public void ModifidMessage()
        {
            MessageBox.Show("Изменения вступят в силу после перезапуска программы", "Изменения сохранены", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GSM.Settings.Default["Email"] = textBox1.Text;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["Password"] = PasswordBox.Text;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["ImapServer"] = ImapServerBox.Text;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["ImapPort"] = (int)ImapPortBox.Value;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["SmtpServer"] = SmtpServerBox.Text;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["SmtpPort"] = (int)SmtpPortBox.Value;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["ImapSSL"] = ImapSSLBox.Checked;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["SmtpSSL"] = SmtpSSLBox.Checked;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["EmailInterval"] =(long) emailIntervalBox.Value * 1000;
            GSM.Settings.Default.Save();
            GSM.Settings.Default["ModemInterval"] =(long) modemIntervalBox.Value * 1000;
            GSM.Settings.Default.Save();



            ModifidMessage();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;

            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }
    }


    public class DisplayedModem
    {
        public string PortName { get; set; }
        public string ModemName { get; set; }
        public int ModemId { get; set; }
        public string PhoneNumber { get; set; }
        public string DomainName { get; set; }
        public int ServicePhoneId { get; set; }
    }
}
