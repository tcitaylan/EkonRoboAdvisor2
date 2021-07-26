using System;
using System.Collections.Generic;

namespace RoboAdvisorApi.Models
{
    public partial class Answers
    {
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        public bool? RecordStatus { get; set; }
        public int? UserId { get; set; }
        public int? QuestionId { get; set; }
        public string Answer { get; set; }
        public int? SurveyId { get; set; }

        public virtual Users User { get; set; }
    }
}
