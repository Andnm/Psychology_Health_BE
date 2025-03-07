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
    public class SurveyResults : Entity<Guid>
    {
        [Required]
        public int Score { get; set; }
        [Required]
        public string ResultDescription { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public Guid AccountSurveyId { get; set; }
        [ForeignKey("AccountSurveyId")]
        public AccountSurvey AccountSurvey { get; set; }

        public Guid SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }
        
    }
}
