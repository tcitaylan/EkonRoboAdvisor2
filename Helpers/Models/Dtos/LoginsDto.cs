using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class LoginsDto
    {
        [Key]
        public int RecordId { get; set; }
        public int? UserId { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Explanation { get; set; }
    }
}
