using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class RecordLogs
    {
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public bool? IsSecret { get; set; }
        public string Application { get; set; }
    }
}
