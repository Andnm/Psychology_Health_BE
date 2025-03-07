using HEALTH_SUPPORT.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Entities
{
    public class Survey: Entity<Guid> , IAuditable
    {
        public int MaxScore { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        [Required]
        public Guid SurveyTypeId { get; set; }
        [ForeignKey("SurveyTypeId")]
        public SurveyType SurveyType { get; set; }
        public ICollection<SurveyQuestion> SurveyQuestions { get; set; }
        public ICollection<AccountSurvey> AccountSurveys { get; set; }
        public ICollection<SurveyResults> SurveyResults { get; set; }
    }
}
