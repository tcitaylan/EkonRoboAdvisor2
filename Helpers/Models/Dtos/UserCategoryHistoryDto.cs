
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpers.Models.Dtos
{
    public partial class UserCategoryHistoryDto
    {
        [Key]
        public int RecordId { get; set; }
        
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }        
        public DateTime? RecordDate { get; set; }
        public int? SurveyId { get; set; }
        public int? UserScore { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
    }
}
