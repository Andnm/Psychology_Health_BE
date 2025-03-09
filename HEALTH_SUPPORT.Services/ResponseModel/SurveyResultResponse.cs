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
    public class SurveyResultResponse
    {
        public class GetSurveyResultModel
        {
            public Guid Id { get; set; }
            public int Score { get; set; }
            public string ResultDescription { get; set; }
            public DateTimeOffset CreateAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            public Guid AccountSurveyId { get; set; }
            public Guid SurveyId { get; set; }
            public bool IsDelete { get; set; }
        }
    }
}
