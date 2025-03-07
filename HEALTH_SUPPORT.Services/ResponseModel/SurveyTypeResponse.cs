using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public static class SurveyTypeResponse
    {
        public class GetSurveyTypeModel
        {
         
            public Guid Id { get; set; }
            public string SurveyName { get; set; }

            
        }
    }
}
