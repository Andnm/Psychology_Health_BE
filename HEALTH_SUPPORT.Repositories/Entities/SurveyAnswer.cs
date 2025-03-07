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
    public class SurveyAnswer : Entity<Guid>, IAuditable
    {
        [Required]
        public string Content { get; set; }
        [Required]
        public int Point { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public Guid QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public SurveyQuestion SurveyQuestion { get; set; }

    }
}
