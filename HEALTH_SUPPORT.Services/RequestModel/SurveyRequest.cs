using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public class SurveyRequest
    {
        public class CreateSurveyRequest
        {
            public int MaxScore { get; set; }
            public Guid SurveyTypeId { get; set; }
            public List<SurveyQuestionRequest.CreateSurveyQuestionRequest> QuestionList { get; set; }
        }
        public class UpdateSurveyRequest
        {
            public int MaxScore { get; set; }
            public Guid SurveyTpyeId { get; set; }
            public bool? IsDelete { get; set; }
        }
    }
}
