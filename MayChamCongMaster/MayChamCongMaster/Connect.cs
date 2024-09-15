using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace MayChamCong
{
    class Connect
    {
        private string _connect = @"Data Source=.\SQLEXPRESS;Initial Catalog=MayChamCongAEON;Persist Security Info=True;User ID=sa;Password=123";
        private const string SQLTTDevices = "SELECT DevicesID,DevicesName,DevicesIP,DevicesPort,DevicesMalls,DevicesStatus,DevicesTime FROM Devices";
        private const string SQLTTRecord = "SELECT * FROM Record_Push";
        private const string SQLTTLogFiles = "SELECT * FROM Files";
        private const string SQLTTMalls = "SELECT * FROM Malls";

        private SqlConnection GetConnection()
        {
            SqlConnection connect = new SqlConnection(_connect);
            connect.Open();
            return connect;
        }

        public DataTable ThongtinDevices()
        {
            SqlConnection cnn = GetConnection();
            try
            {
                SqlDataAdapter dar = new SqlDataAdapter(SQLTTDevices, cnn);
                DataTable dv = new DataTable("Devices");
                dar.Fill(dv);
                return dv;

            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }

        public DataTable ThongtinRecord()
        {
            SqlConnection cnn = GetConnection();
            try
            {
                SqlDataAdapter dar = new SqlDataAdapter(SQLTTRecord, cnn);
                DataTable dv = new DataTable("RecordsInfo");
                dar.Fill(dv);
                return dv;

            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }

        public DataTable ThongtinLogFiles()
        {
            SqlConnection cnn = GetConnection();
            try
            {
                SqlDataAdapter dar = new SqlDataAdapter(SQLTTLogFiles, cnn);
                DataTable dv = new DataTable("LogFiles");
                dar.Fill(dv);
                return dv;

            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }

        public DataTable ThongtinMalls()
        {
            SqlConnection cnn = GetConnection();
            try
            {
                SqlDataAdapter dar = new SqlDataAdapter(SQLTTMalls, cnn);
                DataTable dv = new DataTable("Malls");
                dar.Fill(dv);
                return dv;

            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
        }
    }

}
