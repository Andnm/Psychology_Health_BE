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
    public class AccountSurvey : Entity<Guid>, IAuditable
    {
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        [Required]
        public Guid SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public Account Account { get; set; }
        [Required]
        public ICollection<SurveyResults> SurveyResults { get; set; }         

    }
}
