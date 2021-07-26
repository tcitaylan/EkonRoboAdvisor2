using Helpers.Models.Dtos;
using System.Collections.Generic;

namespace Helpers.Models.ViewModels
{
    public class SurveyAnswers
    {
        
        public List<AnswersDto> answers { get; set; }
        
        public int UserID { get; set; }
        
    }
}