using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class SpecialBasketStocks
    {
        public int RecordId { get; set; }
        public int? SpecialBasketId { get; set; }
        public int? SymbolId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? RecordUserId { get; set; }
        public int? RecordStatus { get; set; }
    }
}
