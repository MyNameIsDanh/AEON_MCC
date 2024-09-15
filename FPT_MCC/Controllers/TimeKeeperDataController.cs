using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FPT_MCC.Filters;
using FPT_MCC.Models;

namespace FPT_MCC.Controllers
{
    [BasicAuthentication]
    public class TimeKeeperDataController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets All Data from MCC and POS
        /// </summary>
        public IEnumerable<Record> GetData()
        {
            log.Info("Log Started!");
            var db = new MayChamCongAEONEntities();
            return db.Records;
        }

        /// <summary>
        /// Gets Data from Store Id
        /// </summary>
        /// <param name="StoreId">The ID of Mall or POS| Value = '-1' to get All Value</param>
        /// <param name="FrDate">From Date (Format: YYYYMMDD)</param>
        /// <param name="ToDate">To Date (Format: YYYYMMDD)</param>
        public IHttpActionResult GetRecord(string FrDate, string ToDate, string StoreId = "-1")
        {
            DateTime fr_date = DateTime.Today;
            fr_date = DateTime.ParseExact(FrDate, "yyyyMMdd", null);
            DateTime to_date = DateTime.Today;
            to_date = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var db = new MayChamCongAEONEntities();
            db.Database.CommandTimeout = 300;
            string str_qry = "";
            if (StoreId == "-1")
            {
                str_qry = string.Format("Select T0.R_ID, T0.R_MCC_ID, T0.R_UserMaCC, T0.R_Year, T0.R_Month, T0.R_Day, T0.R_Hour, T0.R_Minute, T0.R_Second " +
                    "from Records T0 inner join Devices T1 on T0.R_MCC_ID = T1.DevicesID where ISNULL(T1.DevicesEating,'N') = 'N' AND " +
                    "CONVERT(date,CONCAT(T0.R_Day,'/',T0.R_Month,'/',T0.R_Year),103) between '{0}' and '{1}'" +
                    "UNION ALL " +
                    "Select FTPID, -99 as \"FTPPOSID\", FTPUserMaCC, FTPYear, FTPMonth, FTPDay,FTPHour,FTPMinute,FTPSecond from FTP where "+
                    "CONVERT(date,CONCAT(FTPDay,'/',FTPMonth,'/',FTPYear),103) between '{2}' and '{3}'"
                    , fr_date.ToString("MM/dd/yyyy")
                    , to_date.ToString("MM/dd/yyyy")
                    , fr_date.ToString("MM/dd/yyyy")
                    , to_date.ToString("MM/dd/yyyy"));
            }
            else
            {
                str_qry = string.Format("Select T0.R_ID, T0.R_MCC_ID, T0.R_UserMaCC, T0.R_Year, T0.R_Month, T0.R_Day, T0.R_Hour, T0.R_Minute, T0.R_Second " +
                    "from Records T0 inner join Devices T1 on T0.R_MCC_ID = T1.DevicesID where ISNULL(T1.DevicesEating,'N') = 'N' AND T1.DevicesMalls = '{0}' " +
                    "and CONVERT(date,CONCAT(T0.R_Day,'/',T0.R_Month,'/',T0.R_Year),103) between '{1}' and '{2}' " +
                    "UNION ALL " +
                    "Select FTPID, -99 as \"FTPPOSID\", FTPUserMaCC, FTPYear, FTPMonth, FTPDay,FTPHour,FTPMinute,FTPSecond from FTP where 'A001' = '{3}' " +
                    "and CONVERT(date,CONCAT(FTPDay,'/',FTPMonth,'/',FTPYear),103)  between '{4}' and '{5}'"
                    , StoreId
                    , fr_date.ToString("MM/dd/yyyy")
                    , to_date.ToString("MM/dd/yyyy")
                    , StoreId
                    , fr_date.ToString("MM/dd/yyyy")
                    , to_date.ToString("MM/dd/yyyy"));
                
            }
            log.Info(str_qry);
            
            var query = db.Records.SqlQuery(str_qry).ToList<Record>();
            
            var records = query.FirstOrDefault<Record>();
            if (records == null)
            {
                return NotFound();
            }
            return Ok(query);
        }

        /// <summary>
        /// Push Data to MCC
        /// </summary>
        public HttpResponseMessage Post_Data(MCC_Records_Push[] request)
        {
            var db = new MayChamCongAEONEntities();
            db.MCC_Records_Push.AddRange(request);
            try
            {
                foreach (var error in db.GetValidationErrors())
                {
                    error.Entry.State = EntityState.Detached;
                }
                //Check Unique Contraint in R_SAP_Id column
                foreach (MCC_Records_Push r_check in request)
                {
                    var check_unique = db.MCC_Records_Push.Where(b => b.R_SAP_Id == r_check.R_SAP_Id).FirstOrDefault();
                    if (check_unique != null)
                    {
                        db.Entry(r_check).State = EntityState.Detached;
                        r_check.R_Id = -1;
                    }
                }
                db.SaveChanges();
                //Execute Procedure
                foreach (MCC_Records_Push r_check in request)
                {
                    
                    if (r_check.R_Id >= 1)
                    {
                        try
                        {
                            int row_insert = db.Database.ExecuteSqlCommand("Record_Push_Insert @R_ID", new SqlParameter("@R_ID", r_check.R_Id));
                            log.Info(string.Format("Insert to Record_Push R_Id:{0} | {1}", r_check.R_Id, row_insert));
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, request);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
