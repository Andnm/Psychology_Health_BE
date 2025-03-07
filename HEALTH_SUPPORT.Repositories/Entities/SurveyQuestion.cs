using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HEALTH_SUPPORT.Repositories.Abstraction;

namespace HEALTH_SUPPORT.Repositories.Entities
{
    public class SurveyQuestion : Entity<Guid>, IAuditable
    {
        public string ContentQ { get; set; } 
        public string Options { get; set; } 
        public bool Validity { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }

        public Guid SurveyTypeId { get; set; }
        [ForeignKey("SurveyTypeId")]
        public SurveyType SurveyType { get; set; }

        public Guid SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey{ get; set; }

        public ICollection<SurveyAnswer> SurveyAnswers { get; set; } = new List<SurveyAnswer>();

    }
}
