using HEALTH_SUPPORT.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Entities
{
    public class SurveyType : Entity<Guid>
    {
        [Required]
        public string SurveyName { get; set; }

        public ICollection<SurveyQuestion> SurveyQuestions { get; set; }
        public ICollection<Survey> Surveys { get; set; }
    }
}
