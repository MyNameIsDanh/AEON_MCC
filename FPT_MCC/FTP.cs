//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FPT_MCC
{
    using System;
    using System.Collections.Generic;
    
    public partial class FTP
    {
        public long FTPID { get; set; }
        public int FTPPOSID { get; set; }
        public Nullable<int> FTPYear { get; set; }
        public Nullable<int> FTPMonth { get; set; }
        public Nullable<int> FTPDay { get; set; }
        public Nullable<int> FTPHour { get; set; }
        public Nullable<int> FTPMinute { get; set; }
        public Nullable<int> FTPSecond { get; set; }
        public Nullable<long> FTPFileID { get; set; }
        public string FTPUserMaCC { get; set; }
    
        public virtual File File { get; set; }
        public virtual POS POS { get; set; }
    }
}