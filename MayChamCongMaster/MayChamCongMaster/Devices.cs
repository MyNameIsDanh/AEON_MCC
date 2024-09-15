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

namespace MayChamCongMaster
{
    public partial class Devices : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        AutoSize _form_resize;
        public Devices()
        {
            InitializeComponent();

            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            DataTable Dv = null;
            Dv = new Connect().ThongtinDevices();
            gridviewdevices.DataSource = Dv;
            gridviewdevices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridviewdevices.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

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

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            SqlDataAdapter MyAdapterConnect;
            DataTable MyTableDevicesID = new DataTable();
            MyAdapterConnect = new SqlDataAdapter("SELECT DevicesID FROM Devices", con);
            MyAdapterConnect.Fill(MyTableDevicesID);
            //string DELETEDEIVCES = "DETELE DEVICES";
            //cmd.CommandText = DELETEDEIVCES;
            //cmd.ExecuteNonQuery();
            try
            {
                for (int i = 0; i < gridviewdevices.Rows.Count - 1; i++)
                {
                    string deviceid = gridviewdevices.Rows[i].Cells[0].Value.ToString();
                    string devicename = gridviewdevices.Rows[i].Cells[1].Value.ToString();
                    string deviceip = gridviewdevices.Rows[i].Cells[2].Value.ToString();
                    string deviceport = gridviewdevices.Rows[i].Cells[3].Value.ToString();
                    string devicemall = gridviewdevices.Rows[i].Cells[4].Value.ToString();

                    if (deviceid == "" || devicename == "" || deviceip == "" || deviceport == "" || devicemall == "")
                    {
                        MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Không được để trống");
                        return;
                    }

                    int dem = 0;
                    for (int j = 0; j < MyTableDevicesID.Rows.Count; j++)
                    {
                        if (gridviewdevices.Rows[i].Cells[0].Value.ToString() == MyTableDevicesID.Rows[j][0].ToString())
                        {
                            dem++;
                        }
                    }
                    if (dem == 0)
                    {
                        try
                        {
                            //insert
                            string INSERTDEIVCES = "INSERT INTO Devices (DevicesID,DevicesName,DevicesIP,DevicesPort,DevicesMalls) VALUES ('" + deviceid + "',N'" + devicename + "','" + deviceip + "','" + deviceport + "','" + devicemall + "')";
                            cmd.CommandText = INSERTDEIVCES;
                            cmd.ExecuteNonQuery();

                        }
                        catch
                        {
                            MessageBox.Show("Insert failed!", "Error");
                        }
                    }
                    else
                    {
                        try
                        {
                            //update
                            string UPDATEDEIVCES = "UPDATE Devices SET DevicesName=N'" + devicename + "',DevicesIP='" + deviceip + "',DevicesPort='" + deviceport + "',DevicesMalls='" + devicemall + "' where DevicesID='" + deviceid + "'";
                            cmd.CommandText = UPDATEDEIVCES;
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            MessageBox.Show("Update failed!", "Error");
                        }
                    }
                }
                MessageBox.Show("Successed","Thông báo");
            }
            catch
            {

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            try
            {
                for (int i = 0; i < gridviewdevices.SelectedRows.Count; i++)
                {
                    string deviceid = gridviewdevices.SelectedRows[i].Cells[0].Value.ToString();
                    //delete
                    string DELETEDEIVCES = "DELETE DEVICES WHERE DEVICESID = '" + deviceid + "'";
                    cmd.CommandText = DELETEDEIVCES;
                    cmd.ExecuteNonQuery();
                }
                //refresh gridview
                DataTable Dv = null;
                Dv = new Connect().ThongtinDevices();
                gridviewdevices.DataSource = Dv;
                MessageBox.Show("Deleted successed!","Thông báo");
            }
            catch
            {
                MessageBox.Show("Deleted device failed!","Error");
            }
            con.Close();
        }
    }
}
