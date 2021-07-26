using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class Users
    {
        public Users()
        {
            Answers = new HashSet<Answers>();
            RebalanceHistory = new HashSet<RebalanceHistory>();
            UserBaskets = new HashSet<UserBaskets>();
            UserCategoryHistory = new HashSet<UserCategoryHistory>();
        }

        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public bool? ActiveStatus { get; set; }
        public string Type { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Gsm { get; set; }
        public string Address { get; set; }
        public string NameSurname { get; set; }
        public string CustomerNo { get; set; }
        public string Tckn { get; set; }
        public string Ip { get; set; }
        public bool? Newsletter { get; set; }
        public string Authorization { get; set; }
        public bool? IsBlocked { get; set; }
        public int? FailedLoginCount { get; set; }
        public bool? AlgoTradingPermission { get; set; }
        public bool? LiveDataPermission { get; set; }
        public int? CrId { get; set; }
        public string CrDate { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public virtual ICollection<Answers> Answers { get; set; }
        public virtual ICollection<RebalanceHistory> RebalanceHistory { get; set; }
        public virtual ICollection<UserBaskets> UserBaskets { get; set; }
        public virtual ICollection<UserCategoryHistory> UserCategoryHistory { get; set; }
    }
}
