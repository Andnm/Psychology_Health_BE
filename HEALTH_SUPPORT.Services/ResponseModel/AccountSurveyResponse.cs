using HEALTH_SUPPORT.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public class AccountSurveyResponse
    {
        public class GetAccountSurveysModel
        {
            public Guid Id { get; set; }
            public DateTimeOffset CreateAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            public Guid SurveyId { get; set; }
            public Guid AccountId { get; set; }
            public bool IsDelete { get; set; }
            public List<SurveyResultResponse.GetSurveyResultModel>? ResultList { get; set; } = new();
        }
    }
}
