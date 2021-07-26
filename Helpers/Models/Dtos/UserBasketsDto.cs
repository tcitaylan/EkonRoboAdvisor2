
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class UserBasketsDto
    {
        [Key]
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public bool? Status { get; set; }
        public bool? AutoBalance { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public string Explanation { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
        public int? ContractId { get; set; }
        public decimal? ContractAmount { get; set; }
        public bool? SendSms { get; set; }
        public bool? SendEmail { get; set; }
        public int? PortfolioNotif { get; set; }
        public bool? UserAgreement { get; set; }
    }
}
