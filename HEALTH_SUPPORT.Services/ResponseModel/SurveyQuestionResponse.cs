using HEALTH_SUPPORT.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public class SurveyQuestionResponse
    {
        public class GetSurveyQuestionModel
        {
            public Guid Id { get; set; }
            public string ContentQ { get; set; }
            public string Options { get; set; }
            public bool Validity { get; set; }
            public DateTimeOffset CreateAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            public Guid SurveyTypeId { get; set; }
            public Guid SurveyId { get; set; }
            public bool IsDelete { get; set; }
            public List<SurveyAnswerResponse.GetSurveyAnswerModel> AnswerList { get; set; } = new();
        }
    }
}
