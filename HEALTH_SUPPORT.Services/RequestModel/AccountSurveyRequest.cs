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
    public class AccountSurveyRequest
    {
        public class CreateAccountSurveyModel
        {
            public Guid SurveyId { get; set; }
            public Guid AccountId { get; set; }
        }
    }
}
