using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class TemplateBasketStocksBackup
    {
        public int RecordId { get; set; }
        public int? TemplateBasketId { get; set; }
        public int? SymbolId { get; set; }
        public double? Perc { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? RecordUserId { get; set; }
        public int? RecordStatus { get; set; }
    }
}
