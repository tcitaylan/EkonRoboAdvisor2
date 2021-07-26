using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class RecordLogsDto
    {
        [Key]
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Explanation { get; set; }
        public bool? IsSecret { get; set; }
        public string Application { get; set; }
    }
}
