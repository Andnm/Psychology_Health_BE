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
    public class HealthData : Entity<Guid>
    {
        [Required]
        public int level { get; set; }
        public string description { get; set; }
        public DateTimeOffset FollowUpAppoint { get; set; }
        public Guid AccountId { get; set; }
        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        public Guid PsychologistId { get; set; }
        [ForeignKey("PsychologistId")]
        public Psychologist Psychologist { get; set; }
    }
}
