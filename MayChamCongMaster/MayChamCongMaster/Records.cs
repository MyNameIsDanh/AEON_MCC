using log4net;
using MayChamCong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using zkemkeeper;


namespace MayChamCongMaster
{
    public partial class Records : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        AutoSize _form_resize;

        //khai báo mcc
        public CZKEM[] axCZKEM = new CZKEM[150];//150 may

        //khai báo biến log4net
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public Records()
        {
            InitializeComponent();
            //WindowState = FormWindowState.Maximized;
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            DataTable Dv = null;
            Dv = new Connect().ThongtinRecord();
            gridviewRecords.DataSource = Dv;
            //gridviewRecords.RowTemplate.Height = 35;
            gridviewRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridviewRecords.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //AutoSize
            _form_resize = new AutoSize(this);
            this.Load += _Load;
            this.Resize += _Resize;

            //khởi tạo mcc
            SqlDataAdapter MyAdapterNumbersDevices;
            DataTable MyTableNumbersDevices = new DataTable();
            MyAdapterNumbersDevices = new SqlDataAdapter("SELECT COUNT(*) FROM DEVICES", con);
            MyAdapterNumbersDevices.Fill(MyTableNumbersDevices);

            for (int i = 0; i < Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()) ; i++)
            {
                axCZKEM[i] = new CZKEM();
            }
        }
        private bool[] bIsConnected = new bool[20];
        private int[] iMachineNumber = new int[20];

        private void _Load(object sender, EventArgs e)
        {
            _form_resize._get_initial_size();
        }

        private void _Resize(object sender, EventArgs e)
        {
            _form_resize._resize();
        }


        private void cbbTTRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnect);
            if (cbbTTRecords.Text == "Tất cả")
            {
                SqlDataAdapter adapt = new SqlDataAdapter("select * from Record_Push", con);
                con.Open();
                adapt.Fill(dt);
                con.Close();
                gridviewRecords.DataSource = dt;
            }
            else if (cbbTTRecords.Text == "Error")
            {
                SqlDataAdapter adapt = new SqlDataAdapter("select * from Record_Push where Status1 ='ERROR' or Status2 ='ERROR'", con);
                con.Open();
                adapt.Fill(dt);
                con.Close();
                gridviewRecords.DataSource = dt;
            }
            else
            {
                SqlDataAdapter adapt = new SqlDataAdapter("select * from Record_Push where Status1 = '" + cbbTTRecords.Text + "'", con);
                con.Open();
                adapt.Fill(dt);
                con.Close();
                gridviewRecords.DataSource = dt;
                //gridviewRecords.DataBindings();
            }
        }

        private void btnExec_Click(object sender, EventArgs e)
        {
            bool checkedCell = false;
            int deviceid = 0;
            for (int i = 0; i <= gridviewRecords.Rows.Count - 1; i++)
            {
                //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                SqlConnection con = new SqlConnection(strConnect);
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                //kiểm tra checked rows
                checkedCell = (bool)gridviewRecords.Rows[i].Cells[0].EditedFormattedValue;
                if (checkedCell == true)
                {
                    string fullmalls = gridviewRecords.Rows[i].Cells[12].Value.ToString();
                    SqlDataAdapter MyAdapterNumbersDevices;
                    DataTable MyTableNumbersDevices = new DataTable();
                    MyAdapterNumbersDevices = new SqlDataAdapter("SELECT COUNT(*) FROM DEVICES", con);
                    MyAdapterNumbersDevices.Fill(MyTableNumbersDevices);
                    //đẩy về full malls
                    if (fullmalls == "X")
                    {
                        int demcreate1 = 0;
                        int demduplicate1 = 0;
                        int demerror1 = 0;
                        int demcreate2 = 0;
                        int demduplicate2 = 0;
                        int demerror2 = 0;
                        for (int m = 1; m <= Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()); m++)
                        {
                            
                            bIsConnected[m - 1] = false;
                            //connect MCC theo mã id
                            try
                            {

                                //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                                //SqlConnection con = new SqlConnection(connection);
                                SqlDataAdapter MyAdapterConnect;
                                DataTable MyTableConnect = new DataTable();
                                MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices Where DevicesID = '" + m + "'", con);
                                MyAdapterConnect.Fill(MyTableConnect);
                                //lấy ip, port từ database
                                string temp = MyTableConnect.Rows[0][0].ToString();
                                string port = MyTableConnect.Rows[0][1].ToString();
                                //check null
                                if (temp.Trim() == "" || port.Trim() == "")
                                {
                                    MessageBox.Show("IP and Port " + m + " cannot be null", "Error");
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
                                    MessageBox.Show("Unable to connect the device " + m + ",ErrorCode=" + idwErrorCode.ToString(), "Error");
                                    log.Info("Unable to connect the device " + m + ",ErrorCode=" + idwErrorCode.ToString());
                                }

                            }
                            catch
                            {
                                log.Info("Error when connect the devices " + m);
                                MessageBox.Show("Error when connect the devices " + m);
                            }


                            //push data
                            try
                            {

                                string recordid = gridviewRecords.Rows[i].Cells[1].Value.ToString();
                                //SqlConnection con = new SqlConnection(strConnect);
                                //con.Open();
                                //SqlCommand cmd = new SqlCommand();
                                cmd.Connection = con;
                                SqlDataAdapter MyAdapter;
                                DataTable MyTableRecord_Push = new DataTable();
                                MyAdapter = new SqlDataAdapter("select * from Record_push where R_ID = '" + recordid + "'", con);
                                MyAdapter.Fill(MyTableRecord_Push);
                                //kiem tra ket noi
                                if (bIsConnected[m - 1] == false)
                                {
                                    log.Info("ErrorDBtoMCC, Please connect the device" + m + " first!");
                                    MessageBox.Show("ErrorDBtoMCC, Please connect the device" + m + " first!", "Error!");
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
                                        case "CREATE"://create
                                            if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error")) //check valid_from and status1
                                            {
                                                int idwErrorCode = 0;

                                                bool bEnabled = true;

                                                string sdwEnrollNumber = usercardno;
                                                string sName = personalid;
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
                                                        log.Info("Created Success device "+ m +", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                        MessageBox.Show("Created Success device "+ m +", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demduplicate1++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Create Error device " + m + ", Create failed,ErrorCode=" + idwErrorCode.ToString());
                                                        MessageBox.Show("Create Error device " + m + ", Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                        log.Info("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                        MessageBox.Show("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demcreate1++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Create Error device " + m + ", Create failed,ErrorCode=" + idwErrorCode.ToString());
                                                        MessageBox.Show("Create Error device " + m + ", Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                if (MyTableRecord_Push.Rows[0][8].ToString() == "" || MyTableRecord_Push.Rows[0][8].ToString() == "Error" || MyTableRecord_Push.Rows[0][8].ToString() == "NULL")
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
                                                            log.Info("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                            //cmd.CommandText = UPDATESTATUS;
                                                            //cmd.ExecuteNonQuery();
                                                            demcreate2++;
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                            log.Info("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                            MessageBox.Show("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                            log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                            //cmd.CommandText = UPDATESTATUS;
                                                            //cmd.ExecuteNonQuery();
                                                            demcreate2++;
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                            log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
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
                                            else if (MyTableRecord_Push.Rows[0][8].ToString() != "Create" && MyTableRecord_Push.Rows[0][8].ToString() != "Duplicate")
                                            {
                                                MessageBox.Show("Chưa tới ngày xóa usercard "+ usercardno, "Thông báo");
                                                log.Info("chưa tới ngày xóa recordid "+ recordid);
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            break;

                                        case "UPDATE"://update
                                            if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error"))
                                            {
                                                int idwErrorCode = 0;

                                                bool bEnabled = true;
                                                //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                                string sdwEnrollNumber = usercardno;
                                                string sName = personalid;
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
                                                        log.Info("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                        MessageBox.Show("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                        //update status 
                                                        //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        //cmd.CommandText = UPDATESTATUS;
                                                        //cmd.ExecuteNonQuery();
                                                        demcreate1++;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Create Error device " + m + ", Create failed,ErrorCode=" + idwErrorCode.ToString());
                                                        MessageBox.Show("Create Error device " + m + ", Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                    log.Info("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                    MessageBox.Show("Created Success device " + m + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                    //update status 
                                                    //string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                    //cmd.CommandText = UPDATESTATUS;
                                                    //cmd.ExecuteNonQuery();
                                                    demduplicate1++;
                                                }
                                            }
                                            else if ((MyTableRecord_Push.Rows[0][8].ToString() != "" || MyTableRecord_Push.Rows[0][8].ToString() == "Error" || MyTableRecord_Push.Rows[0][8].ToString() == "NULL"))
                                            {
                                                if (MyTableRecord_Push.Rows[0][10].ToString() == "X")
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
                                                                log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                                MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                                //update status 
                                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                                //cmd.CommandText = UPDATESTATUS;
                                                                //cmd.ExecuteNonQuery();
                                                                demcreate2++;
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                                log.Info("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                                MessageBox.Show("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                                log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                                MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                                //update status 
                                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                                //cmd.CommandText = UPDATESTATUS;
                                                                //cmd.ExecuteNonQuery();
                                                                demcreate2++;
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                                log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                                MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
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
                                                                log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                                MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                                //update status 
                                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                                //cmd.CommandText = UPDATESTATUS;
                                                                //cmd.ExecuteNonQuery();
                                                                demcreate2++;
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                                log.Info("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                                MessageBox.Show("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                                log.Info("Deleted Success device " + m + ", Deleted user number: " + sdwEnrollNumber);
                                                                MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber+ "Successed!");
                                                                //update status 
                                                                //string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                                //cmd.CommandText = UPDATESTATUS;
                                                                //cmd.ExecuteNonQuery();
                                                                demcreate2++;
                                                            }
                                                            else
                                                            {
                                                                axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                                log.Info("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber);
                                                                MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber+ "Successed!");
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
                                            else if (MyTableRecord_Push.Rows[0][8].ToString() != "Create" && MyTableRecord_Push.Rows[0][8].ToString() != "Duplicate")
                                            {
                                                log.Info("Thông báo, Chưa tới ngày xóa recordid "+recordid);
                                                MessageBox.Show("Chưa tới ngày xóa usercard "+usercardno, "Thông báo");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                            break;

                                        case "DELETE"://delete
                                            if (days1 <= ngayHienTai && ngayHienTai <= days2)
                                            {
                                                int idwErrorCode = 0;

                                                bool bEnabled = false;
                                                //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                                string sdwEnrollNumber = usercardno;
                                                string sName = personalid;
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
                                                        log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                        MessageBox.Show("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        cmd.CommandText = UPDATESTATUS;
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[m - 1].GetLastError(ref idwErrorCode);
                                                        log.Info("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString());
                                                        MessageBox.Show("Delete Error device " + m + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
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
                                                        log.Info("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber);
                                                        MessageBox.Show("Deleted Success device "+ m +", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        cmd.CommandText = UPDATESTATUS;
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        log.Info("Deleted Success device " + m + ", Disable user number: " + sdwEnrollNumber);
                                                        MessageBox.Show("Deleted Success device " + m + ", Delete user number: " + sdwEnrollNumber, "Successed!");
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
                                                log.Info("Chưa tới ngày xóa recordid "+recordid);
                                                MessageBox.Show("Chưa tới ngày xóa usercard "+ usercardno, "Thông báo");
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
                                MessageBox.Show("Error when push data to MCC");
                            }
                        }

                        if (demcreate1 == Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()))
                        {
                            
                            //update status 
                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString() + "'";
                            cmd.CommandText = UPDATESTATUS;
                            cmd.ExecuteNonQuery();
                            log.Info("Status1 Create success R_id " + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            //MessageBox.Show("Create success Personal_id " + gridviewRecords.Rows[i].Cells[2].EditedFormattedValue.ToString());
                        }
                        else if (demduplicate1 == Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()))
                        {
                            
                            //update status 
                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString() + "'";
                            cmd.CommandText = UPDATESTATUS;
                            cmd.ExecuteNonQuery();
                            log.Info("Status1 Duplicate R_id " + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            //MessageBox.Show("Duplicate Personal_id " + gridviewRecords.Rows[i].Cells[2].EditedFormattedValue.ToString());
                        }
                        else if (demerror1 == Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()))
                        {
                            
                            //update status 
                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString() + "'";
                            cmd.CommandText = UPDATESTATUS;
                            cmd.ExecuteNonQuery();
                            log.Info("Status1 Eror R_id " + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            //MessageBox.Show("Error Personal_id " + gridviewRecords.Rows[i].Cells[2].EditedFormattedValue.ToString());
                        }
                        if (demcreate2 == Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()))
                        {
                            //update status 
                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString() + "'";
                            cmd.CommandText = UPDATESTATUS;
                            cmd.ExecuteNonQuery();
                            log.Info("Status2 Create success R_id " + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            //MessageBox.Show("Create success Personal_id " + gridviewRecords.Rows[i].Cells[2].EditedFormattedValue.ToString());
                        }
                        else if (demduplicate2 == Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()))
                        {
                            //update status 
                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString() + "'";
                            cmd.CommandText = UPDATESTATUS;
                            cmd.ExecuteNonQuery();
                            log.Info("Status2 Duplicate success R_id " + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            //MessageBox.Show("Duplicate Personal_id " + gridviewRecords.Rows[i].Cells[2].EditedFormattedValue.ToString());
                        }
                        else if (demerror2 == Convert.ToInt32(MyTableNumbersDevices.Rows[0][0].ToString()))
                        {
                            //update status 
                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString() + "'";
                            cmd.CommandText = UPDATESTATUS;
                            cmd.ExecuteNonQuery();
                            //MessageBox.Show(gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            log.Info("Status2 Error R_id " + gridviewRecords.Rows[i].Cells[1].EditedFormattedValue.ToString());
                            //MessageBox.Show("Error Personal_id " + gridviewRecords.Rows[i].Cells[2].EditedFormattedValue.ToString());
                        }
                    }
                    else//đẩy theo 1 máy
                    {
                        //lấy mã MCC
                        deviceid = Convert.ToInt32(gridviewRecords.Rows[i].Cells[10].Value.ToString());
                        bIsConnected[deviceid - 1] = false;
                        //connect MCC theo mã id
                        try
                        {

                            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                            //SqlConnection con = new SqlConnection(connection);
                            SqlDataAdapter MyAdapterConnect;
                            DataTable MyTableConnect = new DataTable();
                            MyAdapterConnect = new SqlDataAdapter("SELECT DevicesIP,DevicesPort FROM Devices Where DevicesID = '" + deviceid + "'", con);
                            MyAdapterConnect.Fill(MyTableConnect);
                            //lấy ip, port từ database
                            string temp = MyTableConnect.Rows[0][0].ToString();
                            string port = MyTableConnect.Rows[0][1].ToString();
                            //check null
                            if (temp.Trim() == "" || port.Trim() == "")
                            {
                                MessageBox.Show("IP and Port " + deviceid + " cannot be null", "Error");
                                log.Info("IP and Port" + deviceid + " cannot be null");
                                return;
                            }
                            int idwErrorCode = 0;

                            //bIsConnected[i] = axCZKEM[i].Connect_Net(temp, Convert.ToInt32(port));
                            bIsConnected[deviceid - 1] = axCZKEM[deviceid - 1].Connect_Net(temp, Convert.ToInt32(port));
                            if (bIsConnected[deviceid - 1] == true)
                            {
                                iMachineNumber[deviceid - 1] = deviceid;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                                axCZKEM[deviceid - 1].RegEvent(iMachineNumber[deviceid], 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                            }
                            else
                            {
                                axCZKEM[deviceid - 1].GetLastError(ref idwErrorCode);
                                MessageBox.Show("Unable to connect the device " + deviceid + ",ErrorCode=" + idwErrorCode.ToString(), "Error");
                                log.Info("Unable to connect the device " + deviceid + ",ErrorCode=" + idwErrorCode.ToString());
                            }

                        }
                        catch (Exception a)
                        {
                            log.Info("Error when connect the devices " + deviceid+",Errorcode: "+a.ToString());
                            MessageBox.Show("Error when connect the devices " + deviceid + ",Errorcode: " + a.ToString());
                        }


                        //push data
                        try
                        {

                            string recordid = gridviewRecords.Rows[i].Cells[1].Value.ToString();
                            //SqlConnection con = new SqlConnection(strConnect);
                            //con.Open();
                            //SqlCommand cmd = new SqlCommand();
                            cmd.Connection = con;
                            SqlDataAdapter MyAdapter;
                            DataTable MyTableRecord_Push = new DataTable();
                            MyAdapter = new SqlDataAdapter("select * from Record_push where R_ID = '" + recordid + "'", con);
                            MyAdapter.Fill(MyTableRecord_Push);
                            //kiem tra ket noi
                            if (bIsConnected[deviceid - 1] == false)
                            {
                                log.Info("ErrorDBtoMCC, Please connect the device" + deviceid + " first!");
                                MessageBox.Show("ErrorDBtoMCC, Please connect the device" + deviceid + " first!", "Error!");
                            }
                            else
                            {

                                string personalid = MyTableRecord_Push.Rows[0][1].ToString();
                                string usercardno = MyTableRecord_Push.Rows[0][3].ToString();
                                string userdayfromwork = MyTableRecord_Push.Rows[0][4].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[0][4].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[0][4].ToString().Substring(6, 2);
                                string userdaytowork = MyTableRecord_Push.Rows[0][5].ToString().Substring(0, 4) + "/" + MyTableRecord_Push.Rows[0][5].ToString().Substring(4, 2) + "/" + MyTableRecord_Push.Rows[0][5].ToString().Substring(6, 2);
                                DateTime days1 = DateTime.ParseExact(userdayfromwork, "yyyy/MM/dd", null);
                                DateTime days2 = DateTime.ParseExact(userdaytowork, "yyyy/MM/dd", null);
                                string ngayHeThong = DateTime.Now.ToString("yyyy/MM/dd");
                                DateTime ngayHienTai = Convert.ToDateTime(ngayHeThong);

                                switch (MyTableRecord_Push.Rows[0][6].ToString())
                                {
                                    case "CREATE"://create
                                        if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error")) //check valid_from and status1
                                        {
                                            int idwErrorCode = 0;

                                            bool bEnabled = true;

                                            string sdwEnrollNumber = usercardno;
                                            string sName = personalid;
                                            string sPassword = "";
                                            int iPrivilege = 0;
                                            string sCardnumber = usercardno;
                                            string cardno = "";
                                            //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                            //Cursor = Cursors.WaitCursor;
                                            axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
                                            //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                                            //lấy user card theo mã nv từ mcc
                                            if (axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                            {
                                                if (axCZKEM[deviceid-1].GetStrCardNumber(out cardno))
                                                {
                                                    string tam = cardno;
                                                }
                                            }
                                            if (cardno == sCardnumber)
                                            {
                                                axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[deviceid-1].SSR_SetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                                {
                                                    log.Info("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                    //MessageBox.Show("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                    log.Info("Create failed on device " + (deviceid) + " usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                    //MessageBox.Show("Create Error on device " + (deviceid) + ", Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                //Cursor = Cursors.Default;
                                            }
                                            else
                                            {
                                                axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[deviceid-1].SSR_SetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                                {
                                                    log.Info("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                    //MessageBox.Show("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                    log.Info("Create failed on device " + (deviceid) + " usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                    //MessageBox.Show("Create Error on device " + (deviceid) + ", Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                //Cursor = Cursors.Default;
                                            }
                                        }
                                        else if (MyTableRecord_Push.Rows[0][7].ToString() != "Create" && MyTableRecord_Push.Rows[0][7].ToString() != "Duplicate")
                                        {
                                            log.Info("Thông báo, Chưa tới ngày valid_from, usernumber " + personalid);
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        if ((MyTableRecord_Push.Rows[0][7].ToString() != "" && MyTableRecord_Push.Rows[0][7].ToString() != "Error") && ngayHienTai >= days2)
                                        {
                                            if (MyTableRecord_Push.Rows[0][8].ToString() == "" || MyTableRecord_Push.Rows[0][8].ToString() == "Error")
                                            {
                                                int idwErrorCode = 0;
                                                string sdwEnrollNumber = personalid;
                                                string sCardnumber = usercardno;
                                                string cardno = "";

                                                axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
                                                //lấy user card theo mã nv từ mcc
                                                if (axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                                {
                                                    if (axCZKEM[deviceid-1].GetStrCardNumber(out cardno))
                                                    {
                                                        string tam = cardno;
                                                    }
                                                }
                                                //check cardno đã tồn tại hay chưa
                                                if (cardno == sCardnumber)
                                                {
                                                    axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                    if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                    {
                                                        log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                        MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        cmd.CommandText = UPDATESTATUS;
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                        log.Info("Delete failed on device " + (deviceid) + " usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                        MessageBox.Show("Delete Error on device " + (deviceid) + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                        //update status 
                                                        string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                        cmd.CommandText = UPDATESTATUS;
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                    axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                    //Cursor = Cursors.Default;
                                                }
                                                else
                                                {
                                                    axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                    if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                    {
                                                        log.Info("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber);
                                                        MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                        cmd.CommandText = UPDATESTATUS;
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                        log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                        MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                        //update status 
                                                        string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                        cmd.CommandText = UPDATESTATUS;
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                    axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                    //Cursor = Cursors.Default;
                                                }
                                            }
                                        }
                                        else if (MyTableRecord_Push.Rows[0][8].ToString() != "Create" && MyTableRecord_Push.Rows[0][8].ToString() != "Duplicate")
                                        {
                                            MessageBox.Show("Chưa tới ngày xóa usercard " + usercardno, "Thông báo");
                                            log.Info("chưa tới ngày xóa recordid " + recordid);
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        break;

                                    case "UPDATE"://update
                                        if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error"))
                                        {
                                            int idwErrorCode = 0;

                                            bool bEnabled = true;
                                            //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[0][8].ToString());
                                            string sdwEnrollNumber = usercardno;
                                            string sName = personalid;
                                            string sPassword = "";
                                            int iPrivilege = 0;
                                            string sCardnumber = usercardno;
                                            string cardno = "";

                                            //Cursor = Cursors.WaitCursor;
                                            axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
                                            axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);//Before you using function SetUserInfo,set the card number to make sure you can upload it to the device

                                            //lấy user card theo mã nv từ mcc
                                            if (axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                            {
                                                if (axCZKEM[deviceid-1].GetStrCardNumber(out cardno))
                                                {
                                                    string tam = cardno;
                                                }
                                            }

                                            //check cardno đã tồn tại hay chưa
                                            if (cardno != sCardnumber)
                                            {//nếu không có cardno trong mcc thì tạo mới user
                                                axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[deviceid-1].SSR_SetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload the user's information(card number included)
                                                {
                                                    log.Info("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                    MessageBox.Show("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                    log.Info("Create failed on device " + (deviceid) + " usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                    MessageBox.Show("Create Error on device " + (deviceid) + ", Create failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                //Cursor = Cursors.Default;
                                            }
                                            else
                                            {
                                                log.Info("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sName + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString());
                                                MessageBox.Show("Created Success on device " + (deviceid) + ", (SSR_)SetUserInfo,UserID:" + sdwEnrollNumber + " Privilege:" + iPrivilege.ToString() + " Enabled:" + bEnabled.ToString(), "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                        else if (MyTableRecord_Push.Rows[0][7].ToString() != "Create" && MyTableRecord_Push.Rows[0][7].ToString() != "Duplicate")
                                        {
                                            MessageBox.Show("Chưa tới ngày tạo usercard " + usercardno, "Thông báo");
                                            log.Info("chưa tới ngày tạo recordid " + recordid);
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status1 = '' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        if ((MyTableRecord_Push.Rows[0][8].ToString() == "" || MyTableRecord_Push.Rows[0][8].ToString() == "Error"))
                                        {
                                            if (MyTableRecord_Push.Rows[0][10].ToString() == "X")
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

                                                    axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
                                                    //lấy user card theo mã nv từ mcc
                                                    if (axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                                    {
                                                        if (axCZKEM[deviceid-1].GetStrCardNumber(out cardno))
                                                        {
                                                            string tam = cardno;
                                                        }
                                                    }
                                                    //check cardno đã tồn tại hay chưa
                                                    if (cardno == sCardnumber)
                                                    {
                                                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                        {
                                                            log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                            log.Info("Delete failed on device " + (deviceid) + " usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                            MessageBox.Show("Delete Error on device " + (deviceid) + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                        //Cursor = Cursors.Default;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                        {
                                                            log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                            log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
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

                                                    axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
                                                    //lấy user card theo mã nv từ mcc
                                                    if (axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                                    {
                                                        if (axCZKEM[deviceid-1].GetStrCardNumber(out cardno))
                                                        {
                                                            string tam = cardno;
                                                        }
                                                    }
                                                    //check cardno đã tồn tại hay chưa
                                                    if (cardno == sCardnumber)
                                                    {
                                                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                        {
                                                            log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                            log.Info("Delete failed on device " + (deviceid) + " usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                            MessageBox.Show("Delete Error on device " + (deviceid) + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                        //Cursor = Cursors.Default;
                                                    }
                                                    else
                                                    {
                                                        axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                        if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                        {
                                                            log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        else
                                                        {
                                                            axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                            log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                            MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                            //update status 
                                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                            cmd.CommandText = UPDATESTATUS;
                                                            cmd.ExecuteNonQuery();
                                                        }
                                                        axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                        axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                        //Cursor = Cursors.Default;
                                                    }
                                                }
                                            }
                                        }
                                        else if (MyTableRecord_Push.Rows[0][8].ToString() != "Create" && MyTableRecord_Push.Rows[0][8].ToString() != "Duplicate")
                                        {
                                            MessageBox.Show("Chưa tới ngày xóa usercard " + usercardno, "Thông báo");
                                            log.Info("chưa tới ngày xóa recordid " + recordid);
                                            //update status 
                                            string UPDATESTATUS = "UPDATE Record_Push SET Status2 = '' WHERE R_ID = '" + recordid + "'";
                                            cmd.CommandText = UPDATESTATUS;
                                            cmd.ExecuteNonQuery();
                                        }
                                        break;

                                    case "DELETE"://delete
                                        if (days1 <= ngayHienTai && (MyTableRecord_Push.Rows[0][7].ToString() == "" || MyTableRecord_Push.Rows[0][7].ToString() == "Error"))
                                        {
                                            int idwErrorCode = 0;

                                            bool bEnabled = false;
                                            //int deviceid = Convert.ToInt32(MyTableRecord_Push.Rows[j][8].ToString());
                                            string sdwEnrollNumber = usercardno;
                                            string sName = personalid;
                                            string sPassword = "";
                                            int iPrivilege = 0;
                                            string sCardnumber = usercardno;
                                            string cardno = "";

                                            //Cursor = Cursors.WaitCursor;
                                            axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], false);
                                            //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                                            //lấy user card theo mã nv từ mcc
                                            if (axCZKEM[deviceid-1].SSR_GetUserInfo(iMachineNumber[deviceid], sdwEnrollNumber, out string name, out string pass, out int privilege, out bool enabled))
                                            {
                                                if (axCZKEM[deviceid-1].GetStrCardNumber(out cardno))
                                                {
                                                    string tam = cardno;
                                                }
                                            }
                                            //check cardno đã tồn tại hay chưa
                                            if (cardno == sCardnumber)
                                            {
                                                axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                {
                                                    log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                    MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Create' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                else
                                                {
                                                    axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                    log.Info("Delete failed on device " + (deviceid) + ", usernumber " + sdwEnrollNumber + ", ErrorCode=" + idwErrorCode.ToString());
                                                    MessageBox.Show("Delete Error on device " + (deviceid) + ", Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                    //update status 
                                                    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Error' WHERE R_ID = '" + recordid + "'";
                                                    cmd.CommandText = UPDATESTATUS;
                                                    cmd.ExecuteNonQuery();
                                                }
                                                axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                //Cursor = Cursors.Default;
                                            }
                                            else
                                            {

                                                axCZKEM[deviceid-1].SetStrCardNumber(sCardnumber);
                                                log.Info("Deleted Success on device " + (deviceid) + ", User number: " + sdwEnrollNumber);
                                                MessageBox.Show("Deleted Success on device " + (deviceid) + ", Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //update status 
                                                string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                cmd.CommandText = UPDATESTATUS;
                                                cmd.ExecuteNonQuery();

                                                //if (axCZKEM[deviceid-1].SSR_DeleteEnrollData(iMachineNumber[deviceid], sdwEnrollNumber, 12))
                                                //{
                                                //    log.Info("Deleted Success, User number: " + sdwEnrollNumber);
                                                //    //MessageBox.Show("Deleted Success, Delete user number: " + sdwEnrollNumber, "Successed!");
                                                //    //update status 
                                                //    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                //    cmd.CommandText = UPDATESTATUS;
                                                //    cmd.ExecuteNonQuery();
                                                //}
                                                //else
                                                //{
                                                //    axCZKEM[deviceid-1].GetLastError(ref idwErrorCode);
                                                //    log.Info("Delete successed usernumber "+ sdwEnrollNumber);
                                                //    //MessageBox.Show("Delete Error, Delete failed,ErrorCode=" + idwErrorCode.ToString(), "Error!");
                                                //    //update status 
                                                //    string UPDATESTATUS = "UPDATE Record_Push SET Status1 = 'Duplicate' WHERE R_ID = '" + recordid + "'";
                                                //    cmd.CommandText = UPDATESTATUS;
                                                //    cmd.ExecuteNonQuery();
                                                //}
                                                axCZKEM[deviceid-1].RefreshData(iMachineNumber[deviceid]);//the data in the device should be refreshed
                                                axCZKEM[deviceid-1].EnableDevice(iMachineNumber[deviceid], true);
                                                //Cursor = Cursors.Default;
                                            }
                                        }
                                        else if (MyTableRecord_Push.Rows[0][7].ToString() != "Create" && MyTableRecord_Push.Rows[0][7].ToString() != "Duplicate")
                                        {
                                            MessageBox.Show("Chưa tới ngày xóa usercard " + usercardno, "Thông báo");
                                            log.Info("chưa tới ngày xóa recordid " + recordid);
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
                            MessageBox.Show("Error when push data to MCC");
                        }
                    }
                }
                con.Close();
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            
            for (int i = 0; i <= gridviewRecords.Rows.Count - 1; i++)
            {
                checkbox1.Selected = true;
                gridviewRecords.Rows[i].Cells[0].Value = true;
            }
        }


        private void btnBoSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i <= gridviewRecords.Rows.Count - 1; i++)
            {
                checkbox1.Selected = true;
                gridviewRecords.Rows[i].Cells[0].Value = false;
            }
        }
        private void gridviewRecords_SelectionChanged(object sender, EventArgs e)
        {
            for (int i = 0; i <= gridviewRecords.SelectedRows.Count - 1; i++)
            {
                checkbox1.Selected = true;
                gridviewRecords.SelectedRows[i].Cells[0].Value = true;
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnect);
            SqlDataAdapter adapt = new SqlDataAdapter("select * from Record_Push Where Personnel_Id='"+ txtTim.Text +"' or Card_Id ='"+ txtTim.Text +"'", con);
            con.Open();
            adapt.Fill(dt);
            con.Close();
            gridviewRecords.DataSource = dt;
        }

        private void Records_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnect);
            SqlDataAdapter adapt = new SqlDataAdapter("select * from Record_Push where Status1 ='ERROR' or Status2 ='ERROR'", con);
            con.Open();
            adapt.Fill(dt);
            con.Close();
            gridviewRecords.DataSource = dt;
        }
    }
}
