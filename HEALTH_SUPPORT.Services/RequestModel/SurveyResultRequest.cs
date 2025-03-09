using HEALTH_SUPPORT.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public class SurveyResultRequest
    {
        public class AddSurveyResultRequest
        {
            public List<Guid>? SurveyAnswerList { get; set; }
            public List<int>? ScoreList { get; set; }
            public string ResultDescription { get; set; }
            public Guid SurveyId { get; set; }
        }
        
        public class UpdateSurveyResultRequest
        {
            public int Score { get; set; }
            public string ResultDescription { get; set; }
            public Guid AccountSurveyId { get; set; }
            public Guid SurveyId { get; set; }
        }
    }
}
