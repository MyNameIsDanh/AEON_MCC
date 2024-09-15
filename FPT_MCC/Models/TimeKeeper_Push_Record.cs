using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FPT_MCC.Models
{
    /// <summary>
    /// Dữ liệu đồn bộ xuống máy chấm công
    /// </summary>
    public class TimeKeeper_Push_Record
    {
        private int r_id;
        private string personnel_id;
        private string store_id;
        private string card_id;
        private string valid_from;
        private string valid_to;
        private string r_type;
        private string status;

        /// <summary>
        /// Record Id - Default = -1
        /// </summary>
        public int R_Id { get => r_id; set => r_id = value; }
        /// <summary>
        /// Mã nhân viên
        /// </summary>
        public string Personnel_Id { get => personnel_id; set => personnel_id = value; }
        /// <summary>
        /// Mã Mall hoặc POS
        /// </summary>
        public string Store_Id { get => store_id; set => store_id = value; }
        /// <summary>
        /// Mã thẻ từ
        /// </summary>
        public string Card_Id { get => card_id; set => card_id = value; }
        /// <summary>
        /// Ngày hiệu lực (Format: yyyyMMdd)
        /// </summary>
        public string Valid_From { get => valid_from; set => valid_from = value; }
        /// <summary>
        /// Ngày hết hiệu lực (Format: yyyyMMdd)
        /// </summary>
        public string Valid_To { get => valid_to; set => valid_to = value; }
        /// <summary>
        /// Loại thay đổi - C: Create | E: Edit | D: Delete
        /// </summary>
        public string R_Type { get => r_type; set => r_type = value; }
        /// <summary>
        /// Tình trạng cập nhật
        /// </summary>
        public string Status { get => status; set => status = value; }
    }
}