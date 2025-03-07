using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public class SurveyTypeRequest
    {
        public class CreateSurveyTypeModel
        {
            [Required(ErrorMessage ="Tên khảo sát không được bỏ trống")]
            public string SurveyName { get; set; }
        }

        public class UpdateSurveyTypeModel
        {
            [Required(ErrorMessage = "Tên khảo sát không được bỏ trống")]
            public string SurveyName { get; set; }
        }
    }
}
