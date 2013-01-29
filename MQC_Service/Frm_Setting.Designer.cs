namespace MQC_Service
{
    partial class Frm_Setting
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
            this.rb_Client = new System.Windows.Forms.RadioButton();
            this.rb_Server = new System.Windows.Forms.RadioButton();
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.nud_timer = new System.Windows.Forms.NumericUpDown();
            this.lb_timer = new System.Windows.Forms.Label();
            this.gb_client = new System.Windows.Forms.GroupBox();
            this.tb_wsurl = new System.Windows.Forms.TextBox();
            this.lb_wsurl = new System.Windows.Forms.Label();
            this.tb_destpath = new System.Windows.Forms.TextBox();
            this.lb_dest = new System.Windows.Forms.Label();
            this.tb_storeid = new System.Windows.Forms.TextBox();
            this.lb_storeid = new System.Windows.Forms.Label();
            this.btn_wstest = new System.Windows.Forms.Button();
            this.lb_msg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nud_timer)).BeginInit();
            this.gb_client.SuspendLayout();
            this.SuspendLayout();
            // 
            // rb_Client
            // 
            this.rb_Client.AutoSize = true;
            this.rb_Client.Location = new System.Drawing.Point(74, 12);
            this.rb_Client.Name = "rb_Client";
            this.rb_Client.Size = new System.Drawing.Size(51, 17);
            this.rb_Client.TabIndex = 1;
            this.rb_Client.Tag = "IsServer";
            this.rb_Client.Text = "Client";
            this.rb_Client.UseVisualStyleBackColor = true;
            this.rb_Client.CheckedChanged += new System.EventHandler(this.rb_Server_CheckedChanged);
            // 
            // rb_Server
            // 
            this.rb_Server.AutoSize = true;
            this.rb_Server.Checked = true;
            this.rb_Server.Location = new System.Drawing.Point(12, 12);
            this.rb_Server.Name = "rb_Server";
            this.rb_Server.Size = new System.Drawing.Size(56, 17);
            this.rb_Server.TabIndex = 0;
            this.rb_Server.TabStop = true;
            this.rb_Server.Tag = "IsServer";
            this.rb_Server.Text = "Server";
            this.rb_Server.UseVisualStyleBackColor = true;
            this.rb_Server.CheckedChanged += new System.EventHandler(this.rb_Server_CheckedChanged);
            // 
            // btn_ok
            // 
            this.btn_ok.Location = new System.Drawing.Point(249, 202);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 2;
            this.btn_ok.Text = "OK";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(330, 202);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_cancel.TabIndex = 3;
            this.btn_cancel.Text = "Cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // nud_timer
            // 
            this.nud_timer.Location = new System.Drawing.Point(48, 39);
            this.nud_timer.Name = "nud_timer";
            this.nud_timer.Size = new System.Drawing.Size(50, 20);
            this.nud_timer.TabIndex = 4;
            // 
            // lb_timer
            // 
            this.lb_timer.AutoSize = true;
            this.lb_timer.Location = new System.Drawing.Point(9, 41);
            this.lb_timer.Name = "lb_timer";
            this.lb_timer.Size = new System.Drawing.Size(33, 13);
            this.lb_timer.TabIndex = 5;
            this.lb_timer.Text = "Timer";
            // 
            // gb_client
            // 
            this.gb_client.Controls.Add(this.lb_msg);
            this.gb_client.Controls.Add(this.btn_wstest);
            this.gb_client.Controls.Add(this.tb_wsurl);
            this.gb_client.Controls.Add(this.lb_wsurl);
            this.gb_client.Controls.Add(this.tb_destpath);
            this.gb_client.Controls.Add(this.lb_dest);
            this.gb_client.Controls.Add(this.tb_storeid);
            this.gb_client.Controls.Add(this.lb_storeid);
            this.gb_client.Location = new System.Drawing.Point(12, 66);
            this.gb_client.Name = "gb_client";
            this.gb_client.Size = new System.Drawing.Size(617, 127);
            this.gb_client.TabIndex = 6;
            this.gb_client.TabStop = false;
            this.gb_client.Text = "Client";
            // 
            // tb_wsurl
            // 
            this.tb_wsurl.Location = new System.Drawing.Point(111, 73);
            this.tb_wsurl.Name = "tb_wsurl";
            this.tb_wsurl.Size = new System.Drawing.Size(427, 20);
            this.tb_wsurl.TabIndex = 5;
            // 
            // lb_wsurl
            // 
            this.lb_wsurl.AutoSize = true;
            this.lb_wsurl.Location = new System.Drawing.Point(11, 80);
            this.lb_wsurl.Name = "lb_wsurl";
            this.lb_wsurl.Size = new System.Drawing.Size(94, 13);
            this.lb_wsurl.TabIndex = 4;
            this.lb_wsurl.Text = "Web Service URL";
            // 
            // tb_destpath
            // 
            this.tb_destpath.Location = new System.Drawing.Point(111, 46);
            this.tb_destpath.Name = "tb_destpath";
            this.tb_destpath.Size = new System.Drawing.Size(250, 20);
            this.tb_destpath.TabIndex = 3;
            // 
            // lb_dest
            // 
            this.lb_dest.AutoSize = true;
            this.lb_dest.Location = new System.Drawing.Point(47, 53);
            this.lb_dest.Name = "lb_dest";
            this.lb_dest.Size = new System.Drawing.Size(58, 13);
            this.lb_dest.TabIndex = 2;
            this.lb_dest.Text = "Inbox Path";
            // 
            // tb_storeid
            // 
            this.tb_storeid.Location = new System.Drawing.Point(111, 19);
            this.tb_storeid.Name = "tb_storeid";
            this.tb_storeid.Size = new System.Drawing.Size(40, 20);
            this.tb_storeid.TabIndex = 1;
            // 
            // lb_storeid
            // 
            this.lb_storeid.AutoSize = true;
            this.lb_storeid.Location = new System.Drawing.Point(59, 26);
            this.lb_storeid.Name = "lb_storeid";
            this.lb_storeid.Size = new System.Drawing.Size(46, 13);
            this.lb_storeid.TabIndex = 0;
            this.lb_storeid.Text = "Store ID";
            // 
            // btn_wstest
            // 
            this.btn_wstest.Location = new System.Drawing.Point(544, 71);
            this.btn_wstest.Name = "btn_wstest";
            this.btn_wstest.Size = new System.Drawing.Size(49, 23);
            this.btn_wstest.TabIndex = 6;
            this.btn_wstest.Text = "Test";
            this.btn_wstest.UseVisualStyleBackColor = true;
            this.btn_wstest.Click += new System.EventHandler(this.btn_wstest_Click);
            // 
            // lb_msg
            // 
            this.lb_msg.AutoSize = true;
            this.lb_msg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lb_msg.ForeColor = System.Drawing.SystemColors.Desktop;
            this.lb_msg.Location = new System.Drawing.Point(108, 101);
            this.lb_msg.Name = "lb_msg";
            this.lb_msg.Size = new System.Drawing.Size(0, 13);
            this.lb_msg.TabIndex = 7;
            // 
            // Frm_Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 234);
            this.Controls.Add(this.gb_client);
            this.Controls.Add(this.lb_timer);
            this.Controls.Add(this.nud_timer);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.rb_Client);
            this.Controls.Add(this.rb_Server);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_Setting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setting";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Frm_Setting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nud_timer)).EndInit();
            this.gb_client.ResumeLayout(false);
            this.gb_client.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rb_Server;
        private System.Windows.Forms.RadioButton rb_Client;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.NumericUpDown nud_timer;
        private System.Windows.Forms.Label lb_timer;
        private System.Windows.Forms.GroupBox gb_client;
        private System.Windows.Forms.TextBox tb_storeid;
        private System.Windows.Forms.Label lb_storeid;
        private System.Windows.Forms.Label lb_dest;
        private System.Windows.Forms.TextBox tb_destpath;
        private System.Windows.Forms.TextBox tb_wsurl;
        private System.Windows.Forms.Label lb_wsurl;
        private System.Windows.Forms.Label lb_msg;
        private System.Windows.Forms.Button btn_wstest;
    }
}