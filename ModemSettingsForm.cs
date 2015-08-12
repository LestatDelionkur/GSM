using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GSM.Models;
using System.Data.Objects;
using System.IO.Ports;
using NLog;
namespace GSM
{
    public partial class ModemSettingsForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private SMSContext context = new SMSContext();
        private IList<ServicePhone> servicePhones;
        private IList<string> computers;
        private Form parent;
        private bool IsEdit = false;
        private Modem modem = null;
        public ModemSettingsForm(Form form)
        {
            this.parent = form;
            string Domain = null;
            InitializeComponent();
            try
            {
                servicePhones = context.ServicePhone.Where(x => x.IsDeleted != true).ToList();
                computers = context.Modem.Where(x => x.Deleted != true).Select(x => x.DomainName).ToList();

                Domain = Environment.MachineName;
                if (!computers.Contains(Domain))
                    computers.Add(Domain);
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения базы данных. Не удалось загрузить данный о модемах. Проверьте подключение к сети Интернет. Детали ошибки:", e);
                MessageBox.Show("Ошибка чтения базы данных. Не удалось загрузить данный о модемах. Проверьте подключение к сети Интернет. ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            this.DomainComboBox.DataSource = computers;
            this.DomainComboBox.Text = Domain;

            this.PhoneComboBox.DataSource = servicePhones;
            this.PhoneComboBox.DisplayMember = "PhoneNumber";
            this.PortNameComboBox.DataSource = SerialPort.GetPortNames().ToList();


        }
        public ModemSettingsForm(Form form, int ModemId)
        {
            this.parent = form;
            string Domain = null;
            InitializeComponent();
            try
            {
                servicePhones = context.ServicePhone.Where(x => x.IsDeleted != true).ToList();
                computers = context.Modem.Where(x => x.Deleted != true).Select(x => x.DomainName).ToList();
                modem = context.Modem.FirstOrDefault(x => x.ModemId == ModemId);
                if (modem == null) throw new Exception();
                Domain = Environment.MachineName;
                if (!computers.Contains(Domain))
                    computers.Add(Domain);
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения базы данных. Не удалось загрузить данный о модемах. Проверьте подключение к сети Интернет. Детали ошибки:", e);
                MessageBox.Show("Ошибка чтения базы данных. Не удалось загрузить данный о модемах. Проверьте подключение к сети Интернет. ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            this.DomainComboBox.DataSource = computers;
            this.PhoneComboBox.DataSource = servicePhones;
            this.PhoneComboBox.ValueMember = "ServicePhoneId";
            this.PhoneComboBox.DisplayMember = "PhoneNumber";
            this.PortNameComboBox.DataSource = SerialPort.GetPortNames().ToList();
            this.IsEdit = true;



            this.DomainComboBox.Text = modem.DomainName;
            this.PhoneComboBox.SelectedValue = modem.ServicePhoneId;
            this.PortNameComboBox.Text = modem.PortName;
            this.ModemNameTextBox.Text = modem.ModemName;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string domain = this.DomainComboBox.Text;
            ServicePhone servicePhone = (ServicePhone)this.PhoneComboBox.SelectedItem;
            string PortName = this.PortNameComboBox.Text;


            if (IsEdit)
            {
                int count = context.Modem.Where(x => x.PortName.Equals(PortName) && x.DomainName.Equals(domain) && x.Deleted != true && x.ModemId != modem.ModemId).ToList().Count;
                if (count > 0)
                {
                    MessageBox.Show("Порт выбранного компьютера уже занят", "Порт занят", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        modem.ServicePhoneId = servicePhone.ServicePhoneId;
                        modem.PortName = PortName;
                        modem.DomainName = domain;
                        modem.ModemName = this.ModemNameTextBox.Text;

                        context.Entry(modem).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        ((Form1)parent).RefreshTable();
                        ((Form1)parent).ModifidMessage();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка изменения модема: " + ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
            else
            {
                int count = context.Modem.Where(x => x.PortName.Equals(PortName) && x.DomainName.Equals(domain) && x.Deleted != true).ToList().Count;
                if (count > 0)
                {
                    MessageBox.Show("Порт выбранного компьютера уже занят", "Порт занят", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    try
                    {
                        Modem modem = new Modem()
                        {
                            ServicePhoneId = servicePhone.ServicePhoneId,
                            PortName = PortName,
                            DomainName = domain,
                            ModemName = this.ModemNameTextBox.Text
                        };
                        context.Modem.Add(modem);
                        context.SaveChanges();
                        ((Form1)parent).RefreshTable();
                        ((Form1)parent).ModifidMessage();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка добавления нового модема: " + ex.Message + ex.InnerException != null ? " " + ex.InnerException.Message : "", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }
    }
}
