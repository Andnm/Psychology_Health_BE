using HEALTH_SUPPORT.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public class SurveyQuestionRequest
    {
        public class CreateSurveyQuestionRequest
        {
            public string ContentQ { get; set; }
            public string Options { get; set; }
            public bool Validity { get; set; }
            public List<SurveyAnswerRequest.CreateSurveyAnswerRequest> AnswersList { get; set; }
        }
        public class UpdateSurveyQuestionRequest
        {
            public string ContentQ { get; set; }
            public string Options { get; set; }
            public bool Validity { get; set; }
            public bool? IsDelete { get; set; }
        }
    }
}
