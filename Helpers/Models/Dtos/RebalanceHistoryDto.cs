
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class RebalanceHistoryDto
    {
        [Key]
        public int RecordId { get; set; }
        public int? UserId { get; set; }
        public DateTime? RecordDate { get; set; }
        public bool? IsAutomatic { get; set; }
        public string Explanation { get; set; }
        public string PortfolioBefore { get; set; }
        public string PortfolioAfter { get; set; }
        public int? ContractId { get; set; }

    }
}
