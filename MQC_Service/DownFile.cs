using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace MQC_Service
{
    public class DownFile
    {

        private string server;
        private string user;
        private string pass;
        private int port;
        private int timeout;
        private bool pasvMode;

        private string messages;
        private string responseStr;
        private long bytesTotal;
        private long fileSize;
        private Socket mainSock;
        private IPEndPoint mainIpEndPoint;
        private Socket listening_sock;
        private Socket dataSock;
        private IPEndPoint data_ipEndPoint;
        private FileStream file;
        private int response;
        private string bucket;

        private string[] filesNameList;

        public DownFile()
        {
            ftpInit();
            ftpParamentSet();
        }

        private void ftpParamentSet()
        {
            
            server = "192.168.1.88";
            user = "kevingu";
            pass = "kevingu0618";
            /*
           server = "31.170.160.96";
           user = "a6552046";
           pass = "qinghua24#";
            * */
            port = 21;
            timeout = 1000;
            pasvMode = false;
        }

        private void ftpInit()
        {
            Disconnect();
            mainSock = null;
            mainIpEndPoint = null;
            listening_sock = null;
            dataSock = null;
            data_ipEndPoint = null;
            file = null;
            bucket = "";
            bytesTotal = 0;
            messages = "";

            filesNameList = null;
        }


        public void testFunc()
        {
            /*
            OpenDownload(@"/public_html/temp.pdf", @"c:/temp.pdf", true);

            while (DoDownload()>0)
            {
            }
            Disconnect();
            */
            getList(@"/");
            for (int i = 0; i < filesNameList.Length;i++ )
            {
                string srcFile = @"/" + filesNameList[i];
                string localFile = @"c:/tmpsss/" + filesNameList[i];
                OpenDownload(srcFile, localFile, true);
                while (DoDownload() > 0)
                {
                }
                DeleteRemoteFile(srcFile);
                Disconnect();
            }
        }

        private void DeleteRemoteFile(string filename)
        {
            //Connect();
            SendCommand("DELE " + filename);
            ReadResponse();
            if (response == 550)
            {
                writeLog(filename + " Not Exist!");
                return;
            }
            else if (response != 213)
            {
                writeLog(responseStr.Replace("\n", " "));
                return;
            }
        }

        public int getList(string folderPath)
        {
            Connect();
            SetBinaryMode(true);
            OpenDataSocket();
            SendCommand("NLST "+folderPath);
            ReadResponse();
            //writeLog(responseStr);
            ConnectDataSocket();

            int tempCount = 0;
            StringBuilder sb = new StringBuilder();
            ArrayList al = new ArrayList();
            int bufferSize = 1024;
            int count = 0;
            do
            {
                byte[] tempBuf = new byte[bufferSize];
                count = dataSock.Receive(tempBuf);
                tempCount += count;
                byte[] recvBuf = new byte[count];
                Buffer.BlockCopy(tempBuf, 0, recvBuf, 0, count);
                al.Add(recvBuf);


            } while (count == bufferSize);

            byte[] revBuf = new byte[tempCount];

            count = 0;
            for (int i = 0; i < al.Count; i++)
            {
                byte[] item = (byte[])al[i];
                item.CopyTo(revBuf, count);
                count += item.Length;
            }
            string context = Encoding.UTF8.GetString(revBuf);
            //writeLog(context);

            filesNameList = context.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            writeLog(filesNameList.Length + "");
            Disconnect();

            for (int i = 0; i < filesNameList.Length; i++)
            {
                writeLog("filesNameList[" + i + "]-->" + filesNameList[i]);
            }
            return filesNameList.Length;
        }

        public void OpenDownload(string remoteFileName, string local_filename, bool resume)
        {
            Connect();
            SetBinaryMode(true);

            bytesTotal = 0;

            try
            {
                fileSize = GetFileSize(remoteFileName);
            }
            catch
            {
                fileSize = 0;
            }

            if (resume && File.Exists(local_filename))
            {
                try
                {
                    file = new FileStream(local_filename, FileMode.Open);
                }
                catch (Exception ex)
                {
                    file = null;
                    writeLog(ex.Message.Replace("\n", " "));
                }

                SendCommand("REST " + file.Length);
                ReadResponse();
                if (response != 350)
                    writeLog(responseStr.Replace("\n", " "));
                file.Seek(file.Length, SeekOrigin.Begin);
                bytesTotal = file.Length;
            }
            else
            {
                try
                {
                    file = new FileStream(local_filename, FileMode.Create);
                }
                catch (Exception ex)
                {
                    file = null;
                    writeLog(ex.Message.Replace("\n", " "));
                }
            }

            OpenDataSocket();
            SendCommand("RETR " + remoteFileName);
            ReadResponse();

            switch (response)
            {
                case 125:
                case 150:
                    break;
                default:
                    file.Close();
                    file = null;
                    writeLog(responseStr.Replace("\n", " "));
                    return;
            }
            
            ConnectDataSocket();

            //return;
        }

        public long DoDownload()
        {
            Byte[] bytes = new Byte[512];
            long bytes_got;

            try
            {
                bytes_got = dataSock.Receive(bytes, bytes.Length, 0);

                if (bytes_got <= 0)
                {
                    
                    ReadResponse();
                    switch (response)
                    {
                        case 226:
                        case 250:
                            break;
                        default:
                            {
                                writeLog(responseStr.Replace("\n", " "));
                                return -1;
                            }
                    }

                    CloseDataSocket();
                    file.Close();
                    file = null;

                    SetBinaryMode(false);

                    return bytes_got;

                }

                file.Write(bytes, 0, (int)bytes_got);
                bytesTotal += bytes_got;
            }
            catch (Exception ex)
            {
                CloseDataSocket();
                file.Close();
                file = null;
                ReadResponse();
                SetBinaryMode(false);
                
                writeLog(ex.Message.Replace("\n", " "));
                return -1;
            }

            return bytes_got;
        }


        private void Fail()
        {
            Disconnect();
            writeLog(responseStr.Replace("\n", " "));
        }

        private void Disconnect()
        {
            CloseDataSocket();
            if (mainSock != null)
            {
                if (mainSock.Connected)
                {
                    SendCommand("QUIT");
                    mainSock.Close();
                }
                mainSock = null;
            }

            if (file != null)
                file.Close();

            mainIpEndPoint = null;
            file = null;
        }

        private void CloseDataSocket()
        {
            if (dataSock != null)
            {
                if (dataSock.Connected)
                {
                    dataSock.Close();
                }
                dataSock = null;
            }

            data_ipEndPoint = null;
        }

        private void ConnectDataSocket()
        {
            if (dataSock != null)
                return;

            try
            {
                dataSock = listening_sock.Accept();
                listening_sock.Close();
                listening_sock = null;

                if (dataSock == null)
                {
                    writeLog("Winsock " + Convert.ToString(System.Runtime.InteropServices.Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                writeLog("No connect" + ex.Message.Replace("\n", " "));
            }
        }

        private void OpenDataSocket()
        {
            if (pasvMode)
            {
                string[] pasv;
                string server;
                int port;

                Connect();
                SendCommand("PASV");
                ReadResponse();
                if (response != 227)
                    Fail();

                try
                {
                    int i1, i2;

                    i1 = responseStr.IndexOf('(') + 1;
                    i2 = responseStr.IndexOf(')') - i1;
                    pasv = responseStr.Substring(i1, i2).Split(',');
                }
                catch (Exception)
                {
                    Disconnect();
                    writeLog("error " + responseStr.Replace("\n", " "));
                    return;
                }

                if (pasv.Length < 6)
                {
                    Disconnect();
                    writeLog("error " + responseStr.Replace("\n", " "));
                    return;
                }

                server = String.Format("{0}.{1}.{2}.{3}", pasv[0], pasv[1], pasv[2], pasv[3]);
                port = (int.Parse(pasv[4]) << 8) + int.Parse(pasv[5]);

                try
                {
                    CloseDataSocket();

                    dataSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

#if NET1
                    data_ipEndPoint = new IPEndPoint(Dns.GetHostByName(server).AddressList[0], port);
#else
                    data_ipEndPoint = new IPEndPoint(System.Net.Dns.GetHostEntry(server).AddressList[0], port);
#endif

                    dataSock.Connect(data_ipEndPoint);

                }
                catch (Exception ex)
                {
                    writeLog("error" + ex.Message.Replace("\n", " "));
                    return;
                }
            }
            else
            {
                Connect();

                try
                {
                    CloseDataSocket();

                    listening_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    string sLocAddr = mainSock.LocalEndPoint.ToString();
                    int ix = sLocAddr.IndexOf(':');
                    if (ix < 0)
                    {
                        writeLog("error" + sLocAddr);
                        return;
                    }
                    string sIPAddr = sLocAddr.Substring(0, ix);
                    System.Net.IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(sIPAddr), 0);

                    listening_sock.Bind(localEP);
                    sLocAddr = listening_sock.LocalEndPoint.ToString();
                    ix = sLocAddr.IndexOf(':');
                    if (ix < 0)
                    {
                        writeLog("error " + sLocAddr);
                    }
                    int nPort = int.Parse(sLocAddr.Substring(ix + 1));


                    listening_sock.Listen(1);
                    string sPortCmd = string.Format("PORT {0},{1},{2}", sIPAddr.Replace('.', ','), nPort / 256, nPort % 256);
                    SendCommand(sPortCmd);
                    ReadResponse();
                    if (response != 200)
                        Fail();
                }
                catch (Exception ex)
                {
                    writeLog("error" + ex.Message.Replace("\n", " "));
                    return;
                }
            }
        }

        private bool Connect()
        {
            if (server == null)
            {
                writeLog("No Ftp Server!");
            }
            if (user == null)
            {
                writeLog("No Ftp Server!");
            }

            if (mainSock != null)
                if (mainSock.Connected)
                    return true;

            try
            {

                mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
#if NET1
                mainIpEndPoint = new IPEndPoint(Dns.GetHostByName(server).AddressList[0], port);
#else
                mainIpEndPoint = new IPEndPoint(System.Net.Dns.GetHostEntry(server).AddressList[0], port);
#endif

                mainSock.Connect(mainIpEndPoint);
            }
            catch (Exception ex)
            {
                writeLog(ex.Message.Replace("\n", " "));
                return false;
            }

            ReadResponse();

            if (response != 220)
                Fail();

            SendCommand("USER " + user);
            ReadResponse();
            switch (response)
            {
                case 331:
                    if (pass == null)
                    {
                        Disconnect();
                        writeLog("No password");
                        return false;
                    }
                    SendCommand("PASS " + pass);
                    ReadResponse();
                    if (response != 230)
                    {
                        Fail();
                        return false;
                    }
                    break;
                case 230:
                    break;
            }

            return true;
        }

        private void SendCommand(string command)
        {
            Byte[] cmd = Encoding.ASCII.GetBytes((command + "\r\n").ToCharArray());

            if (command.Length > 3 && command.Substring(0, 4) == "PASS")
                messages = "\rPASS xxx";
            else
                messages = "\r" + command;

            try
            {
                mainSock.Send(cmd, cmd.Length, 0);
            }
            catch (Exception ex)
            {
                try
                {
                    Disconnect();
                    writeLog(ex.Message.Replace("\n", " "));
                    return;
                }
                catch
                {
                    mainSock.Close();
                    file.Close();
                    mainSock = null;
                    mainIpEndPoint = null;
                    file = null;
                }
            }
        }

        private void ReadResponse()
        {

            string buf;
            messages = "";

            while (true)
            {
                buf = GetLineFromBucket();

                if (Regex.Match(buf, "^[0-9]+ ").Success)
                {
                    responseStr = buf;
                    response = int.Parse(buf.Substring(0, 3));
                    break;
                }
                else
                    messages += Regex.Replace(buf, "^[0-9]+-", "") + "\n";
            }
        }

        private string GetLineFromBucket()
        {
            int i;
            string buf = "";

            if ((i = bucket.IndexOf('\n')) < 0)
            {
                while (i < 0)
                {
                    FillBucket();
                    i = bucket.IndexOf('\n');
                }
            }

            buf = bucket.Substring(0, i);
            bucket = bucket.Substring(i + 1);
            return buf;
        }

        private void FillBucket()
        {
            Byte[] bytes = new Byte[512];
            long bytesgot;
            int msecsPassed = 0;

            while (mainSock.Available < 1)
            {
                System.Threading.Thread.Sleep(50);
                msecsPassed += 50;

                if (msecsPassed > timeout)
                {
                    Disconnect();
                    writeLog("Timeout");
                    return;
                }
            }

            while (mainSock.Available > 0)
            {
                bytesgot = mainSock.Receive(bytes, 512, 0);
                bucket += Encoding.ASCII.GetString(bytes, 0, (int)bytesgot);
                System.Threading.Thread.Sleep(50);
            }
        }

        private void SetBinaryMode(bool mode)
        {
            if (mode)
                SendCommand("TYPE I");
            else
                SendCommand("TYPE A");

            ReadResponse();
            if (response != 200)
                Fail();
        }

        private long GetFileSize(string filename)
        {
            Connect();
            SendCommand("SIZE " + filename);
            ReadResponse();
            if (response == 550)
            {
                writeLog(filename + " Not Exist!");
                return 0;
            }
            else if (response != 213)
            {
                writeLog(responseStr.Replace("\n", " "));
                return 0;
            }
            else
            {
                return Int64.Parse(responseStr.Substring(4));
            }
        }

        public static void writeLog(string mess)
        {
            FileStream fs = new FileStream(@"c:/mcWindowsService.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            m_streamWriter.WriteLine(" -->" + mess);
            m_streamWriter.Flush();
            m_streamWriter.Close();
            fs.Close();
        }
























































    }


   
}
