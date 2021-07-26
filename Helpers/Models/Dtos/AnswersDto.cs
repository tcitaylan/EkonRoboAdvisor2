
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpers.Models.Dtos
{
    public partial class AnswersDto
    {
        [Key]
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public bool? RecordStatus { get; set; }
        public int? UserId { get; set; }
        public int? QuestionId { get; set; }
        public string Answer { get; set; }
        public int? SurveyId { get; set; }

    }
}
