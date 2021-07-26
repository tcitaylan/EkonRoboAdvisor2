using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class SpecialBasketStocksDto
    {
        [Key]
        public int RecordId { get; set; }
        public int? SpecialBasketId { get; set; }
        public int? SymbolId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? RecordUserId { get; set; }
        public int? RecordStatus { get; set; }
    }
}
