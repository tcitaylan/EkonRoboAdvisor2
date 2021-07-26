using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class BasketCategory
    {
        public int RecordId { get; set; }
        public int? TempBasketId { get; set; }
        public int? RiskCategoryId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? CrId { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
