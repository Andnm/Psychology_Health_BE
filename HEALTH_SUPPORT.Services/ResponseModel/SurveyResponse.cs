using HEALTH_SUPPORT.Services.RequestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public class SurveyResponse
    {
        public class GetSurveyModel
        {
            public Guid Id { get; set; }
            public int MaxScore { get; set; }
            public DateTimeOffset CreateAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            [Required]
            public Guid SurveyTypeId { get; set; }
            public bool IsDeleted { get; set; }
        }

        public class GetSurveyDetailsModel
        {
            public Guid Id { get; set; }
            public int MaxScore { get; set; }
            public DateTimeOffset CreateAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            [Required]
            public Guid SurveyTpyeId { get; set; }
            public bool IsDeleted { get; set; }
            public List<SurveyQuestionResponse.GetSurveyQuestionModel> QuestionList { get; set; }
        }
    }
}
