using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using log4net;
using zkemkeeper;
//using Configuration;


namespace MayChamCongServices
{
    public partial class Service1 : ServiceBase
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        public CZKEM[] axCZKEM = new CZKEM[150];//150 may

        //khai báo backgroundprocess
        private BackgroundWorker myWorker = new BackgroundWorker();

        // tạo 2 biến Timer private
        private System.Timers.Timer timer = null;
        private System.Timers.Timer timerAutoSetDeviceTime = null;

        //khai báo biến log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string RunFirstTime = System.Configuration.ConfigurationManager.AppSettings.Get("RunFirstTime");

        class MyCounter
        {
            public static int count = 0;
            public static Mutex MuTexLock = new Mutex();
        }

        public Service1()
        {
            InitializeComponent();
            //khai báo properties của background process
            myWorker.DoWork += new DoWorkEventHandler(myWorker_DoWork);
            myWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
            myWorker.ProgressChanged += new ProgressChangedEventHandler(myWorker_ProgressChanged);
            myWorker.WorkerReportsProgress = true;
            myWorker.WorkerSupportsCancellation = true;

            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            SqlDataAdapter MyAdapterConnect;
            DataTable MyTableDevices = new DataTable();
            MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices", con);
            MyAdapterConnect.Fill(MyTableDevices);

            for (int i = 0; i < MyTableDevices.Rows.Count; i++)
            {
                axCZKEM[i] = new CZKEM();
            }
            //chạy background_worker
            if (RunFirstTime == "true")
                myWorker.RunWorkerAsync();
            //chạy timer tự động đặt thời gian máy chấm công
            //timerAutoSetDeviceTime.Enabled = true;
        }
        private bool[] bIsConnected = new bool[150];//the boolean value identifies whether the device is connected
        //the serial number of the device.After connecting the device ,this value will be changed.
        private int[] iMachineNumber = new int[150];

        public string StrConnect { get => strConnect; set => strConnect = value; }

        private void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            log.Info("BackGround_Worker has completed!");
        }

        private void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            #region addlog
            SqlConnection con = new SqlConnection(strConnect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            SqlDataAdapter MyAdapterConnect;
            DataTable MyTableDevices = new DataTable();
            MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices", con);
            MyAdapterConnect.Fill(MyTableDevices);

            //add file log từ máy POS
            string DirectoryDF = ConfigurationManager.AppSettings.Get("DirectoryDF");
            string DirectoryDT = ConfigurationManager.AppSettings.Get("DirectoryDT");
            string DirectoryBackUp = ConfigurationManager.AppSettings.Get("DirectoryBackUp");
            //string[] filePaths = Directory.GetFiles(@"C:\\PosSeito\\", "*.txt");
            string[] filePaths = Directory.GetFiles(DirectoryDF, "*.txt");
            foreach (string pathtg in filePaths)
            {
                try
                {
                    //Directory.GetFiles(@"D:\\", "*.txt");
                    //String path = @"D:\\Clock100516072019154950.txt";
                    if (!File.Exists(pathtg))
                    {
                        Console.WriteLine("File " + pathtg + " does not exists!");

                    }
                    else
                    {
                        using (Stream readingStream = new FileStream(pathtg, FileMode.Open))
                        {
                            byte[] temp = new byte[10000];
                            UTF8Encoding encoding = new UTF8Encoding(true);

                            int len = 0;

                            len = readingStream.Read(temp, 0, temp.Length);
                            StringBuilder s1 = new StringBuilder();
                            String s = "";
                            int dem = 0;
                            s = encoding.GetString(temp, 0, len); //+"\r\n"+ encoding.GetString(temp, 0, len);
                            string[] stringSeparators = new string[] { "\r\n" };
                            string[] lines = s.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                            //int rowcount = (int)len / 30;
                            //log.Info(lines);
                            string connection1 = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                            SqlConnection con1 = new SqlConnection(connection1);

                            con1.Open();
                            string ADDFILE = "INSERT INTO FILES(FILENAME) VALUES ('" + System.IO.Path.GetFileName(pathtg) + "')";
                            //SqlCommand cmd = new SqlCommand();
                            cmd.Connection = con1;
                            cmd.CommandText = ADDFILE;
                            cmd.ExecuteNonQuery();
                            SqlDataAdapter MyAdapter;
                            DataTable MyTable = new DataTable();
                            MyAdapter = new SqlDataAdapter("select fileid from files where filename = '" + System.IO.Path.GetFileName(pathtg) + "'", con1);
                            MyAdapter.Fill(MyTable);
                            //con.Close();
                            //cmd.Dispose();
                            //log.Info(MyTable.Rows[0][0].ToString());
                            for (int j = 0; j < lines.Length; j++)
                            {
                                //log.Info("into for..."+j);
                                dem++;
                                Console.WriteLine(lines[j]);
                                String maPOS = (lines[j].Substring(0, 3));
                                String year = (lines[j].Substring(4, 4));
                                String month = (lines[j].Substring(9, 2));
                                String day = (lines[j].Substring(12, 2));
                                String hour = (lines[j].Substring(15, 2));
                                String minute = (lines[j].Substring(18, 2));
                                String second = (lines[j].Substring(21, 2));
                                String cardno = (lines[j].Substring(24, 6));
                                s1.Append(maPOS).Append(" ").Append(year).Append(" ").Append(month).Append(" ").Append(day).Append(" ").Append(hour).Append(" ").Append(minute).Append(" ").Append(second).Append(" ").Append(cardno);
                                s1.AppendLine();
                                string ADDFTP = "INSERT INTO FTP(FTPPOSID,FTPYear,FTPMonth,FTPDay,FTPHour,FTPMinute,FTPSecond,FTPUserMaCC,FTPFileID) VALUES('" + maPOS + "','" + year + "','" + month + "','" + day + "','" + hour + "','" + minute + "','" + second + "','" + cardno + "','" + MyTable.Rows[0][0].ToString() + "')";
                                string UPDATEFILE = "UPDATE FILES SET FILESTATUS='checked' WHERE FILEID = '" + MyTable.Rows[0][0].ToString() + "'";
                                cmd.Connection = con1;
                                cmd.CommandText = ADDFTP;
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = UPDATEFILE;
                                cmd.ExecuteNonQuery();

                            }
                            con1.Close();
                            //con1.Dispose();
                            //cmd.Dispose();
                            log.Info("File " + System.IO.Path.GetFileName(pathtg) + ", Records number: " + dem);
                            s1.Clear();
                        }
                        //move file to forder NewFiles
                        //String dirPath = @"C:\FilesPOS\" + string.Format("{0:yyyy}", DateTime.Now.Year.ToString().Trim())
                        //    + string.Format("{0:MM}", DateTime.Now.Month.ToString().Trim())
                        //    + string.Format("{0:dd}", DateTime.Now.Day.ToString().Trim());
                        //String dirPath1 = @"C:\PosSeito\" + System.IO.Path.GetFileName(pathtg);
                        //String dirPath2 = @"C:\FilesPOS\" + string.Format("{0:yyyy}", DateTime.Now.Year.ToString().Trim())
                        //    + string.Format("{0:MM}", DateTime.Now.Month.ToString().Trim())
                        //    + string.Format("{0:dd}", DateTime.Now.Day.ToString().Trim()) + @"\" + System.IO.Path.GetFileName(pathtg);
                        String dirPath = DirectoryDT + string.Format("{0:yyyy}", DateTime.Now.Year.ToString().Trim())
                            + string.Format("{0:MM}", DateTime.Now.Month.ToString().Trim())
                            + string.Format("{0:dd}", DateTime.Now.Day.ToString().Trim());
                        String dirPath1 = DirectoryDF + System.IO.Path.GetFileName(pathtg);
                        String dirPath2 = DirectoryDT + string.Format("{0:yyyy}", DateTime.Now.Year.ToString().Trim())
                            + string.Format("{0:MM}", DateTime.Now.Month.ToString().Trim())
                            + string.Format("{0:dd}", DateTime.Now.Day.ToString().Trim()) + @"\" + System.IO.Path.GetFileName(pathtg);
                        String dirPath3 = DirectoryBackUp;
                        String dirPath4 = DirectoryBackUp + @"\" + System.IO.Path.GetFileName(pathtg);
                        // Kiểm tra xem đường dẫn thư mục tồn tại không.
                        bool exist = Directory.Exists(dirPath);
                        if (!exist)
                        {
                            log.Info(dirPath + " does not exist.");
                            log.Info("Create directory: " + dirPath);

                            // Tạo thư mục.
                            Directory.CreateDirectory(dirPath);
                        }
                        bool exist1 = Directory.Exists(dirPath3);
                        if (!exist1)
                        {
                            log.Info(dirPath3 + " does not exist.");
                            log.Info("Create directory: " + dirPath3);

                            // Tạo thư mục.
                            Directory.CreateDirectory(dirPath3);
                        }

                        if (File.Exists(dirPath2))
                        {
                            File.Delete(dirPath2);
                        }
                        File.Copy(dirPath1, dirPath4);
                        File.Move(dirPath1, dirPath2);
                        if (File.Exists(dirPath2))
                        {
                            log.Info("Move file successed!");
                        }
                    }
                }
                catch (Exception a)
                {
                    log.Info("Error when read file " + System.IO.Path.GetFileName(pathtg) + "ErrorCode: " + a.Message);
                }
            }
            #endregion

            #region call threads

            SqlDataAdapter MyAdapterMalls;
            DataTable MyTableMalls = new DataTable();
            MyAdapterMalls = new SqlDataAdapter("SELECT * FROM Malls", con);
            MyAdapterMalls.Fill(MyTableMalls);

            bool bSetMaxThread = ThreadPool.SetMaxThreads(150, 500);
            //khai báo array multi thread
            Thread[] Devices = new Thread[10];
            try//connect devices
            {
                if (!bSetMaxThread)
                {
                    log.Error("Setting max threads of the threadpool failed!");
                }

                for (int i = 0; i <= MyTableMalls.Rows.Count - 1; i++)
                {
                    string MallID = MyTableMalls.Rows[i][0].ToString();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectMCC), MallID);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error when connect Store! Exception: " + ex.Message);
            }
            #endregion
        }

        public void ConnectMCC(object obj)
        {
            try
            {
                MyCounter.MuTexLock.WaitOne();
                MyCounter.count++;
                MyCounter.MuTexLock.ReleaseMutex();
                //int k = (int)obj;
                string MallID = (string) obj;

                SqlConnection con = new SqlConnection(StrConnect);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                SqlDataAdapter MyAdapterDevices;
                DataTable MyTableDevices = new DataTable();
                MyAdapterDevices =
                    new SqlDataAdapter("SELECT * FROM Devices WHERE DevicesMalls = '" + MallID + "'", con);
                MyAdapterDevices.Fill(MyTableDevices);
                for (int i = 0; i <= MyTableDevices.Rows.Count - 1; i++)
                {
                    int k = Convert.ToInt32(MyTableDevices.Rows[i][0].ToString());
                    var task = Task.Run(() =>
                    {
                        #region connectMCC and set time MCC

                        try
                        {
                            if (con.State == ConnectionState.Closed)
                                con.Open();
                            //lấy ip, port từ database
                            string temp = MyTableDevices.Rows[i][2].ToString();
                            string port = MyTableDevices.Rows[i][3].ToString();
                            //check null
                            if (temp.Trim() == "" || port.Trim() == "")
                            {
                                log.Error(string.Format("DevicesID-{0}: IP and Port cannot be null", k));
                                return;
                            }

                            int idwErrorCode = 0;
                            bIsConnected[k - 1] = axCZKEM[k - 1].Connect_Net(temp, Convert.ToInt32(port));
                            if (bIsConnected[k - 1] == true)
                            {
                                //In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                                iMachineNumber[k - 1] = k;
                                //Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                                axCZKEM[k - 1].RegEvent(iMachineNumber[k - 1], 65535);
                                log.Info(string.Format("DevicesID-{0}: Connect successed to device", k));

                                //cập nhật status devices
                                DateTime now = DateTime.Now;
                                string UPDATESTATUS = "UPDATE Devices SET DevicesStatus='Connected',DevicesTime='" +
                                                      now + "' WHERE DevicesID='" + (k) + "'";

                                cmd.CommandText = UPDATESTATUS;
                                int kq = cmd.ExecuteNonQuery();
                                log.Info(string.Format("DevicesID-{0}: Update Device Status|{1}", k, kq));


                                //set time
                                bool setdevicetime = axCZKEM[k - 1].SetDeviceTime(iMachineNumber[k - 1]);
                                if (setdevicetime)
                                {
                                    log.Info(string.Format("DevicesID-{0}: Set time successed on device!", k));
                                }
                                else
                                {
                                    axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                    log.Info(string.Format("DevicesID-{0}: Set time failed on device! ErrorCode: {1}",
                                        k, idwErrorCode.ToString()));
                                }

                                axCZKEM[k - 1]
                                    .RefreshData(iMachineNumber[k - 1]); //the data in the device should be refreshed

                                int idwYear = 0;
                                int idwMonth = 0;
                                int idwDay = 0;
                                int idwHour = 0;
                                int idwMinute = 0;
                                int idwSecond = 0;
                                if (axCZKEM[k - 1].GetDeviceTime(iMachineNumber[k - 1], ref idwYear, ref idwMonth,
                                    ref idwDay, ref idwHour, ref idwMinute, ref idwSecond)) //show the time
                                {
                                    string device_time = idwYear.ToString() + "-" + idwMonth.ToString() + "-" +
                                                         idwDay.ToString() + " " + idwHour.ToString() + ":" +
                                                         idwMinute.ToString() + ":" + idwSecond.ToString();
                                    log.Info(string.Format("DevicesID-{0}: Device time: {1}!", k, device_time));
                                }
                                else
                                {
                                    axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                    log.Info(string.Format("DevicesID-{0}: Error when get time device! ErrorCode: {1}",
                                        k, idwErrorCode.ToString()));
                                }
                            }
                            else
                            {
                                axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                log.Error(string.Format("DevicesID-{0}: Unable to connect the device ErrorCode={1}", k,
                                    idwErrorCode.ToString()));
                                //cập nhật status devices
                                DateTime now = DateTime.Now;
                                string UPDATESTATUS = "UPDATE Devices SET DevicesStatus='NotConnected',DevicesTime='" +
                                                      now + "' WHERE DevicesID='" + (k) +
                                                      "' AND DevicesStatus='Connected'; ";
                                //cmd.Connection = con;
                                cmd.CommandText = UPDATESTATUS;
                                int kq = cmd.ExecuteNonQuery();
                                log.Info(string.Format("DevicesID-{0}: Update notconnected status|{1}", k, kq));
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("DevicesID-{0}: Error when connect the devices|{1}", k,
                                ex.Message));
                        }
                        finally
                        {
                            con.Close();
                        }

                        #endregion

                        #region load data MCC vào DB

                        try
                        {
                            //get data
                            string sdwEnrollNumber = "";
                            int idwTMachineNumber = 0;
                            int idwEMachineNumber = 0;
                            int idwVerifyMode = 0;
                            int idwInOutMode = 0;
                            int idwYear = 0;
                            int idwMonth = 0;
                            int idwDay = 0;
                            int idwHour = 0;
                            int idwMinute = 0;
                            int idwSecond = 0;
                            int idwWorkcode = 0;
                            int idwErrorCode = 0;
                            int iGLCount = 0;
                            int iIndex = 0;

                            //Disable the device
                            bool kt = axCZKEM[k - 1].EnableDevice(iMachineNumber[k - 1], false);
                            log.Info(string.Format("DevicesID-{0}: Set Device Enable: False|{1}", k, kt));
                            //Read all the attendance records to the memory
                            if (axCZKEM[k - 1].ReadGeneralLogData(iMachineNumber[k - 1]))
                            {
                                int dem = 0;
                                bool flag = true;
                                while (axCZKEM[k - 1].SSR_GetGeneralLogData(iMachineNumber[k - 1], out sdwEnrollNumber,
                                    out idwVerifyMode,
                                    out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute,
                                    out idwSecond, ref idwWorkcode)) //get records from the memory
                                {
                                    iGLCount++;
                                    //con = new SqlConnection(StrConnect);
                                    string sName = "";
                                    string tam = "";
                                    string name = "";
                                    //lấy user info theo mã nv từ mcc
                                    if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1], sdwEnrollNumber, out name,
                                        out string pass, out int privilege, out bool enabled))
                                    {
                                        sName = name;
                                        string cardno = "";
                                        if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                        {
                                            tam = cardno;
                                        }

                                        if (sName == "" || tam == "") //khúc này sửa
                                        {
                                            flag = false;
                                            log.Info(string.Format(
                                                "DevicesID-{0}: Record empty: {1}! cardno {2}, MaCC {3}", k,
                                                sdwEnrollNumber, tam, sName));
                                        }
                                    }
                                    else //ghi log check thêm TH ảo lòi - khúc này e thêm
                                    {
                                        flag = false;
                                        //string cardno = "";
                                        //if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                        //{
                                        //    tam = cardno;
                                        //    log.Info(string.Format("DevicesID-{0}: Temp DataRecord|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}"
                                        //    , k, sName, idwVerifyMode, idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, tam));
                                        //}
                                        //else
                                        //{
                                        //    log.Info(string.Format("DevicesID-{0}: Temp DataRecord|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}"
                                        //    , k, sName, idwVerifyMode, idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, tam));
                                        //}
                                    }

                                    log.Info(string.Format(
                                        "DevicesID-{0}: DataRecord|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|MaCC:{10}|CardNo:{11}",
                                        k, sdwEnrollNumber, idwVerifyMode,
                                        idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, sName,
                                        tam));
                                    try
                                    {
                                        string ADD =
                                            @"INSERT INTO Records(R_MCC_ID,R_UserMaCC,R_Year,R_Month,R_Day,R_Hour,R_Minute,R_Second,R_UserCardNo) VALUES('"
                                            + iMachineNumber[k - 1] + "','" + sName + "','" + idwYear.ToString() +
                                            "','" + idwMonth.ToString() + "','"
                                            + idwDay.ToString() + "','" + idwHour.ToString() + "','" +
                                            idwMinute.ToString() + "','" + idwSecond.ToString() + "','" + tam + "')";
                                        //SqlCommand cmd = new SqlCommand();
                                        //cmd.Connection = con;
                                        cmd.CommandText = ADD;
                                        if (con.State == ConnectionState.Closed)
                                            con.Open();
                                        int kq = cmd.ExecuteNonQuery();
                                        if (kq == 1) dem++;
                                        cmd.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error(string.Format("DevicesID-{0}: Records to DB Error: {1} ", k,
                                            ex.Message));
                                    }
                                    finally
                                    {
                                    }
                                }

                                log.Info(string.Format(
                                    "DevicesID-{0}: Records to DB successed device, MCC_Records: {1}, Records added: {2}",
                                    k, iGLCount, dem));
                                //clear data in MCC
                                if (flag)
                                {
                                    if (axCZKEM[k - 1].ClearGLog(iMachineNumber[k - 1]))
                                    {
                                        axCZKEM[k - 1]
                                            .RefreshData(
                                                iMachineNumber[k - 1]); //the data in the device should be refreshed
                                        log.Info(string.Format(
                                            "DevicesID-{0}: All att Logs have been cleared from teiminal!", k));
                                    }
                                    else
                                    {
                                        axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                        log.Error(string.Format("DevicesID-{0}: Operation failed, ErrorCode={1}", k,
                                            idwErrorCode.ToString()));
                                    }
                                }
                                else
                                {
                                    log.Info(string.Format(
                                        "DevicesID-{0}: Don't clear all att logs because empty records", k));
                                }
                            }
                            else
                            {
                                axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                if (idwErrorCode.ToString() != "-4991" && idwErrorCode != 0)
                                {
                                    log.Error(string.Format(
                                        "DevicesID-{0}: error when reading data from terminal, ErrorCode={1}", k,
                                        idwErrorCode.ToString()));
                                }
                                else
                                {
                                    log.Info(string.Format("DevicesID-{0}: No data from terminal return", k));
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            log.Error(string.Format("DevicesID-{0}: Error when load data MCC to DB, Error:{1}", k,
                                e.Message));
                        }
                        finally
                        {
                            con.Close();
                            //Enable the device
                            bool kt1 = axCZKEM[k - 1].EnableDevice(iMachineNumber[k - 1], true);
                            log.Info(string.Format("DevicesID-{0}: Set Device Enable: True|{1}", k, kt1));
                        }

                        #endregion

                        #region add records_push vào MCC

                        try
                        {
                            if (con.State == ConnectionState.Closed) con.Open();
                            //SqlCommand cmd = new SqlCommand();
                            //cmd.Connection = con;
                            SqlDataAdapter MyAdapter;
                            DataTable MyTableRecord_Push = new DataTable();
                            MyAdapter = new SqlDataAdapter(
                                "select * from Record_push where devicesid = '" + (k) +
                                "' and (Status1 is null or Status1 = '' or Status1 = 'Error' or Status2 is null or Status2 = '' or Status2 = 'Error');",
                                con);
                            MyAdapter.Fill(MyTableRecord_Push);
                            //kiem tra ket noi
                            if (bIsConnected[k - 1] == false)
                            {
                                log.Error(string.Format(
                                    "DevicesID-{0}: ErrorDBtoMCC - Please connect the device first!", k));
                            }
                            else
                            {
                                bool kt = axCZKEM[k - 1].EnableDevice(iMachineNumber[k - 1], false);
                                log.Info(string.Format("DevicesID-{0}: Set Device Enable: False|{1}", k, kt));
                                for (int j = 0; j < MyTableRecord_Push.Rows.Count; j++)
                                {
                                    string recordid = MyTableRecord_Push.Rows[j][0].ToString();
                                    string personalid = MyTableRecord_Push.Rows[j][1].ToString();
                                    int ucardno = -1;
                                    int.TryParse(MyTableRecord_Push.Rows[j][3].ToString(), out ucardno);
                                    string usercardno = ucardno.ToString();
                                    //nếu mã thẻ từ == null thì không làm gì nữa...
                                    if (usercardno == "-1")
                                    {
                                        return;
                                    }

                                    string userdayfromwork = MyTableRecord_Push.Rows[j][4].ToString().Substring(0, 4) +
                                                             "/" + MyTableRecord_Push.Rows[j][4].ToString()
                                                                 .Substring(4, 2) + "/" + MyTableRecord_Push.Rows[j][4]
                                                                 .ToString().Substring(6, 2);
                                    string userdaytowork = MyTableRecord_Push.Rows[j][5].ToString().Substring(0, 4) +
                                                           "/" + MyTableRecord_Push.Rows[j][5].ToString()
                                                               .Substring(4, 2) + "/" + MyTableRecord_Push.Rows[j][5]
                                                               .ToString().Substring(6, 2);
                                    DateTime days1 = DateTime.ParseExact(userdayfromwork, "yyyy/MM/dd", null);
                                    DateTime days2 = DateTime.ParseExact(userdaytowork, "yyyy/MM/dd", null);
                                    string ngayHeThong = DateTime.Now.ToString("yyyy/MM/dd");
                                    DateTime ngayHienTai = Convert.ToDateTime(ngayHeThong);
                                    //check type C/U/D and add to MCC
                                    switch (MyTableRecord_Push.Rows[j][6].ToString())
                                    {
                                        //create
                                        case "CREATE":
                                            if (days1 <= ngayHienTai &&
                                                (MyTableRecord_Push.Rows[j][7].ToString() == "" ||
                                                 MyTableRecord_Push.Rows[j][7].ToString() == "Error")
                                            ) //check valid_from and status1
                                            {
                                                int idwErrorCode = 0;

                                                bool bEnabled = true;

                                                string sdwEnrollNumber = usercardno;
                                                string sName = personalid;
                                                string sPassword = "";
                                                int iPrivilege = 0;
                                                string sCardnumber = usercardno;
                                                string cardno = "";
                                                string sname = "";
                                                //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                                                try
                                                {
                                                    //lấy user card theo mã nv từ mcc
                                                    if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1],
                                                        sdwEnrollNumber, out string name, out string pass,
                                                        out int privilege, out bool enabled))
                                                    {
                                                        if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                                        {
                                                            string tam = cardno;
                                                            sname = name;
                                                        }
                                                    }

                                                    if (cardno == sCardnumber)
                                                    {
                                                        axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[k - 1].SSR_SetUserInfo(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled)
                                                        ) //upload the user's information(card number included)
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Duplicate - Created Success, (SSR_)SetUserInfo, UserID:{1} - Privilege:{2} - Enabled:{3}",
                                                                k, sName, iPrivilege.ToString(), bEnabled.ToString()));
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Duplicate RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Create failed UserMaCC: {1}|{2}", k,
                                                                sName, idwErrorCode.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Error RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[k - 1].SSR_SetUserInfo(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled)
                                                        ) //upload the user's information(card number included)
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Created Success, (SSR_)SetUserInfo, UserID:{1} - Privilege:{2} - Enabled:{3}",
                                                                k, sName, iPrivilege.ToString(), bEnabled.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Create RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Create failed UserMaCC: {1}|{2}", k,
                                                                sName, idwErrorCode.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Error RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(string.Format(
                                                        "DevicesID-{0}: Error when create user recordid: {1}|{2}", k,
                                                        recordid, ex.Message));
                                                }
                                                finally
                                                {
                                                }

                                            }
                                            else if (MyTableRecord_Push.Rows[j][7].ToString() != "Create" &&
                                                     MyTableRecord_Push.Rows[j][7].ToString() != "Duplicate")
                                            {
                                                log.Info(string.Format("DevicesID-{0}: Chưa tới ngày tạo RecordID: {1}",
                                                    k, recordid));
                                                //update status 
                                                string UPDATESTATUS =
                                                    "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid +
                                                    "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                int kq = cmd.ExecuteNonQuery();
                                                log.Info(string.Format(
                                                    "DevicesID-{0}: Update status1 '' RecordID: {1}|{2}", k, recordid,
                                                    kq));
                                            }

                                            if ((MyTableRecord_Push.Rows[j][7].ToString() != "" &&
                                                 MyTableRecord_Push.Rows[j][7].ToString() != "Error") &&
                                                ngayHienTai > days2)
                                            {
                                                if (MyTableRecord_Push.Rows[j][8].ToString() == "" ||
                                                    MyTableRecord_Push.Rows[j][8].ToString() == "Error")
                                                {
                                                    int idwErrorCode = 0;
                                                    string sdwEnrollNumber = usercardno;
                                                    string sName = personalid;
                                                    string sCardnumber = usercardno;
                                                    string cardno = "";
                                                    string sname = "";

                                                    try
                                                    {
                                                        //lấy user card theo mã nv từ mcc
                                                        if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, out string name, out string pass,
                                                            out int privilege, out bool enabled))
                                                        {
                                                            if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                                            {
                                                                string tam = cardno;
                                                                sname = name;
                                                            }
                                                        }

                                                        //check cardno đã tồn tại hay chưa
                                                        if (cardno == sCardnumber)
                                                        {
                                                            axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                            if (axCZKEM[k - 1]
                                                                .SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                                    sdwEnrollNumber, 12))
                                                            {
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                    k, sdwEnrollNumber));
                                                                //update status 
                                                                string UPDATESTATUS =
                                                                    "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" +
                                                                    recordid + "'";
                                                                cmd.CommandText = UPDATESTATUS;
                                                                int kq = cmd.ExecuteNonQuery();
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Update status2 Create RecordID: {1}|{2}",
                                                                    k, recordid, kq));
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                                log.Error(string.Format(
                                                                    "DevicesID-{0}: Delete failed , UserCardNo:{1} | {2}",
                                                                    k, sdwEnrollNumber, idwErrorCode.ToString()));
                                                                //update status 
                                                                string UPDATESTATUS =
                                                                    "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" +
                                                                    recordid + "'";
                                                                cmd.CommandText = UPDATESTATUS;
                                                                int kq = cmd.ExecuteNonQuery();
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Update status2 Error RecordID: {1}|{2}",
                                                                    k, recordid, kq));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                            if (axCZKEM[k - 1]
                                                                .SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                                    sdwEnrollNumber, 12))
                                                            {
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                    k, sdwEnrollNumber));
                                                                //update status 
                                                                string UPDATESTATUS =
                                                                    "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" +
                                                                    recordid + "'";
                                                                cmd.CommandText = UPDATESTATUS;
                                                                int kq = cmd.ExecuteNonQuery();
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Update status2 Create RecordID: {1}|{2}",
                                                                    k, recordid, kq));
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Duplicate - Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                    k, sdwEnrollNumber));
                                                                //update status 
                                                                string UPDATESTATUS =
                                                                    "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" +
                                                                    recordid + "'";
                                                                cmd.CommandText = UPDATESTATUS;
                                                                int kq = cmd.ExecuteNonQuery();
                                                                log.Info(string.Format(
                                                                    "DevicesID-{0}: Update status2 Duplicate RecordID: {1}|{2}",
                                                                    k, recordid, kq));
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        log.Error(string.Format(
                                                            "DevicesID-{0}: Error when delete user RecordID: {1}|{2}",
                                                            k, recordid, ex.Message));
                                                    }
                                                    finally
                                                    {
                                                    }
                                                }
                                            }
                                            else if (MyTableRecord_Push.Rows[j][8].ToString() != "Create" &&
                                                     MyTableRecord_Push.Rows[j][8].ToString() != "Duplicate")
                                            {
                                                //log.Info(string.Format("DevicesID-{0}: Chưa tới ngày xoá RecordID: {1}", k, recordid));
                                                //update status 
                                                string UPDATESTATUS =
                                                    "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid +
                                                    "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                int kq = cmd.ExecuteNonQuery();
                                                //log.Info(string.Format("DevicesID-{0}: Update status2 '' RecordID: {1}|{2}", k, recordid, kq));
                                            }

                                            break;
                                        //update
                                        case "UPDATE":
                                            if (days1 <= ngayHienTai &&
                                                (MyTableRecord_Push.Rows[j][7].ToString() == "" ||
                                                 MyTableRecord_Push.Rows[j][7].ToString() == "Error"))
                                            {
                                                int idwErrorCode = 0;

                                                bool bEnabled = true;
                                                string sdwEnrollNumber = usercardno;
                                                string sName = personalid;
                                                string sPassword = "";
                                                int iPrivilege = 0;
                                                string sCardnumber = usercardno;
                                                string cardno = "";

                                                axCZKEM[k - 1]
                                                    .SetStrCardNumber(
                                                        sCardnumber); //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                                                try
                                                {
                                                    //lấy user card theo mã nv từ mcc
                                                    if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1],
                                                        sdwEnrollNumber, out string name, out string pass,
                                                        out int privilege, out bool enabled))
                                                    {
                                                        if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                                        {
                                                            string tam = cardno;
                                                        }
                                                    }

                                                    //check cardno đã tồn tại hay chưa
                                                    if (cardno != sCardnumber)
                                                    {
                                                        //nếu không có cardno trong mcc thì tạo mới user
                                                        axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[k - 1].SSR_SetUserInfo(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled)
                                                        ) //upload the user's information(card number included)
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Created Success, (SSR_)SetUserInfo, UserID:{1} - Privilege:{2} - Enabled:{3}",
                                                                k, sName, iPrivilege.ToString(), bEnabled.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Create RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Create failed UserMaCC: {1}|{2}", k,
                                                                sName, idwErrorCode.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Error RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[k - 1].SSR_SetUserInfo(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled)
                                                        ) //upload the user's information(card number included)
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Duplicate - Created Success, (SSR_)SetUserInfo, UserID:{1} - Privilege:{2} - Enabled:{3}",
                                                                k, sName, iPrivilege.ToString(), bEnabled.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Duplicate RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Create failed UserMaCC: {1}|{2}", k,
                                                                sName, idwErrorCode.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Error RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }

                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(string.Format(
                                                        "DevicesID-{0}: Error when create user recordid: {1}|{2}", k,
                                                        recordid, ex.Message));
                                                }
                                                finally
                                                {
                                                }
                                            }
                                            else if (MyTableRecord_Push.Rows[j][7].ToString() != "Create" &&
                                                     MyTableRecord_Push.Rows[j][7].ToString() != "Duplicate")
                                            {
                                                log.Info(string.Format("DevicesID-{0}: Chưa tới ngày tạo RecordID: {1}",
                                                    k, recordid));
                                                //update status 
                                                string UPDATESTATUS =
                                                    "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid +
                                                    "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                int kq = cmd.ExecuteNonQuery();
                                                log.Info(string.Format(
                                                    "DevicesID-{0}: Update status1 '' RecordID: {1}|{2}", k, recordid,
                                                    kq));
                                            }

                                            if ((MyTableRecord_Push.Rows[j][8].ToString() == "" ||
                                                 MyTableRecord_Push.Rows[j][8].ToString() == "Error"))
                                            {
                                                if (MyTableRecord_Push.Rows[j][10].ToString() == "X")
                                                {
                                                    string ngayHeThongtg = DateTime.Now.ToString("yyyy/MM/dd");
                                                    DateTime ngayHienTaitg = Convert.ToDateTime(ngayHeThongtg);
                                                    //ngayHienTaitg = ngayHienTaitg.AddDays(-60);
                                                    if (ngayHienTaitg > days2)
                                                    {
                                                        int idwErrorCode = 0;
                                                        string sdwEnrollNumber = usercardno;
                                                        string sName = personalid;
                                                        string sCardnumber = usercardno;
                                                        string cardno = "";
                                                        string sname = "";

                                                        try
                                                        {
                                                            //lấy user card theo mã nv từ mcc
                                                            if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1],
                                                                sdwEnrollNumber, out string name, out string pass,
                                                                out int privilege, out bool enabled))
                                                            {
                                                                if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                                                {
                                                                    string tam = cardno;
                                                                    sname = name;
                                                                }
                                                            }

                                                            //check cardno đã tồn tại hay chưa
                                                            if (cardno == sCardnumber)
                                                            {
                                                                axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                                if (axCZKEM[k - 1]
                                                                    .SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                                        sdwEnrollNumber, 12))
                                                                {
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                        k, sdwEnrollNumber));
                                                                    //update status 
                                                                    string UPDATESTATUS =
                                                                        "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" +
                                                                        recordid + "'";
                                                                    cmd.CommandText = UPDATESTATUS;
                                                                    int kq = cmd.ExecuteNonQuery();
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Update status2 Create RecordID: {1}|{2}",
                                                                        k, recordid, kq));
                                                                }
                                                                else
                                                                {
                                                                    axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                                    log.Error(string.Format(
                                                                        "DevicesID-{0}: Delete failed , UserCardNo:{1} | {2}",
                                                                        k, sdwEnrollNumber, idwErrorCode.ToString()));
                                                                    //update status 
                                                                    string UPDATESTATUS =
                                                                        "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" +
                                                                        recordid + "'";
                                                                    cmd.CommandText = UPDATESTATUS;
                                                                    int kq = cmd.ExecuteNonQuery();
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Update status2 Error RecordID: {1}|{2}",
                                                                        k, recordid, kq));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                                if (axCZKEM[k - 1]
                                                                    .SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                                        sdwEnrollNumber, 12))
                                                                {
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Duplicate - Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                        k, sdwEnrollNumber));
                                                                    //update status 
                                                                    string UPDATESTATUS =
                                                                        "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" +
                                                                        recordid + "'";
                                                                    cmd.CommandText = UPDATESTATUS;
                                                                    int kq = cmd.ExecuteNonQuery();
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Update status2 Duplicate RecordID: {1}|{2}",
                                                                        k, recordid, kq));
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Error when delete user RecordID: {1}|{2}",
                                                                k, recordid, ex.Message));
                                                        }
                                                        finally
                                                        {
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (ngayHienTai > days2)
                                                    {
                                                        int idwErrorCode = 0;
                                                        string sdwEnrollNumber = usercardno;
                                                        string sName = personalid;
                                                        string sCardnumber = usercardno;
                                                        string cardno = "";
                                                        string sname = "";

                                                        try
                                                        {
                                                            //lấy user card theo mã nv từ mcc
                                                            if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1],
                                                                sdwEnrollNumber, out string name, out string pass,
                                                                out int privilege, out bool enabled))
                                                            {
                                                                if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                                                {
                                                                    string tam = cardno;
                                                                    sname = name;
                                                                }
                                                            }

                                                            //check cardno đã tồn tại hay chưa
                                                            if (cardno == sCardnumber)
                                                            {
                                                                axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                                if (axCZKEM[k - 1]
                                                                    .SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                                        sdwEnrollNumber, 12))
                                                                {
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                        k, sdwEnrollNumber));
                                                                    //update status 
                                                                    string UPDATESTATUS =
                                                                        "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" +
                                                                        recordid + "'";
                                                                    cmd.CommandText = UPDATESTATUS;
                                                                    int kq = cmd.ExecuteNonQuery();
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Update status2 Create RecordID: {1}|{2}",
                                                                        k, recordid, kq));
                                                                }
                                                                else
                                                                {
                                                                    axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                                    log.Error(string.Format(
                                                                        "DevicesID-{0}: Delete failed , UserCardNo:{1} | {2}",
                                                                        k, sdwEnrollNumber, idwErrorCode.ToString()));
                                                                    //update status 
                                                                    string UPDATESTATUS =
                                                                        "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" +
                                                                        recordid + "'";
                                                                    cmd.CommandText = UPDATESTATUS;
                                                                    int kq = cmd.ExecuteNonQuery();
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Update status2 Error RecordID: {1}|{2}",
                                                                        k, recordid, kq));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                                if (axCZKEM[k - 1]
                                                                    .SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                                        sdwEnrollNumber, 12))
                                                                {
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Duplicate - Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                        k, sdwEnrollNumber));
                                                                    //update status 
                                                                    string UPDATESTATUS =
                                                                        "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" +
                                                                        recordid + "'";
                                                                    cmd.CommandText = UPDATESTATUS;
                                                                    int kq = cmd.ExecuteNonQuery();
                                                                    log.Info(string.Format(
                                                                        "DevicesID-{0}: Update status2 Duplicate RecordID: {1}|{2}",
                                                                        k, recordid, kq));
                                                                }
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Error when delete user RecordID: {1}|{2}",
                                                                k, recordid, ex.Message));
                                                        }
                                                        finally
                                                        {

                                                        }
                                                    }
                                                }
                                            }
                                            else if (MyTableRecord_Push.Rows[j][8].ToString() != "Create" &&
                                                     MyTableRecord_Push.Rows[j][8].ToString() != "Duplicate")
                                            {
                                                //log.Info(string.Format("DevicesID-{0}: Chưa tới ngày xoá RecordID: {1}", k, recordid));
                                                //update status 
                                                string UPDATESTATUS =
                                                    "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid +
                                                    "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                int kq = cmd.ExecuteNonQuery();
                                                //log.Info(string.Format("DevicesID-{0}: Update status2 '' RecordID: {1}|{2}", k, recordid, kq));
                                            }

                                            break;

                                        case "DELETE": //delete
                                            if (days1 <= ngayHienTai && ngayHienTai <= days2 &&
                                                (MyTableRecord_Push.Rows[j][7].ToString() == "" ||
                                                 MyTableRecord_Push.Rows[j][7].ToString() == "Error"))
                                            {
                                                int idwErrorCode = 0;
                                                bool bEnabled = false;
                                                string sdwEnrollNumber = usercardno;
                                                string sName = personalid;
                                                string sPassword = "";
                                                int iPrivilege = 0;
                                                string sCardnumber = usercardno;
                                                string cardno = "";
                                                string sname = "";

                                                try
                                                {
                                                    //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                                                    //lấy user card theo mã nv từ mcc
                                                    if (axCZKEM[k - 1].SSR_GetUserInfo(iMachineNumber[k - 1],
                                                        sdwEnrollNumber, out string name, out string pass,
                                                        out int privilege, out bool enabled))
                                                    {
                                                        if (axCZKEM[k - 1].GetStrCardNumber(out cardno))
                                                        {
                                                            string tam = cardno;
                                                            sname = name;
                                                        }
                                                    }

                                                    //check cardno đã tồn tại hay chưa
                                                    if (cardno == sCardnumber)
                                                    {
                                                        axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[k - 1].SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, 12))
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                k, sdwEnrollNumber));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Create RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[k - 1].GetLastError(ref idwErrorCode);
                                                            log.Error(string.Format(
                                                                "DevicesID-{0}: Delete failed , UserCardNo:{1} | {2}",
                                                                k, sdwEnrollNumber, idwErrorCode.ToString()));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status2 Error RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[k - 1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[k - 1].SSR_DeleteEnrollData(iMachineNumber[k - 1],
                                                            sdwEnrollNumber, 12))
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                k, sdwEnrollNumber));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Create RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                        else
                                                        {
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Duplicate - Deleted Success , (SSR_)DeleteEnrollData, UserCardNo:{1}",
                                                                k, sdwEnrollNumber));
                                                            //update status 
                                                            string UPDATESTATUS =
                                                                "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" +
                                                                recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            int kq = cmd.ExecuteNonQuery();
                                                            log.Info(string.Format(
                                                                "DevicesID-{0}: Update status1 Duplicate RecordID: {1}|{2}",
                                                                k, recordid, kq));
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    log.Error(string.Format(
                                                        "DevicesID-{0}: Error when delete user RecordID: {1}|{2}", k,
                                                        recordid, ex.Message));
                                                }
                                                finally
                                                {
                                                }
                                            }
                                            else if (MyTableRecord_Push.Rows[j][7].ToString() != "Create" &&
                                                     MyTableRecord_Push.Rows[j][7].ToString() != "Duplicate")
                                            {
                                                log.Info(string.Format("DevicesID-{0}: Chưa tới ngày xoá RecordID: {1}",
                                                    k, recordid));
                                                //update status 
                                                string UPDATESTATUS =
                                                    "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid +
                                                    "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                int kq = cmd.ExecuteNonQuery();
                                                log.Info(string.Format(
                                                    "DevicesID-{0}: Update status1 '' RecordID: {1}|{2}", k, recordid,
                                                    kq));
                                            }

                                            break;
                                    }
                                }

                            }

                        }
                        catch (Exception e)
                        {
                            log.Info("Error when push data to MCC " + (k) + ", Error: " + e.Message);
                        }
                        finally
                        {
                            con.Close();
                            axCZKEM[k - 1].RefreshData(iMachineNumber[k - 1]);
                            try
                            {
                                bool kt1 = axCZKEM[k - 1].EnableDevice(iMachineNumber[k - 1], true);
                                log.Info(string.Format("DevicesID-{0}: Set Device Enable: True|{1}", k, kt1));
                            }
                            catch (Exception ex)
                            {
                                log.Error(string.Format("DevicesID-{0}: Error when set Device Enable: True|{1}", k,
                                    ex.Message));
                            }
                        }

                        try
                        {
                            bool ktt = axCZKEM[k - 1].EnableDevice(iMachineNumber[k - 1], true);
                            log.Info(string.Format("DevicesID-{0}: Set Device Enable After All: True|{1}", k, ktt));
                            axCZKEM[k - 1].Disconnect();
                            bIsConnected[k - 1] = false;
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("DevicesID-{0}: Error when set Device Enable After All: True|{1}",
                                k, ex.Message));
                        }

                        #endregion
                    });
                    bool isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(300000));
                    if (!isCompletedSuccessfully)
                    {
                        log.ErrorFormat(
                            "ConnectMCC MCC {0}: The function has taken longer than the maximum time allowed.", k);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("ConnectMCC Exception: {0}", ex.Message);
            }
            finally
            {
                MyCounter.MuTexLock.WaitOne();
                MyCounter.count--;
                MyCounter.MuTexLock.ReleaseMutex();
            }
        }

        protected override void OnStart(string[] args)
        {
            // Tạo 1 timer từ libary System.Timers
            timer = new System.Timers.Timer();
            // Execute mỗi 1p
            timer.Interval = 60000;
            // Những gì xảy ra khi timer đó dc tick
            timer.Elapsed += timer_Tick;
            // Enable timer
            timer.Enabled = true;
            // Ghi vào log file khi services dc start lần đầu tiên
            log.Info("MayChamCongServices is running!");

            //timerAutoSetDeviceTime = new System.Timers.Timer();
            //timerAutoSetDeviceTime.Interval = 180000;
            //timerAutoSetDeviceTime.Elapsed += timerAutoSetDeviceTime_Tick;
            //timerAutoSetDeviceTime.Enabled = true;
        }

        private void timer_Tick(object sender, ElapsedEventArgs args)
        {
            // Xử lý một vài logic ở đây
            //log.Info("Timer has ticked for doing something!!!");
            //myWorker.RunWorkerAsync();
            //if (args.SignalTime.Minute == 10 && args.SignalTime.Hour == 0)
            if (args.SignalTime.Minute == 10)
            {
                log.InfoFormat("MyCounter.count: {0}", MyCounter.count);
                if (MyCounter.count == 0)
                {
                    try
                    {
                        //log.Info(args.SignalTime);
                        myWorker.RunWorkerAsync();
                    }
                    catch (Exception e)
                    {
                        log.Error(String.Format("Can not run backgroud_worker!|{0}", e.Message));
                    }
                }    
            }
        }


        protected override void OnStop()
        {
            // Ghi log lại khi Services đã được stop
            timer.Enabled = false;
            log.Info("MayChamCongServices has been stop!");
        }

        private void timerAutoSetDeviceTime_Tick(object sender, EventArgs e)
        {
            log.Info(string.Format("Set time on device!"));
            try
            {
                SqlConnection con = new SqlConnection(strConnect);
                SqlDataAdapter MyAdapterDevices;
                System.Data.DataTable MyTableDevices = new System.Data.DataTable();
                MyAdapterDevices = new SqlDataAdapter("SELECT COUNT(*) FROM Devices", con);
                MyAdapterDevices.Fill(MyTableDevices);
                for (int k = 1; k <= Convert.ToInt32(MyTableDevices.Rows[0][0].ToString()); k++)
                {
                    if (bIsConnected[k - 1] == false)
                    {
                        log.Error(string.Format("DevicesID-{0}: Please connect the device first before set time on devices!", k));
                    }
                    else
                    {
                        bool setdevicetime = axCZKEM[k - 1].SetDeviceTime(iMachineNumber[k - 1]);
                        if (setdevicetime)
                        {
                            log.Info(string.Format("DevicesID-{0}: Set time successed on device!", k));
                        }
                        else
                        {
                            log.Info(string.Format("DevicesID-{0}: Set time failed on device!", k));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error when set time device!|{0}", ex.Message));
            }
        }
    }
}
