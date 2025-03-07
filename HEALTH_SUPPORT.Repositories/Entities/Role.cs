using HEALTH_SUPPORT.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Entities
{
    public class Role : Entity<Guid>
    {
        [Required]
        public string Name { get; set; }

        public ICollection<Account> Accounts { get; set; }

    }
}
