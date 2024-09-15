using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using zkemkeeper;
using System.IO;
using log4net;
using MayChamCongMaster;
using System.ServiceProcess;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace MayChamCong
{

    public partial class Form1 : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        AutoSize _form_resize;

        public CZKEM[] axCZKEM = new CZKEM[150];//150 may

        //khao báo background process
        private BackgroundWorker myWorker = new BackgroundWorker();

        //khai báo biến log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Form1()
        {
            InitializeComponent();
            this.Text = "Trang quản lý";

            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            System.Data.DataTable Dv = null;
            Dv = new Connect().ThongtinDevices();
            gridviewdevices.DataSource = Dv;
            gridviewdevices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridviewdevices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //khởi tạo mcc
            for (int i = 0; i < gridviewdevices.Rows.Count; i++)
            {
                axCZKEM[i] = new CZKEM();
            }

            //khai báo properties của background process
            myWorker.DoWork += new DoWorkEventHandler(myWorker_DoWork);
            myWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(myWorker_RunWorkerCompleted);
            myWorker.ProgressChanged += new ProgressChangedEventHandler(myWorker_ProgressChanged);
            myWorker.WorkerReportsProgress = true;
            myWorker.WorkerSupportsCancellation = true;

            ILog log = log4net.LogManager.GetLogger(typeof(Form1));

            //AutoSize
            _form_resize = new AutoSize(this);
            this.Load += _Load;
            this.Resize += _Resize;
        }


        private void _Load(object sender, EventArgs e)
        {
            _form_resize._get_initial_size();
        }

        private void _Resize(object sender, EventArgs e)
        {
            _form_resize._resize();
        }
        //Create Standalone SDK class dynamicly.
        //public zkemkeeper.CZKEM axCZKEM1 = new zkemkeeper.CZKEM();



        #region Communication
        private bool[] bIsConnected = new bool[150];//the boolean value identifies whether the device is connected
        //the serial number of the device.After connecting the device ,this value will be changed.
        private int[] iMachineNumber = new int[150];

        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        private void btnConnect_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            for (int j = 0; j < gridviewdevices.SelectedRows.Count; j++)
            {
                String temp = gridviewdevices.SelectedRows[j].Cells[2].Value.ToString();
                String port = gridviewdevices.SelectedRows[j].Cells[3].Value.ToString();
                if (temp.Trim() == "" || port.Trim() == "")
                {
                    MessageBox.Show("IP and Port " + (j + 1) + "cannot be null", "Error");
                    return;
                }
            }
            for (int i = 0; i <= gridviewdevices.SelectedRows.Count - 1; i++)
            {
                int idwErrorCode = 0;
                string deviceid = gridviewdevices.SelectedRows[i].Cells[0].Value.ToString();
                Cursor = Cursors.WaitCursor;
                if (btnConnect.Text == "DisConnect")
                {
                    axCZKEM[i].Disconnect();
                    bIsConnected[i] = false;
                    btnConnect.Text = "Connect";
                    lblState.Text = "Current State:DisConnected";
                    Cursor = Cursors.Default;
                    return;
                }
                String temp = gridviewdevices.SelectedRows[i].Cells[2].Value.ToString();
                String port = gridviewdevices.SelectedRows[i].Cells[3].Value.ToString();
                bIsConnected[i] = axCZKEM[i].Connect_Net(temp, Convert.ToInt32(port));
                if ((bIsConnected[i] == true))
                {
                    //btnConnect.Text = "DisConnect";
                    //btnConnect.Refresh();
                    //lblState.Text = "Current State:Connected";
                    iMachineNumber[i] = i + 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                    axCZKEM[i].RegEvent(iMachineNumber[i], 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)

                    //cập nhật status devices
                    DateTime now = DateTime.Now;
                    string UPDATESTATUS = "UPDATE Devices SET DevicesStatus='Connected',DevicesTime='" + now + "' WHERE DevicesID='" + deviceid + "'";
                    cmd.Connection = con;
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();

                }
                else
                {
                    axCZKEM[i].GetLastError(ref idwErrorCode);
                    MessageBox.Show("Unable to connect the device " + (i + 1) + ",ErrorCode=" + idwErrorCode.ToString(), "Error");
                    log.Info("Unable to connect the device " + (i + 1) + ",ErrorCode=" + idwErrorCode.ToString());

                    //cập nhật status devices
                    SqlDataAdapter MyAdapterKTStatus;
                    System.Data.DataTable MyTableKTStatus = new System.Data.DataTable();
                    MyAdapterKTStatus = new SqlDataAdapter("SELECT DevicesStatus FROM Devices WHERE DevicesID='" + deviceid + "'", con);
                    MyAdapterKTStatus.Fill(MyTableKTStatus);
                    if (MyTableKTStatus.Rows[0][0].ToString() == "Connected")
                    {
                        DateTime now = DateTime.Now;
                        string UPDATESTATUS = "UPDATE Devices SET DevicesStatus='NotConnected',DevicesTime='" + now + "' WHERE DevicesID='" + deviceid + "'";
                        cmd.Connection = con;
                        cmd.CommandText = UPDATESTATUS;
                        cmd.ExecuteNonQuery();
                    }
                }
                Cursor = Cursors.Default;
            }
            int dem = 0;
            for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            {
                if (bIsConnected[i] == false)
                {
                    dem++;
                }
            }
            if (dem == 0)
            {
                btnConnect.Text = "DisConnect";
                btnConnect.Refresh();
                lblState.Text = "Current State:Connected";
            }
            con.Close();
        }


        #endregion
        #region ButtonInForm
        private void btnExportRecordsData_Click(object sender, EventArgs e)
        {
            #region xuất records từ listview ra file excel
            //try
            //{
            //    using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel workbook|*.xls", ValidateNames = true })
            //    {
            //        if (sfd.ShowDialog() == DialogResult.OK)
            //        {
            //            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            //            Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet);
            //            Worksheet ws = (Worksheet)app.ActiveSheet;
            //            app.Visible = false;
            //            ws.Cells[1, 1] = "MachineNumber";
            //            ws.Cells[1, 2] = "Count";
            //            ws.Cells[1, 3] = "EnrollNumber";
            //            ws.Cells[1, 4] = "VerifyMode";
            //            ws.Cells[1, 5] = "InOutMode";
            //            ws.Cells[1, 6] = "Date";
            //            ws.Cells[1, 7] = "WorkCode";
            //            ws.Cells[1, 8] = "Reserved";

            //            int i = 2;
            //            foreach (ListViewItem item in lvLogs.Items)
            //            {
            //                if (item.SubItems[0].Text != "")
            //                {
            //                    ws.Cells[i, 1] = item.SubItems[0].Text;
            //                }
            //                else
            //                {
            //                    ws.Cells[i, 1] = "";
            //                }
            //                if (item.SubItems[1].Text =="")
            //                {
            //                    ws.Cells[i, 2] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 2] = item.SubItems[1].Text;
            //                }
            //                if (item.SubItems[2].Text =="")
            //                {
            //                    ws.Cells[i, 3] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 3] = item.SubItems[2].Text;
            //                }
            //                if (item.SubItems[3].Text =="")
            //                {
            //                    ws.Cells[i, 4] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 4] = item.SubItems[3].Text;
            //                }
            //                if (item.SubItems[4].Text =="")
            //                {
            //                    ws.Cells[i, 5] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 5] = item.SubItems[4].Text;
            //                }
            //                if (item.SubItems[5].Text =="")
            //                {
            //                    ws.Cells[i, 6] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 6] = item.SubItems[5].Text;
            //                }
            //                if (item.SubItems[6].Text =="")
            //                {
            //                    ws.Cells[i, 7] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 7] = item.SubItems[6].Text;
            //                }
            //                if (item.SubItems[7].Text =="")
            //                {
            //                    ws.Cells[i, 8] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 8] = item.SubItems[7].Text;
            //                }
            //                i++;
            //            }
            //            ws.SaveAs(sfd.FileName, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
            //            app.Quit();
            //            log.Info("Data Records has been successfully exported!");
            //            MessageBox.Show("Data records has been successfully exported!", "Thông báo!");
            //        }
            //    }
            //}
            //catch (Exception a)
            //{
            //    log.Info("Error when export data records, ErrorCode: " + a.ToString());
            //    MessageBox.Show("Error when export data records, ErrorCode: " + a.ToString(), "Error");
            //}
            #endregion

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text|*.csv";
            sfd.Title = "Save file...";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                WriteFileRecords(sfd.FileName);
            }
        }

        private void WriteFileRecords(string file)
        {
            try
            {
                string datefrom = DateTimePickFrom.Value.ToString("yyyyMMdd");
                string dateto = dateTimePickTo.Value.ToString("yyyyMMdd");
                string mall = cbbMall.SelectedValue.ToString();
                //string strConnect = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                SqlConnection con = new SqlConnection(strConnect);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                SqlDataAdapter MyAdapterNumbersDevices;
                DataTable MyTableNumbersRecords = new DataTable();
                string sql = "";
                if (mall == "ALL")
                {
                    sql = string.Format("SELECT * FROM Records WHERE CONVERT(datetime,CONCAT(R_Year,'/',R_Month,'/',R_Day),101) >= '{0}' and CONVERT(datetime,CONCAT(R_Year,'/',R_Month,'/',R_Day),101)<= '{1}';"
                                                , datefrom, dateto);
                }
                else
                {
                    sql = string.Format(@"SELECT T0.* 
                                                    FROM Records T0 INNER JOIN Devices T1 ON T0.R_MCC_ID = T1.DevicesID
                                                    WHERE CONVERT(datetime, CONCAT(T0.R_Year, '/', T0.R_Month, '/', T0.R_Day), 101) >= '{0}' 
                                                    and CONVERT(datetime, CONCAT(T0.R_Year, '/', T0.R_Month, '/', T0.R_Day), 101) <= '{1}' 
                                                    and T1.DevicesMalls = '{2}';"
                                                , datefrom, dateto, mall);
                }
                MyAdapterNumbersDevices = new SqlDataAdapter(sql, con);
                MyAdapterNumbersDevices.Fill(MyTableNumbersRecords);
                StreamWriter sw = new StreamWriter(file, false, Encoding.Unicode);

                sw.Write("R_ID" + "\t");
                sw.Write("R_MCC_ID" + "\t");
                sw.Write("R_UserMaCC" + "\t");
                sw.Write("R_Year" + "\t");
                sw.Write("R_Month" + "\t");
                sw.Write("R_Day" + "\t");
                sw.Write("R_Hour" + "\t");
                sw.Write("R_Minute" + "\t");
                sw.Write("R_Second" + "\t");
                sw.WriteLine("R_UserCardNo");
                for (int i = 0; i < MyTableNumbersRecords.Rows.Count; i++)
                {
                    sw.Write(MyTableNumbersRecords.Rows[i][0].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][1].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][2].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][3].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][4].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][5].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][6].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][7].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][8].ToString() + "\t");
                    sw.Write(MyTableNumbersRecords.Rows[i][9].ToString());
                    sw.WriteLine();
                }
                sw.Close();
                log.Info("Export successed data records to file csv!");
                MessageBox.Show("Export successed data records to file csv!", "Succcessed!");
            }
            catch (Exception a)
            {
                log.Info("Error when export data records to file csv, ErrorCode: " + a.ToString());
                MessageBox.Show("Error when export data records to file csv, ErrorCode: " + a.ToString(), "Error!");
            }
        }

        private void DeleteCardNumber_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            {
                if (bIsConnected[i] == false)
                {
                    MessageBox.Show("Please connect the device" + (i + 1) + " first!", "Error");
                    log.Info("Please connect the device" + (i + 1) + " first!");
                    return;
                }

                if (txtUserID.Text.ToString().StartsWith("0"))
                {
                    MessageBox.Show("Can not delete user info with UserID begin at '0'!", "Error");
                    return;
                }

                if (txtCardnumber.Text.ToString().StartsWith("0"))
                {
                    MessageBox.Show("Can not delete user info with CardNumber begin at '0'!", "Error");
                    return;
                }

                string sdwEnrollNumber = txtCardnumber.Text;
                string sName = "";
                string sPassword = "";
                int iPrivilege = 0;
                bool bEnabled = false;
                string sCardnumber = txtCardnumber.Text;
                string cardno = "";
                string sname = "";
                int idwErrorCode = 0;
                string tam = "";

                Cursor = Cursors.WaitCursor;
                axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device
                try
                {
                    //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                    //lấy user card theo mã nv từ mcc
                    if (axCZKEM[i].SSR_GetUserInfo(iMachineNumber[i], sdwEnrollNumber, out string sdwname, out string pass, out int privilege, out bool enabled))
                    {
                        if (axCZKEM[i].GetStrCardNumber(out cardno))
                        {
                            tam = cardno;
                            sname = sdwname;
                        }
                    }
                    //check cardno đã tồn tại hay chưa
                    if (tam == sCardnumber)
                    {
                        axCZKEM[i].SetStrCardNumber(sCardnumber);
                        if (axCZKEM[i].SSR_DeleteEnrollData(iMachineNumber[i], sdwEnrollNumber, 12))
                        {
                            log.Info("Deleted Success on device " + (i) + ", User cardno: " + sCardnumber);
                            MessageBox.Show("Delete Successed on device " + (i) + ", User cardno: " + sCardnumber, "Thông báo!");
                        }
                        else
                        {
                            axCZKEM[i].GetLastError(ref idwErrorCode);
                            log.Info("Delete failed usercardno " + sCardnumber + ", ErrorCode=" + idwErrorCode.ToString());
                            MessageBox.Show("Delete failed, ErrorCode=" + idwErrorCode.ToString(), "Error!");

                        }
                    }
                    else
                    {
                        log.Info("Deleted Success on device " + (i) + ", User cardno: " + sCardnumber);
                        MessageBox.Show("Deleted Success, Delete user cardno: " + sdwEnrollNumber, "Successed!");
                    }
                }
                catch (Exception a)
                {
                    log.Info("Error in device " + (i) + ", Errorcode: " + a.ToString());
                }
                axCZKEM[i].RefreshData(iMachineNumber[i]);//the data in the device should be refreshed
                axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device

                Cursor = Cursors.Default;
            }
        }
        private void btnGetStrCardNumber_Click(object sender, EventArgs e)
        {
            lvCard.Items.Clear();
            lvCard.BeginUpdate();
            for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            {
                if (bIsConnected[i] == false)
                {
                    MessageBox.Show("Please connect the device" + (i + 1) + " first!", "Error");
                    log.Info("Please connect the device" + (i + 1) + " first!");
                    return;
                }
                string sdwEnrollNumber = "";
                string sName = "";
                string sPassword = "";
                int iPrivilege = 0;
                bool bEnabled = false;
                string sCardnumber = "";

                Cursor = Cursors.WaitCursor;
                axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device
                axCZKEM[i].ReadAllUserID(iMachineNumber[i]);//read all the user information to the memory
                while (axCZKEM[i].SSR_GetAllUserInfo(iMachineNumber[i], out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get user information from memory
                {
                    if (axCZKEM[i].GetStrCardNumber(out sCardnumber))//get the card number from the memory
                    {
                        ListViewItem list = new ListViewItem();
                        list.Text = gridviewdevices.SelectedRows[i].Cells[0].Value.ToString();
                        list.SubItems.Add(sdwEnrollNumber);
                        list.SubItems.Add(sName);
                        list.SubItems.Add(sCardnumber);
                        list.SubItems.Add(iPrivilege.ToString());
                        list.SubItems.Add(sPassword);
                        if (bEnabled == true)
                        {
                            list.SubItems.Add("true");
                        }
                        else
                        {
                            list.SubItems.Add("false");
                        }
                        lvCard.Items.Add(list);
                    }
                }
                axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device

                Cursor = Cursors.Default;
            }
            lvCard.EndUpdate();
        }

        private void btnSetStrCardNumber_Click(object sender, EventArgs e)
        {
            //if (cbPrivilege.Text != "0")
            //{
            //    DialogResult dlr = MessageBox.Show("Bạn có chắc chắn muốn tạo user với quyền admin?","Cảnh báo!!!",MessageBoxButtons.YesNo);
            //    if (dlr == DialogResult.No)
            //    {
            //        return;
            //    }
            //}

            for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            {
                if (bIsConnected[i] == false)
                {
                    MessageBox.Show("Please connect the device" + (i + 1) + " first!", "Error");
                    log.Info("Please connect the device" + (i + 1) + " first!");
                    return;
                }

                if (txtUserID.Text.Trim() == "" || txtCardnumber.Text.Trim() == "")
                {
                    MessageBox.Show("UserID,Privilege,Cardnumber must be inputted first!", "Error");
                    log.Info("UserID,Privilege,Cardnumber must be inputted first!");
                    return;
                }
                int idwErrorCode = 0;

                bool bEnabled = true;
                if (chbEnabled.Checked)
                {
                    bEnabled = true;
                }
                else
                {
                    bEnabled = false;
                }

                if (txtUserID.Text.ToString().StartsWith("0"))
                {
                    MessageBox.Show("Can not add or update user info with UserID begin at '0'!", "Error");
                    return;
                }

                if (txtCardnumber.Text.ToString().StartsWith("0"))
                {
                    MessageBox.Show("Can not add or update user info with CardNumber begin at '0'!", "Error");
                    return;
                }

                string sdwEnrollNumber = txtUserID.Text.Trim();
                string sName = txtName.Text.Trim();
                string sPassword = txtPassword.Text.Trim();

                //0: common user, 1: enroller, 2: administrator, 3: super administrator
                //type admin
                int iPrivilege = 2;
                //type user
                //int iPrivilege = 0;
                string sCardnumber = txtCardnumber.Text.Trim();
                string deviceid = gridviewdevices.SelectedRows[i].Cells[0].Value.ToString();

                //add user into DB
                //SqlConnection con = new SqlConnection(strConnect);
                //con.Open();
                ////SqlConnection con = new SqlConnection(strConnect);
                //SqlDataAdapter MyAdapterUserCardNo;
                //DataTable MyTableUserCardNo = new DataTable();
                //MyAdapterUserCardNo = new SqlDataAdapter("SELECT UsersID FROM UsersInfo", con);
                //MyAdapterUserCardNo.Fill(MyTableUserCardNo);
                //int trung = 0;
                //for (int j = 0; j < MyTableUserCardNo.Rows.Count; j++)
                //{
                //    if (sdwEnrollNumber == MyTableUserCardNo.Rows[j][0].ToString())
                //    {
                //        trung++;
                //    }
                //}
                //if (trung == 0)
                //{
                //    string ADD = "insert into UsersInfo(UsersID,UsersName,UserCardNo,UserPassword,UserStatus,UserDeviceID) values('" + sdwEnrollNumber + "','" + sName + "','" + sCardnumber + "','" + sPassword + "','" + bEnabled.ToString() + "','" + deviceid + "')";
                //    SqlCommand cmd = new SqlCommand();
                //    cmd.Connection = con;
                //    cmd.CommandText = ADD;
                //    cmd.ExecuteNonQuery();
                //    con.Dispose();
                //    cmd.Dispose();
                //}
                //else
                //{
                //    string UPDATE = "UPDATE UsersInfo SET UsersName='" + sName + "',UserCardNo ='" + sCardnumber + "',UserPassword = '" + sPassword + "',UserStatus = '" + bEnabled.ToString() + "',UserDeviceID ='" + deviceid +"' WHERE UsersID ='" + sdwEnrollNumber +"'";
                //    SqlCommand cmd = new SqlCommand();
                //    cmd.Connection = con;
                //    cmd.CommandText = UPDATE;
                //    cmd.ExecuteNonQuery();
                //    con.Dispose();
                //    cmd.Dispose();
                //}
                //add user into MCC
                Cursor = Cursors.WaitCursor;
                axCZKEM[i].EnableDevice(iMachineNumber[i], false);
                axCZKEM[i].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                if (axCZKEM[i].SSR_SetUserInfo(iMachineNumber[i], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                {
                    MessageBox.Show("(SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Success");
                    log.Info("Successed, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                }
                else
                {
                    axCZKEM[i].GetLastError(ref idwErrorCode);
                    MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                    log.Info("Error, Operation failed,ErrorCode=" + idwErrorCode.ToString());
                }
                axCZKEM[i].RefreshData(iMachineNumber[i]);//the data in the device should be refreshed
                axCZKEM[i].EnableDevice(iMachineNumber[i], true);
                Cursor = Cursors.Default;
            }
            txtUserID.Text = "";
            txtName.Text = "";
            txtPassword.Text = "";
            txtCardnumber.Text = "";
        }

        private void btnGetGeneralLogData_Click(object sender, EventArgs e)
        {
            lvLogs.Items.Clear();
            lvLogs.BeginUpdate();
            int iIndex = 0;
            for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            {
                if (bIsConnected[i] == false)
                {
                    MessageBox.Show("Please connect the device " + (i + 1) + " first", "Error");
                    log.Info("Please connect the device " + (i + 1) + " first");
                    return;
                }

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


                Cursor = Cursors.WaitCursor;

                axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device
                if (axCZKEM[i].ReadGeneralLogData(iMachineNumber[i]))//read all the attendance records to the memory
                {
                    while (axCZKEM[i].SSR_GetGeneralLogData(iMachineNumber[i], out sdwEnrollNumber, out idwVerifyMode,
                               out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        string cardno = sdwEnrollNumber;
                        string sname = "";
                        //lấy user card theo mã nv từ mcc
                        if (axCZKEM[i].SSR_GetUserInfo(iMachineNumber[i], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                        {
                            //if (axCZKEM[i].GetStrCardNumber(out string cardno))
                            //{
                            //    string tam = cardno;
                            //    sname = name;
                            //}
                            sname = name;
                        }
                        iGLCount++;
                        lvLogs.Items.Add(gridviewdevices.SelectedRows[i].Cells[0].Value.ToString());
                        lvLogs.Items[iIndex].SubItems.Add(iGLCount.ToString());
                        lvLogs.Items[iIndex].SubItems.Add(sname);//modify by Darcy on Nov.26 2009
                        lvLogs.Items[iIndex].SubItems.Add(idwVerifyMode.ToString());
                        lvLogs.Items[iIndex].SubItems.Add(idwInOutMode.ToString());
                        lvLogs.Items[iIndex].SubItems.Add(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                        lvLogs.Items[iIndex].SubItems.Add(idwWorkcode.ToString());
                        lvLogs.Items[iIndex].SubItems.Add("0");
                        lvLogs.Items[iIndex].SubItems.Add(cardno);
                        iIndex++;
                    }
                }
                else
                {
                    Cursor = Cursors.Default;
                    axCZKEM[i].GetLastError(ref idwErrorCode);

                    if (idwErrorCode != 0)
                    {
                        MessageBox.Show("Reading data from terminal " + (i + 1) + "failed,ErrorCode: " + idwErrorCode.ToString(), "Error " + i);
                        log.Info("Error, Reading data from terminal " + (i + 1) + "failed,ErrorCode: " + idwErrorCode.ToString());
                    }
                    else
                    {
                        MessageBox.Show("No data from terminal " + (i + 1) + " returns!", "Error");
                        log.Info("Error, No data from terminal " + (i + 1) + " returns!");
                    }
                }
                axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device
                Cursor = Cursors.Default;
            }
            iIndex = 0;
            lvLogs.EndUpdate();
        }

        private void btnSaveUserToDB_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            //{
            //    string sdwEnrollNumber = "";
            //    string sName = "";
            //    string sPassword = "";
            //    int iPrivilege = 0;
            //    bool bEnabled = false;
            //    string sCardnumber = "";
            //    string deviceid = gridviewdevices.SelectedRows[i].Cells[0].Value.ToString();
            //    lvCard.Items.Clear();
            //    lvCard.BeginUpdate();
            //    Cursor = Cursors.WaitCursor;
            //    axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device
            //    axCZKEM[i].ReadAllUserID(iMachineNumber[i]);//read all the user information to the memory
            //    while (axCZKEM[i].SSR_GetAllUserInfo(iMachineNumber[i], out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get user information from memory
            //    {
            //        if (axCZKEM[i].GetStrCardNumber(out sCardnumber))
            //        {
            //            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            //            SqlConnection con = new SqlConnection(strConnect);
            //            con.Open();
            //            string ADD = "insert into UsersInfo(Deviceid,UsersName,UserCardNo,UserPassword,UserStatus) values('" +deviceid+"','"+ sName + "','" + sCardnumber + "','" + sPassword + "','" + bEnabled.ToString() + "')";
            //            SqlCommand cmd = new SqlCommand();
            //            cmd.Connection = con;
            //            cmd.CommandText = ADD;
            //            cmd.ExecuteNonQuery();
            //            con.Dispose();
            //            cmd.Dispose();
            //        }
            //    }
            //    axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device
            //    lvCard.EndUpdate();
            //    Cursor = Cursors.Default;
            //}
        }

        private void btnSaveRecordToDB_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
            {
                if (bIsConnected[i] == false)
                {
                    MessageBox.Show("Please connect the device " + (i + 1) + " first", "Error");
                    log.Info("Error, Please connect the device " + (i + 1) + " first");
                    return;
                }

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
                string sName = "";
                int idwErrorCode = 0;
                int iGLCount = 0;
                int iIndex = 0;

                Cursor = Cursors.WaitCursor;
                lvLogs.Items.Clear();
                axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device
                if (axCZKEM[i].ReadGeneralLogData(iMachineNumber[i]))//read all the attendance records to the memory
                {
                    int dem = 0;
                    while (axCZKEM[i].SSR_GetGeneralLogData(iMachineNumber[i], out sdwEnrollNumber, out idwVerifyMode,
                               out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        SqlConnection con = new SqlConnection(strConnect);
                        //SqlDataAdapter MyAdapterUser;
                        //DataTable MyTableUserCardNo = new DataTable();
                        //MyAdapterUser = new SqlDataAdapter("SELECT UserCardNo FROM UsersInfo WHERE UsersID = '"+ sdwEnrollNumber + "'", con);
                        //MyAdapterUser.Fill(MyTableUserCardNo);
                        //MessageBox.Show(sdwEnrollNumber.ToString());
                        string name = "";
                        string pass = "";
                        string tam = "";
                        try
                        {
                            //lấy user card theo mã nv từ mcc
                            if (axCZKEM[i].SSR_GetUserInfo(iMachineNumber[i], sdwEnrollNumber.ToString(), out name, out pass, out int privilege, out bool enabled))
                            {
                                sName = name;
                                if (axCZKEM[i].GetStrCardNumber(out string cardno))
                                {
                                    tam = cardno;
                                }
                            }

                        }
                        catch (Exception a)
                        {
                            MessageBox.Show(a.ToString());
                        }

                        iGLCount++;
                        con.Open();
                        string deviceid = gridviewdevices.SelectedRows[i].Cells[0].Value.ToString();
                        try
                        {
                            //string ADD = "INSERT INTO Records(RecordDevicesID,RecordYear,RecordMonth,RecordDay,RecordHour,RecordMinute,RecordSecond,RecordUserCardNo) VALUES('" + iMachineNumber[i] + "','" + idwYear.ToString() + "','" + idwMonth.ToString() + "','" + idwDay.ToString() + "','" + idwHour.ToString() + "','" + idwMinute.ToString() + "','" + idwSecond.ToString() + "','" + cardno + "')";
                            string ADD = "INSERT INTO Records(R_MCC_ID,R_UserMaCC,R_Year,R_Month,R_Day,R_Hour,R_Minute,R_Second,R_UserCardNo) VALUES('" + deviceid + "','" + sName.ToString() + "','" + idwYear.ToString() + "','" + idwMonth.ToString() + "','" + idwDay.ToString() + "','" + idwHour.ToString() + "','" + idwMinute.ToString() + "','" + idwSecond.ToString() + "','" + tam + "')";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = ADD;
                            cmd.ExecuteNonQuery();
                            dem++;
                            con.Dispose();
                            cmd.Dispose();
                        }
                        catch
                        {
                            MessageBox.Show("Lỗi khi download data", "Thông báo!");
                            log.Info("Error, Error when download data");
                        }
                    }
                    MessageBox.Show("Thêm Thành Công, số lượng: " + dem, "Added Record to your data");
                    log.Info("Thêm Thành Công, số lượng: " + dem);
                }
                else
                {
                    Cursor = Cursors.Default;
                    axCZKEM[i].GetLastError(ref idwErrorCode);

                    if (idwErrorCode != 0)
                    {
                        MessageBox.Show("Reading data from terminal " + (i + 1) + " failed,ErrorCode: " + idwErrorCode.ToString(), "Error " + i);
                        log.Info("Error, Reading data from terminal " + (i + 1) + " failed,ErrorCode: " + idwErrorCode.ToString());
                    }
                    else
                    {
                        MessageBox.Show("No data from terminal " + (i + 1) + " returns!", "Error");
                        log.Info("Error, No data from terminal " + (i + 1) + " returns!");
                    }
                }
                axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device
                Cursor = Cursors.Default;
            }
        }
        #endregion
        private void btnDongBo_Click(object sender, EventArgs e)
        {
            if (btnDongBo.Text == "Turn on sync")
            {
                //Không cho user thao tác trên giao diện khi bật đồng bộ
                btnDongBo.Text = "Turn off sync";
                groupBox1.Enabled = false;
                groupBox7.Enabled = false;

                //start service
                StartService("MayChamCongServices_x64", 60000);
                log.Info("Started service!");
                //timer1.Enabled = true;

                //run backgroundprocess
                //myWorker.RunWorkerAsync();
                return;
            }
            else if (btnDongBo.Text == "Turn off sync")
            {
                btnDongBo.Text = "Turn on sync";
                groupBox1.Enabled = true;
                groupBox7.Enabled = true;

                //stop services
                StopService("MayChamCongServices_x64", 60000);
                log.Info("Stopped service!");
                //timer1.Enabled = false;

                //Trả biến kết nối về false
                for (int i = 0; i <= gridviewdevices.Rows.Count - 1; i++)
                {
                    axCZKEM[i].Disconnect();
                    bIsConnected[i] = false;
                }
                return;

            }
        }
        #region ConnectMCC, Download Records to DB, Push Records to MCC
        public void ConnectMCC(object obj)
        {
            int k = (int)obj;
            //connectMCC
            try
            {
                //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                SqlConnection con = new SqlConnection(strConnect);
                SqlDataAdapter MyAdapterConnect;
                System.Data.DataTable MyTableConnect = new System.Data.DataTable();
                MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices", con);
                MyAdapterConnect.Fill(MyTableConnect);
                //lấy ip, port từ database
                string temp = MyTableConnect.Rows[k][0].ToString();
                string port = MyTableConnect.Rows[k][1].ToString();
                //check null
                if (temp.Trim() == "" || port.Trim() == "")
                {
                    //MessageBox.Show("IP and Port cannot be null", "Error");
                    log.Info("IP and Port" + (k + 1) + " cannot be null");
                    return;
                }
                int idwErrorCode = 0;


                bIsConnected[k] = axCZKEM[k].Connect_Net(temp, Convert.ToInt32(port));
                if (bIsConnected[k] == true)
                {
                    iMachineNumber[k] = k + 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                    axCZKEM[k].RegEvent(iMachineNumber[k], 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                    log.Info("Connected to device " + (k + 1) + ", " + temp);
                }
                else
                {
                    axCZKEM[k].GetLastError(ref idwErrorCode);
                    //MessageBox.Show("Unable to connect the device " + i + ",ErrorCode=" + idwErrorCode.ToString(), "Error");
                    log.Info("Unable to connect the device " + (k + 1) + ",ErrorCode=" + idwErrorCode.ToString() + " " + temp);
                }
            }
            catch (Exception e)
            {
                log.Info("Error when connect the devices " + (k + 1) + ", Error: " + e);
            }


            //load data MCC vào DB
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

                //Cursor = Cursors.WaitCursor;
                //lvLogs.Items.Clear();
                axCZKEM[k].EnableDevice(iMachineNumber[k], false);//disable the device
                if (axCZKEM[k].ReadGeneralLogData(iMachineNumber[k]))//read all the attendance records to the memory
                {
                    int dem = 0;
                    while (axCZKEM[k].SSR_GetGeneralLogData(iMachineNumber[k], out sdwEnrollNumber, out idwVerifyMode,
                               out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                    {
                        SqlConnection con = new SqlConnection(strConnect);
                        //SqlDataAdapter MyAdapterUser;
                        //DataTable MyTableUserCardNo = new DataTable();
                        //MyAdapterUser = new SqlDataAdapter("SELECT UserCardNo FROM UsersInfo WHERE UsersID = '" + sdwEnrollNumber + "'", con);
                        //MyAdapterUser.Fill(MyTableUserCardNo);

                        string cardno = "";
                        //lấy user card theo mã nv từ mcc
                        if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                        {
                            if (axCZKEM[k].GetStrCardNumber(out cardno))
                            {
                                string tam = cardno;
                            }
                        }

                        iGLCount++;
                        con.Open();
                        try
                        {
                            //string ADD = "INSERT INTO Records(RecordDevicesID,RecordYear,RecordMonth,RecordDay,RecordHour,RecordMinute,RecordSecond,RecordUserCardNo) VALUES('" + iMachineNumber[k] + "','" + idwYear.ToString() + "','" + idwMonth.ToString() + "','" + idwDay.ToString() + "','" + idwHour.ToString() + "','" + idwMinute.ToString() + "','" + idwSecond.ToString() + "','" + cardno + "')";
                            string ADD = "INSERT INTO Records(R_MCC_ID,R_UserMaCC,R_Year,R_Month,R_Day,R_Hour,R_Minute,R_Second) VALUES('" + iMachineNumber[k] + "','" + sdwEnrollNumber + "','" + idwYear.ToString() + "','" + idwMonth.ToString() + "','" + idwDay.ToString() + "','" + idwHour.ToString() + "','" + idwMinute.ToString() + "','" + idwSecond.ToString() + "')";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = con;
                            cmd.CommandText = ADD;
                            cmd.ExecuteNonQuery();
                            dem++;
                            con.Dispose();
                            cmd.Dispose();
                        }
                        catch
                        {
                            log.Info("Lỗi khi download data");
                        }

                    }
                    log.Info("Download Records from device " + (k + 1) + "to DB successed, records added: " + dem);
                }
                else
                {
                    //Cursor = Cursors.Default;
                    axCZKEM[k].GetLastError(ref idwErrorCode);

                    if (idwErrorCode != 0)
                    {
                        log.Info("Reading data from terminal " + (k + 1) + " failed,ErrorCode: " + idwErrorCode.ToString());
                    }
                    else
                    {
                        log.Info("No data from terminal " + (k + 1) + " returns!");
                    }
                }

                //clear data in MCC
                if (axCZKEM[k].ClearGLog(iMachineNumber[k]))
                {
                    axCZKEM[k].RefreshData(iMachineNumber[k]);//the data in the device should be refreshed
                    log.Info("All att Logs have been cleared from teiminal!");
                }
                else
                {
                    axCZKEM[k].GetLastError(ref idwErrorCode);
                    log.Info("Operation failed,ErrorCode=" + idwErrorCode.ToString());
                }

                axCZKEM[k].EnableDevice(iMachineNumber[k], true);//enable the device
                //Cursor = Cursors.Default;
            }
            catch (Exception e)
            {
                log.Info("Error when load data MCC to DB, Error: " + e);
            }

            #region add record vào MCC code cũ
            //try
            //{
            //    //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            //    SqlConnection con = new SqlConnection(strConnect);
            //    con.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = con;
            //    SqlDataAdapter MyAdapter;
            //    DataTable MyTableRecord_Push = new DataTable();
            //    MyAdapter = new SqlDataAdapter("select * from Record_push where devicesid = '"+ (k+1) +"' and (Status is null or Status not in ('created','updated','deleted'))", con);
            //    MyAdapter.Fill(MyTableRecord_Push);
            //    //kiem tra ket noi
            //    if (bIsConnected[k] == false)
            //    {
            //        log.Info("ErrorDBtoMCC, Please connect the device" + (k + 1) + " first!");
            //    }
            //    else
            //    {
            //        for (int j = 0; j < MyTableRecord_Push.Rows.Count; j++)
            //        {
            //            string personalid = MyTableRecord_Push.Rows[j][1].ToString();
            //            string usercardno = MyTableRecord_Push.Rows[j][3].ToString();
            //            string userdayfromwork = MyTableRecord_Push.Rows[j][4].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[j][4].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[j][4].ToString().Substring(6, 2);
            //            string userdaytowork = MyTableRecord_Push.Rows[j][5].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[j][5].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[j][5].ToString().Substring(6, 2);
            //            DateTime days1 = Convert.ToDateTime(userdayfromwork);
            //            DateTime days2 = Convert.ToDateTime(userdaytowork);
            //            string ngayHeThong = DateTime.Now.ToString("yyyy/MM/dd");
            //            DateTime ngayHienTai = Convert.ToDateTime(ngayHeThong);
            //            switch (MyTableRecord_Push.Rows[j][6].ToString())
            //            {
            //                case "C"://create
            //                    if (days1 <= ngayHienTai && ngayHienTai <= days2)
            //                    {
            //                        int idwErrorCode = 0;

            //                        bool bEnabled = true;

            //                        string sdwEnrollNumber = personalid;
            //                        string sName = "";
            //                        string sPassword = "";
            //                        int iPrivilege = 1;
            //                        string sCardnumber = usercardno;
            //                        int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
            //                        //Cursor = Cursors.WaitCursor;
            //                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
            //                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            //                        if (axCZKEM[deviceid-1].SSR_SetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
            //                        {
            //                            log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
            //                            //update status 
            //                            string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Created' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '"+ personalid + "'";
            //                            cmd.CommandText = UPDATESTATUS;
            //                            cmd.ExecuteNonQuery();
            //                        }
            //                        else
            //                        {
            //                            axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
            //                            log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
            //                            //update status 
            //                            string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Error' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                            cmd.CommandText = UPDATESTATUS;
            //                            cmd.ExecuteNonQuery();
            //                        }
            //                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
            //                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
            //                        //Cursor = Cursors.Default;

            //                    }
            //                    else
            //                    {
            //                        //update status 
            //                        string UPDATESTATUS = "UPDATE Record_Push SET Status = 'NotCreate' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                        cmd.CommandText = UPDATESTATUS;
            //                        cmd.ExecuteNonQuery();
            //                    }
            //                    break;

            //                case "U"://update
            //                    if (days1 <= ngayHienTai && ngayHienTai <= days2)
            //                    {
            //                        int idwErrorCode = 0;

            //                        bool bEnabled = true;
            //                        int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
            //                        string sdwEnrollNumber = personalid;
            //                        string sName = "";
            //                        string sPassword = "";
            //                        int iPrivilege = 1;
            //                        string sCardnumber = usercardno;

            //                        //Cursor = Cursors.WaitCursor;
            //                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
            //                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

            //                        axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled);
            //                        if (bEnabled == false)
            //                        {
            //                            try
            //                            {
            //                                bEnabled = true;
            //                                axCZKEM[deviceid-1].SSR_EnableUser(iMachineNumber[deviceid], sdwEnrollNumber, bEnabled);
            //                                log.Info("Updated Success, Enable cardnumber: " + sdwEnrollNumber);
            //                                //update status 
            //                                string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Updated' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                                cmd.CommandText = UPDATESTATUS;
            //                                cmd.ExecuteNonQuery();
            //                            }
            //                            catch
            //                            {
            //                                axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
            //                                log.Info("Update Error, Enable cardnumber: " + sdwEnrollNumber + ", Error code: " + idwErrorCode);
            //                                //update status 
            //                                string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Error' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                                cmd.CommandText = UPDATESTATUS;
            //                                cmd.ExecuteNonQuery();
            //                            }
            //                        }
            //                        else
            //                        {
            //                            //update status 
            //                            string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Updated' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                            cmd.CommandText = UPDATESTATUS;
            //                            cmd.ExecuteNonQuery();
            //                            log.Info("Updated Success, Enable cardnumber: " + sdwEnrollNumber);
            //                        }
            //                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
            //                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
            //                        //Cursor = Cursors.Default;

            //                    }
            //                    else
            //                    {
            //                        //update status 
            //                        string UPDATESTATUS = "UPDATE Record_Push SET Status = 'NotUpdate' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                        cmd.CommandText = UPDATESTATUS;
            //                        cmd.ExecuteNonQuery();
            //                    }
            //                    break;

            //                case "D"://delete
            //                    if (days1 <= ngayHienTai && ngayHienTai <= days2)
            //                    {
            //                        int idwErrorCode = 0;

            //                        bool bEnabled = false;
            //                        int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
            //                        string sdwEnrollNumber = personalid;
            //                        string sName = "";
            //                        string sPassword = "";
            //                        int iPrivilege = 1;
            //                        string sCardnumber = usercardno;

            //                        //Cursor = Cursors.WaitCursor;
            //                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
            //                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
            //                                                                 //axCZKEM[i].SSR_EnableUser(iMachineNumber[i], sdwEnrollNumber, bEnabled)
            //                        if (axCZKEM[deviceid-1].SSR_EnableUser(iMachineNumber[deviceid], sdwEnrollNumber, bEnabled))//disable user
            //                        {
            //                            log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
            //                            //update status 
            //                            string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Deleted' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                            cmd.CommandText = UPDATESTATUS;
            //                            cmd.ExecuteNonQuery();
            //                        }
            //                        else
            //                        {
            //                            axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
            //                            log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
            //                            //update status 
            //                            string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Error' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                            cmd.CommandText = UPDATESTATUS;
            //                            cmd.ExecuteNonQuery();
            //                        }
            //                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
            //                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
            //                        //Cursor = Cursors.Default;

            //                    }
            //                    else
            //                    {
            //                        //update status 
            //                        string UPDATESTATUS = "UPDATE Record_Push SET Status = 'NotDelete' WHERE Card_Id = '" + usercardno + "' AND Personnel_ID = '" + personalid + "'";
            //                        cmd.CommandText = UPDATESTATUS;
            //                        cmd.ExecuteNonQuery();
            //                    }
            //                    break;
            //            }
            //        }
            //    }
            //    con.Close();
            //}
            //catch
            //{
            //    log.Info("Error when push data to MCC");
            //}
            #endregion

            #region add records vào MCC code mới
            try
            {
                //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                SqlConnection con = new SqlConnection(strConnect);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                SqlDataAdapter MyAdapter;
                System.Data.DataTable MyTableRecord_Push = new System.Data.DataTable();
                MyAdapter = new SqlDataAdapter("select * from Record_push where devicesid = '" + (k + 1) + "'", con);
                MyAdapter.Fill(MyTableRecord_Push);
                //kiem tra ket noi
                if (bIsConnected[k] == false)
                {
                    log.Info("ErrorDBtoMCC, Please connect the device" + (k + 1) + " first!");
                }
                else
                {
                    for (int j = 0; j < MyTableRecord_Push.Rows.Count; j++)
                    {
                        string recordid = MyTableRecord_Push.Rows[j][0].ToString();
                        string personalid = MyTableRecord_Push.Rows[j][1].ToString();
                        string usercardno = MyTableRecord_Push.Rows[j][3].ToString();
                        string userdayfromwork = MyTableRecord_Push.Rows[j][4].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[j][4].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[j][4].ToString().Substring(6, 2);
                        string userdaytowork = MyTableRecord_Push.Rows[j][5].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[j][5].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[j][5].ToString().Substring(6, 2);
                        DateTime days1 = Convert.ToDateTime(userdayfromwork);
                        DateTime days2 = Convert.ToDateTime(userdaytowork);
                        string ngayHeThong = DateTime.Now.ToString("yyyy/MM/dd");
                        DateTime ngayHienTai = Convert.ToDateTime(ngayHeThong);

                        switch (MyTableRecord_Push.Rows[j][6].ToString())
                        {
                            case "C"://create
                                if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[j][7].ToString() == "" || MyTableRecord_Push.Rows[j][7].ToString() == "Error")) //check valid_from and status1
                                {
                                    int idwErrorCode = 0;

                                    bool bEnabled = true;

                                    string sdwEnrollNumber = personalid;
                                    string sName = "";
                                    string sPassword = "";
                                    int iPrivilege = 0;
                                    string sCardnumber = usercardno;
                                    string cardno = "";
                                    //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                    //Cursor = Cursors.WaitCursor;
                                    axCZKEM[k].EnableDevice(iMachineNumber[k + 1], false);
                                    //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                                    //lấy user card theo mã nv từ mcc
                                    if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                    {
                                        if (axCZKEM[k].GetStrCardNumber(out cardno))
                                        {
                                            string tam = cardno;
                                        }
                                    }
                                    if (cardno == sCardnumber)
                                    {
                                        axCZKEM[k].SetStrCardNumber(sCardnumber);
                                        if (axCZKEM[k].SSR_SetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                        {
                                            log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                            //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            axCZKEM[k].GetLastError(ref idwErrorCode);
                                            log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
                                            //MessageBox.Show("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                        axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                        //Cursor = Cursors.Default;
                                    }
                                    else
                                    {
                                        axCZKEM[k].SetStrCardNumber(sCardnumber);
                                        if (axCZKEM[k].SSR_SetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                        {
                                            log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                            //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            axCZKEM[k].GetLastError(ref idwErrorCode);
                                            log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
                                            //MessageBox.Show("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                        axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                        //Cursor = Cursors.Default;
                                    }
                                }
                                else if (MyTableRecord_Push.Rows[j][7].ToString() != "Create" && MyTableRecord_Push.Rows[j][7].ToString() != "Duplicate")
                                {
                                    log.Info("Error, Chưa tới ngày valid_from, recordid " + recordid);
                                    //update status 
                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                    cmd.CommandText = UPDATESTATUS;
                                    cmd.ExecuteNonQuery();
                                }
                                if ((MyTableRecord_Push.Rows[j][7].ToString() != "" && MyTableRecord_Push.Rows[j][7].ToString() != "Error") && ngayHienTai >= days2)
                                {
                                    if (MyTableRecord_Push.Rows[j][8].ToString() == "" || MyTableRecord_Push.Rows[j][8].ToString() == "Error")
                                    {
                                        int idwErrorCode = 0;
                                        string sdwEnrollNumber = personalid;
                                        string sCardnumber = usercardno;
                                        string cardno = "";

                                        axCZKEM[k].EnableDevice(iMachineNumber[k + 1], false);
                                        //lấy user card theo mã nv từ mcc
                                        if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                        {
                                            if (axCZKEM[k].GetStrCardNumber(out cardno))
                                            {
                                                string tam = cardno;
                                            }
                                        }
                                        //check cardno đã tồn tại hay chưa
                                        if (cardno == sCardnumber)
                                        {
                                            axCZKEM[k].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                            {
                                                log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                axCZKEM[k].GetLastError(ref idwErrorCode);
                                                log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                            axCZKEM[k].EnableDevice(iMachineNumber[k], true);
                                            //Cursor = Cursors.Default;
                                        }
                                        else
                                        {
                                            axCZKEM[k].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                            {
                                                //log.Info("Deleted Success, Delete user number: " + sdwEnrollNumber);
                                                //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                axCZKEM[k].GetLastError(ref idwErrorCode);
                                                log.Info("Deleted Success, Delete user number: " + sdwEnrollNumber);
                                                //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                            axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                            //Cursor = Cursors.Default;
                                        }
                                    }
                                }
                                else
                                {
                                    log.Info("Error, Chưa tới ngày...recordid " + recordid);
                                    //update status 
                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                    cmd.CommandText = UPDATESTATUS;
                                    cmd.ExecuteNonQuery();
                                }
                                break;

                            case "U"://update
                                if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[j][7].ToString() == "" || MyTableRecord_Push.Rows[j][7].ToString() == "Error"))
                                {
                                    int idwErrorCode = 0;

                                    bool bEnabled = true;
                                    //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                    string sdwEnrollNumber = personalid;
                                    string sName = "";
                                    string sPassword = "";
                                    int iPrivilege = 0;
                                    string sCardnumber = usercardno;
                                    string cardno = "";

                                    //Cursor = Cursors.WaitCursor;
                                    axCZKEM[k].EnableDevice(iMachineNumber[k + 1], false);
                                    axCZKEM[k].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                                    //lấy user card theo mã nv từ mcc
                                    if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                    {
                                        if (axCZKEM[k].GetStrCardNumber(out cardno))
                                        {
                                            string tam = cardno;
                                        }
                                    }

                                    //check cardno đã tồn tại hay chưa
                                    if (cardno != sCardnumber)
                                    {//nếu không có cardno trong mcc thì tạo mới user
                                        axCZKEM[k].SetStrCardNumber(sCardnumber);
                                        if (axCZKEM[k].SSR_SetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                        {
                                            log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                            //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            axCZKEM[k].GetLastError(ref idwErrorCode);
                                            log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
                                            //MessageBox.Show("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                        axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                        //Cursor = Cursors.Default;
                                    }
                                    else
                                    {
                                        log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                        //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                        //update status 
                                        string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                        cmd.CommandText = UPDATESTATUS;
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                else if (MyTableRecord_Push.Rows[j][7].ToString() != "Create" && MyTableRecord_Push.Rows[j][7].ToString() != "Duplicate")
                                {
                                    log.Info("Error, Chưa tới ngày valid_from, recordid " + recordid);
                                    //update status 
                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                    cmd.CommandText = UPDATESTATUS;
                                    cmd.ExecuteNonQuery();
                                }
                                if ((MyTableRecord_Push.Rows[j][8].ToString() != "" || MyTableRecord_Push.Rows[j][8].ToString() == "Error"))
                                {
                                    if (MyTableRecord_Push.Rows[j][10].ToString() == "x")
                                    {
                                        string ngayHeThongtg = DateTime.Now.ToString("yyyy/MM/dd");
                                        DateTime ngayHienTaitg = Convert.ToDateTime(ngayHeThongtg);
                                        ngayHienTaitg.AddDays(-60);
                                        if (ngayHienTaitg >= days2)
                                        {
                                            int idwErrorCode = 0;
                                            string sdwEnrollNumber = personalid;
                                            string sCardnumber = usercardno;
                                            string cardno = "";

                                            axCZKEM[k].EnableDevice(iMachineNumber[k + 1], false);
                                            //lấy user card theo mã nv từ mcc
                                            if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                            {
                                                if (axCZKEM[k].GetStrCardNumber(out cardno))
                                                {
                                                    string tam = cardno;
                                                }
                                            }
                                            //check cardno đã tồn tại hay chưa
                                            if (cardno == sCardnumber)
                                            {
                                                axCZKEM[k].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[k].GetLastError(ref idwErrorCode);
                                                    log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                    //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                                axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                                //Cursor = Cursors.Default;
                                            }
                                            else
                                            {
                                                axCZKEM[k].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[k].GetLastError(ref idwErrorCode);
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                                axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                                //Cursor = Cursors.Default;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (ngayHienTai >= days2)
                                        {
                                            int idwErrorCode = 0;
                                            string sdwEnrollNumber = personalid;
                                            string sCardnumber = usercardno;
                                            string cardno = "";

                                            axCZKEM[k].EnableDevice(iMachineNumber[k + 1], false);
                                            //lấy user card theo mã nv từ mcc
                                            if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                            {
                                                if (axCZKEM[k].GetStrCardNumber(out cardno))
                                                {
                                                    string tam = cardno;
                                                }
                                            }
                                            //check cardno đã tồn tại hay chưa
                                            if (cardno == sCardnumber)
                                            {
                                                axCZKEM[k].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[k].GetLastError(ref idwErrorCode);
                                                    log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                    //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                                axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                                //Cursor = Cursors.Default;
                                            }
                                            else
                                            {
                                                axCZKEM[k].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[k].GetLastError(ref idwErrorCode);
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                                axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                                //Cursor = Cursors.Default;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    log.Info("Error, Chưa tới ngày xóa user..." + recordid);
                                    //MessageBox.Show("Chưa tới ngày xóa user...", "Error");
                                    //update status 
                                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                    cmd.CommandText = UPDATESTATUS;
                                    cmd.ExecuteNonQuery();
                                }
                                break;

                            case "D"://delete
                                if (days1 <= ngayHienTai)
                                {
                                    int idwErrorCode = 0;

                                    bool bEnabled = false;
                                    //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                    string sdwEnrollNumber = personalid;
                                    string sName = "";
                                    string sPassword = "";
                                    int iPrivilege = 0;
                                    string sCardnumber = usercardno;
                                    string cardno = "";

                                    //Cursor = Cursors.WaitCursor;
                                    axCZKEM[k].EnableDevice(iMachineNumber[k + 1], false);
                                    //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                                    //lấy user card theo mã nv từ mcc
                                    if (axCZKEM[k].SSR_GetUserInfo(iMachineNumber[k + 1], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                    {
                                        if (axCZKEM[k].GetStrCardNumber(out cardno))
                                        {
                                            string tam = cardno;
                                        }
                                    }
                                    //check cardno đã tồn tại hay chưa
                                    if (cardno == sCardnumber)
                                    {
                                        axCZKEM[k].SetStrCardNumber(sCardnumber);
                                        if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                        {
                                            log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                            //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            axCZKEM[k].GetLastError(ref idwErrorCode);
                                            log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                            //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                        axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                        //Cursor = Cursors.Default;
                                    }
                                    else
                                    {
                                        axCZKEM[k].SetStrCardNumber(sCardnumber);
                                        if (axCZKEM[k].SSR_DeleteEnrollData(iMachineNumber[k + 1], sdwEnrollNumber, 12))
                                        {
                                            log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                            //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            axCZKEM[k].GetLastError(ref idwErrorCode);
                                            log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                            //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        axCZKEM[k].RefreshData(iMachineNumber[k + 1]);//the data in the device should be refreshed
                                        axCZKEM[k].EnableDevice(iMachineNumber[k + 1], true);
                                        //Cursor = Cursors.Default;
                                    }
                                }
                                else
                                {
                                    log.Info("Error, Chưa tới ngày....recordid " + recordid);
                                    //update status 
                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid + "'";
                                    cmd.CommandText = UPDATESTATUS;
                                    cmd.ExecuteNonQuery();
                                }
                                break;
                        }
                    }
                }
                con.Close();
            }
            catch (Exception e)
            {
                log.Info("Error when push data to MCC, Error: " + e);
            }
            #endregion
        }
        #endregion
        #region BackgroundProcess
        protected void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SqlConnection con = new SqlConnection(strConnect);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            con.Open();
            #region addlog
            //add file log từ máy POS
            string[] filePaths = Directory.GetFiles(@"C:\\PosSeito\\", "*.txt");
            foreach (string pathtg in filePaths)
            {
                try
                {
                    //Directory.GetFiles(@"D:\\", "*.txt");
                    //String path = @"D:\\Clock100516072019154950.txt";
                    if (!File.Exists(pathtg))
                    {
                        Console.WriteLine("File " + pathtg + " does not exists!");
                        return;
                    }
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

                        //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                        //SqlConnection con = new SqlConnection(strConnect);

                        //con.Open();
                        string ADDFILE = "INSERT INTO FILES(FILENAME) VALUES ('" + System.IO.Path.GetFileName(pathtg) + "')";
                        //SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = ADDFILE;
                        cmd.ExecuteNonQuery();
                        SqlDataAdapter MyAdapter;
                        System.Data.DataTable MyTable = new System.Data.DataTable();
                        MyAdapter = new SqlDataAdapter("select fileid from files where filename = '" + System.IO.Path.GetFileName(pathtg) + "'", con);
                        MyAdapter.Fill(MyTable);
                        //con.Close();
                        //cmd.Dispose();

                        for (int j = 0; j < lines.Length; j++)
                        {

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
                            cmd.Connection = con;
                            cmd.CommandText = ADDFTP;
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = UPDATEFILE;
                            cmd.ExecuteNonQuery();

                        }
                        //con.Dispose();
                        cmd.Dispose();
                        log.Info("File " + System.IO.Path.GetFileName(pathtg) + ", Records number: " + dem);
                        s1.Clear();
                    }
                    //move file to forder NewFiles
                    string ngay = DateTime.Now.ToString().Trim();
                    ngay = ngay.Substring(3, 2);
                    string thang = DateTime.Now.ToString().Trim();
                    thang = thang.Substring(0, 2);
                    string nam = DateTime.Now.ToString().Trim();
                    nam = nam.Substring(6, 2);
                    String dirPath = @"C:\FilesPOS\" + nam + thang + ngay;
                    String dirPath1 = @"C:\PosSeito\" + System.IO.Path.GetFileName(pathtg);
                    String dirPath2 = @"C:\FilesPOS\" + nam + thang + ngay + @"\" + System.IO.Path.GetFileName(pathtg);

                    // Kiểm tra xem đường dẫn thư mục tồn tại không.
                    bool exist = Directory.Exists(dirPath);
                    if (!exist)
                    {
                        log.Info(dirPath + " does not exist.");
                        log.Info("Create directory: " + dirPath);

                        // Tạo thư mục.
                        Directory.CreateDirectory(dirPath);
                    }
                    if (File.Exists(dirPath2))
                    {
                        File.Delete(dirPath2);
                    }
                    File.Move(dirPath1, dirPath2);
                    if (File.Exists(dirPath2))
                    {
                        log.Info("Move file successed!");
                    }
                }
                catch (Exception a)
                {
                    log.Info("Error when read file " + System.IO.Path.GetFileName(pathtg) + ", Error: " + a);
                }
            }
            #endregion

            #region push records full store
            //SqlConnection con = new SqlConnection(strConnect);
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = con;
            //con.Open();
            SqlDataAdapter MyAdapterConnect;
            System.Data.DataTable MyTableDevices = new System.Data.DataTable();
            MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices", con);
            MyAdapterConnect.Fill(MyTableDevices);

            SqlDataAdapter MyAdapterRecordsFullStore;
            System.Data.DataTable MyTableRecordsFullStore = new System.Data.DataTable();
            MyAdapterRecordsFullStore = new SqlDataAdapter("SELECT * FROM Record_Push WHERE Full_STORE='x'", con);
            MyAdapterRecordsFullStore.Fill(MyTableRecordsFullStore);
            for (int j = 0; j <= MyTableRecordsFullStore.Rows.Count - 1; j++)
            {
                int demcreate1 = 0;
                int demduplicate1 = 0;
                int demerror1 = 0;
                int demcreate2 = 0;
                int demduplicate2 = 0;
                int demerror2 = 0;
                for (int m = 1; m <= MyTableDevices.Rows.Count; m++)
                {

                    bIsConnected[m - 1] = false;
                    //connect MCC theo mã id
                    try
                    {

                        //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                        //SqlConnection con = new SqlConnection(connection);
                        //SqlDataAdapter MyAdapterConnect;
                        System.Data.DataTable MyTableConnect = new System.Data.DataTable();
                        MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices Where DevicesID = '" + m + "'", con);
                        MyAdapterConnect.Fill(MyTableConnect);
                        //lấy ip, port từ database
                        string temp = MyTableConnect.Rows[0][0].ToString();
                        string port = MyTableConnect.Rows[0][1].ToString();
                        //check null
                        if (temp.Trim() == "" || port.Trim() == "")
                        {
                            //MessageBox.Show("IP and Port " + m + " cannot be null", "Error");
                            log.Info("IP and Port" + m + " cannot be null");
                            return;
                        }
                        int idwErrorCode = 0;

                        //bIsConnected[i] = axCZKEM[i].Connect_Net(temp, Convert.ToInt32(port));
                        bIsConnected[m - 1] = axCZKEM[m - 1].Connect_Net(temp, Convert.ToInt32(port));
                        if (bIsConnected[m - 1] == true)
                        {
                            iMachineNumber[m - 1] = m;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                            axCZKEM[m - 1].RegEvent(iMachineNumber[m], 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                        }
                        else
                        {
                            axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                            //MessageBox.Show("Unable to connect the device " + m + ",ErrorCode=" + idwErrorCode.ToString(), "Error");
                            log.Info("Unable to connect the device " + m + ",ErrorCode=" + idwErrorCode.ToString());
                        }

                    }
                    catch
                    {
                        log.Info("Error when connect the devices " + m);
                        //MessageBox.Show("Error when connect the devices " + m);
                    }


                    //push data
                    try
                    {

                        string recordid = MyTableRecordsFullStore.Rows[j][0].ToString();
                        //SqlConnection con = new SqlConnection(strConnect);
                        //con.Open();
                        //SqlCommand cmd = new SqlCommand();
                        //cmd.Connection = con;
                        SqlDataAdapter MyAdapter;
                        System.Data.DataTable MyTableRecord_Push = new System.Data.DataTable();
                        MyAdapter = new SqlDataAdapter("select * from Record_push where R_ID = '" + recordid + "'", con);
                        MyAdapter.Fill(MyTableRecord_Push);
                        //kiem tra ket noi
                        if (bIsConnected[m - 1] == false)
                        {
                            log.Info("ErrorDBtoMCC, Please connect the device" + m + " first!");
                            //MessageBox.Show("ErrorDBtoMCC, Please connect the device" + m + " first!", "Error!");
                        }
                        else
                        {

                            string personalid = MyTableRecord_Push.Rows[0][1].ToString();
                            string usercardno = MyTableRecord_Push.Rows[0][3].ToString();
                            string userdayfromwork = MyTableRecord_Push.Rows[0][4].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[0][4].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[0][4].ToString().Substring(6, 2);
                            string userdaytowork = MyTableRecord_Push.Rows[0][5].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[0][5].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[0][5].ToString().Substring(6, 2);
                            DateTime days1 = Convert.ToDateTime(userdayfromwork);
                            DateTime days2 = Convert.ToDateTime(userdaytowork);
                            string ngayHeThong = DateTime.Now.ToString("yyyy/MM/dd");
                            DateTime ngayHienTai = Convert.ToDateTime(ngayHeThong);
                            switch (MyTableRecord_Push.Rows[0][6].ToString())
                            {
                                case "C"://create
                                    if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error")) //check valid_from and status1
                                    {
                                        int idwErrorCode = 0;

                                        bool bEnabled = true;

                                        string sdwEnrollNumber = personalid;
                                        string sName = "";
                                        string sPassword = "";
                                        int iPrivilege = 0;
                                        string sCardnumber = usercardno;
                                        string cardno = "";
                                        //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                        //Cursor = Cursors.WaitCursor;
                                        axCZKEM[m - 1].EnableDevice(iMachineNumber[m], false);
                                        //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                                        //lấy user card theo mã nv từ mcc
                                        if (axCZKEM[m - 1].SSR_GetUserInfo(iMachineNumber[m], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                        {
                                            if (axCZKEM[m - 1].GetStrCardNumber(out cardno))
                                            {
                                                string tam = cardno;
                                            }
                                        }
                                        if (cardno == sCardnumber)
                                        {
                                            axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[m - 1].SSR_SetUserInfo(iMachineNumber[m], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                            {
                                                log.Info("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                //MessageBox.Show("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                //update status 
                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                //cmd.CommandText = UPDATESTATUS;
                                                //cmd.ExecuteNonQuery();
                                                demduplicate1++;
                                            }
                                            else
                                            {
                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
                                                //MessageBox.Show("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                //update status 
                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                //cmd.CommandText = UPDATESTATUS;
                                                //cmd.ExecuteNonQuery();
                                                demerror1++;
                                            }
                                            axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                            axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                            //Cursor = Cursors.Default;
                                        }
                                        else
                                        {
                                            axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[m - 1].SSR_SetUserInfo(iMachineNumber[m], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                            {
                                                log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                //update status 
                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                //cmd.CommandText = UPDATESTATUS;
                                                //cmd.ExecuteNonQuery();
                                                demcreate1++;
                                            }
                                            else
                                            {
                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
                                                //MessageBox.Show("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                //update status 
                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                //cmd.CommandText = UPDATESTATUS;
                                                //cmd.ExecuteNonQuery();
                                                demerror1++;
                                            }
                                            axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                            axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                            //Cursor = Cursors.Default;
                                        }
                                    }
                                    else if ((MyTableRecord_Push.Rows[0][7].ToString() != "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error") && ngayHienTai >= days2)
                                    {
                                        if (MyTableRecord_Push.Rows[0][8].ToString() == "" || MyTableRecord_Push.Rows[0][8].ToString() == "Error")
                                        {
                                            int idwErrorCode = 0;
                                            string sdwEnrollNumber = personalid;
                                            string sCardnumber = usercardno;
                                            string cardno = "";

                                            axCZKEM[m - 1].EnableDevice(iMachineNumber[m], false);
                                            //lấy user card theo mã nv từ mcc
                                            if (axCZKEM[m - 1].SSR_GetUserInfo(iMachineNumber[m], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                            {
                                                if (axCZKEM[m - 1].GetStrCardNumber(out cardno))
                                                {
                                                    string tam = cardno;
                                                }
                                            }
                                            //check cardno đã tồn tại hay chưa
                                            if (cardno == sCardnumber)
                                            {
                                                axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    //cmd.CommandText = UPDATESTATUS;
                                                    //cmd.ExecuteNonQuery();
                                                    demcreate2++;
                                                }
                                                else
                                                {
                                                    axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                    log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                    //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    //cmd.CommandText = UPDATESTATUS;
                                                    //cmd.ExecuteNonQuery();
                                                    demerror2++;
                                                }
                                                axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                                axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                                //Cursor = Cursors.Default;
                                            }
                                            else
                                            {
                                                axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    //cmd.CommandText = UPDATESTATUS;
                                                    //cmd.ExecuteNonQuery();
                                                    demcreate2++;
                                                }
                                                else
                                                {
                                                    axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                    log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                    //cmd.CommandText = UPDATESTATUS;
                                                    //cmd.ExecuteNonQuery();
                                                    demduplicate2++;
                                                }
                                                axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                                axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                                //Cursor = Cursors.Default;
                                            }
                                        }
                                    }
                                    else if (MyTableRecord_Push.Rows[j][8].ToString() != "Create" && MyTableRecord_Push.Rows[j][8].ToString() != "Duplicate")
                                    {
                                        log.Info("Chưa tới ngày xóa recordid " + recordid);
                                        //update status 
                                        string UPDATESTATUS = "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid + "'";
                                        cmd.CommandText = UPDATESTATUS;
                                        cmd.ExecuteNonQuery();
                                    }
                                    break;

                                case "U"://update
                                    if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error"))
                                    {
                                        int idwErrorCode = 0;

                                        bool bEnabled = true;
                                        //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                        string sdwEnrollNumber = personalid;
                                        string sName = "";
                                        string sPassword = "";
                                        int iPrivilege = 0;
                                        string sCardnumber = usercardno;
                                        string cardno = "";

                                        //Cursor = Cursors.WaitCursor;
                                        axCZKEM[m - 1].EnableDevice(iMachineNumber[m], false);
                                        axCZKEM[m - 1].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                                        //lấy user card theo mã nv từ mcc
                                        if (axCZKEM[m - 1].SSR_GetUserInfo(iMachineNumber[m], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                        {
                                            if (axCZKEM[m - 1].GetStrCardNumber(out cardno))
                                            {
                                                string tam = cardno;
                                            }
                                        }

                                        //check cardno đã tồn tại hay chưa
                                        if (cardno != sCardnumber)
                                        {//nếu không có cardno trong mcc thì tạo mới user
                                            axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[m - 1].SSR_SetUserInfo(iMachineNumber[m], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                            {
                                                log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                //update status 
                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                //cmd.CommandText = UPDATESTATUS;
                                                //cmd.ExecuteNonQuery();
                                                demcreate1++;
                                            }
                                            else
                                            {
                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                log.Info("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString());
                                                //MessageBox.Show("Create Error, Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                //update status 
                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                //cmd.CommandText = UPDATESTATUS;
                                                //cmd.ExecuteNonQuery();
                                                demerror1++;
                                            }
                                            axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                            axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                            //Cursor = Cursors.Default;
                                        }
                                        else
                                        {
                                            log.Info("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                            //MessageBox.Show("Created Success, (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                            //update status 
                                            //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                            //cmd.CommandText = UPDATESTATUS;
                                            //cmd.ExecuteNonQuery();
                                            demduplicate1++;
                                        }
                                    }
                                    else if ((MyTableRecord_Push.Rows[0][8].ToString() != "" || MyTableRecord_Push.Rows[0][8].ToString() == "Error"))
                                    {
                                        if (MyTableRecord_Push.Rows[0][10].ToString() == "x")
                                        {
                                            string ngayHeThongtg = DateTime.Now.ToString("yyyy/MM/dd");
                                            DateTime ngayHienTaitg = Convert.ToDateTime(ngayHeThongtg);
                                            ngayHienTaitg.AddDays(-60);
                                            if (ngayHienTaitg >= days2)
                                            {
                                                int idwErrorCode = 0;
                                                string sdwEnrollNumber = personalid;
                                                string sCardnumber = usercardno;
                                                string cardno = "";

                                                axCZKEM[m - 1].EnableDevice(iMachineNumber[m], false);
                                                //lấy user card theo mã nv từ mcc
                                                if (axCZKEM[m - 1].SSR_GetUserInfo(iMachineNumber[m], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                                {
                                                    if (axCZKEM[m - 1].GetStrCardNumber(out cardno))
                                                    {
                                                        string tam = cardno;
                                                    }
                                                }
                                                //check cardno đã tồn tại hay chưa
                                                if (cardno == sCardnumber)
                                                {
                                                    axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                                    if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                                    {
                                                        log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                        //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demcreate2++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                        //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demerror2++;
                                                    }
                                                    axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                                    axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                                    //Cursor = Cursors.Default;
                                                }
                                                else
                                                {
                                                    axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                                    if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                                    {
                                                        log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                        //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demcreate2++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                        //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demduplicate2++;
                                                    }
                                                    axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                                    axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                                    //Cursor = Cursors.Default;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (ngayHienTai >= days2)
                                            {
                                                int idwErrorCode = 0;
                                                string sdwEnrollNumber = personalid;
                                                string sCardnumber = usercardno;
                                                string cardno = "";

                                                axCZKEM[m - 1].EnableDevice(iMachineNumber[m], false);
                                                //lấy user card theo mã nv từ mcc
                                                if (axCZKEM[m - 1].SSR_GetUserInfo(iMachineNumber[m], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                                {
                                                    if (axCZKEM[m - 1].GetStrCardNumber(out cardno))
                                                    {
                                                        string tam = cardno;
                                                    }
                                                }
                                                //check cardno đã tồn tại hay chưa
                                                if (cardno == sCardnumber)
                                                {
                                                    axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                                    if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                                    {
                                                        log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                        //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demcreate2++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                        //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demerror2++;
                                                    }
                                                    axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                                    axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                                    //Cursor = Cursors.Default;
                                                }
                                                else
                                                {
                                                    axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                                    if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                                    {
                                                        log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                        //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demcreate2++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                        //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demduplicate2++;
                                                    }
                                                    axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                                    axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                                    //Cursor = Cursors.Default;
                                                }
                                            }
                                        }
                                    }
                                    else if (MyTableRecord_Push.Rows[j][8].ToString() != "Create" && MyTableRecord_Push.Rows[j][8].ToString() != "Duplicate")
                                    {
                                        log.Info("Error, Chưa tới ngày xóa user...");
                                        //MessageBox.Show("Chưa tới ngày xóa user...", "Error");
                                        //update status 
                                        string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                        cmd.CommandText = UPDATESTATUS;
                                        cmd.ExecuteNonQuery();
                                    }
                                    break;

                                case "D"://delete
                                    if (days1 <= ngayHienTai && ngayHienTai <= days2)
                                    {
                                        int idwErrorCode = 0;

                                        bool bEnabled = false;
                                        //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                        string sdwEnrollNumber = personalid;
                                        string sName = "";
                                        string sPassword = "";
                                        int iPrivilege = 0;
                                        string sCardnumber = usercardno;
                                        string cardno = "";

                                        //Cursor = Cursors.WaitCursor;
                                        axCZKEM[m - 1].EnableDevice(iMachineNumber[m], false);
                                        //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                                        //lấy user card theo mã nv từ mcc
                                        if (axCZKEM[m - 1].SSR_GetUserInfo(iMachineNumber[m], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                        {
                                            if (axCZKEM[m - 1].GetStrCardNumber(out cardno))
                                            {
                                                string tam = cardno;
                                            }
                                        }
                                        //check cardno đã tồn tại hay chưa
                                        if (cardno == sdwEnrollNumber)
                                        {
                                            axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                            {
                                                log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                //MessageBox.Show("Deleted Success, Disable user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                log.Info("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                            axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                            //Cursor = Cursors.Default;
                                        }
                                        else
                                        {
                                            axCZKEM[m - 1].SetStrCardNumber(sCardnumber);
                                            if (axCZKEM[m - 1].SSR_DeleteEnrollData(iMachineNumber[m], sdwEnrollNumber, 12))
                                            {
                                                log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                log.Info("Deleted Success, Disable user number: " + sdwEnrollNumber);
                                                //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            axCZKEM[m - 1].RefreshData(iMachineNumber[m]);//the data in the device should be refreshed
                                            axCZKEM[m - 1].EnableDevice(iMachineNumber[m], true);
                                            //Cursor = Cursors.Default;
                                        }
                                    }
                                    else
                                    {
                                        log.Info("Chưa tới ngày xóa recordid " + recordid);
                                        //update status 
                                        string UPDATESTATUS = "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid + "'";
                                        cmd.CommandText = UPDATESTATUS;
                                        cmd.ExecuteNonQuery();
                                    }
                                    break;
                            }

                        }
                    }
                    catch
                    {
                        log.Info("Error when push data to MCC");
                        //MessageBox.Show("Error when push data to MCC");
                    }
                }

                if (demcreate1 == MyTableDevices.Rows.Count)
                {

                    //update status 
                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + MyTableRecordsFullStore.Rows[j][0].ToString() + "'";
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();
                    log.Info("Status1 Create success R_id " + MyTableRecordsFullStore.Rows[j][0].ToString());
                }
                else if (demduplicate1 == MyTableDevices.Rows.Count)
                {

                    //update status 
                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + MyTableRecordsFullStore.Rows[j][0].ToString() + "'";
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();
                    log.Info("Status1 Duplicate success R_id " + MyTableRecordsFullStore.Rows[j][0].ToString());
                }
                else if (demerror1 == MyTableDevices.Rows.Count)
                {

                    //update status 
                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + MyTableRecordsFullStore.Rows[j][0].ToString() + "'";
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();
                    log.Info("Status1 Eror R_id " + MyTableRecordsFullStore.Rows[j][0].ToString());
                }
                if (demcreate2 == MyTableDevices.Rows.Count)
                {
                    //update status 
                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + MyTableRecordsFullStore.Rows[j][0].ToString() + "'";
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();
                    log.Info("Status2 Create success R_id " + MyTableRecordsFullStore.Rows[j][0].ToString());
                }
                else if (demduplicate2 == MyTableDevices.Rows.Count)
                {
                    //update status 
                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + MyTableRecordsFullStore.Rows[j][0].ToString() + "'";
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();
                    log.Info("Status2 Duplicate success R_id " + MyTableRecordsFullStore.Rows[j][0].ToString());
                }
                else if (demerror2 == MyTableDevices.Rows.Count)
                {
                    //update status 
                    string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + MyTableRecordsFullStore.Rows[j][0].ToString() + "'";
                    cmd.CommandText = UPDATESTATUS;
                    cmd.ExecuteNonQuery();
                    log.Info("Status2 Error R_id " + MyTableRecordsFullStore.Rows[j][0].ToString());
                }
            }
            con.Close();
            #endregion

            #region call threads
            bool bSetMaxThread = ThreadPool.SetMaxThreads(20, 500);
            //khai báo array multi thread
            Thread[] Devices = new Thread[10];
            try//connect devices
            {
                if (!bSetMaxThread)
                {
                    log.Info("Setting max threads of the threadpool failed!");
                }

                for (int i = 0; i <= gridviewdevices.Rows.Count - 1; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectMCC), i);
                    //if (i != gridviewdevices.Rows.Count)
                    //{

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectMCC), i);
                    //Devices[i] = new Thread(new ThreadStart(() => ConnectMCC(i)));
                    //Devices[i].Start();
                    //}
                }
            }
            catch (Exception a)
            {
                log.Info("Error when connect devices, Error: " + a + "!");
            }
            #endregion

            #region push data SAP to DB
            //try
            //{
            //    string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            //    SqlConnection con = new SqlConnection(connection);
            //    con.Open();
            //    SqlCommand cmd = new SqlCommand();
            //    cmd.Connection = con;
            //    SqlDataAdapter MyAdapter;
            //    DataTable MyTableRecord_Push = new DataTable();
            //    MyAdapter = new SqlDataAdapter("select * from Record_push", con);
            //    MyAdapter.Fill(MyTableRecord_Push);
            //    for (int i=0;i<MyTableRecord_Push.Rows.Count;i++)
            //    {
            //        string usercardno = MyTableRecord_Push.Rows[i][3].ToString();
            //        string userdaytowork = MyTableRecord_Push.Rows[i][4].ToString().Substring(0,4)+"/"+ MyTableRecord_Push.Rows[i][4].ToString().Substring(4,2)+"/"+ MyTableRecord_Push.Rows[i][4].ToString().Substring(6, 2);
            //        DateTime days = Convert.ToDateTime(userdaytowork);
            //        string ngayHeThong = DateTime.Now.ToString("yyyy/MM/dd");
            //        DateTime ngayHienTai = Convert.ToDateTime(ngayHeThong);
            //        switch (MyTableRecord_Push.Rows[i][6].ToString())
            //        {
            //            case "C":
            //                if (days <= ngayHienTai)
            //                {
            //                    //create record to DB
            //                    string ADDRECORD = "INSERT INTO UsersInfo(UserCardNo,UsersDayToWork,UserStatus) VALUES ('"+ usercardno +"','"+ userdaytowork.ToString() +"','Enable')";
            //                    cmd.CommandText = ADDRECORD;
            //                    cmd.ExecuteNonQuery();
            //                    //update status 
            //                    string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Created' WHERE Card_Id = '" + usercardno +"'";
            //                    cmd.CommandText = UPDATESTATUS;
            //                    cmd.ExecuteNonQuery();
            //                }
            //                else
            //                {
            //                    //update status 
            //                    string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Creating' WHERE Card_Id = '" + usercardno + "'";
            //                    cmd.CommandText = UPDATESTATUS;
            //                    cmd.ExecuteNonQuery();
            //                }
            //                break;
            //            case "U":
            //                if (days <= ngayHienTai)
            //                {
            //                    //create record to DB
            //                    string UPDATERECORD = "UPDATE UsersInfo SET UsersDayToWork='" + userdaytowork.ToString() + "',UserStatus='Enable' WHERE UserCardNo='"+ usercardno + "'";
            //                    cmd.CommandText = UPDATERECORD;
            //                    cmd.ExecuteNonQuery();
            //                    //update status 
            //                    string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Updated' WHERE Card_Id = '" + usercardno + "'";
            //                    cmd.CommandText = UPDATESTATUS;
            //                    cmd.ExecuteNonQuery();
            //                }
            //                else
            //                {
            //                    //update status 
            //                    string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Updating' WHERE Card_Id = '" + usercardno + "'";
            //                    cmd.CommandText = UPDATESTATUS;
            //                    cmd.ExecuteNonQuery();
            //                }
            //                break;
            //            case "D":
            //                if (days <= ngayHienTai)
            //                {
            //                    //create record to DB
            //                    string DELETERECORD = "DELETE UsersInfo WHERE UserCardNo='" + usercardno + "'";
            //                    cmd.CommandText = DELETERECORD;
            //                    cmd.ExecuteNonQuery();
            //                    //update status 
            //                    string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Deleted' WHERE Card_Id = '" + usercardno + "'";
            //                    cmd.CommandText = UPDATESTATUS;
            //                    cmd.ExecuteNonQuery();
            //                }
            //                else
            //                {
            //                    //update status 
            //                    string UPDATESTATUS = "UPDATE Record_Push SET Status = 'Deleting' WHERE Card_Id = '" + usercardno + "'";
            //                    cmd.CommandText = UPDATESTATUS;
            //                    cmd.ExecuteNonQuery();
            //                }
            //                break;
            //        }
            //    }
            //}
            //catch
            //{
            //    log.Info("Error when push data to DB!");
            //}
            #endregion

            #region load data MCC vào DB
            //for (int i = 0; i <= 9; i++)
            //{ 
            //    //int i = e.Argument as Int32;

            //    //kiểm tra máy chấm công đang kết nối thì đẩy dữ liệu từ máy chấm công về DB
            //    while (bIsConnected[i])
            //    {
            //        string sdwEnrollNumber = "";
            //        int idwTMachineNumber = 0;
            //        int idwEMachineNumber = 0;
            //        int idwVerifyMode = 0;
            //        int idwInOutMode = 0;
            //        int idwYear = 0;
            //        int idwMonth = 0;
            //        int idwDay = 0;
            //        int idwHour = 0;
            //        int idwMinute = 0;
            //        int idwSecond = 0;
            //        int idwWorkcode = 0;

            //        int idwErrorCode = 0;
            //        int iGLCount = 0;
            //        int iIndex = 0;

            //        Cursor = Cursors.WaitCursor;
            //        lvLogs.Items.Clear();
            //        axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device
            //        if (axCZKEM[i].ReadGeneralLogData(iMachineNumber[i]))//read all the attendance records to the memory
            //        {
            //            while (axCZKEM[i].SSR_GetGeneralLogData(iMachineNumber[i], out sdwEnrollNumber, out idwVerifyMode,
            //                       out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
            //            {
            //                iGLCount++;
            //                string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            //                SqlConnection con = new SqlConnection(connection);
            //                con.Open();
            //                //string ADD = "insert into Record(ID,UserID,YearMonth,CheckHours) values('" + iGLCount + "','" + sdwEnrollNumber + "','" + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + "','" + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString() + "')";
            //                string ADD = "INSERT INTO RecordsInfo(RecordDevicesID,RecordYear,RecordMonth,RecordDay,RecordHour,RecordMinute,RecordSecond,RecordUserCardNo) VALUES('" + iMachineNumber[i] + "','" + idwYear.ToString() + "','" + idwMonth.ToString() + "','" + idwDay.ToString() + "','" + idwHour.ToString() + "','" + idwMinute.ToString() + "','" + idwSecond.ToString() + "','" + sdwEnrollNumber + "')";
            //                SqlCommand cmd = new SqlCommand();
            //                cmd.Connection = con;
            //                cmd.CommandText = ADD;
            //                cmd.ExecuteNonQuery();
            //                con.Dispose();
            //                cmd.Dispose();
            //            }
            //        }
            //        else
            //        {
            //            Cursor = Cursors.Default;
            //            axCZKEM[i].GetLastError(ref idwErrorCode);

            //            if (idwErrorCode != 0)
            //            {
            //                //MessageBox.Show("Reading data from terminal " + i + " failed,ErrorCode: " + idwErrorCode.ToString(), "ErrorDevices " + i);
            //                log.Info("Reading data from terminal " + i + " failed,ErrorCode: " + idwErrorCode.ToString());
            //            }
            //            else
            //            {
            //                //MessageBox.Show("No data from terminal " + i + " returns!", "ErrorDevices " + i);
            //                log.Info("No data from terminal " + i + " returns!");
            //            }
            //        }
            //        axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device
            //        Cursor = Cursors.Default;
            //        Thread.Sleep(18000);
            //    }
            //}
            #endregion

        }
        protected void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        protected void myWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            ServiceController controller = new ServiceController("MayChamCongServices_x64");
            if (controller.Status == ServiceControllerStatus.Running)
            {
                MessageBox.Show("MayChamCongServices_x64 is running", "Thông báo!");
                btnDongBo.Text = "Turn on sync";
                groupBox1.Enabled = false;
                groupBox7.Enabled = false;
            }

            if (controller.Status == ServiceControllerStatus.Stopped)
            {
                MessageBox.Show("MayChamCongServices_x64 is stopped", "Thông báo!");
                btnDongBo.Text = "Turn off sync";
                groupBox1.Enabled = true;
                groupBox7.Enabled = true;
            }

            LoadComboboxStore();
        }

        private void LoadComboboxStore()
        {
            try
            {
                DataRow dr;


                SqlConnection con = new SqlConnection(strConnect);
                con.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT MallsID FROM Malls", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                dr = dt.NewRow();
                dr.ItemArray = new object[] { "ALL" };
                dt.Rows.InsertAt(dr, 0);

                cbbMall.ValueMember = "MallsID";
                cbbMall.DisplayMember = "MallsID";
                cbbMall.DataSource = dt;

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnXemCTRecords_Click(object sender, EventArgs e)
        {
            this.Hide();
            Records f = new Records();
            f.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            myWorker.RunWorkerAsync();
        }

        private void btnXemCTLogFile_Click(object sender, EventArgs e)
        {
            this.Hide();
            Logfile f = new Logfile();
            f.Show();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            string svcStatus = service.Status.ToString();
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        //stop services
        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void btnClearGLog_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Confirm?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
                {
                    if (bIsConnected[i] == false)
                    {
                        MessageBox.Show("Please connect the device first", "Error");
                        return;
                    }
                    int idwErrorCode = 0;

                    lvLogs.Items.Clear();
                    axCZKEM[i].EnableDevice(iMachineNumber[i], false);//disable the device

                    //if (axCZKEM[i].ClearKeeperData(iMachineNumber[i]))//clear all data in MCC
                    //{
                    //    MessageBox.Show("All data have been cleared from teiminal!", "Success");
                    //}
                    if (axCZKEM[i].ClearGLog(iMachineNumber[i]))
                    {
                        axCZKEM[i].RefreshData(iMachineNumber[i]);//the data in the device should be refreshed
                        MessageBox.Show("All att Logs have been cleared from teiminal!", "Success");
                    }
                    else
                    {
                        axCZKEM[i].GetLastError(ref idwErrorCode);
                        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                    }
                    axCZKEM[i].EnableDevice(iMachineNumber[i], true);//enable the device
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(strConnect);
            SqlDataAdapter MyAdapterDevices;
            System.Data.DataTable MyTableDevices = new System.Data.DataTable();
            MyAdapterDevices = new SqlDataAdapter("SELECT COUNT(*) FROM Devices", con);
            MyAdapterDevices.Fill(MyTableDevices);
            for (int k = 0; k <= Convert.ToInt32(MyTableDevices.Rows[0][0].ToString()) - 1; k++)
            {
                if (bIsConnected[k] == false)
                {
                    MessageBox.Show("Please connect the device" + (k + 1) + " first!", "Error");
                    log.Info("Please connect the device" + (k + 1) + " first!");
                }
                else
                {
                    bool setdevicetime = axCZKEM[k].SetDeviceTime(iMachineNumber[k]);
                    if (setdevicetime)
                    {
                        log.Info("Set time successed on device " + (k + 1));
                        MessageBox.Show("Set time successed on device " + (k + 1), "Thông báo");
                    }
                    else
                    {
                        log.Info("Set time failed on device " + (k + 1));
                        MessageBox.Show("Set time failed on device " + (k + 1), "Lỗi!!!");
                    }
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //System.Data.DataTable Dv = null;
            //Dv = new Connect().ThongtinDevices();
            //gridviewdevices.DataSource = Dv;
            //gridviewdevices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //gridviewdevices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void btnrefresh_Click(object sender, EventArgs e)
        {
            System.Data.DataTable Dv = null;
            Dv = new Connect().ThongtinDevices();
            gridviewdevices.DataSource = Dv;
            gridviewdevices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridviewdevices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnExportStrCardNumber_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text|*.csv";
            sfd.Title = "Save file...";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                WriteFile(sfd.FileName);
            }

            #region xuất file excel
            //try
            //{
            //    using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel workbook|*.xls", ValidateNames = true })
            //    {
            //        if (sfd.ShowDialog() == DialogResult.OK)
            //        {
            //            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            //            Workbook wb = app.Workbooks.Add(XlSheetType.xlWorksheet);
            //            Worksheet ws = (Worksheet)app.ActiveSheet;
            //            app.Visible = false;
            //            ws.Cells[1, 1] = "MachineNumber";
            //            ws.Cells[1, 2] = "UserID";
            //            ws.Cells[1, 3] = "Name";
            //            ws.Cells[1, 4] = "CardNumber";
            //            ws.Cells[1, 5] = "Privilege";
            //            ws.Cells[1, 6] = "Password";
            //            ws.Cells[1, 7] = "Enabled";


            //            int i = 2;
            //            foreach (ListViewItem item in lvCard.Items)
            //            {
            //                if (item.SubItems[0].Text != "")
            //                {
            //                    ws.Cells[i, 1] = item.SubItems[0].Text;
            //                }
            //                else
            //                {
            //                    ws.Cells[i, 1] = "";
            //                }
            //                if (item.SubItems[1].Text == "")
            //                {
            //                    ws.Cells[i, 2] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 2] = item.SubItems[1].Text;
            //                }
            //                if (item.SubItems[2].Text == "")
            //                {
            //                    ws.Cells[i, 3] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 3] = item.SubItems[2].Text;
            //                }
            //                if (item.SubItems[3].Text == "")
            //                {
            //                    ws.Cells[i, 4] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 4] = item.SubItems[3].Text;
            //                }
            //                if (item.SubItems[4].Text == "")
            //                {
            //                    ws.Cells[i, 5] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 5] = item.SubItems[4].Text;
            //                }
            //                if (item.SubItems[5].Text =="")
            //                {
            //                    ws.Cells[i, 6] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 6] = item.SubItems[5].Text;
            //                }
            //                if (item.SubItems[6].Text =="")
            //                {
            //                    ws.Cells[i, 7] = "";

            //                }
            //                else
            //                {
            //                    ws.Cells[i, 7] = item.SubItems[6].Text;
            //                }
            //                i++;
            //            }
            //            ws.SaveAs(sfd.FileName, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, XlSaveAsAccessMode.xlNoChange, XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
            //            app.Quit();
            //            log.Info("UserInfo has been successfully exported!");
            //            MessageBox.Show("UserInfo has been successfully exported!", "Thông báo!");
            //        }
            //    }
            //}
            //catch (Exception a)
            //{
            //    log.Info("Error when export userinfo, ErrorCode: "+a.ToString());
            //    MessageBox.Show("Error when export userinfo, ErrorCode: " + a.ToString(),"Error");
            //}
            #endregion


        }

        void WriteFile(string file)
        {
            #region xuất file text .csv
            try
            {
                #endregion
                StreamWriter sw = new StreamWriter(file, false, Encoding.Unicode);
                sw.Write("MachineNumber" + "\t");
                sw.Write("UserID" + "\t");
                sw.Write("Name" + "\t");
                sw.Write("CardNumber" + "\t");
                sw.Write("Privilege" + "\t");
                sw.Write("Password" + "\t");
                sw.WriteLine("Enabled");
                foreach (ListViewItem item in lvCard.Items)
                {
                    sw.Write("'" + item.SubItems[0].Text + "\t");
                    sw.Write("'" + item.SubItems[1].Text + "\t");
                    sw.Write("'" + item.SubItems[2].Text + "\t");
                    sw.Write("'" + item.SubItems[3].Text + "\t");
                    sw.Write("'" + item.SubItems[4].Text + "\t");
                    sw.Write("'" + item.SubItems[5].Text + "\t");
                    sw.Write("'" + item.SubItems[6].Text + "\t");
                    sw.WriteLine();
                }
                sw.Close();
                log.Info("Export successed userinfo to csv!");
                MessageBox.Show("Export file userinfo successed!");
            }
            catch (Exception a)
            {
                log.Info("Error when export userinfo, ErrorCode: " + a.ToString());
                MessageBox.Show("Error when export userinfo, ErrorCode: " + a.ToString(), "Error");
            }
        }

        private void btnExportRecordsData_Click_1(object sender, EventArgs e)
        {

        }
    }
}
