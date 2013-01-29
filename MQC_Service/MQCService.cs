﻿using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.Odbc;
using MQC_Service.MQCWS;
using System.Runtime.InteropServices;



namespace MQC_Service
{
    public partial class MQCService : Form
    {
        static Timer timer = new Timer();
        static string connString = ConfigurationManager.ConnectionStrings["ConnString"].ToString();
        static string clientDSN = MQC_Service.Properties.Settings.Default.ClientDSN;
        static string clientDBConnString = "DSN=" + clientDSN + ";UID=adm0;PWD=systemcom;";
        static int runTimer = Convert.ToInt32(MQC_Service.Properties.Settings.Default.RunTimer);
        bool firstRun = true;
        //bool IsRunning = false;
        bool exitFlag = false;
        
        public MQCService()
        {
            InitializeComponent();
           
            this.WindowState = FormWindowState.Minimized;
            Construct_ContextMenu_item();

            timer.Tick += new EventHandler(TimerProcessor);
            timer.Interval = runTimer;
            timer.Start();

            /*while (exitFlag == false)
            {
                Application.DoEvents();
            }*/
        }

        private void Construct_ContextMenu_item()
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem mItemSetting = new MenuItem();
            contextMenu.MenuItems.Add(mItemSetting);
            mItemSetting.Index = 0;
            mItemSetting.Text = "S&etting";
            mItemSetting.Click += new EventHandler(mItemSetting_Click);

            MenuItem mItemSeparator = new MenuItem();
            contextMenu.MenuItems.Add(mItemSeparator);
            mItemSeparator.Index = 1;
            mItemSeparator.Text = "-";

            MenuItem mItemExit = new MenuItem();
            contextMenu.MenuItems.Add(mItemExit);
            //contextMenu.MenuItems.AddRange(new MenuItem[] { mItemExit });
            mItemExit.Index = 2;
            mItemExit.Text = "E&xit";
            mItemExit.Click += new EventHandler(mItemExit_Click);

            notifyIcon.ContextMenu = contextMenu;
        }

        private static void mItemSetting_Click(object Sender, EventArgs e)
        {
            //ConfigurationManager.RefreshSection("appSettings");
            Frm_Setting fSetting = new Frm_Setting();
            fSetting.Show();
        }

        private static void mItemExit_Click(object Sender, EventArgs e)
        {
            timer.Stop();
            Application.Exit();
        }

        private void TimerProcessor(Object myObject, EventArgs e)
        {

            timer.Stop();
            ClearMemory();
            Int32 IsServer = Convert.ToInt32(MQC_Service.Properties.Settings.Default.IsServer);
            if (IsServer == 1)  // Server Service
            {
                string str_SQL = "SELECT distinct mqc_bt_loc_id, mqc_loc_filepath " +
                    "FROM t_mqc_batch, t_mqc_loc " +
                    "WHERE mqc_bt_cmpl_time IS NULL AND mqc_bt_loc_id = mqc_loc_id";
                DataTable dt_loc_list;
                DataAccess da;

                try
                {
                    da = new DataAccess(connString);
                    dt_loc_list = da.GetDataTable(str_SQL);

                    if (dt_loc_list.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt_loc_list.Rows)
                        {
                            CreateBatchXML(row[0].ToString(), row[1].ToString());
                        }
                    }
                }
                catch
                { }
            }
            else  // Client Service
            {
                // Check and Get file from server
                Client_GetFile();

                // Check if system batch is in the DB, if not Insert.  This function only check one throughout each run time
                if (firstRun)
                {
                    Insert_SysBatch();
                    firstRun = false;
                }

                // Check files and Update market DB
                Client_UpdateDB();

                /*if (!IsRunning) // Only process it when the process is complete from previous call
                {
                    IsRunning = true;
                    // Check and Get file from server
                    Client_GetFile();

                    // Check if system batch is in the DB, if not Insert.  This function only check one throughout each run time
                    if (firstRun)
                    {
                        Insert_SysBatch();
                        firstRun = false;
                    }

                    // Check files and Update market DB
                    Client_UpdateDB();
                    IsRunning = false;
                }*/
                timer.Enabled = true;
            }
        }

        #region 
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                MQCService.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion




        #region Server Function
        private void CreateBatchXML(string loc_id, string filepath)
        {
            string str_SQL = "SELECT mqc_bt_id, mqc_bt_upc, mqc_bt_price, mqc_bt_new_price, mqc_bt_date, mqc_bt_start_time, mqc_bt_type, mqc_bt_bu_type, mqc_bt_discnt_start_date, mqc_bt_discnt_end_date, mqc_bt_discnt_limit " +
                "FROM t_mqc_batch " +
                "WHERE mqc_bt_loc_id = " + loc_id + " AND mqc_bt_cmpl_time IS NULL";

            DataTable dt_batch;
            DataAccess da;

            string fileName = filepath + loc_id.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";

            try
            {
                da = new DataAccess(connString);
                dt_batch = da.GetDataTable(str_SQL);

                if (dt_batch.Rows.Count > 0)
                {
                    dt_batch.TableName = "Batch";
                    dt_batch.DataSet.DataSetName = "Batches";
                    dt_batch.WriteXml(fileName);
                    Complete_Batch(dt_batch);
                }
                
            }
            catch
            { }
        }

        private void Complete_Batch(DataTable dt)
        {
            DataAccess da;

            if (dt.Rows.Count > 0)
            { 
                foreach (DataRow row in dt.Rows)
                {
                    string str_SQL = "Update t_mqc_batch SET mqc_bt_cmpl_time = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' Where mqc_bt_id = " + row[0].ToString();

                    try
                    {
                        da = new DataAccess(connString);
                        da.ExecuteSql(str_SQL);
                    }
                    catch
                    { }
                }
            }
        }
        #endregion

        #region Client Function
        private string Get_File_Name(string fullFilePath)
        {
            string fileName = fullFilePath.Substring(fullFilePath.LastIndexOf("\\")+1);

            return fileName;
        }

        private void Client_GetFile()
        {
            int clientStoreID = Convert.ToInt32(MQC_Service.Properties.Settings.Default.ClientStoreID);
            string clientDest = MQC_Service.Properties.Settings.Default.ClientDest;
            string url = MQC_Service.Properties.Settings.Default.MQC_Service_localhost_MQC_WebService;

            if (clientDest == "" || !Directory.Exists(clientDest)) { return; }
            if (url.Trim() == "") { return; }

            object[] args = new object[1];
            args[0] = clientStoreID;

            try
            {
                string filePath = (string)WebServiceHelper.Function(url, "HaveFile", args);
                if (filePath == null) filePath = "";

                if (filePath != "") // if there is pending file, download it
                {
                    try
                    {
                        System.IO.FileStream fsl = null;
                        byte[] bl = null;
                        args[0] = filePath;
                        bl = (byte[])WebServiceHelper.Function(url, "DownloadFile", args);
                        fsl = new FileStream(clientDest + Get_File_Name(filePath), FileMode.Create);
                        fsl.Write(bl, 0, bl.Length);
                        fsl.Close();
                        fsl = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //IsRunning = false;
                        return;
                    }
                    finally
                    {
                        bool isRemoved = (bool) WebServiceHelper.Function(url, "RemoveFile", args);

                        if (isRemoved)
                        {
                            object[] args2 = new object[3];
                            args2[0] = clientStoreID;
                            args2[1] = 0;
                            args2[2] = Get_File_Name(filePath) + " download completed.";
                            WebServiceHelper.Function(url, "WriteLog", args2);
                        }
                        else
                        {
                            MessageBox.Show("Fail to remove file from server.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {}

            
            /*MQC_WebService ws = new MQC_WebService();

            try
            {
                ws.Url = url;
                string filePath = ws.HaveFile(clientStoreID);   // Check if there is pending file
                if (filePath == null) filePath = "";

                if (filePath != "") // if there is pending file, download it
                {
                    try
                    {
                        System.IO.FileStream fsl = null;
                        byte[] bl = null;
                        bl = ws.DownloadFile(filePath);
                        fsl = new FileStream(clientDest + Get_File_Name(filePath), FileMode.Create);
                        fsl.Write(bl, 0, bl.Length);
                        fsl.Close();
                        fsl = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        IsRunning = false;
                        return;
                    }
                    finally
                    {
                        if (ws.RemoveFile(filePath))
                        {
                            ws.WriteLog(clientStoreID, 0, Get_File_Name(filePath) + " download completed.");
                        }
                        else
                        {
                            MessageBox.Show("Fail to remove file from server.");
                        }
                    }
                }
            }
            catch
            { }*/
        }

        private void Client_UpdateDB()
        {
            string fileName = Client_FileExists();

            if (fileName != "")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                string id, upc, price, newPrice, exeDate, exeTime, type, buType, disStDate, disEdDate, disLimit;
                id = "";
                upc = "";
                newPrice = "";
                exeDate = "";
                exeTime = "";
                type = "";
                buType = "";
                disStDate = "";
                disEdDate = "";
                disLimit = "";

                // Get Elements
                XmlNodeList nodeList = xmlDoc.SelectNodes("/Batches/Batch");  // Get all Batch node

                foreach (XmlNode node in nodeList)
                {
                    id = node.SelectSingleNode("mqc_bt_id").InnerText;
                    upc = node.SelectSingleNode("mqc_bt_upc").InnerText;
                    price = node.SelectSingleNode("mqc_bt_price").InnerText;
                    newPrice = node.SelectSingleNode("mqc_bt_new_price").InnerText;
                    exeDate = node.SelectSingleNode("mqc_bt_date").InnerText;
                    exeTime = node.SelectSingleNode("mqc_bt_start_time").InnerText;
                    type = node.SelectSingleNode("mqc_bt_type").InnerText;
                    buType = node.SelectSingleNode("mqc_bt_bu_type").InnerText;
                    if (buType != "P")
                    {
                        disStDate = node.SelectSingleNode("mqc_bt_discnt_start_date").InnerText;
                        disEdDate = node.SelectSingleNode("mqc_bt_discnt_end_date").InnerText;
                        disLimit = node.SelectSingleNode("mqc_bt_discnt_limit").InnerText;
                    }

                    try
                    {
                        OdbcConnection conn = new OdbcConnection(clientDBConnString);
                        conn.Open();
                        OdbcCommand comm = conn.CreateCommand();
                        if (buType == "P") 
                        {
                            comm.CommandText = "INSERT INTO dba.t_batch_updt_detail " + 
                                "SELECT NULL, 0, '" + upc + "'," + price + "," + newPrice + ",'P','" + 
                                Convert.ToDateTime(exeDate).ToString("yyyy-MM-dd") + "','" + exeTime + "', NULL,'" +
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + type + "', '" + buType + "', NULL, NULL, NULL";
                        }
                        else
                        {
                            comm.CommandText = "INSERT INTO dba.t_batch_updt_detail " + 
                                "SELECT NULL, 0, '" + upc + "'," + price + "," + newPrice + ",'P','" + 
                                Convert.ToDateTime(exeDate).ToString("yyyy-MM-dd") + "','" + exeTime + "', NULL,'" +
                                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + type + "', '" + buType + "', '" +
                                Convert.ToDateTime(disStDate).ToString("yyyy-MM-dd") + "','" +
                                Convert.ToDateTime(disEdDate).ToString("yyyy-MM-dd") + "'," + disLimit;
                        }

                        comm.ExecuteNonQuery();
                        comm.Dispose();
                        conn.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //IsRunning = false;
                    }
                }

                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //IsRunning = false;
                }
            }
        }

        // Check if any file in the folder
        private string Client_FileExists()
        {
            string clientDest = MQC_Service.Properties.Settings.Default.ClientDest;
            
            if (clientDest == "" || !Directory.Exists(clientDest)) { return ""; }

            string[] filePaths = Directory.GetFiles(clientDest);

            if (filePaths.Length > 0)
            {
                return filePaths[0].ToString();
            }

            return "";
        }

        // Insert the System Batch if it is not in the system
        private void Insert_SysBatch()
        {
            bool hasRow = false;

            try
            {
                OdbcConnection conn = new OdbcConnection(clientDBConnString);
                conn.Open();
                OdbcCommand comm = conn.CreateCommand();
                comm.CommandText = "SELECT 1 FROM dba.t_batch_updt_lst WHERE bu_id = 0";
                OdbcDataReader rd = comm.ExecuteReader();
                hasRow = rd.HasRows;
                rd.Close();

                if (!hasRow)
                {
                    comm.CommandText = "INSERT INTO dba.t_batch_updt_lst SELECT 0, 'SYS00001', 'System Batch', 1, NULL, NULL, 'N', NULL, NULL, 'N', 'P', NULL, NULL";
                    comm.ExecuteNonQuery();
                    comm.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void MQCService_Load(object sender, EventArgs e)
        {

        }
    }
}
