using MayChamCong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MayChamCongMaster
{
    public partial class Logfile : Form
    {
        private string strConnect = ConfigurationManager.ConnectionStrings["ConnectDBSQL"].ConnectionString;
        AutoSize _form_resize;
        public Logfile()
        {
            InitializeComponent();
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            SqlConnection con = new SqlConnection(strConnect);
            con.Open();
            DataTable Dv = null;
            Dv = new Connect().ThongtinLogFiles();
            gridviewlogfile.DataSource = Dv;
            //gridviewlogfile.RowTemplate.Height = 40;
            gridviewlogfile.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridviewlogfile.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
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

        private void btnChonFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog result = new OpenFileDialog();

            //openFileDialog1.RestoreDirectory = true;
            if (result.ShowDialog() == DialogResult.OK) // Test result.
            {
                //Do whatever you want
                txtboxFileName.Text = System.IO.Path.GetFileName(result.FileName.Trim());
            }
        }

        private void btnUpdateLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                string DirectoryDF = ConfigurationManager.AppSettings.Get("DirectoryDF");
                string DirectoryDT = ConfigurationManager.AppSettings.Get("DirectoryDT");
                //String path = @"C:\\PosSeito\\" + txtboxFileName.Text;
                String path = DirectoryDF + txtboxFileName.Text;
                if (!File.Exists(path))
                {
                    Console.WriteLine("File " + path + " does not exists!");
                    return;
                }
                using (Stream readingStream = new FileStream(path, FileMode.Open))
                {
                    byte[] temp = new byte[10000];
                    UTF8Encoding encoding = new UTF8Encoding(true);

                    int len = 0;

                    len = readingStream.Read(temp, 0, temp.Length);
                    StringBuilder s1 = new StringBuilder();
                    String s = "";
                    int dem = 0;
                    s = encoding.GetString(temp, 0, len); //+"\r\n"+ encoding.GetString(temp, 0, len);
                    //s = s.Split("\r\n")[2];
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = s.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    //int rowcount = (int)len / 30;

                    //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                    SqlConnection con = new SqlConnection(strConnect);

                    con.Open();
                    //string ADDFILE = "UPDATE FILES WHERE FILENAME = '" + txtboxFileName.Text + "'";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    SqlDataAdapter MyAdapter;
                    DataTable MyTableFileName = new DataTable();
                    MyAdapter = new SqlDataAdapter("SELECT FileName FROM Files", con);
                    MyAdapter.Fill(MyTableFileName);
                    //kiểm tra tồn tại file trong table Files hay chưa
                    int trung = 0;
                    for (int j=0;j<MyTableFileName.Rows.Count;j++)
                    {
                        if (txtboxFileName.Text == MyTableFileName.Rows[j][0].ToString())
                        {
                            trung++;
                        }
                    }
                    if (trung == 0)
                    {
                        string INSERTFILE = "INSERT INTO Files(FileName) VALUES('" + txtboxFileName.Text + "')";
                        cmd.CommandText = INSERTFILE;
                        cmd.ExecuteNonQuery();
                    }

                    //cmd.CommandText = ADDFILE;
                    //cmd.ExecuteNonQuery();

                    DataTable MyTable = new DataTable();
                    MyAdapter = new SqlDataAdapter("select fileid from files where filename = '" + txtboxFileName.Text + "'", con);
                    MyAdapter.Fill(MyTable);
                    //con.Close();
                    //cmd.Dispose();
                    //kiểm tra xem đã có dữ liệu hay chưa
                    DataTable MyTableDB = new DataTable();
                    MyAdapter = new SqlDataAdapter("select count(*) from FTP where FTPFileID = '" + MyTable.Rows[0][0].ToString() + "'", con);
                    MyAdapter.Fill(MyTableDB);
                    int kt = Convert.ToInt32(MyTableDB.Rows[0][0].ToString());
                    if (kt != 0)
                    {
                        string DELETEFTP = "DELETE FROM FTP WHERE FTPFileID = '" + MyTable.Rows[0][0].ToString() + "'";
                        //cmd.Connection = con;
                        cmd.CommandText = DELETEFTP;
                        cmd.ExecuteNonQuery();
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
                            string UPDATEFILE = "UPDATE FILES SET FILESTATUS='updated' WHERE FILEID = '" + MyTable.Rows[0][0].ToString() + "'";
                            cmd.CommandText = ADDFTP;
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = UPDATEFILE;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
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
                            string UPDATEFILE = "UPDATE FILES SET FILESTATUS='updated' WHERE FILEID = '" + MyTable.Rows[0][0].ToString() + "'";
                            cmd.CommandText = ADDFTP;
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = UPDATEFILE;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    con.Dispose();
                    cmd.Dispose();
                    MessageBox.Show("Số lượng record: " + dem, "Kết quả");
                    s1.Clear();
                }

                //refresh data in gridview
                //string connectionStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
                DataTable dt = new DataTable();
                SqlConnection connect = new SqlConnection(strConnect);
                if (cbbTTLogFile.Text == "Tất cả")
                {
                    SqlDataAdapter adapt = new SqlDataAdapter("select * from Files", connect);
                    connect.Open();
                    adapt.Fill(dt);
                    connect.Close();
                    gridviewlogfile.DataSource = dt;
                }
                else
                {
                    SqlDataAdapter adapt = new SqlDataAdapter("select * from Files where FileStatus = '" + cbbTTLogFile.Text + "'", connect);
                    connect.Open();
                    adapt.Fill(dt);
                    connect.Close();
                    gridviewlogfile.DataSource = dt;
                    //gridviewRecords.DataBindings();
                }

                //move file to forder NewFiles
                //String dirPath = @"C:\NewFiles";
                //String dirPath1 = @"C:\" + txtboxFileName.Text;
                //String dirPath2 = @"C:\NewFiles\" + txtboxFileName.Text;

                string ngay = DateTime.Now.Day.ToString().Trim();
                string thang = DateTime.Now.Month.ToString().Trim();
                string nam = DateTime.Now.Year.ToString().Trim();
                //String dirPath = @"C:\FilesPOS\" + nam + thang + ngay;
                //String dirPath1 = @"C:\PosSeito\" + System.IO.Path.GetFileName(txtboxFileName.Text);
                //String dirPath2 = @"C:\FilesPOS\" + nam + thang + ngay + @"\" + System.IO.Path.GetFileName(txtboxFileName.Text);
                String dirPath = DirectoryDT + nam + thang + ngay;
                String dirPath1 = DirectoryDF + System.IO.Path.GetFileName(txtboxFileName.Text);
                String dirPath2 = DirectoryDT + nam + thang + ngay + @"\" + System.IO.Path.GetFileName(txtboxFileName.Text);
                // Kiểm tra xem đường dẫn thư mục tồn tại không.
                bool exist = Directory.Exists(dirPath);
                if (!exist)
                {
                    MessageBox.Show(dirPath + " does not exist.");
                    MessageBox.Show("Create directory: " + dirPath);

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
                    MessageBox.Show("Move file successed!","Thông báo");
                }
            }
            catch (Exception a)
            {
                MessageBox.Show("Lỗi khi update file!, Error: "+a.ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string connection = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConnect);
            if (cbbTTLogFile.Text == "Tất cả")
            {
                SqlDataAdapter adapt = new SqlDataAdapter("select * from Files", con);
                con.Open();
                adapt.Fill(dt);
                con.Close();
                gridviewlogfile.DataSource = dt;
            }
            else
            {
                SqlDataAdapter adapt = new SqlDataAdapter("select * from Files where FileStatus = '" + cbbTTLogFile.Text + "'", con);
                con.Open();
                adapt.Fill(dt);
                con.Close();
                gridviewlogfile.DataSource = dt;
                //gridviewRecords.DataBindings();
            }
        }
    }
}
