using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class Symbols
    {
        public int RecordId { get; set; }
        public string Name { get; set; }
        public string Sym { get; set; }
        public string Market { get; set; }
        public string Type { get; set; }
        public string Explanation { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? RecordStatus { get; set; }
        public int? CrId { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
