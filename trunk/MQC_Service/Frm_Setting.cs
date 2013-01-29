using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace MQC_Service
{
    public partial class Frm_Setting : Form
    {
        public Frm_Setting()
        {
            InitializeComponent();
        }

        private void Frm_Setting_Load(object sender, EventArgs e)
        {
            Initialize_Form_Value();            
        }

        private void Initialize_Form_Value()
        {
            string clientDest = MQC_Service.Properties.Settings.Default.ClientDest;
            int IsServer = Convert.ToInt32(MQC_Service.Properties.Settings.Default.IsServer);
            int clientStoreID = Convert.ToInt32(MQC_Service.Properties.Settings.Default.ClientStoreID);
            int runTimer = Convert.ToInt32(MQC_Service.Properties.Settings.Default.RunTimer);

            nud_timer.Value = (runTimer / 1000);
            lb_msg.Text = "";


            if (IsServer == 1)  // Set as server
            {
                rb_Server.Checked = true;
                gb_client.Enabled = false;
            }
            else  // Set as Client
            {
                rb_Client.Checked = true;
                gb_client.Enabled = true;
                tb_destpath.Text = clientDest;
                tb_storeid.Text = clientStoreID.ToString();
                tb_wsurl.Text = MQC_Service.Properties.Settings.Default.MQC_Service_localhost_MQC_WebService;
            }


        }

        private bool IsValidate()
        {
            if (rb_Client.Checked)
            {
                double num;
                if (tb_storeid.Text.Trim() == null || tb_storeid.Text.Trim() == "" || !double.TryParse(tb_storeid.Text.Trim(), out num))
                {
                    MessageBox.Show("Invalid Store ID.  It must be a number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (tb_destpath.Text.Trim() == "")
                {
                    MessageBox.Show("Inbox Path is needed for Client Service", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!Directory.Exists(tb_destpath.Text.Trim()))
                {
                    MessageBox.Show("Inbox Path does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (tb_wsurl.Text.Trim() == "")
                {
                    MessageBox.Show("Web Service URL is needed for Client Service", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (nud_timer.Value <= 0)
            {
                MessageBox.Show("Invalid Timer.  It must be great than 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ServiceExists()
        {
            int clientStoreID = Convert.ToInt32(MQC_Service.Properties.Settings.Default.ClientStoreID);
            //MQCWS.MQC_WebService ws = new MQCWS.MQC_WebService();

            try
            {
                object[] args = new object[1];
                args[0] = clientStoreID;
                string strText = (string)WebServiceHelper.Function(tb_wsurl.Text.Trim(), "HaveFile", args);

                //ws.Url = tb_wsurl.Text.Trim();
                //string strTest = ws.HaveFile(clientStoreID);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rb_Server_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Server.Checked)
            {
                gb_client.Enabled = false;
            }
            else
            {
                gb_client.Enabled = true;
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (IsValidate())
            {
                Configuration cf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (rb_Server.Checked)
                {
                    MQC_Service.Properties.Settings.Default.IsServer = "1";
                    MQC_Service.Properties.Settings.Default.ClientDest = "";
                    MQC_Service.Properties.Settings.Default.ClientStoreID = "0";
                    MQC_Service.Properties.Settings.Default.MQC_Service_localhost_MQC_WebService = "";
                }
                else
                {
                    MQC_Service.Properties.Settings.Default.IsServer = "0";
                    MQC_Service.Properties.Settings.Default.ClientDest = tb_destpath.Text.Trim();
                    MQC_Service.Properties.Settings.Default.ClientStoreID = tb_storeid.Text.Trim();
                    MQC_Service.Properties.Settings.Default.MQC_Service_localhost_MQC_WebService = tb_wsurl.Text.Trim();
                }

                MQC_Service.Properties.Settings.Default.RunTimer = (nud_timer.Value * 1000).ToString();

                MQC_Service.Properties.Settings.Default.Save();
                MQC_Service.Properties.Settings.Default.Upgrade();
                this.Close();
            }
        }

        private void btn_wstest_Click(object sender, EventArgs e)
        {
            if (ServiceExists())
            { 
                lb_msg.Text = "Test OK.";
            }
            else
            {
                lb_msg.Text = "Service not available.";
            }
        }
    }
}
