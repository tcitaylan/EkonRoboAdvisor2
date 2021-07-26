using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class TemplateBaskets
    {
        public int RecordId { get; set; }
        public string Name { get; set; }
        public string Explanation { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? RecordUserId { get; set; }
        public int? RecordStatus { get; set; }
    }
}
