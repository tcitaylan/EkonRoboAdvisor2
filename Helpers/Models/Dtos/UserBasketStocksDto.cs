
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class UserBasketStocksDto
    {
        [Key]
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? SymbolId { get; set; }
        public double? Lot { get; set; }
        public DateTime? LastChange { get; set; }
        public int? UserBasketId { get; set; }
        public double? AvgPrice { get; set; }
        public int? CrId { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
