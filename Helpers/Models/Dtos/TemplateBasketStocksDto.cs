using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class TemplateBasketStocksDto
    {
        [Key]
        public int RecordId { get; set; }
        public int? TemplateBasketId { get; set; }
        public int? SymbolId { get; set; }
        public double? Perc { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? RecordUserId { get; set; }
        public int? RecordStatus { get; set; }
    }
}
