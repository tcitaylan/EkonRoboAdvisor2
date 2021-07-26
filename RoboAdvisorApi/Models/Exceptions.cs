using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class Exceptions
    {
        public int RecordId { get; set; }
        public string Application { get; set; }
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string Target { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? UserId { get; set; }
        public string Server { get; set; }
        public string Parameters { get; set; }
        public string AjaxParameters { get; set; }
    }
}
