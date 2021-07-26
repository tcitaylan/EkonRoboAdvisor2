using Helpers.Models.Dtos;
using System.Collections.Generic;

namespace Helpers.Models.ViewModels
{
    public class Survey
    {
        public List<SurveyQuestionsDto> SurveryQuestion { get; set; }
        public List<AnswersDto> Answer { get; set; }
    }
}