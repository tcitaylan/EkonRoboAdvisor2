using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class UserCategoryHistory
    {
        public int RecordId { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? SurveyId { get; set; }
        public int? UserScore { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }

        public virtual RiskCategories Category { get; set; }
        public virtual Users User { get; set; }
    }
}
