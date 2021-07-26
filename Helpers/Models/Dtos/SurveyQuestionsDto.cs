using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class SurveyQuestionsDto
    {
        [Key]
        public int RecordId { get; set; }
        public string Question { get; set; }
        public string AnswerType { get; set; }
        public string AnswerProps { get; set; }
        public string ScoreProps { get; set; }
        public bool? IsActive { get; set; }
        public int? RecordStatus { get; set; }
        public int? ListOrder { get; set; }
        public int? CrId { get; set; }
        public DateTime? CrDate { get; set; }
        public int? UpId { get; set; }
        public string UpDate { get; set; }
    }
}
