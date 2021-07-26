using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class SymbolDataDto
    {
        [Key]
        public int RecordId { get; set; }
        public int? FundId { get; set; }
        public DateTime? Date { get; set; }
        public double? Value { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? CrId { get; set; }
        public int? UpId { get; set; }
        public int? UpDate { get; set; }
    }
}
