using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class Logins
    {
        public int RecordId { get; set; }
        public int? UserId { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Explanation { get; set; }
    }
}
