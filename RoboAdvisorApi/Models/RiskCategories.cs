using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class RiskCategories
    {
        public RiskCategories()
        {
            UserCategoryHistory = new HashSet<UserCategoryHistory>();
        }

        public int RecordId { get; set; }
        public string CategoryName { get; set; }
        public string Explanation { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? MaxValue { get; set; }
        public int? MinValue { get; set; }
        public int? RecordStatus { get; set; }
        public int? CrId { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
        public byte[] Img { get; set; }
        public string Imgext { get; set; }

        public virtual ICollection<UserCategoryHistory> UserCategoryHistory { get; set; }
    }
}
