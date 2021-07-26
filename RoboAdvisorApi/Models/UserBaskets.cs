using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class UserBaskets
    {
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
        public int? TemplateBasketId { get; set; }

        public virtual Users User { get; set; }
    }
}
