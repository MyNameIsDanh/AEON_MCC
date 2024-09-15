using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FPT_MCC.Models
{
    /// <summary>
    /// Dữ liệu chấm công từ MCC và POS
    /// </summary>
    [Table("Records")]
    public class TimeKeeper_Record
    {
        private int r_id;
        private int term_ID;
        private int year;
        private int month;
        private int day;
        private int hour;
        private int minute;
        private int second;
        private int card_ID;

        /// <summary>
        /// Phương thức khởi tạo
        /// </summary>
        public TimeKeeper_Record(int r_id, int term_ID, int year, int month, int day, int hour, int minute, int second, int card_ID)
        {
            this.r_id = r_id;
            this.term_ID = term_ID;
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.card_ID = card_ID;
        }
        /// <summary>
        /// Record Id - Primary Key
        /// </summary>
        [Key]
        [Column("R_Id")]
        public int R_Id { get => r_id; set => r_id = value; }
        /// <summary>
        /// Mã thiết bị
        /// </summary>
        [Column("R_MCC_Id")]
        public int Term_ID { get => term_ID; set => term_ID = value; }
        /// <summary>
        /// Mã thẻ từ
        /// </summary>
        [Column("R_Card_Id")]
        public int Card_ID { get => card_ID; set => card_ID = value; }
        /// <summary>
        /// Năm
        /// </summary>
        [Column("R_Year")]
        public int Year { get => year; set => year = value; }
        /// <summary>
        /// Tháng
        /// </summary>
        [Column("R_Month")]
        public int Month { get => month; set => month = value; }
        /// <summary>
        /// Ngày
        /// </summary>
        [Column("R_Day")]
        public int Day { get => day; set => day = value; }
        /// <summary>
        /// Giờ
        /// </summary>
        [Column("R_Hour")]
        public int Hour { get => hour; set => hour = value; }
        /// <summary>
        /// Phút
        /// </summary>
        [Column("R_Minute")]
        public int Minute { get => minute; set => minute = value; }
        /// <summary>
        /// Giây
        /// </summary>
        [Column("R_Second")]
        public int Second { get => second; set => second = value; }

    }
}