using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class TemplateBasketBackTests
    {
        public int BacktestId { get; set; }
        public int? TemplateBasketId { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Result { get; set; }
    }
}
