using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class ExceptionsDto
    {
        [Key]
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
