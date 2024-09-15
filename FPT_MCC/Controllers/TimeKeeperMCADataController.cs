using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FPT_MCC.Controllers
{
    public class TimeKeeperMCADataController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets Data May Cham An from Store Id
        /// </summary>
        /// <param name="MCA_StoreId">The ID of Mall or POS| Value = '-1' to get All Value</param>
        /// <param name="FrDate">From Date (Format: YYYYMMDD)</param>
        /// <param name="ToDate">To Date (Format: YYYYMMDD)</param>
        public IHttpActionResult GetRecord_MayChamAn(string FrDate, string ToDate, string MCA_StoreId = "-1")
        {
            DateTime fr_date = DateTime.Today;
            fr_date = DateTime.ParseExact(FrDate, "yyyyMMdd", null);
            DateTime to_date = DateTime.Today;
            to_date = DateTime.ParseExact(ToDate, "yyyyMMdd", null);
            var db = new MayChamCongAEONEntities();
            db.Database.CommandTimeout = 300;
            string str_qry = "";
            if (MCA_StoreId == "-1")
            {
                str_qry = string.Format("Select T0.R_ID, T0.R_MCC_ID, T0.R_UserMaCC, T0.R_Year, T0.R_Month, T0.R_Day, T0.R_Hour, T0.R_Minute, T0.R_Second " +
                    "from Records T0 inner join Devices T1 on T0.R_MCC_ID = T1.DevicesID where ISNULL(T1.DevicesEating,'N') = 'Y' AND " +
                    "CONVERT(date,CONCAT(T0.R_Day,'/',T0.R_Month,'/',T0.R_Year),103) between '{0}' and '{1}'"
                    , fr_date.ToString("MM/dd/yyyy")
                    , to_date.ToString("MM/dd/yyyy"));
            }
            else
            {
                str_qry = string.Format("Select T0.R_ID, T0.R_MCC_ID, T0.R_UserMaCC, T0.R_Year, T0.R_Month, T0.R_Day, T0.R_Hour, T0.R_Minute, T0.R_Second " +
                    "from Records T0 inner join Devices T1 on T0.R_MCC_ID = T1.DevicesID where ISNULL(T1.DevicesEating,'N') = 'Y' AND T1.DevicesMalls = '{0}' " +
                    "and CONVERT(date,CONCAT(T0.R_Day,'/',T0.R_Month,'/',T0.R_Year),103) between '{1}' and '{2}' "
                    , MCA_StoreId
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
    }
}
